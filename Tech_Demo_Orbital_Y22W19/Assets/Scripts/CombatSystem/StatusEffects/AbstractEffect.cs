using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Effects
{
    public abstract class AbstractEffect : ScriptableObject
    {
        [SerializeField]
        protected string statusName;

        [SerializeField]
        protected int flatAttackBonus;

        [SerializeField]
        protected float percentageAttackBonus = 1;

        [SerializeField]
        protected int flatDefenceBonus;

        [SerializeField]
        protected float percentageDefenceBonus = 1;

        [SerializeField]
        protected int flatHealthRecovery;

        [SerializeField]
        protected int rangeModifier;

        [SerializeField]
        protected Sprite icon;

        [SerializeField]
        protected string description;

        public string Description => description;

        public Sprite Icon => icon;

        protected int startTime;

        public float PercentageAttackBonus => percentageAttackBonus;

        public float PercentageDefenceBonus => percentageDefenceBonus;

        public int FlatAttackBonus => flatAttackBonus;
        public int FlatDefenceBonus => flatDefenceBonus;
        public int FlatHealthRecovery => flatHealthRecovery;

        public int RangeModifier => rangeModifier;

        public string StatusName => statusName;

        public int StartTime => startTime;


        public override int GetHashCode()
        {
            return statusName.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (!(other is AbstractEffect))
            {
                return false;
            }

            return ((AbstractEffect)other).statusName.Equals(statusName);
        }
    }
}
