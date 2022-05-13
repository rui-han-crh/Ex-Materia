using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using System.Linq;
using static ColorLookUp.ColorPalette;

public class UnitCombat
{
    public static int COST_TO_ATTACK = 60;

    private readonly UnitManager unitManager;
    private readonly TileManager tileManager;
    private readonly UnitMovement unitMovement;
    private readonly KeyboardControls keyboardControls;
    private readonly LineRenderer lineRenderer;

    private Action<InputAction.CallbackContext> leftClickHandler;

    private Camera mainCamera;

    private HashSet<Vector3Int> rangeTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> aimTiles = new HashSet<Vector3Int>();

    private Vector3Int attackFromCoordinates;
    private Vector3Int targetCoordinates;
    private bool hasSightToTarget;

    public UnitCombat(  KeyboardControls keyboardControls, 
                        UnitManager unitManager,
                        LineRenderer lineRenderer,
                        TileManager tileManager,
                        UnitMovement unitMovement)
    {
        this.keyboardControls = keyboardControls;
        this.lineRenderer = lineRenderer;
        this.unitManager = unitManager;
        this.tileManager = tileManager;
        this.unitMovement = unitMovement;
        mainCamera = Camera.main;
    }

    public void SubscribeControls()
    {
        leftClickHandler = (InputAction.CallbackContext _) => OnLeftClick();
        keyboardControls.Mouse.LeftClick.performed += leftClickHandler;
    }

    public void UnsubscribeControls()
    {
        keyboardControls.Mouse.LeftClick.performed -= leftClickHandler;
    }

    public void EnterCombatPhase()
    {
        lineRenderer.positionCount = 0;
        tileManager.TileIndicators.ClearAllTiles();
        DrawRangeTiles();
    }


    public void ExitCombatPhase()
    {
        ClearRangeTiles();
        rangeTiles.Clear();
        hasSightToTarget = false;
        lineRenderer.positionCount = 0;
        targetCoordinates = default;
        attackFromCoordinates = default;
    }

    public void SelectUnitToAttack(Vector3Int targetPosition)
    {
        ClearAim();

        Vector3Int[] tilesHit;
        Vector3Int sourcePosition;

        hasSightToTarget = IsTargetAttackable(targetPosition, out sourcePosition, out tilesHit);

        if (hasSightToTarget)
        {
            attackFromCoordinates = sourcePosition;
            targetCoordinates = tilesHit[tilesHit.Length - 1];
            DrawAim(sourcePosition, tilesHit, true);
        }
        else
        {
            DrawAim(sourcePosition, tilesHit, false);
        }
    }

    public void SelectUnitToAttack(Vector3Int offensivePosition, Vector3Int targetPosition, Unit offensiveUnit, bool showAim = true)
    {
        ClearAim();

        Vector3Int[] tilesHit;
        Vector3Int sourcePosition;

        hasSightToTarget = IsTargetAttackable(  offensivePosition, 
                                                targetPosition,
                                                new HashSet<Vector3Int> { targetPosition },
                                                offensiveUnit.ActionPointsLeft,
                                                out sourcePosition, 
                                                out tilesHit);

        if (hasSightToTarget)
        {
            attackFromCoordinates = sourcePosition;
            targetCoordinates = tilesHit[tilesHit.Length - 1];
            if (showAim)
            {
                DrawAim(sourcePosition, tilesHit, true);
            }
        }
        else
        {
            if (showAim)
            {
                DrawAim(sourcePosition, tilesHit, false);
            }
        }
    }

    public void AttackTargetedUnit()
    {
        AttackTargetedUnit(unitManager.GetPositionByUnit(unitManager.SelectedUnit));
    }

    public void AttackTargetedUnit(Vector3Int unitOriginalPosition)
    {
        Unit selectedUnit = unitManager.SelectedUnit;

        if (!hasSightToTarget || selectedUnit.ActionPointsLeft < COST_TO_ATTACK)
        {
            Debug.Log($"Attack failed, has sight? -> {hasSightToTarget}, enough AP? -> {selectedUnit.ActionPointsLeft >= COST_TO_ATTACK}");
            return;
        }
        Debug.Log($"Attacking {targetCoordinates}");

        unitManager.Enqueue(
            unitMovement.MoveToDestinationRoutine(unitManager.SelectedUnit, new Queue<Node>(new Node[] { new Node(attackFromCoordinates) }))
            );

        unitManager.Enqueue(
            DecreaseTargetHealth(unitManager.GetUnitByPosition(targetCoordinates))
            );

        unitManager.Enqueue(
            unitMovement.MoveToDestinationRoutine(unitManager.SelectedUnit, new Queue<Node>(new Node[] { new Node(unitOriginalPosition) }))
            );

        unitManager.SelectedUnit.UseActionPoints(COST_TO_ATTACK);
        unitManager.UpdateActionPointsText();
    }

