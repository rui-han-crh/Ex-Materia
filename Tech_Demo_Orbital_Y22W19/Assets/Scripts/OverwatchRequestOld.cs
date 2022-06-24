using UnityEngine;

public class OverwatchRequestOld : MapActionRequestOld
{
    public static readonly int TIME_CONSUMED = 100;
    public OverwatchRequestOld(GameMapOld previousMap, Vector3Int sourcePosition) :
        base(previousMap, MapActionType.Overwatch, sourcePosition, 0)
    {

    }

    public override float GetUtility()
    {
        return PreviousMap.EvaluateCurrentPositionSafety() - PreviousMap.EuclideanDistanceToNearestRival(ActingUnitPosition);
    }

    public override string ToString()
    {
        return $"{ActingUnit.Name} overwatch at {ActingUnitPosition}";
    }
}