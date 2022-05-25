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
    private string conversationStartNode;

    // internal properties not exposed to editor
    private DialogueRunner dialogueRunner;
    private bool interactable = true;
    // I need my outline / something else here
    private bool isCurrentConversation = false;
    private float defaultIndicatorIntensity;
    private Outline redOutline; 

    public void Start()
    {
        redOutline = GetComponent<Outline>();
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
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

    //    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }
}
