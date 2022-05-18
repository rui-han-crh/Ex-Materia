//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.Tilemaps;
//using static ColorLookUp.ColorPalette;

//public class UnitMovementOld
//{
//    private readonly IUnitManager unitManager;
//    private readonly LineRenderer pathLine;
//    private readonly TileManager tileManager;
//    private readonly TileBase tileIndicator;

//    private Queue<Node> unitMovementPath = new Queue<Node>();
//    private int currentPathCost;

//    private float movementSpeed;

//    private bool unitIsMoving;

//    private HashSet<Vector3Int> reachableTiles;
//    private readonly List<Node> pathDrawn = new List<Node>();

//    private readonly KeyboardControls keyboardControls;
//    private readonly Camera mainCamera;

//    private Action<InputAction.CallbackContext> leftClickHandler;
//    private Action<InputAction.CallbackContext> rightClickHandler;

//    public int PathCost => currentPathCost;
//    public HashSet<Vector3Int> ReachableTiles => reachableTiles;

//    public Animator unitAnimator;

//    public readonly bool isHeadless;


//    public UnitMovementOld(KeyboardControls keyboardControls,
//                        GameManager unitManager, 
//                        LineRenderer pathLine, 
//                        TileManager tileManager,
//                        float movementSpeed)
//    {
//        this.keyboardControls = keyboardControls;
//        this.unitManager = unitManager;
//        this.pathLine = pathLine;
//        this.tileManager = tileManager;
//        this.movementSpeed = movementSpeed;
//        this.tileIndicator = tileManager.SelectorTile;
//        this.isHeadless = false;
//        mainCamera = Camera.main;
//        reachableTiles = new ReachableTilesFinder(unitManager, tileManager).GetReachableTilePositions();
//    }

//    public UnitMovementOld(GameManager unitManager,
//                        TileManager tileManager,
//                        float movementSpeed)
//    {
//        this.unitManager = unitManager;
//        this.tileManager = tileManager;
//        this.movementSpeed = movementSpeed;
//        this.tileIndicator = tileManager.SelectorTile;
//        this.isHeadless = true;
//        reachableTiles = new ReachableTilesFinder(unitManager, tileManager).GetReachableTilePositions();
//    }

//    public void SubscribeControls()
//    {
//        if (isHeadless)
//        {
//            return;
//        }
//        leftClickHandler = (InputAction.CallbackContext _) => LeftClickPressed();
//        rightClickHandler = (InputAction.CallbackContext _) => RightClickPressed();
//        keyboardControls.Mouse.LeftClick.performed += leftClickHandler;
//        keyboardControls.Mouse.RightClick.performed += rightClickHandler;
//    }

//    public void UnsubscribeControls()
//    {
//        if (isHeadless)
//        {
//            return;
//        }
//        keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
//        keyboardControls.Mouse.RightClick.performed -= rightClickHandler;
//    }

//    public void EnterMovementPhase()
//    {
//        reachableTiles = new ReachableTilesFinder(unitManager, tileManager).GetReachableTilePositions();
//        if (!isHeadless)
//        {
//            unitAnimator = unitManager.GetSelectedUnit().Behaviour.GetComponentInChildren<Animator>();
//        }
//        DrawSelectableTiles();
//    }

//    public void ExitMovementPhase()
//    {
//        ClearSelectableTiles();
//        unitMovementPath.Clear();
//        currentPathCost = 0;
//        pathLine.positionCount = 0;
//    }

//    private void LeftClickPressed()
//    {
//        if (isHeadless || unitManager.IsOverUI())
//        {
//            return;
//        }

//        Vector2 mousePosition = keyboardControls.Mouse.MousePosition.ReadValue<Vector2>();

//        Tilemap tileMap = tileManager.Ground;
//        Unit unitSelected = unitManager.GetSelectedUnit();

//        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
//        Vector3Int gridPosition = tileMap.WorldToCell(mousePosition);

