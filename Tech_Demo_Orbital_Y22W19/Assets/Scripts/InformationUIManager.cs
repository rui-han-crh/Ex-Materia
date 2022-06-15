using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class InformationUIManager : MonoBehaviour
{
    private static readonly int ONE_HUNDRED_PERCENT = 100;
    private static readonly string TWO_DECIMAL_PLACES = "n2";

    private static InformationUIManager instance;

    public static InformationUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<InformationUIManager>();
            }
            return instance;
        }
    }

    [SerializeField]
    private CommandButtonsBehaviour commandButtonsBehaviour;

    [SerializeField]
    private CharacterStatsUIBehaviour opponentUIBehaviour;

    public TMP_Text TimeNeededText => commandButtonsBehaviour.TimeText;
    public TMP_Text ActionPointText => commandButtonsBehaviour.ActionPointText;

    [SerializeField]
    private TMP_Text resultantDamageText;

    [SerializeField]
    private TMP_Text chanceToHitText;


    private void Awake()
    {
        opponentUIBehaviour.gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        TurnAllUIOff();
    }

    public void SetOpponentDetails(UnitOld unit)
    {
        opponentUIBehaviour.SetName(unit.Name);
        opponentUIBehaviour.SetAvatar(unit.CharacterHeadAvatar);
        opponentUIBehaviour.SetUnitStats(unit);
    }

    public void SetResultantDamageDealt(int resultantDamage)
    {
        resultantDamageText.text = resultantDamage.ToString();
    }

    public void SetChanceToHitText(float chanceToHit)
    {
        chanceToHitText.text = (chanceToHit * ONE_HUNDRED_PERCENT).ToString(TWO_DECIMAL_PLACES);
    }

    public void WaitButtonActivated()
    {
        TurnAllUIOff();
        commandButtonsBehaviour.SetButtonActive(GameManager.Command.Wait, true);
        commandButtonsBehaviour.SetPromptActive(CommandButtonsBehaviour.Prompt.Time, true);
        commandButtonsBehaviour.SetPromptActive(CommandButtonsBehaviour.Prompt.ActionPoints, true);
    }

    public void MoveButtonActivated()
    {
        TurnAllUIOff();
        commandButtonsBehaviour.SetButtonActive(GameManager.Command.Movement, true);
        commandButtonsBehaviour.SetPromptActive(CommandButtonsBehaviour.Prompt.Time, true);
        commandButtonsBehaviour.SetPromptActive(CommandButtonsBehaviour.Prompt.ActionPoints, true);
    }

    public void CombatButtonActivated()
    {
        TurnAllUIOff();
        commandButtonsBehaviour.SetButtonActive(GameManager.Command.Attack, true);
        commandButtonsBehaviour.SetPromptActive(CommandButtonsBehaviour.Prompt.Time, true);
        commandButtonsBehaviour.SetPromptActive(CommandButtonsBehaviour.Prompt.ActionPoints, true);
    }

    public void OverwatchButtonActivated()
    {
        TurnAllUIOff();

        TimeNeededText.text = (OverwatchRequest.TIME_CONSUMED).ToString();

        commandButtonsBehaviour.SetButtonActive(GameManager.Command.Overwatch, true);
        commandButtonsBehaviour.SetPromptActive(CommandButtonsBehaviour.Prompt.Time, true);
        commandButtonsBehaviour.SetPromptActive(CommandButtonsBehaviour.Prompt.ActionPoints, true);
    }

    public void TurnAllUIOff()
    {
        SetAllTextToDefault();
        commandButtonsBehaviour.DisableAllUI();
    }

    public void SetAllTextToDefault()
    {
        commandButtonsBehaviour.SetPromptTextToDefault();
    }

    public void SetTimeAndAPRequiredText(int timeNeeded, int apNeeded)
    {
        commandButtonsBehaviour.TimeText.text = timeNeeded.ToString();
        commandButtonsBehaviour.ActionPointText.text = apNeeded.ToString();
    }

    public void SetTimeAndAPRequiredText(int apNeeded)
    {
        commandButtonsBehaviour.TimeText.text = Mathf.CeilToInt((float)apNeeded / GameManager.Instance.CurrentUnit.Speed).ToString();
        commandButtonsBehaviour.ActionPointText.text = apNeeded.ToString();
    }
}
