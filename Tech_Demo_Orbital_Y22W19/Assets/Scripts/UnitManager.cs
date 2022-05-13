using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class UnitManager : MonoBehaviour
{
    public static Vector3 UNIT_GRID_OFFSET = Vector2.one / 2;
    public static Vector3 UNIT_WORLD_OFFSET = new Vector3(0, 0.25f, 0);

    private KeyboardControls keyboardControls;
    private Camera mainCamera;

    private Dictionary<Vector3Int, Unit> positionUnitMap = new Dictionary<Vector3Int, Unit>();
    private Dictionary<Unit, Vector3Int> unitPositionMap = new Dictionary<Unit, Vector3Int>();

    [SerializeField]
    private float unitSpeed = 2.5f;

    private bool unitIsMoving = false;
    private bool endTurnPressed = false;

    [SerializeField]
    private GameObject[] unitGameObjects;
    private Unit[] unitArray;

    [SerializeField]
    private Unit unitSelected;

    private TileManager tileManager;
    private Tilemap tileMap;

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

    private UnitMovement unitMovement;
    private UnitCombat unitCombat;
    private Adversary adversary;

    private bool isOverUI;

    public Unit SelectedUnit => unitSelected;
    public Dictionary<Vector3Int, Unit> PositionsOfUnits => positionUnitMap;

    public bool IsOverUI => isOverUI;
    public bool RoutineIsEmpty => routineQueue.Count == 0;

    private void Awake()
    {
        keyboardControls = new KeyboardControls();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        keyboardControls.Enable();
    }

    private void OnDisable()
    {
        keyboardControls?.Disable();
    }

    public void Enqueue(IEnumerator action)
    {
        routineQueue.Enqueue(action);
    }

    /// <summary>
    /// Called only once by Unity Engine
    /// </summary>
    private void Start()
    {
        // Assign the tileManager for map retrieval
        tileManager = GetComponent<TileManager>();
        tileMap = tileManager.Ground;

        // Assign the units
        unitArray = new Unit[unitGameObjects.Length];
        for (int i = 0; i < unitGameObjects.Length; i++)
        {
            GameObject unitGO = unitGameObjects[i];
            UnitBehaviour unitBehaviour = unitGO.GetComponent<UnitBehaviour>();
            unitBehaviour.AssignHealthBarElement(Instantiate(healthBarPrefab, healthBarCollection));

            unitArray[i] = unitBehaviour.Unit;

            Vector3Int gridPosition = tileMap.WorldToCell(unitGO.transform.position);
            unitGO.transform.position = tileMap.CellToWorld(gridPosition);
            positionUnitMap[gridPosition] = unitArray[i];
            unitPositionMap[unitArray[i]] = gridPosition;
        }

        // Allocate the queueManager
        queueManager = new UnitQueueManager(unitArray);
        foreach (Unit unit in queueManager.AllUnits)
        {
            GameObject headAvatarHolder = new GameObject(unit + "_headAvatar");

            Image headAvatarImage = headAvatarHolder.AddComponent<Image>();
            headAvatarImage.sprite = unit.CharacterHeadAvatar;
            headAvatarImage.color = unit.behaviour.GetComponentInChildren<SpriteRenderer>().color;

            headAvatarHolder.AddComponent<CharacterHeadAvatarBehaviour>().Unit = unit;
            headAvatarHolder.transform.SetParent(queueDisplay.transform, false);
        }

        unitSelected = queueManager.GetCurrentUnit();

        UpdateActionPointsText();

        // Auxillary class creation
        unitMovement = new UnitMovement(keyboardControls, this, pathLine, tileManager, unitSpeed);
        unitCombat = new UnitCombat(keyboardControls, this, pathLine, tileManager, unitMovement);
        adversary = new Adversary(this, unitMovement, unitCombat, tileManager);

        // Draw selectable tiles
        unitMovement.DrawSelectableTiles();
        EnterMovementPhase();
    }


    private void Update()
    {
        if ((lastRoutine == null || !lastRoutine.Running) && routineQueue.Count > 0)
        {
            lastRoutine = new Task(routineQueue.Dequeue());
        }

        isOverUI = EventSystem.current.IsPointerOverGameObject();
    }

    public void UpdateActionPointsText()
    {
        actionPointsLeftField.text = queueManager?.GetCurrentUnit()?.ActionPointsLeft.ToString();
    }

    public Unit GetUnitByPosition(Vector3Int gridPosition)
    {
        return positionUnitMap[gridPosition];
    }

    public Vector3Int GetPositionByUnit(Unit unit)
    {
        return unitPositionMap[unit];
    }

    public void UpdateUnitPosition(Unit unit, Vector3Int position)
    {
        positionUnitMap.Remove(GetPositionByUnit(unit));
        unitPositionMap.Remove(unit);

        unitPositionMap[unit] = position;
        positionUnitMap[position] = unit;
    }

    private void EnterMovementPhase()
    {
        unitCombat.UnsubscribeControls();
        unitCombat.ExitCombatPhase();
        unitMovement.EnterMovementPhase();
        unitMovement.SubscribeControls();
    }

    private void EnterCombatPhase()
    {
        unitMovement.UnsubscribeControls();
        unitMovement.ExitMovementPhase();
        unitCombat.EnterCombatPhase();
        unitCombat.SubscribeControls();
    }


    private void CheckUnitAndPositionsMatch()
    {
        Debug.Assert(positionUnitMap.Count == unitPositionMap.Count
            && positionUnitMap.Count == unitArray.Length
            && unitPositionMap.Count == unitArray.Length, "Length different");

        foreach (Vector3Int position in positionUnitMap.Keys)
        {
            Unit unit = positionUnitMap[position];
            Debug.Assert(unit != null, $"{unit} cannot be found in positionUnitMap");
            Debug.Assert(position == unitPositionMap[positionUnitMap[position]], $"position of the {unit} is different");
        }

        string unitPos = "";
        string posUnit = "";

        foreach (Unit unit in unitPositionMap.Keys)
        {
            unitPos += unit + ": " + unitPositionMap[unit] + ",  ";
        }

        foreach (Vector3Int pos in positionUnitMap.Keys)
        {
            posUnit += pos + ": " + positionUnitMap[pos] + ",  ";
        }
        Debug.Log(unitPos);
        Debug.Log(posUnit);
    }

    // All button tied events must be prefixed with OnClick...

    public void OnClickEnterMovementPhase()
    {
        if (routineQueue.Count == 0)
            routineQueue.Enqueue(EnterMovementPhaseRoutine());
    }

    public void OnClickEnterCombatPhase()
    {
        if (routineQueue.Count == 0)
            routineQueue.Enqueue(EnterCombatPhaseRoutine());
    }

    public void OnClickMoveUnit()
    {
        unitMovement.MoveUnit();
        routineQueue.Enqueue(EnterMovementPhaseRoutine());
    }

    public void OnClickAttackUnit()
    {
        unitCombat.AttackTargetedUnit();
        routineQueue.Enqueue(EnterCombatPhaseRoutine());
    }

    public void OnClickEndTurn()
    {
        //prepares the EndTurn Coroutine

        if (!endTurnPressed)
        {
            endTurnPressed = true;
            routineQueue.Enqueue(EndTurnRoutine());
        } else
        {
            Debug.LogError("End turn was already pressed");
        }
    }

    // All queueable routines must be suffixed with ...Routine

    private IEnumerator EnterMovementPhaseRoutine()
    {
        EnterMovementPhase();
        yield return null;
    }

    private IEnumerator EnterCombatPhaseRoutine()
    {
        EnterCombatPhase();
        yield return null;
    }

    private IEnumerator EndTurnRoutine()
    {
        unitMovement.ClearSelectableTiles();

        pathLine.positionCount = 0;
        // queue update does not depend on current unit position -> It is safe to update while unit is moving
        // Just in case, only updates queue once unit has stopped
        while (unitIsMoving)
        {
            yield return null;
        }

        queueManager.GetCurrentUnit().ResetActionPoints();

        queueManager.UpdateQueue();

        unitSelected = queueManager.GetCurrentUnit();
        UpdateQueueDisplay();
        UpdateActionPointsText();

        if (unitSelected.Faction != Faction.Friendly)
        {
            endTurnPressed = false;
            routineQueue.Enqueue(adversary.DecisionRoutine());
        }
        else
        {
            EnterMovementPhase();
            endTurnPressed = false;
        }
        CheckUnitAndPositionsMatch();
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
