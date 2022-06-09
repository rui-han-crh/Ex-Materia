using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct UnitData
{
    public int Health;
    public int UnitSpeed;
    public int Attack;
    public int Defence;
    public int Risk;
    public int Time;
    public int ActionPoints;
    public Faction Faction;
}