using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitBehaviour : MonoBehaviour
{
    [SerializeField]
    private Sprite characterHeadAvatar;
    [SerializeField]
    private UnitData originalUnitData;
    [SerializeField]
    private UnitData currentUnitData;

    [SerializeField]
    private bool isCompletelyNew;

    public UnitOld InitialiseUnit(int startingTime)
    {
        if (isCompletelyNew)
        {
            return new UnitOld(name, characterHeadAvatar, originalUnitData).AddTime(startingTime);
        }
        else
        {
            return new UnitOld(name, characterHeadAvatar, originalUnitData, currentUnitData).AddTime(startingTime);
        }
    }

    public Faction Faction => currentUnitData.Faction;

    public Sprite CharacterAvatar => characterHeadAvatar;
}
