using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;

public class WaitRequest : MapActionRequest
{
    private readonly int skillPointsReplenished;

    public int SkillPointsReplenished => skillPointsReplenished;

    public WaitRequest(Unit actingUnit, int timeSpent) 
        : base(actingUnit, 0, timeSpent, RequestType.Wait)
    {
        this.skillPointsReplenished = ConvertTimeToSP(timeSpent);
    }

    public override int GetUtility(GameMap map)
    {
        if (calculatedUtilities.ContainsKey(map))
        {
            return calculatedUtilities[map];
        }

        calculatedUtilities[map] = map.EvaluatePositionSafetyOf(ActingUnit);
        return calculatedUtilities[map];
    }

    public static int ConvertTimeToSP(int timeValue)
    {
        return timeValue / 20;
    }
}
