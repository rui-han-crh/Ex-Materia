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

    private readonly Sprite characterHeadAvatar;


    private readonly UnitStatusEffects unitStatusEffects;
    private readonly UnitData originalUnitData;
    private readonly UnitData currentUnitData;

    public Unit(string name, Sprite avatar, UnitData originalData, UnitData currentData)
    {
        this.name = name;
        this.characterHeadAvatar = avatar;
        originalUnitData = originalData;
        currentUnitData = currentData;
        unitStatusEffects = default;
    }

    public Unit(string name, Sprite avatar, UnitData originalData) : this(name, avatar, originalData, originalData)
    {
    }

    private Unit(Unit oldUnit, int newHealth, int newTime, int newActionPoints)
    {
        this.name = oldUnit.name;
        this.characterHeadAvatar = oldUnit.CharacterHeadAvatar;
        this.originalUnitData = oldUnit.originalUnitData;
        this.currentUnitData = oldUnit.currentUnitData;

        this.currentUnitData.Health = newHealth;
        this.currentUnitData.Time = newTime;
        this.currentUnitData.ActionPoints = Mathf.Clamp(newActionPoints, 0, oldUnit.MaxActionPoints);

        this.unitStatusEffects = oldUnit.unitStatusEffects;
    }

    private Unit (Unit oldUnit)
    {
        this.name = oldUnit.name;
        this.characterHeadAvatar = oldUnit.CharacterHeadAvatar;
        this.originalUnitData = oldUnit.originalUnitData;
        this.currentUnitData = oldUnit.currentUnitData;

        this.unitStatusEffects = oldUnit.unitStatusEffects;
    }

    private Unit(Unit oldUnit, UnitStatusEffects unitStatusEffects)
    {
        this.name = oldUnit.name;
        this.characterHeadAvatar = oldUnit.CharacterHeadAvatar;
        this.originalUnitData = oldUnit.originalUnitData;
        this.currentUnitData = oldUnit.currentUnitData;

        this.unitStatusEffects = unitStatusEffects;
    }

    public Faction Faction => currentUnitData.Faction;

    public string Name => name;

    public int MaxHealth => originalUnitData.Health;
    public int MaxActionPoints => originalUnitData.ActionPoints;

    public int Time => currentUnitData.Time;

    public int Speed => currentUnitData.UnitSpeed;

    public int Attack => currentUnitData.Attack;

    public int Defence => currentUnitData.Defence;

    public int Range => 7;

    public int Risk => currentUnitData.Risk;

    public int ActionPointsLeft => currentUnitData.ActionPoints;

    public UnitStatusEffects UnitStatusEffects => unitStatusEffects;

    public Sprite CharacterHeadAvatar => characterHeadAvatar;

    public int ActionPointsUsed => MaxActionPoints - ActionPointsLeft;

    public int Health => currentUnitData.Health;

    public Unit DecreaseHealth(int decreaseAmount)
    {
        return new Unit(this,
                        Mathf.Clamp(Health - decreaseAmount, 0, MaxHealth),
                        Time,
                        ActionPointsLeft);
    }

    public Unit IncreaseHealth(int increaseAmount)
    {
        return DecreaseHealth(-increaseAmount);
    }

    public Unit UseActionPoints(int amountUsed)
    {
        return new Unit(this, Health, Time + amountUsed * Speed, ActionPointsLeft - amountUsed);
    }

    public Unit ResetActionPoints()
    {
        return new Unit(this, Health, Time, MaxActionPoints);
    }

    public Unit ResetUnitToOriginal()
    {
        return new Unit(Name, CharacterHeadAvatar, originalUnitData);
    }

    public Unit ReplenishActionPoints(int amountToReplenish)
    {
        Debug.Log("Replenishing AP: " + amountToReplenish.ToString());
        return new Unit(this, Health, Time, ActionPointsLeft + amountToReplenish);
    }

    public Unit AddTime(int amount)
    {
        return new Unit(this, Health, Time + amount, ActionPointsLeft);
    }

    public Unit ApplyStatus(UnitStatusEffects.Status status)
    {
        UnitStatusEffects newStatus = unitStatusEffects.ApplyStatus(status);
        return new Unit(this, newStatus);
    }

    public Unit RemoveStatus(UnitStatusEffects.Status status)
    {
        UnitStatusEffects newStatus = unitStatusEffects.RemoveStatus(status);
        return new Unit(this, newStatus);
    }

    public int CompareTo(Unit other)
    {
        return this.Time - other.Time;
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
        return $"{name} : HP = {Health}, AP = {ActionPointsLeft}, TIME = {Time}";
    }

    public Unit Clone()
    {
        return new Unit(this);
    }
}