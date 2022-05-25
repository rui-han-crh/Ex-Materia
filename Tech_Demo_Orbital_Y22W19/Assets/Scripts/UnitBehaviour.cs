using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBehaviour : MonoBehaviour
{
    [SerializeField]
    private int health = 100;

    [SerializeField]
    private int unitSpeed = 1;

    [SerializeField]
    private int attack = 20;

    [SerializeField]
    private int defence = 10;

    [SerializeField]
    private int risk = 10;

    [SerializeField]
    private Sprite characterHeadAvatar;

    [SerializeField]
    private int actionPointsPerTurn;

    [SerializeField]
    private Faction faction;

    public Unit InitialiseUnit(int startingTime)
    {
        return new Unit(name, health, faction, characterHeadAvatar, attack, defence, unitSpeed, risk, actionPointsPerTurn).AddTime(startingTime);
    }

    public Faction Faction => faction;

    public Sprite CharacterAvatar => characterHeadAvatar;
}
