using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public GameObject APNeededUI => apUIGameObject;

    [SerializeField]
    private GameObject timeUIGameObject;

    public GameObject TimeUI => timeUIGameObject;

    [SerializeField]
    private GameObject aimUIGameObject;

    [SerializeField]
    private GameObject moveButtonGameObject;

    [SerializeField]
    private GameObject attackButtonGameObject;

    [SerializeField]
    private GameObject overwatchButtonGameObject;

    [SerializeField]
    private GameObject waitButtonGameObject;

    private void OnDisable()
    {
        TurnAllUIOff();
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
        Debug.Log("Plong");
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
    }
}
