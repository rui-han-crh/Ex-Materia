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
            Overwatch
        }

        public enum SceneState
        {
            Selection,
            CombatMode,
            MovementMode,
            OpponentTurn,
            TurnEnded,
            GameOver,
            OpponentThinking
        }

        private SceneState gameState;

        public SceneState GameState => gameState;

        public static readonly Vector3 UNIT_WORLD_GROUND_OFFSET = new Vector3(0, 0.25f, 0);
        public static readonly Vector3 UNIT_WORLD_BODY_OFFSET = new Vector3(0, 0.5f, 0);
        public static readonly Vector3 HEALTH_BAR_WORLD_OFFSET = new Vector3(0, 0.75f, 0);

        public static readonly float UNIT_INTERPOLATION_SPEED = 2f;

        [SerializeField]
        private UnitManager unitManager;

        [SerializeField]
        private TileMapFacade tileManager;

        [SerializeField]
        private Tilemap ground;

        private RoutineManager routineManager;

        private GameMap currentMap;

        public GameMap CurrentMap => currentMap;


        private KeyboardControls keyboardControls;
        private Camera mainCamera;

        // ActionHandlers:
        private Action<InputAction.CallbackContext> leftClickHandler = _ => { };
        private Action<InputAction.CallbackContext> doubleLeftClickHandler = _ => { };

        private Action<InputAction.CallbackContext> rightClickHandler = _ => { };

        private Action<InputAction.CallbackContext> middleHoldHandler = _ => { };

        private CancellationTokenSource tokenSource;

        public Vector3Int CurrentActingUnitPosition => currentMap[currentMap.CurrentActingUnit];

        private bool isPointerOverUI;

        public bool IsPointerOverUI => isPointerOverUI;

        public Unit CurrentActingUnit => currentMap.CurrentActingUnit;

        private List<Vector3Int> indicatedTiles = new List<Vector3Int>();
        private int savedActionCost;
        private bool executeLastActionAllowed;
        private bool autoPlayQueued;
        private int timeToWait;

        private void OnEnable()
        {
            keyboardControls.Enable();
        }

        private void OnDisable()
        {
            keyboardControls?.Disable();
            tokenSource?.Cancel();
        }

        private void Awake()
        {
            Unit.ResetClass();
            keyboardControls = new KeyboardControls();
            routineManager = gameObject.AddComponent<RoutineManager>();
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
            GlobalResourceManager.Instance.LineRenderer.positionCount = 0;
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
            if (!routineManager.IsDormant())
            {
                return;
            }

            MapActionRequest request;

            switch (command)
            {
                case CommandType.Movement:
                    if (!executeLastActionAllowed)
                    {
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

                default:
                    throw new ArgumentOutOfRangeException(nameof(command), $"SelectCommand does not take the parameter {nameof(command)}");
            }

            //canvasLinearAnimation.ToggleUIWhen(ScriptedAnimations.Instance.AllTasksFinished, characterSheetIndex);

            ParseRequest(request);
            StateReset();
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
                    routineManager.Enqueue(CombatCoroutineGenerator.EnactAttackRequest(CurrentMap, attackRequest));
                    break;

                case MapActionRequest.RequestType.Wait:
                    WaitRequest waitRequest = (WaitRequest)request;
                    currentMap = currentMap.DoAction(waitRequest);
                    break;

                case MapActionRequest.RequestType.Overwatch:
                    OverwatchRequest overwatchRequest = (OverwatchRequest)request;
                    currentMap = currentMap.DoAction(overwatchRequest);
                    break;

                default:
                    Debug.LogError($"There was no action carried out for {request.Type}");
                    break;
            }

            routineManager.Enqueue(LateInvoke(HealthBarManager.Instance.UpdateHealthBars, currentMap.GetUnits(unit => true)));
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
                    .Where(x => x.ActingUnit.HasStatusEffect(CombatSystem.Entities.UnitStatusEffects.Overwatch));


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

                    routineManager.Enqueue(CombatCoroutineGenerator.EnactAttackRequest(currentMap, incomingAttacks.First()));

                    currentMap = currentMap.DoAction(incomingAttacks.First());

                    routineManager.Enqueue(
                        LateInvoke(HealthBarManager.Instance.UpdateHealthBars, currentMap.GetUnits(unit => true)));

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
            isPointerOverUI = EventSystem.current.IsPointerOverGameObject();

            switch (gameState)
            {
                case SceneState.TurnEnded:
                    StateReset();
                    routineManager.Enqueue(CombatCoroutineGenerator.DisableDeadUnits(CurrentMap));

                    UnitQueueManager.Instance.UpdateUnitQueue(currentMap.GetUnits(unit => true));

                    CameraController.Instance.FocusOn(
                        CellToWorld(CurrentActingUnitPosition) + UNIT_WORLD_BODY_OFFSET,
                        CombatUIManager.FAST_FOCUS_DURATION);


                    if (currentMap.CurrentActingUnit == null)
                    {
                        gameState = SceneState.GameOver;
                        break;
                    }


                    if (currentMap.CurrentActingUnit.Faction == Unit.UnitFaction.Enemy)
                    {
                        gameState = SceneState.OpponentTurn;
                    }
                    else
                    {
                        CanvasManager.Instance.ActivateUI(CanvasManager.UIType.CharacterSheet);
                        CombatUIManager.Instance.OnEnable();
                        gameState = SceneState.Selection;
                    }
                    
                    break;

                case SceneState.OpponentTurn:
                    CanvasManager.Instance.DeactivateUI(CanvasManager.UIType.CharacterSheet);
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


            ParseRequest(bestRequest);
        }
    }
}
