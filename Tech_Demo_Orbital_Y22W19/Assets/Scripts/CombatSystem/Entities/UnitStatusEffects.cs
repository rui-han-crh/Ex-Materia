using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Effects;
using CombatSystem.Facade;
using System.Linq;

namespace CombatSystem.Entities
{
    public class UnitStatusEffects
    {
        private readonly StatusEffect[] effects;

        private readonly float percentageAttackModifier = 1;

        private readonly int flatAttackModifier;

        private readonly float percentageDefenceModifier = 1;
        
        private readonly int flatDefenceModifier;

        private readonly int rangeModifier;

        public float PercentageAttackModifier => percentageAttackModifier;

        public int FlatAttackModifier => flatAttackModifier;

        public float PercentageDefenceModifier => percentageDefenceModifier;

        public int FlatDefenceModifier => flatDefenceModifier;

        public int RangeModifier => rangeModifier;


        private readonly UnitStatusEffectsFacade collection = UnitStatusEffectsFacade.Instance;

        public static UnitStatusEffects None()
        {
            return new UnitStatusEffects();
        }

        public UnitStatusEffects()
        {
            effects = new StatusEffect[UnitStatusEffectsFacade.Instance.Count];
        }

        private UnitStatusEffects(StatusEffect[] effects)
        {
            this.effects = effects;

            IEnumerable<StatusEffect> filteredEffects = effects.Where(x => x != null);

            percentageAttackModifier = filteredEffects.Aggregate(1.0f, (current, next) => current + next.PercentageAttackBonus);
            flatAttackModifier = filteredEffects.Aggregate(0, (current, next) => current + next.FlatAttackBonus);
            percentageDefenceModifier = filteredEffects.Aggregate(1.0f, (current, next) => current + next.PercentageDefenceBonus);
            flatDefenceModifier = filteredEffects.Aggregate(0, (current, next) => current + next.FlatDefenceBonus);
            rangeModifier = filteredEffects.Aggregate(0, (current, next) => current + next.RangeModifier);
        }

        public UnitStatusEffects AddStatusEffect(string effectName, int startTime)
        {
            StatusEffect[] newEffects = (StatusEffect[])effects.Clone();

            StatusEffect effect = collection.GetEffect(effectName);
            effect = effect.SetStartTime(startTime);
            newEffects[collection.GetEffectIndex(effect)] = effect;
            return new UnitStatusEffects(newEffects);
        }

        public UnitStatusEffects RemoveStatusEffect(string effectName)
        {
            StatusEffect[] newEffects = (StatusEffect[])effects.Clone();

            newEffects[collection.GetEffectIndex(effectName)] = null;
            return new UnitStatusEffects(newEffects);
        }


        public UnitStatusEffects CleanEffectsBasedOnTime(int currentTime)
        {
            StatusEffect[] newEffects = (StatusEffect[])effects.Clone();

            for (int i = 0; i < newEffects.Length; i++)
            {
                if (newEffects[i] != null && newEffects[i].GetEndTime() < currentTime)
                {
                    newEffects[i] = null;
                }
            }
            return new UnitStatusEffects(newEffects);
        }


        public bool HasEffect(string effectName)
        {
            Debug.Log(string.Join(", ", (object[])effects));

            if (effects[collection.GetEffectIndex(effectName)] == null)
                Debug.Log($"Does not have {effectName}");

            return effects[collection.GetEffectIndex(effectName)] != null;
        }
    }
}
