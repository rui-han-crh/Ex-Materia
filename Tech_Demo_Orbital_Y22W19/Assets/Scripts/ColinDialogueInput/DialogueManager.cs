using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;


//this is just a test class out of context to load stuff in and out//

/*
 * The DialogueManager could possible hold the flow of starting nodes?
 * Design idea: Either YarnManager / DialogueManager holds a number representing the state 
 * e.g: 0 --> Prelogue 
 * 1 --> tutorial 
 * 2 --> first boss post credit
 * 3 --> second boss post credit
 * .... 5 --> ending scene 
 * 
 */

//Another singleton test?
public class DialogueManager : MonoBehaviour
{
    private static DialogueManager instance;
    public static DialogueManager Instance 
    { 
        get 
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DialogueManager>();
            }
            return instance; 
        } 
    }


    public float NextActionTime = 30.0f;
    public float interpolationPeriod = 20.0f;
    public int sceneNumber = 0;
    public Dictionary<int, string> startingNodeDB;

    private KeyboardControls keyboardControls;

    [SerializeField]
    private LineView lineView;

    private Action<InputAction.CallbackContext> singleLeftClick;

    //private void OnEnable()
    //{
    //    SubscribeLeftClick();
    //}

    //private void OnDisable()
    //{
    //    UnsubscribeLeftClick();
    //}

    public void EnableContinue()
    {
        SubscribeLeftClickContinue();
    }

    public void DisableContinue()
    {
        UnsubscribeLeftClickContinue();
    }
    void Awake()
    {
        keyboardControls = new KeyboardControls();
    }

    private void SubscribeLeftClickContinue() //this is only for continuing dialogue!
    {
       
        print("Left Click Continue Enabled, should only be dialogue!");
        keyboardControls.Enable();
        singleLeftClick = _ => lineView.OnContinueClicked(); //arrowFunction continuing LC!
        keyboardControls.Mouse.LeftClick.performed += singleLeftClick;
    }

    private void UnsubscribeLeftClickContinue()
    {
        keyboardControls.Mouse.LeftClick.performed -= singleLeftClick;
        keyboardControls.Disable(); 
    }
    // Start is called before the first frame update
    void Start()
    {
        //initialize startingNode DB, if not already initialized 
        //increment dialogueNumber and play dialogue
        sceneNumber += 1;
        YarnManager.Instance.StartConvoAuto("StartEvelynAndOlivia"); //just to test if i can do this?
        //doesn't go to the next one?
        
    }

    void Update()
    {
        if (Time.time >= NextActionTime) {
           TestAction();
        }
        
    }
    // Update is called once per frame
    void TestAction()
    {
       YarnManager.Instance.StartConvoButton("FirstMeetLucien");
    }
}