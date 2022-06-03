using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InformationUIManager : MonoBehaviour
{
    // so much hard coding ughhhhh >:(

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
    private GameObject apUIGameObject;
    private TMP_Text apNeededText;


    [SerializeField]
    private GameObject timeUIGameObject;
    private TMP_Text timeNeededText;

    [SerializeField]
    private GameObject aimUIGameObject;
    private TMP_Text chanceToHitText;

    [SerializeField]
    private GameObject moveButtonGameObject;

    [SerializeField]
    private GameObject attackButtonGameObject;

    [SerializeField]
    private GameObject overwatchButtonGameObject;

    [SerializeField]
    private GameObject waitButtonGameObject;

    public TMP_Text TimeNeededText => timeNeededText;
    public TMP_Text APNeededText => apNeededText;
    public GameObject TimeUI => timeUIGameObject;
    public TMP_Text ChanceToHitText => chanceToHitText;

    private void OnDisable()
    {
        TurnAllUIOff();
    }

    private void Awake()
    {
        chanceToHitText = aimUIGameObject.GetComponentInChildren<TMP_Text>();
        timeNeededText = timeUIGameObject.GetComponentInChildren<TMP_Text>();
        apNeededText = apUIGameObject.GetComponentInChildren<TMP_Text>();
    }

    public void WaitButtonActivated()
    {
        TurnAllUIOff();
        waitButtonGameObject.SetActive(true);
        apUIGameObject.SetActive(true);
        timeUIGameObject.SetActive(true);
    }

    public void MoveButtonActivated()
    {
        TurnAllUIOff();
        moveButtonGameObject.SetActive(true);
        apUIGameObject.SetActive(true);
        timeUIGameObject.SetActive(true);
    }

    public void CombatButtonActivated()
    {
        TurnAllUIOff();
        attackButtonGameObject.SetActive(true);
        apUIGameObject.SetActive(true);
        aimUIGameObject.SetActive(true);
        timeUIGameObject.SetActive(true);
    }

    public void OverwatchButtonActivated()
    {
        TurnAllUIOff();

        InformationUIManager.Instance.TimeNeededText.text =
            (OverwatchRequest.TIME_CONSUMED).ToString();

        overwatchButtonGameObject.SetActive(true);
        apUIGameObject.SetActive(true);
        timeUIGameObject.SetActive(true);
    }

    public void TurnAllUIOff()
    {
        apUIGameObject.SetActive(false);
        timeUIGameObject.SetActive(false);
        aimUIGameObject.SetActive(false);
        moveButtonGameObject.SetActive(false);
        attackButtonGameObject.SetActive(false);
        overwatchButtonGameObject.SetActive(false);
        waitButtonGameObject.SetActive(false);
        SetAllTextToDefault();
    }

    public void SetAllTextToDefault()
    {
        chanceToHitText.text = "0";
        APNeededText.text = "0";
        timeNeededText.text = "0";
    }
}