    public HashSet<Unit> GetAttackableUnits(Unit unit, Vector3Int source, int range)
    {
        HashSet<Vector3Int> rangeTiles = GetRangedTiles(source, range);

        return new HashSet<Unit>(rangeTiles
                                    .Intersect(unitManager.PositionsOfUnits.Keys)
                                    .Where(x => IsTargetAttackable(source, x, rangeTiles, unit.ActionPointsLeft, out _, out _))
                                    .Select(x => unitManager.GetUnitByPosition(x)));
    }

    public HashSet<Unit> GetReceivableAttacks(Vector3Int tilePosition)
    {
        HashSet<Unit> units = new HashSet<Unit>();
        foreach (KeyValuePair<Vector3Int, Unit> positionAndUnit in unitManager.PositionsOfUnits)
        {
            Vector3Int position = positionAndUnit.Key;
            Unit unit = positionAndUnit.Value;

            HashSet<Vector3Int> unitRange = GetRangedTiles(position, unit.Range);
            if (IsTargetAttackable(position, tilePosition, unitRange, unit.ActionPointsLeft, out _, out _))
            {
                units.Add(unit);
            }
        }
        return units;
    }


    private void OnLeftClick()
    {
        if (unitManager.IsOverUI)
        {
            return;
        }

        Vector2 mousePosition = keyboardControls.Mouse.MousePosition.ReadValue<Vector2>();
        Tilemap tileMap = tileManager.Ground;

        mousePosition = mainCamera.ScreenToWorldPoint(mousePosition);
        Vector3Int gridPosition = tileMap.WorldToCell(mousePosition);

        // Removed the ability to select units, the queue will do it for us

        if (unitManager.SelectedUnit.Faction == Faction.Friendly && unitManager.PositionsOfUnits.ContainsKey(gridPosition)) 
        {
            SelectUnitToAttack(gridPosition);
        }
    }

    private static HashSet<Vector3Int> GetRangedTiles(Vector3Int source, int range)
    {
        Vector3Int[] rangeBorder = ReachableTilesFinder.GetRangeBorder(source, range);
        HashSet<Vector3Int> rangeBorderHashed = new HashSet<Vector3Int>(rangeBorder);

        Queue<Vector3Int> pollingQueue = new Queue<Vector3Int>();
        Queue<Vector3Int> offerQueue = new Queue<Vector3Int>();
        pollingQueue.Enqueue(source);

        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        while (offerQueue.Count > 0 || pollingQueue.Count > 0)
        {
            while (pollingQueue.Count > 0)
            {
                Vector3Int current = pollingQueue.Dequeue();

                for (float angle = 0; angle < Mathf.PI * 2; angle += Mathf.PI / 2)
                {
                    Vector3Int directionVector = new Vector3Int((int)Mathf.Sin(angle), (int)Mathf.Cos(angle), 0);
                    Vector3Int neighbourVector = current + directionVector;

                    if (!visited.Contains(neighbourVector) && !rangeBorderHashed.Contains(neighbourVector))
                    {
                        visited.Add(neighbourVector);
                        offerQueue.Enqueue(neighbourVector);
                    }
                }
            }
            Queue<Vector3Int> temp = pollingQueue;
            pollingQueue = offerQueue;
            offerQueue = temp;
        }

        visited.UnionWith(rangeBorderHashed);

        return visited;
    }

    private void DrawRangeTiles()
    {
        ClearRangeTiles();

        if (rangeTiles.Count > 0)
        {
            foreach (Vector3Int tilePosition in rangeTiles)
            {
                tileManager.TileIndicators.SetTile(tilePosition, tileManager.SelectorTile);
                tileManager.TileIndicators.SetTileFlags(tilePosition, TileFlags.None);
                tileManager.TileIndicators.SetColor(tilePosition, YELLOW_TRANSLUCENT);
            }
        }

        rangeTiles = GetRangedTiles(unitManager.GetPositionByUnit(unitManager.SelectedUnit), unitManager.SelectedUnit.Range);

        foreach(Vector3Int tilePosition in rangeTiles)
        {
            tileManager.TileIndicators.SetTile(tilePosition, tileManager.SelectorTile);
            tileManager.TileIndicators.SetTileFlags(tilePosition, TileFlags.None);
            tileManager.TileIndicators.SetColor(tilePosition, YELLOW_TRANSLUCENT);
        }
    }

    private void ClearRangeTiles()
    {
        foreach(Vector3Int tilePosition in rangeTiles)
        {
            tileManager.TileIndicators.SetTile(tilePosition, null);
        }
    }

