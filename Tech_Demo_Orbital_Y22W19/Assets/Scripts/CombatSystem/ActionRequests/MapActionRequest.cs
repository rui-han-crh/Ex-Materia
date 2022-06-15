using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities;

public abstract class MapActionRequest 
{
    public enum RequestType
    {
        Attack,
        Movement,
        Wait,
        Overwatch
    }

    private readonly Unit actingUnit;

    private readonly int timeSpent;

    private readonly int actionPointCost;

    private readonly RequestType requestType;



    public Unit ActingUnit => actingUnit;

    public int TimeSpent => timeSpent;

    public int ActionPointCost => actionPointCost;

    public RequestType Type => requestType;

    public MapActionRequest(Unit actingUnit, int actionPointCost, int timeSpent)
    {
        this.actingUnit = actingUnit;
        this.timeSpent = timeSpent;
        this.actionPointCost = actionPointCost;
    }

    public abstract int GetUtility(GameMap map);
}
