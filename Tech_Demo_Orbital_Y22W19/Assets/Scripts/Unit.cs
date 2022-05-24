using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// Fully immutable struct representing unit data
/// </summary>
public struct Unit : IComparable<Unit>
{
    private readonly string name;
    private readonly int time;

    private readonly int maximumActionPoints;

    private readonly Sprite characterHeadAvatar;

    private readonly int actionPointsUsed;

    private readonly int maxHealth;

    private readonly int unitSpeed;
    private readonly int risk;

    private readonly int health;
    private readonly int attack;
    private readonly int defence;

    private readonly UnitStatusEffects unitStatusEffects;

    private readonly Faction faction;


    public Unit(string name, 
                int startingHealth, 
                Faction faction, 
                Sprite characterHeadAvatar,
                int attack,
                int defence,
                int unitSpeed,
                int confidence,
                int maximumActionPoints,
                bool onOverwatch = false)
    {
        this.name = name;
        this.maxHealth = startingHealth;
        this.health = startingHealth;
        this.characterHeadAvatar = characterHeadAvatar;
        this.time = 0;
        this.attack = attack;
        this.defence = defence;
        this.unitSpeed = unitSpeed;
        this.risk = confidence;
        this.maximumActionPoints = maximumActionPoints;
        this.faction = faction;
        this.actionPointsUsed = 0;

        this.unitStatusEffects = new UnitStatusEffects();
    }

    private Unit(Unit oldUnit, int newHealth, int newTime, int actionPointsUsed)
    {
        this.name = oldUnit.name;
        this.faction = oldUnit.Faction;
        this.characterHeadAvatar = oldUnit.CharacterHeadAvatar;
        this.attack = oldUnit.Attack;
        this.defence = oldUnit.Defence;
        this.maximumActionPoints = oldUnit.maximumActionPoints;
        this.maxHealth = oldUnit.maxHealth;
        this.unitSpeed = oldUnit.unitSpeed;
        this.risk = oldUnit.risk;

        this.health = newHealth;
        this.time = newTime;
        this.actionPointsUsed = actionPointsUsed;

        this.unitStatusEffects = oldUnit.unitStatusEffects;
    }

    private Unit (Unit oldUnit)
    {
        this.name = oldUnit.name;
        this.faction = oldUnit.Faction;
        this.characterHeadAvatar = oldUnit.CharacterHeadAvatar;
        this.attack = oldUnit.Attack;
        this.defence = oldUnit.Defence;
        this.maximumActionPoints = oldUnit.maximumActionPoints;
        this.maxHealth = oldUnit.maxHealth;
        this.health = oldUnit.health;
        this.time = oldUnit.time;
        this.unitSpeed = oldUnit.unitSpeed;
        this.risk = oldUnit.risk;
        this.actionPointsUsed = oldUnit.actionPointsUsed;

        this.unitStatusEffects = oldUnit.unitStatusEffects;
    }

    private Unit(Unit oldUnit, UnitStatusEffects unitStatusEffects)
    {
        this.name = oldUnit.name;
        this.faction = oldUnit.Faction;
        this.characterHeadAvatar = oldUnit.CharacterHeadAvatar;
        this.attack = oldUnit.Attack;
        this.defence = oldUnit.Defence;
        this.maximumActionPoints = oldUnit.maximumActionPoints;
        this.maxHealth = oldUnit.maxHealth;
        this.health = oldUnit.health;
        this.time = oldUnit.time;
        this.unitSpeed = oldUnit.unitSpeed;
        this.risk = oldUnit.risk;
        this.actionPointsUsed = oldUnit.actionPointsUsed;

        this.unitStatusEffects = unitStatusEffects;
    }

    public Faction Faction => faction;

    public string Name => name;

    public int MaxHealth => maxHealth;
    public int MaxActionPoints => maximumActionPoints;

    public int Time => time;

    public int Speed => unitSpeed;

    public int Attack => attack;

    public int Defence => defence;

    public int Range => 7;

    public int Risk => risk;

    public int ActionPointsLeft => maximumActionPoints - actionPointsUsed;

    public Sprite CharacterHeadAvatar => characterHeadAvatar;

    public int ActionPointsUsed => actionPointsUsed;

    public int Health => health;

    public Unit DecreaseHealth(int decreaseAmount)
    {
        return new Unit(this,
                        Mathf.Clamp(health - decreaseAmount, 0, maxHealth),
                        Time,
                        ActionPointsUsed);
    }

    public Unit IncreaseHealth(int increaseAmount)
    {
        return DecreaseHealth(-increaseAmount);
    }

    public Unit UseActionPoints(int amount)
    {
        return new Unit(this,
                        Health,
                        time + amount * unitSpeed,
                        actionPointsUsed + amount);
    }

    public Unit ResetActionPoints()
    {
        return new Unit(this,
                        Health,
                        time,
                        0);
    }

    public Unit AddTime(int amount)
    {
        return new Unit(this, Health, time + amount, actionPointsUsed);
    }

    public Unit ApplyOverwatchStatus()
    {
        UnitStatusEffects newStatus = new UnitStatusEffects();
        newStatus.OnOverwatch = true;
        return new Unit(this, newStatus);
    }

    public int CompareTo(Unit other)
    {
        return this.time - other.time;
    }

    /// <summary>
    /// Units are the same if their names are the same
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (!(obj is Unit))
        {
            return false;
        }
        return name.Equals(((Unit)obj).name);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }

    public override string ToString()
    {
        return $"{name} : HP = {health}, AP = {ActionPointsLeft}, TIME = {time}";
    }

    public Unit Clone()
    {
        return new Unit(this);
    }
}