//        if (unitSelected.Faction == Faction.Friendly)
//        {
//            if (!unitIsMoving && !unitManager.GetPositionsOfUnits().ContainsKey(gridPosition))
//            {
//                SelectMoveToTile(gridPosition);
//            }
//        }
//    }

//    private void RightClickPressed()
//    {
//        if (isHeadless || unitManager.IsOverUI())
//        {
//            return;
//        }

//        Vector2 mousePosition = keyboardControls.Mouse.MousePosition.ReadValue<Vector2>();

//        Tilemap tileMap = tileManager.Ground;
//        Unit unitSelected = unitManager.GetSelectedUnit();

//        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
//        Vector3Int gridPosition = tileMap.WorldToCell(mousePosition);

//        // Removed the ability to select units, the queue will do it for us

//        if (unitSelected.Faction == Faction.Friendly)   // player may only control friendly units
//        {
//            if (!unitIsMoving && !unitManager.GetPositionsOfUnits().ContainsKey(gridPosition))
//            {
//                AddMoveToTile(gridPosition);
//            }
//        }
//    }

//    public void SelectMoveToTile(Vector3Int gridPosition)
//    {
//        if (unitIsMoving || !reachableTiles.Contains(gridPosition))
//        {
//            Debug.Log("Not reachable");
//            return;
//        }
//        unitMovementPath.Clear();
//        currentPathCost = 0;

//        Vector3Int unitPosition = unitManager.GetPositionByUnit(unitManager.GetSelectedUnit());
//        if (unitManager.GetSelectedUnit() != null && tileManager.Ground.HasTile(gridPosition))
//        {
//            // Chance that two units may overlap into the same tile if executed concurrently -> DO NOT EXECUTE CONCURRENTLY!
//            Node sourceNode = new Node(unitPosition);
//            Node destinationNode = new Node(gridPosition);

//            Pathfinder2D pathFinder = new Pathfinder2D(unitManager.GetSelectedUnit(), sourceNode, destinationNode, tileManager);

//            foreach (Node node in pathFinder.FindDirectedPath())
//            {
//                unitMovementPath.Enqueue(node);
//            }

//            currentPathCost = pathFinder.PathCost;

//            DrawPathTiles();
//            DrawPathLine();
//        }
//    }

//    public void AddMoveToTile(Vector3Int gridPosition)
//    {
//        if (unitIsMoving || !reachableTiles.Contains(gridPosition))
//        {
//            Debug.Log("Not reachable");
//            return;
//        }

//        if (unitMovementPath.Count == 0)
//        {
//            SelectMoveToTile(gridPosition);
//            return;
//        }

//        if (unitManager.GetSelectedUnit() != null && tileManager.Ground.HasTile(gridPosition))
//        {
//            // Chance that two units may overlap into the same tile if executed concurrently -> DO NOT EXECUTE CONCURRENTLY!
//            Node sourceNode = new Node(tileManager.Ground.WorldToCell(pathLine.GetPosition(pathLine.positionCount - 1)));
//            Node destinationNode = new Node(gridPosition);

//            Pathfinder2D pathFinder = new Pathfinder2D(unitManager.GetSelectedUnit(), sourceNode, destinationNode, tileManager, currentPathCost);

//            foreach (Node node in pathFinder.FindDirectedPath())
//            {
//                unitMovementPath.Enqueue(node);
//            }

//            currentPathCost = pathFinder.PathCost;

//            if (!isHeadless)
//            {
//                DrawPathTiles();
//                DrawPathLine();
//            }
//        }
//    }

//    private void DrawPathTiles()
//    {
//        foreach (Node node in pathDrawn)
//        {
//            tileManager.TileIndicators.SetTile(node.Coordinates, null);
//            if (reachableTiles.Contains(node.Coordinates))
//            {
//                tileManager.TileIndicators.SetTile(node.Coordinates, tileIndicator);
//            }
//        }
//        pathDrawn.Clear();

