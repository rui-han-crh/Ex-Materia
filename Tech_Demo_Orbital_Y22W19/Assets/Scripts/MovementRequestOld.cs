using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class MovementRequestOld : MapActionRequestOld
{
    private readonly Vector3Int[] path;

    public Vector3Int DestinationPosition => path[path.Length - 1];

    public Vector3Int[] Path => path;


    public MovementRequestOld(GameMapOld previousMap, Vector3Int sourcePosition, Vector3Int[] path, int actionPoints)
        : base(previousMap, MapActionType.Movement, sourcePosition, actionPoints)
    {
        this.path = path;
    }

    public override float GetUtility()
    {
        // Must be NON-POSITIVE, represents the potential damage received at destination

        GameMapOld nextMap = PreviousMap.DoAction(this);

        // Evaluates THIS ACTING UNIT safety according to the NEXT MAP.
        // **This is not the same as EvaluateCurrentPositionSafety!**

        List<Vector3Int> allRivalPosition = nextMap.AllUnitPositions
            .Where(rivalPosition => nextMap.GetUnitByPosition(rivalPosition).Faction != ActingUnit.Faction).ToList();

        float utility = 0;

        foreach (Vector3Int rivalPosition in allRivalPosition)
        {
            UnitOld rivalUnit = nextMap.GetUnitByPosition(rivalPosition);
            AttackRequestOld hypotheticalRequest = nextMap.QueryAttackability(rivalPosition, DestinationPosition, rivalUnit.Range);
            if (hypotheticalRequest.Successful)
            {
                // must be negative here
                utility += Mathf.Min(-UnitCombat.MINIMUM_DAMAGE_DEALT, ActingUnit.Defence - hypotheticalRequest.ChanceToHit * rivalUnit.Attack);
            }
        }
        
        utility = -utility > ActingUnit.Risk ? utility : 0;

        return utility - PreviousMap.EuclideanDistanceToNearestRival(DestinationPosition) + GetAttackRating();
    }


    /// <summary>
    /// Evaluates the potential to attack another rival unit from the destination
    /// as a result of carrying out this movement request onto the game map.
    /// </summary>
    /// <returns></returns>
    public float GetAttackRating()
    {
        // Must be postive

        GameMapOld nextMap = PreviousMap.DoAction(this);

        List<Vector3Int> allRivalPosition = nextMap.AllUnitPositions
            .Where(rivalPosition => nextMap.GetUnitByPosition(rivalPosition).Faction != ActingUnit.Faction).ToList();

        float attackRating = 0;

        foreach (Vector3Int rivalPosition in allRivalPosition)
        {
            UnitOld rivalUnit = nextMap.GetUnitByPosition(rivalPosition);
            AttackRequestOld hypotheticalRequest = nextMap.QueryAttackability(DestinationPosition, rivalPosition, ActingUnit.Range);
            if (hypotheticalRequest.Successful)
            {
                attackRating += Mathf.Max(UnitCombat.MINIMUM_DAMAGE_DEALT, hypotheticalRequest.ChanceToHit * ActingUnit.Attack - rivalUnit.Defence);
            }
        }
        return attackRating;
    }

    public override string ToString()
    {
        return $"{ActingUnit.Name} moves from {ActingUnitPosition} to {DestinationPosition}";
    }
}