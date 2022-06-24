using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

/// <summary>
/// Fully immutable struct representing unit data
/// </summary>
public struct UnitOld : IComparable<UnitOld>
{
    private readonly string name;

    private readonly Sprite characterHeadAvatar;


    private readonly UnitStatusEffects unitStatusEffects;
    private readonly UnitData originalUnitData;
    private readonly UnitData currentUnitData;

    public UnitOld(string name, Sprite avatar, UnitData originalData, UnitData currentData)
    {
        this.name = name;
        this.characterHeadAvatar = avatar;
        originalUnitData = originalData;
        currentUnitData = currentData;
        unitStatusEffects = default;
    }

    public UnitOld(string name, Sprite avatar, UnitData originalData) : this(name, avatar, originalData, originalData)
    {
    }

    private UnitOld(UnitOld oldUnit, int newHealth, int newTime, int newActionPoints)
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

    private UnitOld (UnitOld oldUnit)
    {
        this.name = oldUnit.name;
        this.characterHeadAvatar = oldUnit.CharacterHeadAvatar;
        this.originalUnitData = oldUnit.originalUnitData;
        this.currentUnitData = oldUnit.currentUnitData;

        this.unitStatusEffects = oldUnit.unitStatusEffects;
    }

    private UnitOld(UnitOld oldUnit, UnitStatusEffects unitStatusEffects)
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

    public UnitOld DecreaseHealth(int decreaseAmount)
    {
        return new UnitOld(this,
                        Mathf.Clamp(Health - decreaseAmount, 0, MaxHealth),
                        Time,
                        ActionPointsLeft);
    }

    public UnitOld IncreaseHealth(int increaseAmount)
    {
        return DecreaseHealth(-increaseAmount);
    }

    public UnitOld UseActionPoints(int amountUsed)
    {
        return new UnitOld(this, Health, Time + amountUsed * Speed, ActionPointsLeft - amountUsed);
    }

    public UnitOld ResetActionPoints()
    {
        return new UnitOld(this, Health, Time, MaxActionPoints);
    }

    public UnitOld ResetUnitToOriginal()
    {
        return new UnitOld(Name, CharacterHeadAvatar, originalUnitData);
    }

    public UnitOld ReplenishActionPoints(int amountToReplenish)
    {
        Debug.Log("Replenishing AP: " + amountToReplenish.ToString());
        return new UnitOld(this, Health, Time, ActionPointsLeft + amountToReplenish);
    }

    public UnitOld AddTime(int amount)
    {
        return new UnitOld(this, Health, Time + amount, ActionPointsLeft);
    }

    public UnitOld ApplyStatus(UnitStatusEffects.Status status)
    {
        UnitStatusEffects newStatus = unitStatusEffects.ApplyStatus(status);
        return new UnitOld(this, newStatus);
    }

    public UnitOld RemoveStatus(UnitStatusEffects.Status status)
    {
        UnitStatusEffects newStatus = unitStatusEffects.RemoveStatus(status);
        return new UnitOld(this, newStatus);
    }

    public int CompareTo(UnitOld other)
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
        if (!(obj is UnitOld))
        {
            return false;
        }
        return name.Equals(((UnitOld)obj).name);
    }

    public override int GetHashCode()
    {
        return name.GetHashCode();
    }

    public override string ToString()
    {
        return $"{name} : HP = {Health}, AP = {ActionPointsLeft}, TIME = {Time}";
    }

    public UnitOld Clone()
    {
        return new UnitOld(this);
    }
}