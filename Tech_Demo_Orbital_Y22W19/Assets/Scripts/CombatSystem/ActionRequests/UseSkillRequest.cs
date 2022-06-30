using CombatSystem.Entities;
using CombatSystem.Facade;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseSkillRequest : MapActionRequest
{
    public string SkillName => skillName;

    private readonly string skillName;

    public UseSkillRequest(Unit actingUnit, string skillName, RequestType requestType) 
        : base(actingUnit, 0, 0, requestType)
    {
        this.skillName = skillName;
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
