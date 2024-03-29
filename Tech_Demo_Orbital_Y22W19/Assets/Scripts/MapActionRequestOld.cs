﻿using UnityEngine;

/// <summary>
/// A representation of an action that translates one map to another.
/// Has children MovementRequest and AttackRequest.
/// </summary>
public abstract class MapActionRequestOld
{
    private readonly MapActionType mapActionType;

    private readonly UnitOld actingUnit;
    private readonly Vector3Int sourcePosition;

    private readonly GameMapOld previousMap;

    private readonly int actionPointCost;

    public GameMapOld PreviousMap => previousMap;

    public Vector3Int ActingUnitPosition => sourcePosition;

    public int ActionPointCost => actionPointCost;

    public MapActionType ActionType => mapActionType;

    public UnitOld ActingUnit => actingUnit;

    public MapActionRequestOld(GameMapOld previousMap, MapActionType actionType, Vector3Int sourcePosition, int actionPoints)
    {
        this.previousMap = previousMap;
        this.actingUnit = previousMap.CurrentUnit;
        this.sourcePosition = sourcePosition;
        this.mapActionType = actionType;
        this.actionPointCost = actionPoints;
    }

    public MapActionRequestOld(GameMapOld previousMap, UnitOld actingUnit, MapActionType actionType, int actionPoints)
    {
        this.previousMap = previousMap;
        this.actingUnit = actingUnit;
        this.sourcePosition = previousMap.GetPositionByUnit(actingUnit);
        this.mapActionType = actionType;
        this.actionPointCost = actionPoints;
    }

    public GameMapOld GetNextMap()
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