using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class Adversary
{
    private UnitManager unitManager;
    private TileManager tileManager;
    private UnitMovement unitMovement;
    private UnitCombat unitCombat;
    private float unitSpeed;

    public Adversary(UnitManager unitManager, UnitMovement unitMovement, UnitCombat unitCombat, TileManager tileManager)
    {
        this.unitManager = unitManager;
        this.tileManager = tileManager;
        this.unitMovement = unitMovement;
        this.unitCombat = unitCombat;
    }

    public void Decision()
    {
        Unit unit = unitManager.SelectedUnit;
        Vector3Int unitPosition = unitManager.GetPositionByUnit(unit);

        unitMovement.EnterMovementPhase();
        MinHeap<Node> heap = new MinHeap<Node>();
        Debug.Log($"AP of this unit: {unitManager.SelectedUnit.ActionPointsLeft}");

        foreach (Vector3Int tilePosition in unitMovement.ReachableTiles)
        {
            int outgoingDamageRating = GetOutgoingDamageRating(unit, tilePosition);
            int incomingDamageRating = GetIncomingDamageRating(unit, tilePosition);

            int combatRating;
            if (outgoingDamageRating < incomingDamageRating)
            {
                Pathfinder2D pathfinder = new Pathfinder2D(unit, new Node(unitPosition), new Node(tilePosition), tileManager);
                pathfinder.FindDirectedPath();
                combatRating = incomingDamageRating + pathfinder.PathCost;
            } else
            {
                combatRating = incomingDamageRating - outgoingDamageRating;
            }

            Node node = new Node(tilePosition);
            node.Value = combatRating;

            heap.Add(node);
        }

        Node destination = heap.Extract();
        Debug.Log($"Decision to move to: {destination.Coordinates} {destination.Value}");
        unitMovement.SelectMoveToTile(destination.Coordinates);
        //unitCombat.SelectUnitToAttack(GetBestAttackableUnit(unitPosition));
        unitCombat.AttackSelectedUnit();
        unitMovement.MoveUnit();
        unitManager.OnClickEndTurn();
    }

    private Vector3Int GetBestAttackableUnit(Vector3Int position)
    {
        return Vector3Int.zero;
    }

    private int GetOutgoingDamageRating(Unit unit, Vector3Int tilePosition)
    {
        return Mathf.Max(unitCombat.GetAttackableUnits(unit, tilePosition, unit.Range)
                                            .Where(x => x.Faction != Faction.Friendly)
                                            .Select(x => Mathf.Max(1, unit.Attack - x.Defence)).ToArray());
    }

    private int GetIncomingDamageRating(Unit unit, Vector3Int tilePosition)
    {
        return unitCombat.GetReceivableAttacks(tilePosition).Select(x => Mathf.Max(1, x.Attack - unit.Defence)).Sum();
    }
}