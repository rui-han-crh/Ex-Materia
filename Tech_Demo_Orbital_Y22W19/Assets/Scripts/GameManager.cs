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

public partial class GameManager : MonoBehaviour
{
    public static Vector3 UNIT_WORLD_OFFSET = new Vector3(0, 0.25f, 0);

    [SerializeField]
    private float interpolationSpeed = 2.5f;
    [SerializeField]
    private GameObject[] unitGameObjects;

    [SerializeField]
    protected Tilemap fullCoverTilemap;
    [SerializeField]
    private Tilemap halfCoverTilemap;
    [SerializeField]
    private Tilemap groundTilemap;
    [SerializeField]
    private Tilemap tileHighlights;

    [SerializeField]
    private Tile tileHighlightIndicator;

    // no property drawers :(
    [SerializeField]
    private TileCost[] tileCosts;
    // END

    private Dictionary<TileBase, int> tileCostMapping = new Dictionary<TileBase, int>();

    private KeyboardControls keyboardControls;
    private Camera mainCamera;

    private int turnsElapsed = 0;

    private bool endTurnPressed = false;

    private Dictionary<string, GameObject> nameUnitGameObjectMapping = new Dictionary<string, GameObject>();

    [SerializeField]
    private GameObject queueDisplay;

    [SerializeField]
    private TMP_Text actionPointsLeftField;

    [SerializeField]
    private GameObject healthBarPrefab;

    [SerializeField]
    private RectTransform healthBarCollection;

    [SerializeField]
    private LineRenderer pathLine;

    private UnitQueueManager queueManager;

    private Queue<IEnumerator> routineQueue = new Queue<IEnumerator>();
    private Task lastRoutine;

    protected GameMap currentMap;

    protected bool isOverUI;

    private GameState gameState = GameState.Selection;

    // ActionHandlers:
    private Action<InputAction.CallbackContext> leftClickHandler;
    private Action<InputAction.CallbackContext> rightClickHandler;

    // Cached variables
    private Vector3Int? currentTileSelected;
    private IEnumerable<Vector3Int> pathPositionsLastDrawn = new List<Vector3Int>();
    private bool executeLastActionAllowed;
    private bool autoPlayQueued;

    //Async Token
    private CancellationTokenSource tokenSource;

    public bool IsOverUI => isOverUI;

    public Vector3Int CurrentUnitPosition => currentMap.CurrentUnitPosition;
    public Unit CurrentUnit => currentMap.CurrentUnit;

    public GameMap CurrentMap => currentMap;

    public bool IsRoutineEmpty()
    {
        return routineQueue.Count == 0;
    }
    private void Awake()
    {
        keyboardControls = new KeyboardControls();
    }

