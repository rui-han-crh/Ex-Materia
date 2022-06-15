using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;

public class WaitRequest : MapActionRequest
{
    private readonly int actionPointsReplenished;

    public int ActionPointsReplenished => actionPointsReplenished;

    public WaitRequest(Unit actingUnit, int timeSpent, int actionPointsReplenished) 
        : base(actingUnit, 0, timeSpent, RequestType.Wait)
    {
        this.actionPointsReplenished = actionPointsReplenished;
    }

    public override int GetUtility(GameMap map)
    {
        return map.EvaluatePositionSafetyOf(ActingUnit);
    }
}
