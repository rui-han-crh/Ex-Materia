using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Yarn.Unity;


//I don't actually have a real purpose for this class yet

/*
 * The DialogueManager could possible hold the flow of starting nodes?
 * Design idea: Either YarnManager / DialogueManager holds a number representing the state 
 * e.g: 0 --> Prelogue 
 * 1 --> tutorial 
 * 2 --> first boss post credit
 * 3 --> second boss post credit
 * .... 5 --> ending scene 
 * We can have a hashmap, that has a list of nodes that I can launch sequentially based on the curr scene 
 * 
 */

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


    private KeyboardControls keyboardControls;

    [SerializeField]
    private LineView lineView;

    private Action<InputAction.CallbackContext> singleLeftClick;

    private DialogueRunner dialogueRunner;

    public void OnEnable()
    {
        keyboardControls?.Enable();
    }

    public void OnDisable()
    {
        keyboardControls?.Disable();
    }

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
        if (lineView == null)
        {
            lineView = FindObjectOfType<LineView>();
        }

        dialogueRunner = GetComponent<DialogueRunner>();

        if (dialogueRunner.dialogueViews.Any(x => x == null))
        {
            dialogueRunner.dialogueViews = FindObjectsOfType<DialogueViewBase>();
        }
    }

    /**
     * In subscribing / unsubbing, I only enable it when I need it
     * My KBC are switched off whenever I'm not actively using them!
     */

    private void SubscribeLeftClickContinue() //this is only for continuing dialogue!
    {
        keyboardControls.Enable();
        singleLeftClick = _ => lineView.OnContinueClicked(); //arrowFunction continuing LC!
        keyboardControls.Mouse.LeftClick.performed += singleLeftClick;
        keyboardControls.Mouse.Interact.performed += singleLeftClick;
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
        //YarnManager.Instance.StartConvoAuto("StartEvelynAndOlivia"); //just to test if i can do this?
        //doesn't go to the next one?
        
    }

    void Update()
    {

    }
    // Update is called once per frame
}