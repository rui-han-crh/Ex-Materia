using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class MovementRequest : MapActionRequest
{
    private readonly Vector3Int destinationPosition;

    public Vector3Int DestinationPosition => destinationPosition;

    public MovementRequest(GameMap previousMap, Vector3Int sourcePosition, Vector3Int targetPosition, int actionPoints)
        : base(previousMap, MapActionType.Movement, sourcePosition, actionPoints)
    {
        destinationPosition = targetPosition;
    }

    public override float GetUtility()
    {
        // Must be non-positive

        GameMap nextMap = PreviousMap.DoAction(this);

        List<Vector3Int> allRivalPosition = nextMap.AllUnitPositions
            .Where(rivalPosition => nextMap.GetUnitByPosition(rivalPosition).Faction != ActingUnit.Faction).ToList();

        float utility = 0;

        foreach (Vector3Int rivalPosition in allRivalPosition)
        {
            Unit rivalUnit = nextMap.GetUnitByPosition(rivalPosition);
            AttackRequest hypotheticalRequest = nextMap.QueryAttackability(rivalPosition, destinationPosition, rivalUnit.Range);
            if (hypotheticalRequest.Successful)
            {
                // must be negative here
                utility += Mathf.Min(-UnitCombat.MINIMUM_DAMAGE_DEALT, ActingUnit.Defence - hypotheticalRequest.ChanceToHit * rivalUnit.Attack);
            }
        }
        
        return -utility > ActingUnit.Risk ? utility : 0;
    }

    public float GetAttackRating()
    {
        // Must be postive

        GameMap nextMap = PreviousMap.DoAction(this);

        List<Vector3Int> allRivalPosition = nextMap.AllUnitPositions
            .Where(rivalPosition => nextMap.GetUnitByPosition(rivalPosition).Faction != ActingUnit.Faction).ToList();

        float attackRating = 0;

        foreach (Vector3Int rivalPosition in allRivalPosition)
        {
            Unit rivalUnit = nextMap.GetUnitByPosition(rivalPosition);
            AttackRequest hypotheticalRequest = nextMap.QueryAttackability(destinationPosition, rivalPosition, ActingUnit.Range);
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