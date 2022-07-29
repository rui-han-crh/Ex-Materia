using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitMenuBehaviour : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private TMP_Text timeText;

    [SerializeField]
    private TMP_Text skillPointsText;

    [SerializeField]
    private Button exitButton;

    [SerializeField]
    private Button confirmButton;

    private FadeAnimation fadeAnimation;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        fadeAnimation = GetComponent<FadeAnimation>();
        canvasGroup = GetComponent<CanvasGroup>();

        slider.onValueChanged.AddListener(value => { 
            OnSliderValueChanged(value);
            CombatSceneManager.Instance.SetTimeToWait((int)value);
        });

        exitButton.onClick.AddListener(() => { fadeAnimation.SetAnimationState(false); canvasGroup.interactable = false; });
        confirmButton.onClick.AddListener(() => {
            fadeAnimation.SetAnimationState(false);
            canvasGroup.interactable = false;
            CombatSceneManager.Instance.SelectCommand(CombatSceneManager.CommandType.Wait);
        });
    }

    public void SetTimeText(int time)
    {
        timeText.text = time.ToString();
    }

    public void SetSkillPointsText(int skillPoints)
    {
        skillPointsText.text = skillPoints.ToString();
    }

    public void CancelMenu()
    {
        exitButton.onClick.Invoke();
    }

    private void OnSliderValueChanged(float value)
    {
        timeText.text = value.ToString();
        skillPointsText.text = WaitRequest.ConvertTimeToSP((int)value).ToString();
    }

}
