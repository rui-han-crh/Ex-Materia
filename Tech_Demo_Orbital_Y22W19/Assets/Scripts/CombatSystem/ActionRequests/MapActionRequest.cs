using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;

public abstract class MapActionRequest 
{
    public enum RequestType
    {
        Movement = 5,
        Attack = 4,
        Overwatch = 3,
        Wait = 2,
        Skill = 6
    }

    private readonly Unit actingUnit;

    private readonly int timeSpent;

    private readonly int actionPointCost;

    private readonly RequestType requestType;

    protected Dictionary<GameMap, int> calculatedUtilities = new Dictionary<GameMap, int>();

    public Unit ActingUnit => actingUnit;

    public int TimeSpent => timeSpent;

    public int ActionPointCost => actionPointCost;

    public RequestType Type => requestType;

    public MapActionRequest(Unit actingUnit, int actionPointCost, int timeSpent, RequestType requestType)
    {
        this.actingUnit = actingUnit;
        this.timeSpent = timeSpent;
        this.actionPointCost = actionPointCost;
        this.requestType = requestType;
    }

    public abstract int GetUtility(GameMap map);
}
