using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace CombatSystem.Entities
{
    public class Unit
    {
        public enum UnitFaction
        {
            Friendly,
            Enemy
        }

        public enum RiskCalculationMethod
        {
            StaticInteger,
            DivisorOfHealth
        }

        private static int identityCount = 1;

        private readonly int identity;

        private readonly string name;

        private readonly UnitFaction faction;

        private readonly UnitProperties baseProperties;

        private readonly UnitProperties currentProperties;

        private readonly UnitStatusEffects statusEffects;

        private readonly Func<Unit, int> riskCalculatingFunction;

        private readonly int time;

        public int Identity => identity;

        public string Name => name;

        public int Attack => currentProperties.Attack;

        public int Defence => currentProperties.Defence;

        public int Range => currentProperties.Range;

        public int Risk => riskCalculatingFunction(this);

        public int CurrentHealth => currentProperties.CurrentHealth;

        public int MaxHealth => currentProperties.MaxHealth;

        public int CurrentActionPoints => currentProperties.CurrentActionPoints;

        public int MaxActionPoints => currentProperties.MaxActionPoints;

        public int Time => time;

        public UnitFaction Faction => faction;

        public static void ResetClass()
        {
            identityCount = 1;
        }

        private Unit()
        {
            identity = -1;
            currentProperties = new UnitProperties();
            baseProperties = new UnitProperties();
            statusEffects = UnitStatusEffects.None;
            time = 0;
            this.faction = UnitFaction.Friendly;
            this.riskCalculatingFunction = _ => Defence;
        }

        private Unit(string name, UnitProperties properties, UnitFaction faction)
        {
            this.name = name;
            this.faction = faction;
            this.identity = identityCount++;
            this.baseProperties = properties;
            this.currentProperties = properties;
            this.statusEffects = UnitStatusEffects.None;
            this.time = 0;
            this.riskCalculatingFunction = _ => Defence;
        }

        private Unit(string name, UnitProperties properties, int time, UnitFaction faction) : this(name, properties, faction)
        {
            this.time = time;
            this.riskCalculatingFunction = _ => Defence;
        }

        private Unit(string name, 
            UnitProperties properties, 
            int time, 
            UnitFaction faction, 
            int riskParameter, 
            RiskCalculationMethod method) 
            : this(name, properties, time, faction)
        {
            switch (method)
            {
                case RiskCalculationMethod.StaticInteger:
                    this.riskCalculatingFunction = _ => riskParameter;
                    break;

                case RiskCalculationMethod.DivisorOfHealth:
                    this.riskCalculatingFunction = x => x.CurrentHealth / riskParameter;
                    break;
            }
        }

        private Unit(Unit oldUnit)
        {
            this.name = oldUnit.Name;
            this.identity = oldUnit.identity;
            this.baseProperties = oldUnit.baseProperties;
            this.currentProperties = oldUnit.currentProperties;
            this.statusEffects = oldUnit.statusEffects;
            this.time = oldUnit.time;
            this.faction = oldUnit.faction;
            this.riskCalculatingFunction = oldUnit.riskCalculatingFunction;
        }

        private Unit(UnitProperties properties, Unit oldUnit) : this(oldUnit)
        {
            this.currentProperties = properties;
        }

        private Unit(UnitStatusEffects statusEffects, Unit oldUnit) : this(oldUnit)
        {
            this.statusEffects = statusEffects;
        }

        private Unit(int time, Unit oldUnit) : this(oldUnit)
        {
            this.time = time;
        }

        public static Unit CreateNewUnit(UnitProperties properties, UnitFaction faction = UnitFaction.Friendly)
        {
            return new Unit("DUMMY_NO_NAME", properties, faction);
        }

        public static Unit CreateNewUnit(UnitProperties properties, int time, UnitFaction faction)
        {
            return new Unit("DUMMY_NO_NAME", properties, time, faction);
        }

        public static Unit CreateNewUnit(string name, UnitProperties properties, int time = 0, UnitFaction faction = UnitFaction.Friendly)
        {
            return new Unit(name, properties, time, faction);
        }

        public static Unit CreateNewUnit(string name, UnitProperties properties, int time, UnitFaction faction, int risk, RiskCalculationMethod method)
        {
            return new Unit(name, properties, time, faction, risk, method);
        }

        public Unit ChangeAttack(int newAttack)
        {
            UnitProperties newUnitProperties = currentProperties;
            newUnitProperties.Attack = newAttack;

            return new Unit(newUnitProperties, this);
        }

        public Unit ChangeDefence(int newDefence)
        {
            UnitProperties newUnitProperties = currentProperties;
            newUnitProperties.Defence = newDefence;

            return new Unit(newUnitProperties, this);
        }

        public Unit ChangeRange(int newRange)
        {
            UnitProperties newUnitProperties = currentProperties;
            newUnitProperties.Range = newRange;

            return new Unit(newUnitProperties, this);
        }

        public Unit ChangeHealth(int newHealth)
        {
            UnitProperties newUnitProperties = currentProperties;
            newUnitProperties.CurrentHealth = Mathf.Clamp(newHealth, 0, currentProperties.MaxHealth);

            return new Unit(newUnitProperties, this);
        }

        public Unit ChangeActionPoints(int newActionPoints)
        {
            UnitProperties newUnitProperties = currentProperties;
            newUnitProperties.CurrentActionPoints = Mathf.Clamp(newActionPoints, 0, MaxActionPoints);

            return new Unit(newUnitProperties, this);
        }

        public Unit ChangeTime(int newTime)
        {
            return new Unit(newTime, this);
        }

        public UnitStatusEffects GetUnitStatusEffects()
        {
            return statusEffects;
        }

        public bool HasStatusEffect(UnitStatusEffects effects)
        {
            return statusEffects.HasFlag(effects);
        }

        public Unit ApplyStatusEffect(UnitStatusEffects effect)
        {
            return new Unit(statusEffects | effect, this);
        }

        public Unit RemoveStatusEffect(UnitStatusEffects effect)
        {
            return new Unit(statusEffects & ~effect, this);
        }

        public override int GetHashCode()
        {
            return identity;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Unit))
            {
                return false;
            }

            return identity == ((Unit)obj).identity;
        }

        public override string ToString()
        {
            return name;
        }

    }
}
