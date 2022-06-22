using System;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour //should be able to interact with yarn also
{

    public event Action OnEnded = delegate { };

    [SerializeField]
    public string startNode;

    //this int is basically what thing it's on now

    private int currentStage;

    //stages are basically dialgoueNodes

    private string[] stageNames = new string[] { "Start", "IntroTutorial", "MoveTutorial", "ShootTutorial", "DuckTutorial" };


    ////icon positions to serialize
    //[SerializeField]
    //public Button MovementIcon;
    //[SerializeField]
    //public Button CombatIcon;
    //[SerializeField]
    //public Button OverwatchIcon;

    [SerializeField]
    public DialogueRunner dr;

    [SerializeField]
    public GameObject DialogueCanvas;

    [SerializeField]
    public Button[] buttonActions;

    [SerializeField]
    public GameObject actionUI;

    private bool isInConfirmed = false;


    public void Start()
    {
        //YarnManager.Instance.AddDialogueManager(dr);
        //BeginConvo("StartEvelynAndOlivia");
        //1) Disable all buttons 
        foreach (Button button in buttonActions)
        {
            button.gameObject.SetActive(false); //shouldn't be able to click a single button 
        }
        buttonActions[0].gameObject.SetActive(true);
        //actionUI.SetActive(false);
        //startButton.enabled = true;
        //DialogueCanvas.SetActive(true);
        //YarnManager.Instance.StartConvoAuto("WrongOption");
        //DialogueCanvas.SetActive(false);
        //StartStage(currentStage);
    }

    //Essentially starts the stage 

    private void StartStage(int stageNumber)
    {
        //if(stageNumber == 0)
        //{
        //    DialogueCanvas.SetActive(true);
        //    YarnManager.Instance.StartConvoAuto("OliviaIntro");
        //}
    }

    public void BeginConvo(string thisNode)
    {
        DialogueCanvas.gameObject.SetActive(true);
        YarnManager.Instance.StartConvoAuto(thisNode);
        YarnManager.Instance.OnEnded += InvokeDelegate;
    }

    private void InvokeDelegate()
    {
        DialogueCanvas.gameObject.SetActive(false);
        OnEnded();
        FlushEventHandlers();
    }

    public void FlushEventHandlers()
    {
        OnEnded = delegate { };
    }


    public void detectRaycast()
    {
        //if it has been confirmed, when you double click anywhere,
        //It is a confirmed message to attack / interact with something!
    }

    //Potential bug: I don't know how to cancel 
    public void AwaitingConfirm()
    {
        isInConfirmed = true;
        //Debug.Log("the truth will set you free");
    }





}
