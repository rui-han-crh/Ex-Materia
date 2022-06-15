using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;

public class OverwatchRequest : MapActionRequest
{
    public static readonly int TIME_SPENT = 100;

    private readonly GameMapData gameMapData;

    public OverwatchRequest(Unit actingUnit) : base(actingUnit, 0, TIME_SPENT)
    {

    }

    public override int GetUtility(GameMap map)
    {
        return map.EvaluatePositionSafetyOf(ActingUnit) - map.FindCostToNearestRival(ActingUnit);
    }
}
