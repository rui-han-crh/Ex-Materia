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
    // TODO: pass a string as the starting node 
    [SerializeField] 
    private string conversationStartNode;

    [SerializeField]
    private CharacterDatabase db;

    // internal properties not exposed to editor
    private DialogueRunner dialogueRunner;
    private bool interactable = true;
    // I need my outline / something else here
    private bool isCurrentConversation = false;
    private float defaultIndicatorIntensity;
    private Outline redOutline;
    private Image expression;
    private Dictionary<string, FigureCharacter> characterMap = new Dictionary<string, FigureCharacter>();   

    public void Start()
    {
        expression = GetComponent<Image>();
        redOutline = GetComponent<Outline>();
  
        foreach (FigureCharacter c in db.characterList)
        {
            characterMap.Add(c.CharName, c);
        }
        //actually not v sure if the dictionary should be initialized here or in CharacterDat
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>(); //potentially need to redo
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        

        //{
        //    defaultIndicatorIntensity = lightIndicatorObject.intensity;
        //    lightIndicatorObject.intensity = 0;
        //}
    }

    //public void FixedUpdate()
    //{
    //    //on the frame the LMB is pressed, start the dialogue runner!
    //    if (Input.GetMouseButtonDown(0) && interactable && !dialogueRunner.IsDialogueRunning)
    //    {
    //        StartConversation();
    //    }
    //}



    //public void OnClick()
    //{
    //    if (interactable && !dialogueRunner.IsDialogueRunning)
    //    {
    //        StartConversation();
    //    }
    //}

    public void Interact()
    {
        if(interactable && !dialogueRunner.IsDialogueRunning)
        {
            StartConversation();
        }
    }

    [YarnCommand("indicatorOn")]
    public void indicatorOn()
    {
        if (redOutline != null)
        {
            redOutline.enabled = true;
        }
    }

    [YarnCommand("indicatorOff")]

    public void indicatorOff()
    {
        if (redOutline != null)
        {
            redOutline.enabled = false;
        }
    }

    private void StartConversation()
    {
        Debug.Log($"Started conversation with {name}.");
        isCurrentConversation = true;
        // if (lightIndicatorObject != null) {
        //     lightIndicatorObject.intensity = defaultIndicatorIntensity;
        // }
        
        dialogueRunner.StartDialogue(conversationStartNode);
        if (redOutline != null)
        {
            redOutline.enabled = true;
        }
    }

    private void EndConversation()
    {
        if (isCurrentConversation)
        {
            // if (lightIndicatorObject != null) {
            //     lightIndicatorObject.intensity = 0;
            // }
            isCurrentConversation = false;
            Debug.Log($"Started conversation with {name}.");
        }

        if (redOutline != null)
        {
            redOutline.enabled = false;
        }
    }

    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }

    [YarnCommand("load")]

    public void StartingLoad(string charName, string emotion)
    {
        expression.sprite = characterMap[charName].GetEmotion(emotion);
    }
}
