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


    public static Vector3 UNIT_WORLD_OFFSET = new Vector3(0, 0.25f, 0);
    public static Vector3 HEALTH_BAR_WORLD_OFFSET = new Vector3(0, 1f, 0);

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

    private Dictionary<string, GameObject> nameUnitGameObjectMapping = new Dictionary<string, GameObject>();
    private Dictionary<GameObject, GameObject> unitToHealthbarMapping = new Dictionary<GameObject, GameObject>();

    [SerializeField]
    private CharacterSheetBehaviour characterSheetBehaviour;

    [SerializeField]
    private GameObject healthBarPrefab;

    [SerializeField]
    private RectTransform healthBarCollection;

    [SerializeField]
    private LineRenderer pathLine;

    [SerializeField]
    private UnitQueueManager queueManager;

    [SerializeField]
    private LinearAnimation canvasLinearAnimation;
    [SerializeField]
    private int characterSheetIndex;

    // Routine Queue
    private Queue<IEnumerator> routineQueue = new Queue<IEnumerator>();
    private Task lastRoutine;

    private GameMap currentMap;

    private bool isOverUI;

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

    public bool IsRoutineQueueDormant()
    {
        return lastRoutine == null || !lastRoutine.Running;
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
        for (int i = 0; i < unitGameObjects.Length; i++)
        {
            GameObject unitObject = unitGameObjects[i];

            if (!unitObject.activeSelf)
            {
                continue;
            }

            Unit unit = unitObject.GetComponentInChildren<UnitBehaviour>().InitialiseUnit(i);
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
        queueManager.InitialiseQueue(unitGameObjects, currentMap.AllUnits.ToDictionary(unit => unit.Name, unit => unit));
        gameState = GameState.TurnEnded;
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
    }

    public void UpdateCharacterSheet()
    {
        characterSheetBehaviour.SetCurrentUnitShowing(currentMap.CurrentUnit);
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
                TileDrawer.Draw(tileHighlights, currentMap.GetReachableTiles().Keys, tileHighlightIndicator);
                break;

            case GameState.AwaitCombat:
                StateReset();
                gameState = GameState.Combat;
                TileDrawer.Draw(tileHighlights, currentMap.GetRange(), tileHighlightIndicator);
                TileDrawer.SetColorToTiles(tileHighlights, currentMap.GetRange(), ColorPalette.YELLOW_TRANSLUCENT);
                break;

            case GameState.Refresh:
                StateReset();
                canvasLinearAnimation.UIToActivePosition(characterSheetIndex);
                gameState = GameState.Selection;
                break;

            case GameState.TurnEnded:
                RedrawHealthBar();
                UpdateCharacterSheet();
                queueManager.UpdateUnitQueue(currentMap.AllUnits.ToDictionary(unit => unit.Name, unit => unit));
                Debug.Log(currentMap);
                gameState = currentMap.CurrentUnit.Faction == Faction.Friendly ? GameState.Refresh : GameState.OpponentTurn;
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
            else if (!queueManager.IsPlayingAnimation && !autoPlayQueued && gameState == GameState.OpponentTurn)
            {
                AutoPlay();
            }
        }

    }

    private void RepresentMapToScene()
    {
        foreach (Vector3Int position in currentMap.AllUnitPositions)
        {
            Unit unit = currentMap.GetUnitByPosition(position);
            nameUnitGameObjectMapping[unit.Name].transform.position = groundTilemap.CellToWorld(position);
        }
    }

    private void InitialiseHealthBar()
    {
        foreach (GameObject unit in unitGameObjects)
        {
            GameObject healthbar = Instantiate(healthBarPrefab, healthBarCollection);
            unitToHealthbarMapping[unit] = healthbar;
            healthbar.GetComponent<BarFillBehaviour>().SetParent(unit.transform);
        }
    }

    private void RedrawHealthBar()
    {
        foreach (Unit unit in currentMap.AllUnits)
        {
            GameObject unitGameObject = nameUnitGameObjectMapping[unit.Name];
            unitToHealthbarMapping[unitGameObject].GetComponent<BarFillBehaviour>().UpdateBarFillImage(unit.Health, unit.MaxHealth);
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
        Debug.Log("Wait pressed");
        if (routineQueue.Count == 0)
        {
            WaitRequest waitRequest = new WaitRequest(CurrentMap, CurrentUnitPosition);
            routineQueue.Enqueue(CurrentUnitRecoverAP(waitRequest));
        }
    }
}
