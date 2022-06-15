using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;
using CombatSystem.Consultants;

public class MovementRequest : MapActionRequest
{
    public enum Outcome
    {
        NoValidPath,
        Successful,
        Pending
    }

    private Outcome outcome;

    private Vector3Int destination;

    public Vector3Int Destination => destination;

    public bool Successful => outcome == Outcome.Successful;

    public MovementRequest(Unit actingUnit, Vector3Int destination, int actionPointCost, int timeSpent, Outcome outcome) 
        : base(actingUnit, actionPointCost, timeSpent, RequestType.Movement)
    {
        this.destination = destination;
        this.outcome = outcome;
    }

    public IEnumerable<Vector3Int> CalculateShortestPath(GameMap map)
    {
        return MovementConsultant.FindShortestPath(map[ActingUnit], Destination, map.Data);
    }

    public override int GetUtility(GameMap map)
    {
        // Must be NON-POSITIVE, represents the potential damage received at destination

        //^Changed, first checks the potential damage recieved at destination, then sees if attacks will benefit

        GameMap nextMap = map.DoAction(this);

        // Evaluates THIS ACTING UNIT safety according to the NEXT MAP.
        // **This is not the same as EvaluatePositionSafetyOf in GameMap!**

        IEnumerable<Unit> allRivalPositions = nextMap.GetUnits(unit => unit.Faction != ActingUnit.Faction);

        int utility = nextMap.EvaluatePositionSafetyOf(ActingUnit);
        utility = -utility > ActingUnit.Risk ? utility : 0;

        foreach (Unit rival in allRivalPositions)
        {
            AttackRequest hypotheticalRequest = CombatConsultant.SimulateAttack(map.CurrentActingUnit, rival, map.Data);
            if (hypotheticalRequest.Successful)
            {
                utility += Mathf.Max(CombatConsultant.MINIMUM_DAMAGE_DEALT, 
                    (int)(hypotheticalRequest.ChanceToHit * hypotheticalRequest.PotentialDamageDealt));
            }
        }

        return utility - map.FindCostToNearestRival(ActingUnit);
    }
}