//        foreach (Node node in unitMovementPath)
//        {
//            tileManager.TileIndicators.SetTile(node.Coordinates, tileIndicator);
//            tileManager.TileIndicators.SetTileFlags(node.Coordinates, TileFlags.None);
//            if (node.IsReachable)
//            {
//                tileManager.TileIndicators.SetColor(node.Coordinates, LIGHT_BLUE_TRANSLUCENT);
//            } 
//            else
//            {
//                tileManager.TileIndicators.SetColor(node.Coordinates, LIGHT_RED_TRANSLUCENT);
//            }
//            pathDrawn.Add(node);
//        }
//    }


//    private void DrawPathLine()
//    {
//        pathLine.positionCount = unitMovementPath.Count + 1;
//        Node[] unitMovementNodes = unitMovementPath.ToArray();
//        // Start from self
//        pathLine.SetPosition(0, unitManager.GetSelectedUnit().WorldPosition + GameManager.UNIT_WORLD_OFFSET);
//        for (int i = 0; i < unitMovementNodes.Length; i++) 
//        {
//            pathLine.SetPosition(i + 1, tileManager.Ground.CellToWorld(unitMovementNodes[i].Coordinates) + GameManager.UNIT_WORLD_OFFSET);
//        }

//        if (currentPathCost > unitManager.GetSelectedUnit().ActionPointsLeft)
//        {
//            pathLine.startColor = Color.red;
//            pathLine.endColor = Color.red;
//        } 
//        else
//        {
//            pathLine.startColor = Color.blue;
//            pathLine.endColor = Color.blue;
//        }
//    }

//    public void DrawSelectableTiles()
//    {
//        foreach (Vector3Int position in reachableTiles)
//        {
//            tileManager.TileIndicators.SetTile(position, tileIndicator);
//        }
//    }

//    public void ClearSelectableTiles()
//    {
//        tileManager.TileIndicators.ClearAllTiles();
//    }

//    public void MoveUnit()
//    {
//        if (isHeadless)
//        {
//            return;
//        }

//        if (unitManager.GetSelectedUnit().ActionPointsLeft < currentPathCost)
//        {
//            Debug.Log("Too expensive " + currentPathCost);
//            return;
//        }

//        pathLine.positionCount = 0;
//        unitManager.GetSelectedUnit().UseActionPoints(PathCost);

//        ((GameManager) unitManager).UpdateActionPointsText();
//        unitManager.Enqueue(MoveToDestinationRoutine(unitManager.GetSelectedUnit(), unitMovementPath));
//    }

//    public IEnumerator MoveToDestinationRoutine(Unit unit, Queue<Node> path)
//    {
//        unitIsMoving = true;
//        pathDrawn.Clear();
//        ClearSelectableTiles();

//        // No other unit may move while the current unit is moving
//        // (transferred, removed) unitIsMoving = true;
//        Vector3 destination = unit.WorldPosition;

//        while (path.Count > 0)
//        {
//            destination = tileManager.Ground.CellToWorld(path.Dequeue().Coordinates);
//            float startTime = Time.time;
//            Vector3 source = unit.WorldPosition;
//            float journeyLength = Vector3.Distance(destination, unit.WorldPosition);

//            Vector3 directionVector = destination - source;
//            unitAnimator?.SetInteger("xDirection", Math.Sign(directionVector.x));
//            unitAnimator?.SetInteger("yDirection", Math.Sign(directionVector.y));

//            while (Vector3.Distance(unit.WorldPosition, destination) > 0.01f)
//            {
//                float distanceCovered = (Time.time - startTime) * movementSpeed;
//                float fractionOfJourney = Mathf.Min(1, distanceCovered / journeyLength);
//                unit.WorldPosition = Vector3.Lerp(source, destination, fractionOfJourney);
//                yield return null;
//            }
//            unit.WorldPosition = destination;
//        }

//        unitMovementPath.Clear();
//        currentPathCost = 0;

//        unitIsMoving = false;

//        unitManager.UpdateUnitPosition(unit, tileManager.Ground.WorldToCell(destination));
//    }
//}