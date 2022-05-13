using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

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

    public IEnumerator DecisionRoutine()
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
            int weight;
            if (outgoingDamageRating < incomingDamageRating)
            {
                Pathfinder2D pathfinder = new Pathfinder2D(unit, new Node(unitPosition), new Node(tilePosition), tileManager);
                pathfinder.FindDirectedPath();
                combatRating = incomingDamageRating + pathfinder.PathCost;
                weight = 0;
            } else
            {
                combatRating = incomingDamageRating - outgoingDamageRating;
                weight = 1;
            }

            Node node = new Node(tilePosition, weight);
            node.Value = combatRating;

            heap.Add(node);
        }

        Node destination = heap.Extract();
        Debug.Log($"Decision to move to: {destination.Coordinates} {destination.Value}");

        while (!unitManager.RoutineIsEmpty)
        {
            yield return null;
        }

        unitMovement.SelectMoveToTile(destination.Coordinates);
        unitMovement.MoveUnit();


        if (destination.Weight == 1)    // If the adversary is planning to attack
        {
            Unit bestCandidate = GetBestAttackableUnit(unitPosition);
            if (bestCandidate != null)
            {
                unitCombat.SelectUnitToAttack(destination.Coordinates, unitManager.GetPositionByUnit(bestCandidate), unit, false);
                Debug.Log($"{unitManager.SelectedUnit} wants to attack {bestCandidate}");
                unitCombat.AttackTargetedUnit(destination.Coordinates);
            }
        }

        unitManager.OnClickEndTurn();
    }

    private Unit GetBestAttackableUnit(Vector3Int position)
    {
        Unit currentUnit = unitManager.SelectedUnit;
        Vector3Int unitPosition = unitManager.GetPositionByUnit(currentUnit);

        Unit[] unitsAttackable = unitCombat.GetAttackableUnits(currentUnit, unitPosition, currentUnit.Range).ToArray();

        int currentMax = int.MinValue;
        Unit bestCandidate = null;
        for (int i = 0; i < unitsAttackable.Length; i++)
        {
            if (currentUnit.Attack - unitsAttackable[i].Defence > currentMax)
            {
                bestCandidate = unitsAttackable[i];
                currentMax = currentUnit.Attack - unitsAttackable[i].Defence;
            }
        }
        return bestCandidate;
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