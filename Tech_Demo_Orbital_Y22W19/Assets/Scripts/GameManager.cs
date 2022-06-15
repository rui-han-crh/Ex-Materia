using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem;
using System;
using ColorLookUp;
using System.Threading;
using Banzan.Lib.Utility;

// maybe split this class up more, theres like 600 lines here
public partial class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }
            return instance;
        }
    }

    public enum Command
    {
        Movement,
        Attack,
        Wait,
        Overwatch
    }


    public static readonly Vector3 UNIT_WORLD_OFFSET = new Vector3(0, 0.25f, 0);
    public static readonly Vector3 HEALTH_BAR_WORLD_OFFSET = new Vector3(0, 1f, 0);
    public static readonly float UNIT_INTERPOLATION_SPEED = 2f;


    private Tilemap fullCoverTilemap;
    private Tilemap halfCoverTilemap;
    private Tilemap groundTilemap;
    private Tilemap tileHighlights;
    private Tile blockSelectorTile;
    private TileCost[] tileCosts;


    // Routine Queue
    private Queue<IEnumerator> routineQueue = new Queue<IEnumerator>();
    private Task lastRoutine;

    private GameMap currentMap;

    private bool isOverUI;

    private GameState gameState = GameState.Selection;

    private KeyboardControls keyboardControls;
    private Camera mainCamera;

    // Mappers:
    private Dictionary<TileBase, int> tileCostMapping = new Dictionary<TileBase, int>();

    private Dictionary<string, GameObject> nameUnitGameObjectMapping = new Dictionary<string, GameObject>();
    private HashSet<GameObject> markedForDeletion = new HashSet<GameObject>();
    private Dictionary<GameObject, GameObject> unitToHealthbarMapping = new Dictionary<GameObject, GameObject>();

    // ActionHandlers:
    private Action<InputAction.CallbackContext> leftClickHandler;
    private Action<InputAction.CallbackContext> rightClickHandler;

    // Cached variables
    private Vector3Int? currentTileSelected;
    private IEnumerable<Vector3Int> pathPositionsLastDrawn = new List<Vector3Int>();
    private int lastActionCost;
    private bool executeLastActionAllowed;
    private bool autoPlayQueued;

    //Async Token
    private CancellationTokenSource tokenSource;

    public bool IsOverUI => isOverUI;

    public Vector3Int CurrentUnitPosition => currentMap.CurrentUnitPosition;
    public UnitOld CurrentUnit => currentMap.CurrentUnit;

    public GameMap CurrentMap => currentMap;

    public bool IsRoutineEmpty()
    {
        return routineQueue.Count == 0;
    }

    public bool IsRoutineQueueDormant()
    {
        return lastRoutine == null || !lastRoutine.Running;
    }

    private void Awake()
    {
        keyboardControls = new KeyboardControls();

        fullCoverTilemap = tileMapCollection.FullCoverTilemap;
        halfCoverTilemap = tileMapCollection.HalfCoverTilemap;
        groundTilemap = tileMapCollection.GroundTilemap;
        tileHighlights = tileMapCollection.TileHighlights;
        blockSelectorTile = tileMapCollection.BlockSelectorTile;
        tileCosts = tileMapCollection.TileCosts;

        characterStatsUIBehaviour = screenUI.CharacterStatsUIBehaviour;
        canvasLinearAnimation = screenUI.CanvasLinearAnimation;

        pathLine = GetComponent<LineRenderer>();
    }

    private void Start()
    {
        tokenSource = new CancellationTokenSource();

        mainCamera = Camera.main;

        Dictionary<Vector3Int, UnitOld> unitPositionMap = new Dictionary<Vector3Int, UnitOld>();
        for (int i = 0; i < unitGameObjects.Length; i++)
        {
            GameObject unitObject = unitGameObjects[i];

            if (!unitObject.activeSelf)
            {
                continue;
            }

            UnitOld unit = unitObject.GetComponentInChildren<UnitBehaviour>().InitialiseUnit(1 + i * 2);
            Debug.Assert(unit.Name != null);
            Vector3Int unitCellPosition = groundTilemap.WorldToCell(unitObject.transform.position);
            unitPositionMap.Add(unitCellPosition, unit);
            unitObject.transform.position = groundTilemap.CellToWorld(unitCellPosition);
            nameUnitGameObjectMapping.Add(unit.Name, unitObject);
        }

        foreach (TileCost item in tileCosts)
            tileCostMapping.Add(item.tileBase, item.cost);

        List<Vector3Int> fullCoverPositions = new List<Vector3Int>();
        foreach (Vector3Int position in fullCoverTilemap.cellBounds.allPositionsWithin)
        {
            if (fullCoverTilemap.HasTile(position))
            {
                fullCoverPositions.Add(position);
            }
        }

        List<Vector3Int> halfCoverPositions = new List<Vector3Int>();
        foreach (Vector3Int position in halfCoverTilemap.cellBounds.allPositionsWithin)
        {
            if (halfCoverTilemap.HasTile(position))
            {
                halfCoverPositions.Add(position);
            }
        }

        Dictionary<Vector3Int, int> groundTileData = new Dictionary<Vector3Int, int>();
        foreach (Vector3Int position in groundTilemap.cellBounds.allPositionsWithin)
        {
            if (groundTilemap.HasTile(position))
            {
                groundTileData.Add(position, tileCostMapping[groundTilemap.GetTile(position)]); // PLACEHOLDER PLEASE CHANGE
            }
        }

        currentMap = new GameMap(unitPositionMap, fullCoverPositions, halfCoverPositions, groundTileData.Keys, groundTileData);
        Debug.Log($"Initialised: {currentMap}");

        InitialiseHealthBar();
        UnitQueueManager.Instance.InitialiseQueue(unitGameObjects, currentMap.AllUnits.ToDictionary(unit => unit.Name, unit => unit));
        gameState = GameState.TurnEnded;

        LineRaytracer r = new LineRaytracer();
        bool res = r.Trace(groundTilemap.WorldToCell(unitGameObjects[0].transform.position), 
            groundTilemap.WorldToCell(unitGameObjects[1].transform.position), 
            GameMap.UNIT_GRID_OFFSET, 
            currentMap.MapData);
    }

    private void OnEnable()
    {
        keyboardControls.Enable();
    }

    private void OnDisable()
    {
        keyboardControls?.Disable();
        tokenSource.Cancel();
    }

    public void Enqueue(IEnumerator action)
    {
        routineQueue.Enqueue(action);
    }

    public void SubscribeToMovementListener()
    {
        Debug.Log(currentMap);
        keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
        keyboardControls.Mouse.RightClick.performed -= rightClickHandler;

        leftClickHandler = _ => MovementSelectionListener();
        gameState = GameState.AwaitMovement;

        keyboardControls.Mouse.LeftClick.performed += leftClickHandler;
    }

    public void SubscribeToCombatListener()
    {
        keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
        keyboardControls.Mouse.RightClick.performed -= rightClickHandler;

        leftClickHandler = _ => CombatSelectionListener();
        gameState = GameState.AwaitCombat;

        keyboardControls.Mouse.LeftClick.performed += leftClickHandler;
    }

    public void UnsubscribeAllControls()
    {
        keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
        keyboardControls.Mouse.RightClick.performed -= rightClickHandler;
        StateReset();
    }

    public void StateReset()
    {
        currentTileSelected = null;
        tileHighlights.ClearAllTiles();
        pathLine.positionCount = 0;
        pathPositionsLastDrawn = new List<Vector3Int>();
        executeLastActionAllowed = false;
        lastActionCost = 0;
    }

    public void UpdateCharacterSheet()
    {
        characterStatsUIBehaviour.SetUnitStats(currentMap.CurrentUnit);
    }


    private void Update()
    {
        /////////////////////////////////////
        //          Mouse Over UI          //
        /////////////////////////////////////
        isOverUI = EventSystem.current.IsPointerOverGameObject();


        ////////////////////////////////////
        //          State related         //
        ////////////////////////////////////
        switch (gameState)
        {
            case GameState.AwaitMovement:
                StateReset();
                gameState = GameState.Movement;
                TileDrawer.Draw(tileHighlights, currentMap.GetReachableTiles().Keys, blockSelectorTile);
                break;

            case GameState.AwaitCombat:
                StateReset();
                gameState = GameState.Combat;
                TileDrawer.Draw(tileHighlights, currentMap.GetRange(), blockSelectorTile);
                TileDrawer.SetColorToTiles(tileHighlights, currentMap.GetRange(), ColorPalette.YELLOW_TRANSLUCENT);
                break;

            case GameState.Refresh:
                StateReset();
                canvasLinearAnimation.UIToActivePosition(characterSheetIndex);
                gameState = GameState.Selection;
                break;

            case GameState.TurnEnded:
                RemoveDeadUnits();
                RedrawHealthBar();
                UpdateCharacterSheet();
                
                UnitQueueManager.Instance.UpdateUnitQueue(currentMap.AllUnits.ToDictionary(unit => unit.Name, unit => unit));

                InformationUIManager.Instance.SetAllTextToDefault();

                if (currentMap.IsGameOver())
                {
                    gameState = GameState.GameOver;
                }
                else
                {
                    gameState = currentMap.CurrentUnit.Faction == Faction.Friendly ? GameState.Refresh : GameState.OpponentTurn;
                }
                break;

            case GameState.GameOver:
                // temporary
                if (mainLight != null)
                    Destroy(mainLight);
                break;

            default:
                break;
        }

        //////////////////////////////////////
        //          Routine related         //
        //////////////////////////////////////
        if (IsRoutineQueueDormant())
        {
            if (!IsRoutineEmpty())
            {
                lastRoutine = new Task(routineQueue.Dequeue());
            }
            else if (!UnitQueueManager.Instance.IsPlayingAnimation && !autoPlayQueued && gameState == GameState.OpponentTurn)
            {
                AutoPlay();
            }
        }

    }

    private void RepresentMapToScene()
    {
        foreach (Vector3Int position in currentMap.AllUnitPositions)
        {
            UnitOld unit = currentMap.GetUnitByPosition(position);
            nameUnitGameObjectMapping[unit.Name].transform.position = groundTilemap.CellToWorld(position);
        }
    }

    private void InitialiseHealthBar()
    {
        foreach (GameObject unit in unitGameObjects)
        {
            GameObject healthbar = Instantiate(healthBarDataBag.HealthBarPrefab, healthBarDataBag.HealthBarCollection);
            unitToHealthbarMapping[unit] = healthbar;
            healthbar.GetComponent<BarFillBehaviour>().SetParent(unit.transform);
        }
    }

    private void RedrawHealthBar()
    {
        foreach (string name in nameUnitGameObjectMapping.Keys)
        {
            if (!currentMap.AllUnits.Select(x => x.Name).Contains(name))
            {
                GameObject unitGameObject = nameUnitGameObjectMapping[name];
                BarFillBehaviour barFillBehaviour = unitToHealthbarMapping[unitGameObject].GetComponent<BarFillBehaviour>();
                barFillBehaviour.UpdateBarFillImage(0, barFillBehaviour.CurrentMaxHealth);
            }
        }

        foreach (UnitOld unit in currentMap.AllUnits)
        {
            GameObject unitGameObject = nameUnitGameObjectMapping[unit.Name];
            unitToHealthbarMapping[unitGameObject].GetComponent<BarFillBehaviour>().UpdateBarFillImage(unit.Health, unit.MaxHealth);
        }
    }

    private void RemoveDeadUnits()
    {
        IEnumerable<string> differenceUnitNames = nameUnitGameObjectMapping.Keys.ToArray().Except(currentMap.AllUnits.Select(x => x.Name));
        foreach (string differenceUnitName in differenceUnitNames)
        {
            markedForDeletion.Add(nameUnitGameObjectMapping[differenceUnitName]);
            Enqueue(DestroyUnitDelayed(differenceUnitName, 1));
        }
    }

    public void ClearAllHighlights()
    {
        tileHighlights.ClearAllTiles();
        pathLine.positionCount = 0;
    }


    
    [EnumAction(typeof(Command))]
    // Unity will not expose enums in the inspector for event so we have to resort to some weird hacks
    // Read: https://forum.unity.com/threads/ability-to-add-enum-argument-to-button-functions.270817/
    public void SelectCommand(int commandEnumIndex)
    {
        SelectCommand((Command)commandEnumIndex);
    }


    /// <summary>
    /// Performs a command as a request given to the current game map.
    /// </summary>
    /// <param name="command"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SelectCommand(Command command)
    {
        if (!IsRoutineQueueDormant())
        {
            return;
        }

        MapActionRequest request;

        switch (command)
        {
            case Command.Movement:
                if (!executeLastActionAllowed)
                {
                    return;
                }

                request = new MovementRequest(CurrentMap, CurrentUnitPosition, pathPositionsLastDrawn.ToArray(), lastActionCost);
                break;

            case Command.Attack:
                if (!executeLastActionAllowed)
                {
                    return;
                }
                canvasLinearAnimation.UIToDeactivePosition(OPPONENT_UI_INDEX);

                request = new AttackRequest(CurrentMap,
                                            pathPositionsLastDrawn.First(),
                                            pathPositionsLastDrawn.Last(),
                                            AttackStatus.Success,
                                            pathPositionsLastDrawn.ToArray(),
                                            lastActionCost);
                break;

            case Command.Wait:
                request = new WaitRequest(CurrentMap, CurrentUnitPosition, timeToWait);
                break;

            case Command.Overwatch:
                request = new OverwatchRequest(CurrentMap, CurrentUnitPosition);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(command), $"SelectCommand does not take the parameter {nameof(command)}");
        }

        canvasLinearAnimation.ToggleUIWhen(ScriptedAnimations.Instance.AllTasksFinished, characterSheetIndex);

        ParseRequest(request);
    }

    /// <summary>
    /// Sets the appropriate flags and values for the unit's animator to derive the appropriate
    /// animation to play.
    /// </summary>
    /// <param name="unitName"></param>
    /// <param name="booleanFlag"></param>
    /// <param name="state"></param>
    /// <param name="xDirection"></param>
    /// <param name="yDirection"></param>
    public void UnitAnimatorPerform(string unitName, string booleanFlag, bool state, float xDirection, float yDirection)
    {
        GameObject unitGameObject = nameUnitGameObjectMapping[unitName];
        Animator unitAnimator = unitGameObject.GetComponentInChildren<Animator>();
        UnitAnimatorPerform(unitAnimator, booleanFlag, state, xDirection, yDirection);
    }


    public void UnitAnimatorPerform(Animator animator, string booleanFlag, bool state, float xDirection, float yDirection)
    {
        if (animator == null)
        {
            return;
        }

        if (xDirection != 0f || yDirection != 0f)
        {
            animator.SetFloat("xDirection", xDirection);
            animator.SetFloat("yDirection", yDirection);
        }

        animator.SetBool(booleanFlag, state);
    }


    public void UnitAnimatorPerform(Animator animator, string booleanFlag, bool state)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetBool(booleanFlag, state);
    }


    public void UnitAnimatorPerform(string unitName, string booleanFlag, bool state)
    {
        GameObject unitGameObject = nameUnitGameObjectMapping[unitName];
        Animator unitAnimator = unitGameObject.GetComponentInChildren<Animator>();
        UnitAnimatorPerform(unitAnimator, booleanFlag, state);
    }


    /// <summary>
    /// Applies the information contained by the MapAction to the representation of
    /// the game map shown on screen.
    /// </summary>
    /// <param name="request"></param>
    public void ParseRequest(MapActionRequest request)
    {
        int cost;
        switch (request.ActionType)
        {
            case MapActionType.Movement:
                MovementRequest movementRequest = (MovementRequest)request;
                UnitOld movingUnit = request.ActingUnit;

                cost = movementRequest.ActionPointCost;

                List<Vector3Int> pathList = movementRequest.Path.ToList();

                IEnumerable<AttackRequest> positionsAttacked = currentMap.OverwatchersCanAttackAny(movementRequest.Path);
                positionsAttacked.OrderBy(x => Vector3.Magnitude(CurrentUnitPosition - x.TargetPosition));
                int startingIndex = 0;

                foreach (AttackRequest attackRequestByOverwatch in positionsAttacked)
                {
                    if (attackRequestByOverwatch.ActingUnit.Faction == movementRequest.ActingUnit.Faction)
                    {
                        continue;
                    }

                    int endingIndex = pathList.IndexOf(attackRequestByOverwatch.TargetPosition);

                    ////////////////////// Move current unit to a segment of the path ////////////////////
                    Enqueue(LerpGameObjectByUnitID(CurrentUnit.Name,
                        pathList.GetRange(startingIndex, endingIndex - startingIndex + 1), 0));


                    ///////////////////////////// Overwatching unit attacks /////////////////////////////
                    Enqueue(MoveToPositionAndAttack(attackRequestByOverwatch));

                    startingIndex = endingIndex;
                }

                Enqueue(LerpGameObjectByUnitID(CurrentUnit.Name,
                    pathList.GetRange(startingIndex, pathList.Count - startingIndex), cost));

                break;

            case MapActionType.Attack:
                AttackRequest attackRequest = (AttackRequest)request;
                cost = attackRequest.ActionPointCost;

                Vector3Int initialUnitPosition = request.ActingUnitPosition;
                Enqueue(MoveToPositionAndAttack(attackRequest));
                break;

            case MapActionType.Wait:
                WaitRequest waitRequest = (WaitRequest)request;
                routineQueue.Enqueue(CurrentUnitRecoverAP(waitRequest));
                break;

            case MapActionType.Overwatch:
                OverwatchRequest overwatchRequest = (OverwatchRequest)request;
                routineQueue.Enqueue(CurrentUnitOverwatch(overwatchRequest));
                break;

            default:
                break;
        }
    }
}
