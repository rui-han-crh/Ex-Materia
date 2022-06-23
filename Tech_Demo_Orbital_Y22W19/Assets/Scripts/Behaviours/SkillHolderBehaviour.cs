using CombatSystem.Effects;
using CombatSystem.Facade;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillHolderBehaviour : MonoBehaviour
{
    [SerializeField]
    private Image skillIcon;

    [SerializeField]
    private TMP_Text description;

    [SerializeField]
    private TMP_Text costField;

    public void UpdateDescription(string descriptionText)
    {
        description.text = descriptionText;
    }

    public void UpdateIcon(Sprite sprite)
    {
        skillIcon.sprite = sprite;
        skillIcon.preserveAspect = true;
    }

    public void UpdateCost(int cost)
    {
        costField.text = cost.ToString();
    }

    public void SetWithStatusEffect(StatusEffect skill)
    {
        UpdateIcon(skill.Icon);
        UpdateDescription(skill.Description);
        UpdateCost(skill.SkillPointCost);
    }

    public void SetWithStatusEffect(string skillName)
    {
        SetWithStatusEffect(UnitStatusEffectsFacade.Instance.GetEffect(skillName));
    }
}
