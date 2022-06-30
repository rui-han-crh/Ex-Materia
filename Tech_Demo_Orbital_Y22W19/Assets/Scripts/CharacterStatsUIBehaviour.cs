using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CombatSystem.Entities;
using Facades;
using Managers;

public class CharacterStatsUIBehaviour : MonoBehaviour
{
    [SerializeField]
    private Image characterAvatar;

    [SerializeField]
    private TMP_Text characterName;

    [SerializeField]
    private BarFillBehaviour healthBar;

    [SerializeField]
    private TMP_Text defenceValueText;

    [SerializeField]
    private TMP_Text attackValueText;

    [SerializeField]
    private TMP_Text speedValueText;

    [SerializeField]
    private BarFillBehaviour skillPointsBar;

    [SerializeField]
    private SkillHolderBehaviour basicSkillHolder;

    [SerializeField]
    private SkillHolderBehaviour ultimateSkillHolder;

    public void SetAvatar(Sprite sprite)
    {
        characterAvatar.sprite = sprite;
    }

    public void SetHealthBar(int currentHealth, int totalHealth)
    {
        healthBar.UpdateBarFillImage(currentHealth, totalHealth);
    }

    public void SetSkillPointsBar(int currentSkillPoints, int totalSkillPoints)
    {
        skillPointsBar?.UpdateBarFillImage(currentSkillPoints, totalSkillPoints);
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

    public void SetActionPointsBar(int a, int b)
    { 
        // dead code
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
        SetName(unit.Name);
        SetAvatar(UnitManager.Instance[unit.Identity].CharacterAvatar);
        SetHealthBar(unit.CurrentHealth, unit.MaxHealth);
        SetSkillPointsBar(unit.CurrentSkillPoints, unit.MaxSkillPoints);
        SetDefenceValue(unit.Defence);
        SetAttackValue(unit.Attack);
        SetSpeedValue(unit.Range);

        basicSkillHolder?.SetWithStatusEffect(unit.BasicSkillName);
        ultimateSkillHolder?.SetWithStatusEffect(unit.UltimateSkillName);
    }
}
