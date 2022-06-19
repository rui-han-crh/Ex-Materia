using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;


public class OverwatchRequest : MapActionRequest
{
    public static readonly int TIME_SPENT = 1000;

    public OverwatchRequest(Unit actingUnit) : base(actingUnit, 0, TIME_SPENT, RequestType.Overwatch)
    {

    }

    public override int GetUtility(GameMap map)
    {
        if (calculatedUtilities.ContainsKey(map))
        {
            return calculatedUtilities[map];
        }

        int utility = map.EvaluatePositionSafetyOf(ActingUnit);
        utility = -utility > ActingUnit.Risk ? utility : 0;

        calculatedUtilities[map] = utility - map.FindCostToNearestRival(ActingUnit);
        return calculatedUtilities[map];
    }
}
