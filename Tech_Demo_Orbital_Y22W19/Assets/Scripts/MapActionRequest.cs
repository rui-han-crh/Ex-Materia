using UnityEngine;

/// <summary>
/// A representation of an action that translates one map to another.
/// Has children MovementRequest and AttackRequest.
/// </summary>
public abstract class MapActionRequest
{
    private readonly MapActionType mapActionType;

    private readonly Unit actingUnit;
    private readonly Vector3Int sourcePosition;

    private readonly GameMap previousMap;

    private readonly int actionPointCost;

    public GameMap PreviousMap => previousMap;

    public Vector3Int ActingUnitPosition => sourcePosition;

    public int ActionPointCost => actionPointCost;

    public MapActionType ActionType => mapActionType;

    public Unit ActingUnit => actingUnit;

    public MapActionRequest(GameMap previousMap, MapActionType actionType, Vector3Int sourcePosition, int actionPoints)
    {
        this.previousMap = previousMap;
        this.actingUnit = previousMap.CurrentUnit;
        this.sourcePosition = sourcePosition;
        this.mapActionType = actionType;
        this.actionPointCost = actionPoints;
    }

    public GameMap GetNextMap()
    {
        return previousMap.DoAction(this);
    }

    //public static MapActionRequest Wait(GameMap previousMap)
    //{
    //    Vector3Int currentUnitPosition = previousMap.GetPositionByUnit(previousMap.CurrentUnit);

    //    return new MapActionRequest(previousMap, MapActionType.Wait, currentUnitPosition, currentUnitPosition, 100);
    //}

    public abstract float GetUtility();
}