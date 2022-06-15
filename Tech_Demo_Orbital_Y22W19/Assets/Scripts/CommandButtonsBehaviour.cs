using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommandButtonsBehaviour : MonoBehaviour
{
    private static CommandButtonsBehaviour instance;

    public static CommandButtonsBehaviour Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<CommandButtonsBehaviour>();
            }
            return instance;
        }
    }

    public enum Prompt
    {
        Time,
        ActionPoints
    }

    [SerializeField]
    private GameObject moveButtonGameObject;

    [SerializeField]
    private GameObject attackButtonGameObject;

    [SerializeField]
    private GameObject overwatchButtonGameObject;

    [SerializeField]
    private GameObject waitButtonGameObject;

    [SerializeField]
    private GameObject timeUIGameObject;

    [SerializeField]
    private GameObject apUIGameObject;

    private TMP_Text apText;
    private TMP_Text timeText;

    public TMP_Text ActionPointText => apText;
    public TMP_Text TimeText => timeText;

    private void Awake()
    {
        apText = apUIGameObject.GetComponentInChildren<TMP_Text>();
        timeText = timeUIGameObject.GetComponentInChildren<TMP_Text>();
    }

    public void SetButtonActive(GameManagerOld.Command command, bool isActive)
    {
        switch (command)
        {
            case GameManagerOld.Command.Attack:
                attackButtonGameObject.SetActive(isActive);
                break;

            case GameManagerOld.Command.Movement:
                moveButtonGameObject.SetActive(isActive);
                break;

            case GameManagerOld.Command.Wait:
                waitButtonGameObject.SetActive(isActive);
                break;

            case GameManagerOld.Command.Overwatch:
                overwatchButtonGameObject.SetActive(isActive);
                break;
        }
    }

    public void SetPromptActive(Prompt prompt, bool isActive)
    {
        switch (prompt)
        {
            case Prompt.Time:
                timeUIGameObject.SetActive(isActive);
                break;

            case Prompt.ActionPoints:
                apUIGameObject.SetActive(isActive);
                break;
        }
    }

    public void SetPromptTextToDefault()
    {
        ActionPointText.text = "0";
        timeText.text = "0";
    }

    public void DisableAllUI()
    {
        attackButtonGameObject.SetActive(false);
        moveButtonGameObject.SetActive(false);
        waitButtonGameObject.SetActive(false);
        overwatchButtonGameObject.SetActive(false);
        apUIGameObject.SetActive(false);
        timeUIGameObject.SetActive(false);
    }

}