    private void Start()
    {
        tokenSource = new CancellationTokenSource();

        mainCamera = Camera.main;

        Dictionary<Vector3Int, Unit> unitPositionMap = new Dictionary<Vector3Int, Unit>();
        foreach (GameObject unitObject in unitGameObjects)
        {
            if (!unitObject.activeSelf)
            {
                continue;
            }

            Unit unit = unitObject.GetComponentInChildren<UnitBehaviour>().Unit;
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

        MapActionRequest testRequest = new MovementRequest(currentMap, Vector3Int.zero, Vector3Int.zero, 0);
        MapActionRequest testARequest = new AttackRequest(currentMap, Vector3Int.zero, Vector3Int.zero, AttackStatus.IllegalTarget, new Vector3Int[0], 0);

        List<MapActionRequest> actions = new List<MapActionRequest>() { testARequest, testRequest };
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

    public void StateReset()
    {
        currentTileSelected = null;
        tileHighlights.ClearAllTiles();
        pathLine.positionCount = 0;
        pathPositionsLastDrawn = new List<Vector3Int>();
        executeLastActionAllowed = false;
    }

    private void MovementSelectionListener()
    {
        if (isOverUI)
            return;
        
        Vector2 mousePosition = keyboardControls.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3Int gridPosition = groundTilemap.WorldToCell(mousePosition);

        TileDrawer.SetColorToTiles(tileHighlights, pathPositionsLastDrawn, tileHighlightIndicator.color);

        if (groundTilemap.HasTile(gridPosition))
        {
            currentTileSelected = gridPosition;
            currentMap.FindShortestPathTo(gridPosition, out IEnumerable<Vector3Int> pathPositions);
            pathPositionsLastDrawn = pathPositions;
            TileDrawer.SetColorToTiles(tileHighlights, pathPositions, ColorPalette.LIGHT_BLUE_TRANSLUCENT);
            LineDrawer.DrawLineOnTileMap(tileHighlights, pathLine, pathPositions, currentMap.CurrentUnitPosition);
            LineDrawer.ColorLine(pathLine, Color.blue);
            executeLastActionAllowed = true;
        }
    }

    private void CombatSelectionListener()
    {
        if (isOverUI)
            return;

        Vector2 mousePosition = keyboardControls.Mouse.MousePosition.ReadValue<Vector2>();
        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Vector3Int gridPosition = groundTilemap.WorldToCell(mousePosition);

        TileDrawer.SetColorToTiles(tileHighlights, pathPositionsLastDrawn, ColorPalette.YELLOW_TRANSLUCENT);
        AttackRequest request = currentMap.IsAttackableAt(gridPosition);
        AttackStatus status = request.Status;

        switch (status)
        {
            case AttackStatus.Success:
                pathPositionsLastDrawn = request.TilesHit;
                TileDrawer.SetColorToTiles(tileHighlights, request.TilesHit, ColorPalette.LIGHT_GREEN_TRANSLUCENT);
                LineDrawer.DrawLineOnTileMap(groundTilemap, pathLine, new Vector3Int[] { request.TilesHit.First(), request.TilesHit.Last() });
                LineDrawer.ColorLine(pathLine, ColorPalette.DARK_GREEN);
                executeLastActionAllowed = true;
                break;

            case AttackStatus.IllegalTarget:
                StateReset();
                break;

            default:
                pathPositionsLastDrawn = request.TilesHit;
                TileDrawer.SetColorToTiles(tileHighlights, request.TilesHit, ColorPalette.LIGHT_RED_TRANSLUCENT);
                LineDrawer.DrawLineOnTileMap(groundTilemap, pathLine, new Vector3Int[] { request.TilesHit.First(), request.TilesHit.Last() });
                LineDrawer.ColorLine(pathLine, Color.red);
                executeLastActionAllowed = false;
                break;
        }
    }

    private void Update()
    {
        if ((lastRoutine == null || !lastRoutine.Running) && routineQueue.Count > 0)
        {
            lastRoutine = new Task(routineQueue.Dequeue());
        }

        if (routineQueue.Count == 0 && (lastRoutine == null || !lastRoutine.Running) 
            && !autoPlayQueued && currentMap.CurrentUnit.Faction != Faction.Friendly)
        {
            Debug.Log($"The map now is {currentMap}");
            autoPlayQueued = true;
            AutoPlay();
        }

        isOverUI = EventSystem.current.IsPointerOverGameObject();

        switch (gameState)
        {
            case GameState.AwaitMovement:
                StateReset();
                gameState = GameState.Movement;
                TileDrawer.Draw(tileHighlights, currentMap.GetReachableTiles().Keys, tileHighlightIndicator);
                break;

            case GameState.AwaitCombat:
                StateReset();
                gameState = GameState.Combat;
                TileDrawer.Draw(tileHighlights, currentMap.GetRange(), tileHighlightIndicator);
                TileDrawer.SetColorToTiles(tileHighlights, currentMap.GetRange(), ColorPalette.YELLOW_TRANSLUCENT);
                break;

            case GameState.Combat:
                break;

            case GameState.Selection:
                StateReset();
                break;
        }
        
    }

    public void UpdateActionPointsText()
    {
        actionPointsLeftField.text = queueManager.GetCurrentUnit().ActionPointsLeft.ToString();
    }

    private void RepresentMapToScene()
    {
        foreach (Vector3Int position in currentMap.AllUnitPositions)
        {
            Unit unit = currentMap.GetUnitByPosition(position);
            nameUnitGameObjectMapping[unit.Name].transform.position = groundTilemap.CellToWorld(position);
        }
    }

    public void ClearAllHighlights()
    {
        tileHighlights.ClearAllTiles();
        pathLine.positionCount = 0;
    }

    public void OnClickMoveUnit()
    {
        if (routineQueue.Count == 0 && executeLastActionAllowed)
        {
            int cost = currentMap.FindShortestPathTo(currentTileSelected.Value, out IEnumerable<Vector3Int> path);
            Enqueue(LerpGameObject(nameUnitGameObjectMapping[currentMap.CurrentUnit.Name], path, cost));
        }
    }

    public void OnClickAttackUnit()
    {
        if (routineQueue.Count == 0 && executeLastActionAllowed)
        {
            int cost = UnitCombat.ATTACK_COST;
            Vector3Int initialUnitPosition = currentMap.CurrentUnitPosition;

            routineQueue.Enqueue(LerpGameObject(nameUnitGameObjectMapping[currentMap.CurrentUnit.Name],
                                    new Vector3Int[] { pathPositionsLastDrawn.First() }, 0, false));

            routineQueue.Enqueue(DoAttackAction(pathPositionsLastDrawn.Last(), pathPositionsLastDrawn.ToArray(), cost));

            routineQueue.Enqueue(LerpGameObject(nameUnitGameObjectMapping[currentMap.CurrentUnit.Name],
                                    new Vector3Int[] { initialUnitPosition }, 0, false));
        }
    }

    public void OnClickWait()
    {
        executeLastActionAllowed = true;
        if (routineQueue.Count == 0 && executeLastActionAllowed)
        {
            WaitRequest waitRequest = new WaitRequest(CurrentMap, CurrentUnitPosition);
            routineQueue.Enqueue(CurrentUnitRecoverAP(waitRequest));
        }
    }

    public void OnClickEndTurn()
    {
        //prepares the EndTurn Coroutine

        if (!endTurnPressed)
        {
            endTurnPressed = true;
            routineQueue.Enqueue(EndTurnRoutine());
        }
        else
        {
            Debug.LogError("End turn was already pressed");
        }
    }

    // All queueable routines must be suffixed with ...Routine

    private IEnumerator EndTurnRoutine()
    {
        yield break;
        //unitMovement.ClearSelectableTiles();

        pathLine.positionCount = 0;
        // queue update does not depend on current unit position -> It is safe to update while unit is moving
        // Just in case, only updates queue once unit has stopped
        
        //while (unitIsMoving)
        //{
        //    yield return null;
        //}

        queueManager.GetCurrentUnit().ResetActionPoints();

        queueManager.UpdateQueue();

        //unitSelected = queueManager.GetCurrentUnit();
        UpdateQueueDisplay();
        UpdateActionPointsText();

        //if (unitSelected.Faction != Faction.Friendly)
        //{
        //    endTurnPressed = false;
        //    routineQueue.Enqueue(adversary.DecisionRoutine());
        //}
        //else
        //{
        //    EnterMovementPhase();
        //    endTurnPressed = false;
        //}
        //CheckUnitAndPositionsMatch();
        yield return null;
    }

    private void UpdateQueueDisplay()
    {
        foreach (Transform headAvatarHolder in queueDisplay.transform)
        {
            Unit unit = headAvatarHolder.GetComponent<CharacterHeadAvatarBehaviour>().Unit;
            headAvatarHolder.SetSiblingIndex(queueManager.GetUnitIndex(unit));
        }
    }
}
