using System;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class TutorialManager : MonoBehaviour //should be able to interact with yarn also
{


    private const float DOUBLE_CLICK_TIME = .2f;
    private float lastClickTime;

    public event Action OnEnded = delegate { };

    [SerializeField]
    public Camera MainCamera;
    [SerializeField]
    public string startNode;

    //this int is basically what thing it's on now

    private int currentStage;

    //stages are basically dialgoueNodes

    private string[] stageNames = new string[] { "Start", "IntroTutorial", "MoveTutorial", "ShootTutorial", "DuckTutorial" };



    [SerializeField]
    public DialogueRunner dr;

    [SerializeField]
    public GameObject DialogueCanvas;

    [SerializeField]
    public Button[] buttonActions;

    [SerializeField]
    public GameObject actionUI;

    private bool isInConfirmed = false;


    private void Update()
    {
         if(Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME && isInConfirmed)
            {
                //do left click 
                Vector3 doubleClickPos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
                Debug.Log(doubleClickPos);
            }
            else
            {
                //do something normal left click?
            }

            lastClickTime = Time.time;

        }
    }

    public void DisableAllCombatButtons()
    {
        foreach (Button button in buttonActions)
        {
            button.gameObject.SetActive(false); //shouldn't be able to click a single button 
        }
    }

  

    public void StartPhase(int stageIndex)
    {
        DisableAllCombatButtons();
        //play dialogue 
        //end dialogue 
        //1) Enable the correct button 
        buttonActions[stageIndex].gameObject.SetActive(true);
    }
    public void Start()
    {
        //YarnManager.Instance.AddDialogueManager(dr);
        //BeginConvo("StartEvelynAndOlivia");
        //1) Disable all buttons 
        DisableAllCombatButtons();
        StartPhase(0);
        //BeginConvo(...);




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


    public void activateCanvas()
    {

    }

    //Potential bug: I don't know how to cancel 
    public void AwaitingConfirm()
    {
        isInConfirmed = true;
        Debug.Log("Awaiting Confirm");
        //playDialogue(sceneNumber); --> now double click on the new 
        //endDialogue(scneneNumber);
        //inputActions.
    }

    //I actually don't know how to get to the cancel OverlayButton, but it's subbed there
    public void DeselectAction()
    {
        isInConfirmed = false;
    }





}
