using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CombatSystem.Entities
{
    public struct UnitProperties
    {
        private int maxHealth;

        public int MaxHealth
        {
            get => maxHealth;
            set => maxHealth = Mathf.Max(0, value);
        }

        private int currentHealth;

        public int CurrentHealth
        {
            get => currentHealth;
            set => currentHealth = Mathf.Clamp(value, 0, MaxHealth);
        }

        public int Defence;

        public int Attack;

        private int range;

        public int Range
        {
            get => range;
            set => range = Mathf.Max(0, value);
        }

        private int maxActionPoints;

        private int currentActionPoints;

        private int maxSkillPoints;

        private int currentSkillPoints;

        public int MaxSkillPoints => maxSkillPoints;

        public int CurrentSkillPoints
        {
            get => currentSkillPoints;
            set => currentSkillPoints = Mathf.Clamp(value, 0, maxSkillPoints);
        }

        public int MaxActionPoints
        {
            get => maxActionPoints;
            set => maxActionPoints = Mathf.Max(value, 0);
        }

        public int CurrentActionPoints
        {
            get => currentActionPoints;
            set => currentActionPoints = Mathf.Clamp(value, 0, MaxActionPoints);
        }

        public UnitProperties(
            int maxHealth,
            int currentHealth,
            int defence,
            int attack,
            int range,
            int maxActionPoints,
            int currentActionPoints) : this()
        {
            this.MaxHealth = maxHealth;
            this.CurrentHealth = currentHealth;
            this.Defence = defence;
            this.Attack = attack;
            this.Range = range;
            this.maxActionPoints = maxActionPoints;
            this.currentActionPoints = currentActionPoints;
        }

        public UnitProperties(
            int maxHealth,
            int currentHealth,
            int defence,
            int attack,
            int range,
            int maxActionPoints,
            int maxSkillPoints,
            int currentSkillPoints) : this()
        {
            this.MaxHealth = maxHealth;
            this.CurrentHealth = currentHealth;
            this.Defence = defence;
            this.Attack = attack;
            this.Range = range;
            this.maxActionPoints = maxActionPoints;
            this.currentActionPoints = maxActionPoints;
            this.maxSkillPoints = maxSkillPoints;
            this.currentSkillPoints = currentSkillPoints;
        }
    }
}
