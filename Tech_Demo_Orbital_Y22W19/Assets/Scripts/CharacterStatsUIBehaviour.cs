using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CombatSystem.Entities;

public class CharacterStatsUIBehaviour : MonoBehaviour
{
    [SerializeField]
    private Image characterAvatar;

    [SerializeField]
    private TMP_Text characterName;

    [SerializeField]
    private BarFillBehaviour healthBar;

    [SerializeField]
    private BarFillBehaviour actionPointsBar;

    [SerializeField]
    private TMP_Text defenceValueText;

    [SerializeField]
    private TMP_Text attackValueText;

    [SerializeField]
    private TMP_Text speedValueText;

    public void SetAvatar(Sprite sprite)
    {
        characterAvatar.sprite = sprite;
    }

    public void SetHealthBar(int currentHealth, int totalHealth)
    {
        healthBar.UpdateBarFillImage(currentHealth, totalHealth);
    }

    public void SetActionPointsBar(int currentActionPoints, int totalActionPoints)
    {
        actionPointsBar?.UpdateBarFillImage(currentActionPoints, totalActionPoints);
    }

    public void SetName(string name)
    {
        characterName.text = name;
    }

    public void SetDefenceValue(int defenceValue)
    {
        defenceValueText.text = defenceValue.ToString();
    }

    public void SetAttackValue(int attackValue)
    {
        attackValueText.text = attackValue.ToString();
    }

    public void SetSpeedValue(int speedValue)
    {
        speedValueText.text = speedValue.ToString();
    }

    public void SetUnitStats(UnitOld unit)
    {
        SetHealthBar(unit.Health, unit.MaxHealth);
        SetActionPointsBar(unit.ActionPointsLeft, unit.MaxActionPoints);
        SetDefenceValue(unit.Defence);
        SetAttackValue(unit.Attack);
        SetSpeedValue(unit.Speed);
    }

    public void SetUnitStats(Unit unit)
    {
        SetHealthBar(unit.CurrentHealth, unit.MaxHealth);
        SetActionPointsBar(unit.CurrentActionPoints, unit.MaxActionPoints);
        SetDefenceValue(unit.Defence);
        SetAttackValue(unit.Attack);
        SetSpeedValue(-1);
    }
}
