using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Effects;

namespace CombatSystem.Facade
{
    [CreateAssetMenu(fileName = "_UnitStatusEffectsDatabase", menuName = "Assets/CombatSystem/UnitStatusEffectsDatabase")]
    public class UnitStatusEffectsFacade : ScriptableObject
    {
        private static UnitStatusEffectsFacade instance;

        public static UnitStatusEffectsFacade Instance
        {
            get
            {
                Debug.Assert(instance != null, "UnitStatusEffectsDatabase does not exist in the scene, consider adding one. " +
                    "Otherwise, if you are in the Editor, this could be happening because the game has stopped, but the Enemy AI task was not killed.");
                return instance;
            }
        }

        [SerializeField]
        private StatusEffect[] statusEffects;

        private Dictionary<string, StatusEffect> effectsByName;
        private Dictionary<StatusEffect, int> effectsIndex;

        public int Count => statusEffects.Length;

        private void Awake()
        {
            instance = this;

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
