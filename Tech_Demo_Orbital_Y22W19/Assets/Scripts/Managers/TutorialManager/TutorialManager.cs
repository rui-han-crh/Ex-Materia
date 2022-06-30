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

    private float nextReminderTime = 0.0f;
    private float lastClickTime;

    public event Action OnEnded = delegate { };

    [SerializeField]
    public Camera MainCamera;

    //this int is basically what thing it's on now

    private int currentStage = 0;

    //booleans to signify which stage it's at
    private bool officialStart = false;
    private bool overwatchInstructionsPlayed = false;
    private bool abilitiesInstructionsPlayed = false;

    //stages are basically dialgoueNodes
    private string[] stageNames = new string[] {"MoveTutorial", "ShootTutorial", "SkipTutorial", "HideTutorial" , "AbilitiesTutorial", "AbilitiesTutorial", "EndTutorialInstructions" };
    private string[] stageIntermediate = new string[] { "MoveIntermediate", "ShootIntermediate", "SkipIntermediate", "OverwatchIntermediate", "AbilitiesIntermediate", "AbilitiesIntermediate"};

    //[SerializeField]
    //private Interactable[] interactables; 


    private readonly Vector3[] checkPoints = new Vector3[] { new Vector3(0.5f, -5.25f, -10.0f)};

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
        DisableAllCombatButtons();
    }
    

    /*
     * EntryPointToInitiateConvo
     */

    private void StartConvo(string newScript)
    { 
        //Sets script, then playes after a 1.0 second delay!
        DialogueHolder.SetYarnScriptName(newScript);
        Invoke("PlayInteraction", 0.35f);
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
                if (distClick < 1.0) //In proper range!
                {
                    Debug.Log("Correct pos");
                    Debug.Log(doubleClickPos);

                    //We can go onto the next phase 
                    currentStage += 1;
                    Invoke("StartPhase", 1.5f);

                } 
                else
                {
                    Debug.Log("Wrong pos");
                    Debug.Log("The distance is: " + distClick);
                    Debug.Log(doubleClickPos);
                }
                
            }

            lastClickTime = Time.time;

        }
        //Pressing control can initate the start / Overwatch. 
        //These are 2 key breaks where I'm unsure where the player currently is 
        //Hence, pressing control to bring it up allows me to figure out what he's currently doing 
        //And what to proceed with next 
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)))
        {
            switch (currentStage)
            {
                case 0:
                    if (!officialStart)
                    {
                        officialStart = true;
                        StartPhase();
                    }
                    break;

                case 3:
                    if (!overwatchInstructionsPlayed)
                    {
                        overwatchInstructionsPlayed = true;
                        StartPhase();
                        nextReminderTime = Time.time + 7.5f; //Time taken to play next set of instructions 
                    }
                    else
                    {
                        PlayReminder();
                    }
                    break;

                case 5:
                    PlayReminder();
                    break;
            }
        }
    }
    

    //The only way to call this, is thru a control click!
    private void PlayReminder()
    {
        if (Time.time > nextReminderTime)
        {
            StartConvo(stageIntermediate[currentStage]);
            nextReminderTime = Time.time + 3.5f;
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



    public void StartPhase()
    {
        Debug.Log("Current phase is: " + currentStage);
        DisableAllCombatButtons();
        DeselectAction(); //setActive == False!
        StartConvo(stageNames[currentStage]);
        Debug.Log("Starting Convo @ : " + stageNames[currentStage]); //stageNames are just instructions
        //end dialogue 
        //1) Enable the correct button 
        DisableAllCombatButtons();
        for (int i = 0; i <= currentStage; i++)
        {
            buttonActions[i].gameObject.SetActive(true);
        }
    }

    //NOTE: This currentStage adds2, we skip the skip function! (Straight to OW / Hide explanation)
    public void ConfirmAttack()
    {
        if (currentStage == 1) //confirm that it's on attacking stage 
        { 
            //while other characters are moving in the background
            currentStage += 2; 
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
        }
    }

    public void AwaitingConfirmAttack() 
    {
        Debug.Log("Current stage is: " + currentStage);
        if (currentStage == 1)
        {
            Debug.Log("Awaiting Confirm: " + stageIntermediate[currentStage]);
            StartConvo(stageIntermediate[currentStage]);
        }
    }

    public void ConfirmOverwatch()
    {
        if (currentStage == 3)
        {
            Debug.Log("Confirm used Overwatch, moving onto ability explanation");
            //Increment by 2, to confirm use of ow.
            currentStage += 2;
            Invoke("StartPhase", 2.5f);
            
        }
    }

    public void ConfirmAbility()
    {
        if (currentStage == 5)
        {
            Debug.Log("Confirm used ability, moving onto ending");
            //Increment by 1, to confirm use of ability, moving onto end of tutorial
            currentStage += 1;
            Invoke("StartPhase", 2.5f);
        }
    }

    //I actually don't know how to get to the cancel OverlayButton, but it's here in case I do need it 
    public void DeselectAction()
    {
        isInConfirmed = false;
    }





}