    private bool IsTargetAttackable(Vector3Int offensiveUnitPosition,
                                    Vector3Int targetPosition,
                                    HashSet<Vector3Int> rangeTiles,
                                    int actionsPointsOfUnit,
                                    out Vector3Int sourcePosition, 
                                    out Vector3Int[] tilesHit)
    {
        if (actionsPointsOfUnit < COST_TO_ATTACK || !rangeTiles.Contains(targetPosition))
        {
            sourcePosition = offensiveUnitPosition;
            tilesHit = new Vector3Int[] { targetPosition };
            return false; // Not in range or not enough AP
        }

        LineRaytracer lineRaytracer = new LineRaytracer();

        bool hasSightToTarget = lineRaytracer.Trace(offensiveUnitPosition, targetPosition, UnitManager.UNIT_GRID_OFFSET, tileManager);

        if (hasSightToTarget)
        {
            sourcePosition = offensiveUnitPosition;
            tilesHit = lineRaytracer.TilesHit;
            return true;
        }
        else
        {
            Vector3Int tileAhead = lineRaytracer.TilesHit[0];
            float unitVector = Vector3.Distance(tileAhead, offensiveUnitPosition);

            if (!tileManager.Obstacles.HasTile(tileAhead) || unitVector > 1)
            {
                sourcePosition = offensiveUnitPosition;
                tilesHit = lineRaytracer.TilesHit;
                return false; // The unit is not against a wall
            }

            Vector3Int vectorToWall = tileAhead - offensiveUnitPosition;
            Vector3Int peekCoordinates;
            bool canPeekAndShoot;

            for (float angle = -Mathf.PI / 2; angle <= Mathf.PI / 2; angle += Mathf.PI)
            {
                peekCoordinates = offensiveUnitPosition + RotateVector(vectorToWall, angle);
                if (tileManager.Obstacles.HasTile(peekCoordinates))
                {
                    continue; // There is an obstacle obstructing a peek
                }

                canPeekAndShoot = lineRaytracer.Trace(peekCoordinates, targetPosition, UnitManager.UNIT_GRID_OFFSET, tileManager);

                if (canPeekAndShoot)
                {
                    sourcePosition = peekCoordinates;
                    tilesHit = lineRaytracer.TilesHit;
                    return true;
                }
            }
        }

        sourcePosition = offensiveUnitPosition;
        tilesHit = lineRaytracer.TilesHit;
        return false;
    }

    private bool IsTargetAttackable(Vector3Int targetPosition, out Vector3Int sourcePosition, out Vector3Int[] tilesHit)
    {
        return IsTargetAttackable(unitManager.GetPositionByUnit(unitManager.SelectedUnit),
                                  targetPosition,
                                  rangeTiles,
                                  unitManager.SelectedUnit.ActionPointsLeft,
                                  out sourcePosition,
                                  out tilesHit);
    }

    private void DrawAim(Vector3Int sourcePosition, Vector3Int[] tilesOverlapping, bool canHit = true)
    {
        if (tilesOverlapping.Length == 0)
        {
            return;
        }

        tileManager.TileIndicators.SetColor(sourcePosition, canHit ? LIGHT_GREEN_TRANSLUCENT : LIGHT_RED_TRANSLUCENT);
        aimTiles.Add(sourcePosition);
        lineRenderer.positionCount = 2;

        lineRenderer.startColor = canHit ? DARK_GREEN : Color.red;
        lineRenderer.endColor = canHit ? DARK_GREEN : Color.red;
        lineRenderer.SetPosition(0, tileManager.Ground.CellToWorld(sourcePosition) + UnitManager.UNIT_WORLD_OFFSET);
        lineRenderer.SetPosition(1, tileManager.Ground.CellToWorld(tilesOverlapping[tilesOverlapping.Length - 1]) + UnitManager.UNIT_WORLD_OFFSET);

        foreach (Vector3Int tile in tilesOverlapping)
        {
            aimTiles.Add(tile);
            tileManager.TileIndicators.SetColor(tile, canHit ? LIGHT_GREEN_TRANSLUCENT : LIGHT_RED_TRANSLUCENT);
        }
    }

    private void ClearAim()
    {
        lineRenderer.positionCount = 0;

        foreach (Vector3Int tile in aimTiles)
        {
            tileManager.TileIndicators.SetColor(tile, YELLOW);
        }
        aimTiles.Clear();
    }

    private IEnumerator DecreaseTargetHealth(Unit unit)
    {
        int effectiveDamage = Math.Min(unit.Defence - unitManager.SelectedUnit.Attack, 0);
        unit.ChangeHealth(effectiveDamage);
        unit.Behaviour.UpdateHealthBar();
        yield return null;
    }

    private Vector3Int RotateVector(Vector3Int vector, float radians)
    {
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);

        return new Vector3Int(
            Mathf.RoundToInt(cos * vector.x - sin * vector.y),
            Mathf.RoundToInt(sin * vector.x + cos * vector.y),
            0);
    }
}