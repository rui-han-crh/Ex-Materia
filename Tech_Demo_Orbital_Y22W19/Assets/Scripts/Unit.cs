using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
public class Unit : IComparable<Unit>
{
    private readonly string name;
    private int exhaustion;

    private readonly int maximumActionPoints;

    private Sprite characterHeadAvatar;
    public UnitBehaviour behaviour;

    private int actionPointsUsed;

    private readonly int maxHealth;

    private int health;
    private int attack;
    private int defence;

    private Faction faction;


    public Unit(UnitBehaviour behaviour, 
                string name, 
                int health, 
                Faction faction, 
                Sprite characterHeadAvatar, 
                int exhaustion,
                int attack,
                int defence,
                int maximumActionPoints)
    {
        this.name = name;
        this.maxHealth = health;
        this.health = health;
        this.characterHeadAvatar = characterHeadAvatar;
        this.exhaustion = exhaustion;
        this.attack = attack;
        this.defence = defence;
        this.maximumActionPoints = maximumActionPoints;
        this.behaviour = behaviour;
        this.faction = faction;
    }

    public Faction Faction => faction;

    public int Exhaustion => exhaustion;

    public int Attack => attack;

    public int Defence => defence;

    public int Range => 5;

    public int ActionPointsLeft => maximumActionPoints - actionPointsUsed;

    public Sprite CharacterHeadAvatar => characterHeadAvatar;

    public Vector3 WorldPosition
    {
        get { return behaviour.transform.position; }
        set { behaviour.transform.position = value;  }
    }

    public int ActionPointsUsed => actionPointsUsed;

    public UnitBehaviour Behaviour => behaviour;

    public int Health => health;

    public void ChangeHealth(int changeInHealth)
    {
        health = Mathf.Clamp(health + changeInHealth, 0, maxHealth);
    }

    public void UseActionPoints(int amount)
    {
        actionPointsUsed = actionPointsUsed + amount;
        exhaustion += amount;
    }

    public void ResetActionPoints()
    {
        actionPointsUsed = 0;
    }

    public int CompareTo(Unit other)
    {
        return this.exhaustion - other.exhaustion;
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
        return name;
    }
}