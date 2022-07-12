using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using System;
using System.Threading;
using Banzan.Lib.Utility;
using CombatSystem.Entities;
using Facades;
using CombatSystem.Consultants;
using UnityEngine.Tilemaps;
using CoroutineGenerators;
using Managers.Subscribers;
using UnityEngine.Extensions;
using ColorLookUp;
using UnityEngine.EventSystems;
using AsyncTask = System.Threading.Tasks.Task;
using System.Collections;
using UnityEngine.InputSystem.UI;
using CombatSystem.Facade;

namespace Managers
{
    public class CombatSceneManager : MonoBehaviour
    {
        public static CombatSceneManager instance;

        public static CombatSceneManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<CombatSceneManager>();
                }
                return instance;
            }
        }

        public enum CommandType
        {
            Movement,
            Attack,
            Wait,
            Overwatch,
            BasicSkill,
            UltimateSkill
        }

        public enum SceneState
        {
            Selection,
            CombatMode,
            MovementMode,
            OpponentTurn,
            TurnEnded,
            Lost,
            OpponentThinking,
            Won
        }

        private SceneState gameState;

        public SceneState GameState => gameState;

        public static readonly Vector3 UNIT_WORLD_GROUND_OFFSET = new Vector3(0, 0.25f, 0);
        public static readonly Vector3 UNIT_WORLD_BODY_OFFSET = new Vector3(0, 0.5f, 0);
        public static readonly Vector3 HEALTH_BAR_WORLD_OFFSET = new Vector3(0, 0.75f, 0);

        public static readonly float UNIT_INTERPOLATION_SPEED = 2f;

        [SerializeField]
        private bool isPaused;

        public bool IsPaused => isPaused;

        [SerializeField]
        private UnitManager unitManager;

        [SerializeField]
        private TileManager tileManager;

        [SerializeField]
        private Tilemap ground;

        [SerializeField]
        private bool gameOverAllowed = true;

        public bool GameOverAllowed => gameOverAllowed;

        private bool isInitialised = false;

        private RoutineManager routineManager;

        private GameMap currentMap;

        public GameMap CurrentMap => currentMap;


        private KeyboardControls keyboardControls;
        private Camera mainCamera;

        public LineRenderer IndicatorLine
        {
            get;
            private set;
        }

        // ActionHandlers:
        private Action<InputAction.CallbackContext> leftClickHandler = _ => { };
        private Action<InputAction.CallbackContext> doubleLeftClickHandler = _ => { };

        private Action<InputAction.CallbackContext> rightClickHandler = _ => { };

        private Action<InputAction.CallbackContext> middleHoldHandler = _ => { };

        private CancellationTokenSource tokenSource;

        public Vector3Int CurrentActingUnitPosition => currentMap[currentMap.CurrentActingUnit];

        public bool IsPointerOverUI 
        {
            get
            {
                // [Only works well while there is not PhysicsRaycaster on the Camera)
                //EventSystem eventSystem = EventSystem.current;
                //return (eventSystem != null && eventSystem.IsPointerOverGameObject());

                // [Works with PhysicsRaycaster on the Camera. Requires New Input System. Assumes mouse.)
                if (EventSystem.current == null)
                {
                    return false;
                }
                RaycastResult lastRaycastResult = ((InputSystemUIInputModule)EventSystem.current.currentInputModule).GetLastRaycastResult(Mouse.current.deviceId);
                const int uiLayer = 5;
                return lastRaycastResult.gameObject != null && lastRaycastResult.gameObject.layer == uiLayer;
            }
        }

        public Unit CurrentActingUnit => currentMap.CurrentActingUnit;

        private List<Vector3Int> indicatedTiles = new List<Vector3Int>();
        private int savedActionCost;
        private bool executeLastActionAllowed;
        private bool autoPlayQueued;
        private int timeToWait;

        public void SetIsPaused(bool state)
        {
            isPaused = state;
        }

        public void SetGameOverAllowed(bool state)
        {
            gameOverAllowed = state;
        }

        public void Initialise(UnitManager unitManager, TileManager tileManager)
        {
            if (!isInitialised)
            {
                this.unitManager = unitManager;
                this.tileManager = tileManager;
                this.ground = tileManager.Ground;
                isInitialised = true;
            }
        }

        private void OnEnable()
        {
            keyboardControls.Enable();
        }

        private void OnDisable()
        {
            keyboardControls?.Disable();
            tokenSource?.Cancel();
            isPaused = true;
        }

        private void Awake()
        {
            Unit.ResetClass();

            keyboardControls = new KeyboardControls();
            routineManager = gameObject.AddComponent<RoutineManager>();

            if (instance != null)
            {
                Debug.LogError("CombatSceneManager is designed as a singleton, but more than one instance was discovered during this runtime. " +
                    "Please remove the other instances and refer to only one instance of CombatSceneManager");
            }

            IndicatorLine = gameObject.AddComponent<LineRenderer>();

            IndicatorLine.positionCount = 0;
            IndicatorLine.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            IndicatorLine.startWidth = 0.06f;
            IndicatorLine.endWidth = 0.06f;
            IndicatorLine.numCornerVertices = 64;
            IndicatorLine.sortingOrder = 3;
        }

        private void Start()
        {
            tokenSource = new CancellationTokenSource();
            mainCamera = Camera.main;
            currentMap = GameMap.MakeNewMap(unitManager.CreateUnitCensus(), tileManager.CreateTileCensus());

            IEnumerable<Unit> units = currentMap.GetUnits(unit => true);

            HealthBarManager.Instance.InitialiseHealthBars(units);

            UnitQueueManager.Instance.InitialiseQueue(units);

            gameState = SceneState.TurnEnded;
        }

        public Vector3 CellToWorld(Vector3Int cellCoordinates)
        {
            return ground.CellToWorld(cellCoordinates);
        }

        public Vector3Int WorldToCell(Vector3 worldCoordinates)
        {
            return ground.WorldToCell(worldCoordinates);
        }

        public void SetLastActionAllowed(bool state)
        {
            executeLastActionAllowed = state;
        }

        public void SetTimeToWait(int time)
        {
            timeToWait = time;
        }

        public void SetSavedActionCost(int actionPoints)
        {
            savedActionCost = actionPoints;
        }

        public void SetIndicatedTiles(IEnumerable<Vector3Int> indicationPositions)
        {
            this.indicatedTiles.Clear();
            foreach (Vector3Int indicationPosition in indicationPositions)
            {
                indicatedTiles.Add(indicationPosition);
            }
        }

        public void BeginCombat()
        {
            TileManager.Instance.IndicatorMap.ClearAllTiles();
            SubscribeToCombatListener();
            TileDrawer.Draw(TileManager.Instance.IndicatorMap,
                CombatConsultant.GetAllRangePositions(currentMap.Data, CurrentActingUnit),
                TileManager.Instance.Indicator,
                ColorPalette.YELLOW_TRANSLUCENT
                );
        }

        public void BeginMovement()
        {
            TileManager.Instance.IndicatorMap.ClearAllTiles();
            SubscribeToMovementListener();
            TileDrawer.Draw(TileManager.Instance.IndicatorMap, 
                MovementConsultant.GetAllMovementPositions(currentMap.Data, CurrentActingUnit),
                TileManager.Instance.Indicator
                );
        }

        private void SubscribeToMovementListener()
        {
            UnsubscribeAllControls();

            keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
            keyboardControls.Mouse.RightClick.performed -= rightClickHandler;

            leftClickHandler = _ => AuxillarySubscribers.SubscribeMovementSelection(keyboardControls.Mouse.MousePosition);
            doubleLeftClickHandler = _ => SelectCommand(CommandType.Movement);

            StateReset();
            gameState = SceneState.MovementMode;

            keyboardControls.Mouse.LeftClick.performed += leftClickHandler;
            keyboardControls.Mouse.DoubleLeftClick.performed += doubleLeftClickHandler;
        }

        private void SubscribeToCombatListener()
        {
            UnsubscribeAllControls();

            keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
            keyboardControls.Mouse.RightClick.performed -= rightClickHandler;

            leftClickHandler = _ => AuxillarySubscribers.SubscribeCombatSelection(keyboardControls.Mouse.MousePosition);
            middleHoldHandler = _ => AuxillarySubscribers.AttackReviewSubscriber(false);

            StateReset();
            gameState = SceneState.CombatMode;

            keyboardControls.Mouse.LeftClick.performed += leftClickHandler;
            keyboardControls.Mouse.MiddleButtonHold.performed += middleHoldHandler;
        }

        public void UnsubscribeAllControls()
        {
            keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
            keyboardControls.Mouse.DoubleLeftClick.performed -= doubleLeftClickHandler;

            keyboardControls.Mouse.RightClick.performed -= rightClickHandler;

            keyboardControls.Mouse.MiddleButtonHold.performed -= middleHoldHandler;
            StateReset();
        }

        public void StateReset()
        {
            SetLastActionAllowed(false);
            SetIndicatedTiles(new Vector3Int[0]);

            //InformationUIManager.Instance.SetCharacterDetails(CurrentActingUnit);
            CombatUIManager.Instance.UpdateCurrentActingUnitInformation();
            TileManager.Instance.IndicatorMap.ClearAllTiles();
            IndicatorLine.positionCount = 0;
        }


        [EnumAction(typeof(CommandType))]
        // Unity will not expose enums in the inspector for event so we have to resort to some weird hacks
        // Read: https://forum.unity.com/threads/ability-to-add-enum-argument-to-button-functions.270817/
        public void SelectCommand(int commandEnumIndex)
        {
            SelectCommand((CommandType)commandEnumIndex);
        }


        /// <summary>
        /// Performs a command as a request given to the current game map.
        /// </summary>
        /// <param name="command"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public void SelectCommand(CommandType command)
        {
            Debug.Log($"Command selected {command}");

            if (isPaused || !routineManager.IsDormant())
            {
                Debug.Log("Not allowed, the routine queue was not dormant or combat paused");
                return;
            }

            MapActionRequest request;

            switch (command)
            {
                case CommandType.Movement:
                    if (!executeLastActionAllowed)
                    {
                        Debug.Log("Command Intercepted: Not allowed");
                        return;
                    }

                    savedActionCost = MovementConsultant.GetPathCost(indicatedTiles, currentMap.Data);

                    request = new MovementRequest(
                        CurrentActingUnit, 
                        indicatedTiles.Last(),
                        savedActionCost,
                        savedActionCost, 
                        MovementRequest.Outcome.Pending);

                    break;

                case CommandType.Attack:
                    if (!executeLastActionAllowed)
                    {
                        return;
                    }

                    CanvasManager.Instance.DeactivateUI(CanvasManager.UIType.OpponentSheet);

                    request = CombatConsultant.SimulateAttack(
                        CurrentActingUnit, CurrentMap[indicatedTiles.Last()], currentMap.Data, considerActionPoints: true);
                    break;

                case CommandType.Wait:
                    Debug.Log(timeToWait);
                    request = new WaitRequest(CurrentActingUnit, timeToWait, timeToWait);
                    break;

                case CommandType.Overwatch:
                    request = new OverwatchRequest(CurrentActingUnit);
                    break;

                case CommandType.BasicSkill:
                    request = new UseSkillRequest(CurrentActingUnit, CurrentActingUnit.BasicSkillName, MapActionRequest.RequestType.Skill);
                    break;

                case CommandType.UltimateSkill:
                    request = new UseSkillRequest(CurrentActingUnit, CurrentActingUnit.UltimateSkillName, MapActionRequest.RequestType.Skill);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(command), $"SelectCommand does not take the parameter {nameof(command)}");
            }

            //canvasLinearAnimation.ToggleUIWhen(ScriptedAnimations.Instance.AllTasksFinished, characterSheetIndex);

            ParseRequest(request);
            StateReset();
            UnsubscribeAllControls();
        }


        /// <summary>
        /// Applies the information contained by the MapAction to the representation of
        /// the game map shown on screen.
        /// </summary>
        /// <param name="request"></param>
        public void ParseRequest(MapActionRequest request)
        {
            switch (request.Type)
            {
                case MapActionRequest.RequestType.Movement:
                    MovementRequest movementRequest = (MovementRequest)request;

                    EnqueueMovementSequence(movementRequest);
                    break;

                case MapActionRequest.RequestType.Attack:

                    AttackRequest attackRequest = (AttackRequest)request;

                    currentMap = currentMap.DoAction(attackRequest);
                    bool defenderIsAlive = currentMap[currentMap[attackRequest.TargetUnit]].CurrentHealth > 0;
                    routineManager.Enqueue(CombatCoroutineGenerator.EnactAttackRequest(currentMap, attackRequest, defenderIsAlive));

                    break;

                case MapActionRequest.RequestType.Wait:
                    WaitRequest waitRequest = (WaitRequest)request;
                    currentMap = currentMap.DoAction(waitRequest);
                    break;

                case MapActionRequest.RequestType.Overwatch:
                    OverwatchRequest overwatchRequest = (OverwatchRequest)request;
                    currentMap = currentMap.DoAction(overwatchRequest);
                    break;

                case MapActionRequest.RequestType.Skill:
                    UseSkillRequest useSkillRequest = (UseSkillRequest)request;
                    currentMap = currentMap.DoAction(useSkillRequest);
                    break;

                default:
                    Debug.LogError($"There was no action carried out for {request.Type}");
                    break;
            }

            routineManager.Enqueue(LateInvoke(HealthBarManager.Instance.UpdateHealthBars, currentMap.GetUnits(unit => true)));

            routineManager.Enqueue(CombatCoroutineGenerator.DisableDeadUnits(CurrentMap));
            routineManager.Enqueue(LateInvoke(() => currentMap = currentMap.ClearOffDeadUnits()));

            routineManager.Enqueue(LateInvoke(() => gameState = SceneState.TurnEnded));

            CombatUIManager.Instance.OnDisable();

            Debug.Log($"{currentMap.CurrentActingUnit.Name}, {currentMap.CurrentActingUnit.Time}");
        }


        private void EnqueueMovementSequence(MovementRequest movementRequest)
        {
            IEnumerable<Vector3Int> path = movementRequest.CalculateShortestPath(CurrentMap);

            Vector3Int lastPosition = path.First();

            Vector3 lastDirection = Vector3.zero;

            GameObject actingUnitGameObject = unitManager.GetGameObjectOfUnit(movementRequest.ActingUnit);

            Animator animator = actingUnitGameObject.GetComponentInChildren<Animator>();

            foreach (Vector3Int position in path)
            {
                if (position == lastPosition)
                {
                    continue;
                }

                Vector3 direction = Vector3.Normalize((position - lastPosition).Rotate(45));

                GameMapData gameMapData = currentMap.Data.ChangeUnitPosition(CurrentActingUnit, position);

                IEnumerable<AttackRequest> incomingAttacks = CombatConsultant.GetIncomingAttacks(gameMapData, CurrentActingUnit)
                    .Where(x => x.ActingUnit.HasStatusEffect("Overwatch"));


                if (direction != lastDirection || incomingAttacks.Count() > 0)
                {
                    routineManager.Enqueue(
                        CombatCoroutineGenerator.MoveUnitToPosition(actingUnitGameObject, ground.CellToWorld(lastPosition)));

                    routineManager.Enqueue(
                        CombatCoroutineGenerator.PerformAnimation(animator, "isMoving", true, direction.x, direction.y));

                    lastDirection = Vector3.Normalize(direction);
                }

                lastPosition = position;

                if (incomingAttacks.Count() > 0)
                {
                    routineManager.Enqueue(
                        CombatCoroutineGenerator.MoveUnitToPosition(actingUnitGameObject, ground.CellToWorld(position)));

                    routineManager.Enqueue(CombatCoroutineGenerator.PerformAnimation(animator, "isMoving", false));

                    currentMap = currentMap.DoAction(incomingAttacks.First());
                    bool defenderIsAlive = currentMap.GetSimilarUnit(movementRequest.ActingUnit).CurrentHealth > 0;
                    Debug.Log($"After the overwatch attack, defender {CurrentActingUnit} is alive -> {defenderIsAlive} ");

                    routineManager.Enqueue(
                        CombatCoroutineGenerator.EnactAttackRequest(currentMap, incomingAttacks.First(), defenderIsAlive));

                    routineManager.Enqueue(
                        LateInvoke(HealthBarManager.Instance.UpdateHealthBars, currentMap.GetUnits(unit => true)));

                    if (CurrentActingUnit.CurrentHealth <= 0)
                    {
                        return;
                    }

                    routineManager.Enqueue(CombatCoroutineGenerator.PerformAnimation(animator, "isMoving", true));
                }

            }

            routineManager.Enqueue(
                CombatCoroutineGenerator.MoveUnitToPosition(actingUnitGameObject, ground.CellToWorld(lastPosition)));

            currentMap = currentMap.DoAction(movementRequest);

            routineManager.Enqueue(CombatCoroutineGenerator.PerformAnimation(animator, "isMoving", false));
        }

        private void Update()
        {
            if (isPaused)
            {
                return;
            }

            switch (gameState)
            {
                case SceneState.TurnEnded:
                    StateReset();

                    UnitQueueManager.Instance.UpdateUnitQueue(currentMap.GetUnits(unit => true));

                    CameraController.Instance.FocusOn(
                        CellToWorld(CurrentActingUnitPosition) + UNIT_WORLD_BODY_OFFSET,
                        CombatUIManager.FAST_FOCUS_DURATION);

                    if (gameOverAllowed)
                    {
                        if (currentMap.IsLost())
                        {
                            gameState = SceneState.Lost;
                            CombatUIManager.Instance.ShowLoseScreen();
                            Debug.Log("The game is over: Lost");
                            break;
                        }
                        else if (currentMap.IsWon())
                        {
                            gameState = SceneState.Won;
                            CombatUIManager.Instance.ShowWinScreen();
                            Debug.Log("The game is over: Won");
                            break;
                        }
                    }


                    if (currentMap.CurrentActingUnit.Faction != Unit.UnitFaction.Friendly)
                    {
                        gameState = SceneState.OpponentTurn;
                    }
                    else
                    {
                        //CanvasManager.Instance.ActivateUI(CanvasManager.UIType.CharacterSheet);
                        CombatUIManager.Instance.OnEnable();
                        gameState = SceneState.Selection;
                    }
                    
                    break;

                case SceneState.OpponentTurn:
                    //CanvasManager.Instance.DeactivateUI(CanvasManager.UIType.CharacterSheet);
                    Autoplay();
                    gameState = SceneState.OpponentThinking;
                    break;

            }
        }

        private IEnumerator LateInvoke<T>(Action<T> func, T args)
        {
            yield return null;
            func.Invoke(args);
        }

        private IEnumerator LateInvoke(Action func)
        {
            yield return null;
            func.Invoke();
        }

        public async void Autoplay()
        {
            IEnumerator WaitForUnpause(MapActionRequest bestRequest)
            {
                yield return new WaitForEndOfFrame();
                if (isPaused)
                {
                    StartCoroutine(WaitForUnpause(bestRequest));
                }
                else
                {
                    ParseRequest(bestRequest);
                }
            }

            Debug.Log("Please wait");

            await AsyncTask.Delay(1000);
            MapActionRequest bestRequest = await AsyncTask.Run(() =>
            {
                MapActionRequest bestRequest = CurrentMap.GetKthBestAction(0);
                return bestRequest;
            },
            tokenSource.Token
            );

            Debug.Log($"{bestRequest} Utility: {bestRequest.GetUtility(CurrentMap)}");

            StartCoroutine(WaitForUnpause(bestRequest));
        }
    }
}
