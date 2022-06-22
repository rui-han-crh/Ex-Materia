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
    private  Vector3 WORLD_OFFSET = 10 * Vector3.back;
    private float lastClickTime;

    public event Action OnEnded = delegate { };

    [SerializeField]
    public Camera MainCamera;
    [SerializeField]
    public string startNode;

    //this int is basically what thing it's on now

    private int currentStage = 0;

    //stages are basically dialgoueNodes

    private string[] stageNames = new string[] {"MoveTutorial", "ShootTutorial", "DuckTutorial" };
    private string[] stageIntermediate = new string[] { "MoveIntermediate", "ShootIntermediate", "DuckTutorial" };


    public Vector3[] checkPoints = new Vector3[] { new Vector3(-3.0f, -4.0f, -10.0f), new Vector3() };

    [SerializeField]
    public DialogueRunner dr;

    [SerializeField]
    public GameObject DialogueCanvas;

    [SerializeField]
    public Button[] buttonActions;

    [SerializeField]
    public GameObject actionUI;

    private bool isInConfirmed = false;

    IEnumerator ResetNextScene()
    {
        yield return new WaitForSeconds(5);
    }

    IEnumerator ResetLonger()
    {   //alternatively, this can be a button that pops up, when he's ready to go!
        yield return new WaitForSeconds(10);
    }


    public void Start()
    {
        Debug.Log("Playing Node message: Start");
        StartCoroutine("ResetLonger");
        StartPhase(currentStage);
    }
    private void Update()
    {
         if(Input.GetMouseButtonDown(0))
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME && isInConfirmed)
            {
                //do left click 
                Vector3 doubleClickPos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 currentCheckpoint = checkPoints[currentStage] + WORLD_OFFSET;
                Debug.Log("position I clicked = : " + doubleClickPos);
                Debug.Log("Checkpoint = " + currentCheckpoint);
                float distClick = Vector3.Distance(currentCheckpoint, doubleClickPos);
                if (distClick < 0.5) 
                {
                    Debug.Log("Correct pos");
                    Debug.Log(doubleClickPos);

                    //We can go onto the next phase 
                    StartCoroutine("ResetNextScene");
                    currentStage += 1;
                    StartPhase(currentStage);

                } 
                else
                {
                    Debug.Log("Wrong pos");
                    Debug.Log("The distance is: " + distClick);
                    Debug.Log(doubleClickPos); ;
                }
                
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
        Debug.Log("Current phase is: " + stageIndex);
        DisableAllCombatButtons();
        DeselectAction(); //setActive == False!
        //play dialogue
        Debug.Log("Starting Convo @ : " + stageNames[stageIndex]); //stageNames are just instructions
        //end dialogue 
        //1) Enable the correct button 
        DisableAllCombatButtons();
        buttonActions[stageIndex].gameObject.SetActive(true);
    }

    //Essentially starts the stage 

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
        Debug.Log("Now playing intermediary: " + stageIntermediate[currentStage]);
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
