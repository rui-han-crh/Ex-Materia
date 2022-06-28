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
    //private  Vector3 WORLD_OFFSET = 10 * Vector3.back;
    private float lastClickTime;

    public event Action OnEnded = delegate { };

    [SerializeField]
    public Camera MainCamera;

    //this int is basically what thing it's on now

    private int currentStage = 0;



    //stages are basically dialgoueNodes
    private string[] stageNames = new string[] {"MoveTutorial", "ShootTutorial", "HideTutorial" };
    private string[] stageIntermediate = new string[] { "MoveIntermediate", "ShootIntermediate", "HideIntermediate" };


    private readonly Vector3[] checkPoints = new Vector3[] { new Vector3(0.5f, -5.25f, -10.0f)};


    [SerializeField]
    public GameObject DialogueCanvas;

    [SerializeField]
    public Button[] buttonActions;

    [SerializeField]
    public GameObject actionUI;

    private bool isInConfirmed = false;


    //Serialize the interactables that can play dialogue!
    public Interactable dialogueHolder;
    public Interactable Myself;
    public SpeakingInteractionChangeable DialogueHolder;


    public void Awake()
    {
    }


    public void Start()
    {
        Debug.Log("Playing Node message: Start");
        StartConvo("Start");
        StartPhase(currentStage);
    }
    

    /*
     * EntryPointToInitiateConvo
     */

    private void StartConvo(string newScript)
    {
        //Sets script, then playes after a 1.5 second delay!
        DialogueHolder.SetYarnScriptName(newScript);
        Invoke("PlayInteraction", 0.5f);
    }

    private void PlayInteraction()
    {
        dialogueHolder.Interact(Myself);
    }



    private void Update()
    //no choice need to check for double click to confirm movement on update
    //Especially for the thing 
    {
         if(Input.GetMouseButtonDown(0) && currentStage == 0)
        {
            float timeSinceLastClick = Time.time - lastClickTime;

            if (timeSinceLastClick <= DOUBLE_CLICK_TIME && isInConfirmed)
            {
                //do left click 
                Vector3 doubleClickPos = MainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 currentCheckpoint = checkPoints[currentStage];
                Debug.Log("position I clicked = : " + doubleClickPos);
                Debug.Log("Checkpoint = " + currentCheckpoint);
                float distClick = Vector3.Distance(currentCheckpoint, doubleClickPos);
                if (distClick < 0.5) 
                {
                    Debug.Log("Correct pos");
                    Debug.Log(doubleClickPos);

                    //We can go onto the next phase 
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

        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl) && currentStage == 2)) {
            Debug.Log("Now playing the remainder of the dialogue");
            StartConvo(stageNames[currentStage]);
            EnableAllCombatButtons();
            currentStage += 1; //doesn't trigger anything else

            
        }
    }

    public void DisableAllCombatButtons()
    {
        foreach (Button button in buttonActions)
        {
            button.gameObject.SetActive(false); //shouldn't be able to click a single button 
        }
    }

    public void EnableAllCombatButtons()
    {
        foreach (Button button in buttonActions)
        {
            button.gameObject.SetActive(true); //shouldn't be able to click a single button 
        }
    }



    public void StartPhase(int stageIndex)
    {
        Debug.Log("Current phase is: " + stageIndex);
        DisableAllCombatButtons();
        DeselectAction(); //setActive == False!
        StartConvo(stageNames[stageIndex]);
        Debug.Log("Starting Convo @ : " + stageNames[stageIndex]); //stageNames are just instructions
        //end dialogue 
        //1) Enable the correct button 
        DisableAllCombatButtons();
        for (int i = 0; i <= stageIndex; i++)
        {
            buttonActions[i].gameObject.SetActive(true);
        }
    }

    
    public void ConfirmAttack()
    {
        if (currentStage == 1) //confirm that it's on attacking stage 
        { 
            //while other characters are moving in the background
            currentStage += 1; //--> Now, update will catch whenever they press control
        }
    }
    //Potential bug: I don't know how to cancel 
    public void AwaitingConfirmMovement()
    {
        if (currentStage == 0)
        {
            isInConfirmed = true;
            Debug.Log("Awaiting Confirm: " + stageIntermediate[currentStage]);
            Debug.Log("Now playing intermediary: " + stageIntermediate[currentStage]);
            StartConvo(stageIntermediate[currentStage]);
            //endDialogue(scneneNumber);
        }
    }

    public void AwaitingConfirmAttack() 
    {
        if (currentStage == 1)
        {
            Debug.Log("Awaiting Confirm: " + stageIntermediate[currentStage]);
            StartConvo(stageIntermediate[currentStage]);
        }
    }

    public void AwaitingConfirmOverwatch()
    {
        if (currentStage == 2)
        {
            //doSomething.
        }
    }

    //I actually don't know how to get to the cancel OverlayButton, but it's subbed there
    public void DeselectAction()
    {
        isInConfirmed = false;
    }





}
