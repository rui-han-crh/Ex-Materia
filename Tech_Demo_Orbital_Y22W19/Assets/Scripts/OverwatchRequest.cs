using UnityEngine;

public class OverwatchRequest : MapActionRequest
{
    public static readonly int TIME_CONSUMED = 100;
    public OverwatchRequest(GameMap previousMap, Vector3Int sourcePosition) :
        base(previousMap, MapActionType.Overwatch, sourcePosition, 0)
    {

    }

    public override float GetUtility()
    {
        return PreviousMap.EvaluateCurrentPositionSafety();
    }

    public override string ToString()
    {
        return $"{ActingUnit.Name} overwatch at {ActingUnitPosition}";
    }
}