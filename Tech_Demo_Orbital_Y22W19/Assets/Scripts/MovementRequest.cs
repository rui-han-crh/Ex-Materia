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
        GameMap nextMap = PreviousMap.DoAction(this);

        List<Vector3Int> allRivalPosition = nextMap.AllUnitPositions
            .Where(rivalPosition => nextMap.GetUnitByPosition(rivalPosition).Faction != ActingUnit.Faction).ToList();

        float rivalAttackRating = 0;

        foreach (Vector3Int rivalPosition in allRivalPosition)
        {
            Unit rivalUnit = nextMap.GetUnitByPosition(rivalPosition);
            AttackRequest hypotheticalRequest = nextMap.QueryAttackability(rivalPosition, destinationPosition, rivalUnit.Range * 2);
            if (hypotheticalRequest.Successful)
            {
                rivalAttackRating += hypotheticalRequest.ChanceToHit * rivalUnit.Attack;
            }
        }
        return ActingUnit.Defence - rivalAttackRating;
    }

    public float GetAttackRating()
    {
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
                attackRating += hypotheticalRequest.ChanceToHit * ActingUnit.Attack - rivalUnit.Defence;
            }
        }
        return attackRating;
    }

    public override string ToString()
    {
        return $"{ActingUnit.Name} moves from {ActingUnitPosition} to {DestinationPosition}";
    }
}