using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace CombatSystem.Effects
{
    [CreateAssetMenu(fileName = "GeneralStatus", menuName = "CombatStatusEffects/GeneralStatus", order = 1)]
    public class StatusEffect : AbstractEffect
    {
        [SerializeField]
        protected int duration;

        [SerializeField]
        protected int skillPointCost;

        public int Duration => duration;

        public int SkillPointCost => skillPointCost;

        public int GetEndTime()
        {
            return startTime + duration;
        }

        public StatusEffect SetStartTime(int startTime)
        {
            StatusEffect newEffect = Instantiate(this);
            newEffect.startTime = startTime;
            return newEffect;
        }
    }
}
