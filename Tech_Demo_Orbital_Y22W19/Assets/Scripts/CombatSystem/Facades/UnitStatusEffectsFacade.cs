using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Effects;

namespace CombatSystem.Facade
{
    public class UnitStatusEffectsFacade : MonoBehaviour
    {
        private static UnitStatusEffectsFacade instance;

        public static UnitStatusEffectsFacade Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<UnitStatusEffectsFacade>();
                    Debug.Assert(instance != null, "UnitStatusEffectsFacade does not exist in the scene, consider adding one");
                }
                return instance;
            }
        }

        [SerializeField]
        private StatusEffect[] statusEffects;

        private Dictionary<string, StatusEffect> effectsByName;
        private Dictionary<StatusEffect, int> effectsIndex;

        public int Count => statusEffects.Length;

        public void Awake()
        {
            effectsByName = new Dictionary<string, StatusEffect>();
            effectsIndex = new Dictionary<StatusEffect, int>();

            int i = 0;
            foreach (StatusEffect effect in statusEffects)
            {
                effectsByName.Add(effect.StatusName, effect);
                effectsIndex.Add(effect, i++);
            }
        }

        public StatusEffect GetEffect(string name)
        {
            return effectsByName[name];
        }

        public int GetEffectIndex(StatusEffect effect)
        {
            return effectsIndex[effect];
        }

        public int GetEffectIndex(string name)
        {
            return effectsIndex[GetEffect(name)];
        }

    }
}
