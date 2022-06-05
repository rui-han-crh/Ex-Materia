using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class YarnInteractable : MonoBehaviour
{
    // internal properties exposed to editor
    [SerializeField]
    public Sprite Empty;
    // TODO: pass a string as the starting node 
    private string beginNode = "";
    // internal properties not exposed to editor
    private DialogueRunner dialogueRunner;
    public bool interactable = false;
    // I need my outline / something else here
    private bool isCurrentConversation = false;
    private Button expression;
    private Dictionary<string, FigureCharacter> characterMap = new Dictionary<string, FigureCharacter>();


    public void Rewake()
    {
        expression = GetComponent<Button>();
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>(); //potentially need to redo
        //dialogueRunner.Stop(); //Bug: Whenever a DL is found, the given UI will start it from the scene immediately!
        dialogueRunner.onDialogueComplete.AddListener(EndConversation); //called whenever the conversation reaches the end of the file?
        //For a test, uncomment this! The following function should be hooked from somewhere else//
        //this.SetInterctable("StartEvelynAndOlivia"); --> Small test to ensure this works?

    }

    public void SetInterctable(string startingNode)
    {
        expression.gameObject.SetActive(true);
        interactable = true; //this means he's already awaiting for a call!
        beginNode = startingNode;
    }

    /*
     * BUG: OBJECT REF NOT SET TO INSTANCE OF OBJ
     */

    public void StartImmediate(string nextNode)
    {
        beginNode = nextNode;
        expression.gameObject.SetActive(true);
        interactable = true; //failSafe in case DL.isDLRunning doesn't work
        Interact(); //auto interact?
    }

    //this is just a placholder for another action to be added?
    public void Interact()
    {
        //check 3 things: Correct startNode, interactable AND legit dialogue running
        if (interactable && !dialogueRunner.IsDialogueRunning && beginNode != "")
        {
           
            StartConversation();
        }
    }

    private void StartConversation()
    {
        Debug.Log($"Started conversation with {name}. at " + beginNode);
        isCurrentConversation = true;
        dialogueRunner.StartDialogue(beginNode);
    }

    //auto hits endconversation when it's done!
    private void EndConversation()
    {
        if (isCurrentConversation)
        {
            isCurrentConversation = false;
            Debug.Log($"Ended conversation with {name}.");
            EndScene();
        }
    }


    //disable can leave the button there, but not exit the scene? 
    //I can't think of a use for it 
    [YarnCommand("disable")]
    public void DisableConversation()
    {
        isCurrentConversation = false;
        interactable = false;
        beginNode = ""; //for failsafe purposes :(
    }

    [YarnCommand("load")]

    public void StartingLoad(string charName, string emotion)
    {
        print("Loading " + charName + " with emotion: " + emotion);
        expression.image.sprite = YarnManager.Instance.characterMap[charName].GetEmotion(emotion);
    }

    /*
     * An endScene triggers an end of conversation;
     * And signals YarnManager to close shop
     */

    [YarnCommand("endScene")]
    public void EndScene()
    {
        DisableConversation();
        expression.gameObject.SetActive(false);
        YarnManager.Instance.EndConvoSequence();

    }
}