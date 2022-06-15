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
            GameOver
        }

        private SceneState gameState;

        public SceneState GameState => gameState;

        public static readonly Vector3 UNIT_WORLD_OFFSET = new Vector3(0, 0.25f, 0);
        public static readonly Vector3 HEALTH_BAR_WORLD_OFFSET = new Vector3(0, 1f, 0);
        public static readonly float UNIT_INTERPOLATION_SPEED = 2f;

        [SerializeField]
        private UnitManager unitManager;

        [SerializeField]
        private TileMapFacade tileManager;

        [SerializeField]
        private Tilemap ground;

        private RoutineManager routineManager = new RoutineManager();

        private GameMap currentMap;

        public GameMap CurrentMap => currentMap;


        private KeyboardControls keyboardControls;
        private Camera mainCamera;

        // ActionHandlers:
        private Action<InputAction.CallbackContext> leftClickHandler;
        private Action<InputAction.CallbackContext> rightClickHandler;

        private CancellationTokenSource tokenSource;

        public Vector3Int CurrentActingUnitPosition => currentMap[currentMap.CurrentActingUnit];

        public Unit CurrentActingUnit => currentMap.CurrentActingUnit;

        private List<Vector3Int> indicatedTiles = new List<Vector3Int>();
        private int lastActionCost;
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
            tokenSource.Cancel();
        }

        private void Awake()
        {
            Unit.ResetClass();
            keyboardControls = new KeyboardControls();
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

        public void BeginCombat()
        {
            SubscribeToCombatListener();

        }

        public void BeginMovement()
        {
            SubscribeToMovementListener();

        }

        private void SubscribeToMovementListener()
        {
            Debug.Log(currentMap);
            keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
            keyboardControls.Mouse.RightClick.performed -= rightClickHandler;

            //leftClickHandler = _ => MovementSelectionListener();
            gameState = SceneState.MovementMode;

            keyboardControls.Mouse.LeftClick.performed += leftClickHandler;
        }

        private void SubscribeToCombatListener()
        {
            keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
            keyboardControls.Mouse.RightClick.performed -= rightClickHandler;

            //leftClickHandler = _ => CombatSelectionListener();
            gameState = SceneState.CombatMode;

            keyboardControls.Mouse.LeftClick.performed += leftClickHandler;
        }

        private void UnsubscribeAllControls()
        {
            keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
            keyboardControls.Mouse.RightClick.performed -= rightClickHandler;
            StateReset();
        }

        public void StateReset()
        {
        }

        public void DisableDeadUnits()
        {
            IEnumerable<Unit> deadUnits = currentMap.GetUnits(x => x.CurrentHealth <= 0);
            foreach (Unit unit in deadUnits)
            {
                unitManager.RemoveUnit(unit, delay: 1);
            }
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


                    request = new MovementRequest(
                        CurrentActingUnit, 
                        indicatedTiles.Last(), 
                        lastActionCost, 
                        lastActionCost, 
                        MovementRequest.Outcome.Pending);

                    break;

                case CommandType.Attack:
                    if (!executeLastActionAllowed)
                    {
                        return;
                    }

                    //canvasLinearAnimation.UIToDeactivePosition(OPPONENT_UI_INDEX);

                    request = AttackRequest.CreatePendingRequest(
                        CurrentActingUnit,
                        CurrentMap[indicatedTiles.Last()],
                        indicatedTiles.ToArray(),
                        lastActionCost,
                        lastActionCost);
                    break;

                case CommandType.Wait:
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
        }


        /// <summary>
        /// Applies the information contained by the MapAction to the representation of
        /// the game map shown on screen.
        /// </summary>
        /// <param name="request"></param>
        public void ParseRequest(MapActionRequest request)
        {
            int cost;
            switch (request.Type)
            {
                case MapActionRequest.RequestType.Movement:
                    MovementRequest movementRequest = (MovementRequest)request;

                    IEnumerable<Vector3Int> path = movementRequest.CalculateShortestPath(CurrentMap);

                    foreach (Vector3Int position in path)
                    {
                        routineManager.Enqueue(
                            CombatCoroutineGenerator.MoveUnitToPosition(
                                unitManager.GetGameObjectOfUnit(CurrentActingUnit), ground.CellToWorld(position)
                                )
                            );

                        GameMapData gameMapData = currentMap.Data.ChangeUnitPosition(CurrentActingUnit, position);

                        IEnumerable<AttackRequest> incomingAttacks = CombatConsultant.GetIncomingAttacks(gameMapData, CurrentActingUnit);
                        if (incomingAttacks.Count() == 0)
                        {
                            continue;
                        }

                        routineManager.Enqueue(CombatCoroutineGenerator.EnactAttackRequest(currentMap, incomingAttacks.First()));
                    }

                    break;

                case MapActionRequest.RequestType.Attack:
                    AttackRequest attackRequest = (AttackRequest)request;
                    cost = attackRequest.ActionPointCost;

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
                    break;
            }
        }
    }
}
