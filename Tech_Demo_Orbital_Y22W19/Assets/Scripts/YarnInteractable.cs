using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class YarnInteractable : MonoBehaviour, IPointerDownHandler
{
    // internal properties exposed to editor
    [SerializeField] private string conversationStartNode;

    // internal properties not exposed to editor
    private DialogueRunner dialogueRunner;
    private Renderer renderer;
    private bool interactable = true;
    private bool isCurrentConversation = false;
    private float defaultIndicatorIntensity;

    public void Start()
    {
        dialogueRunner = FindObjectOfType<Yarn.Unity.DialogueRunner>();
        dialogueRunner.onDialogueComplete.AddListener(EndConversation);
        renderer = GetComponent<Renderer>();
        // get starter intensity of light then
        // if we're using it as an indicator => hide it 
        //if (lightIndicatorObject != null)
        //{
        //    defaultIndicatorIntensity = lightIndicatorObject.intensity;
        //    lightIndicatorObject.intensity = 0;
        //}
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if (interactable && !dialogueRunner.IsDialogueRunning)
        {
            StartConversation();
        }
    }

    private void StartConversation()
    {
        Debug.Log($"Started conversation with {name}.");
        isCurrentConversation = true;
        if (renderer != null)
        {
            renderer.material.color = Color.red;
        }
        // if (lightIndicatorObject != null) {
        //     lightIndicatorObject.intensity = defaultIndicatorIntensity;
        // }
        dialogueRunner.StartDialogue(conversationStartNode);
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

        if (renderer != null)
        {
            renderer.material.color = Color.clear;
        }
    }

    //    [YarnCommand("disable")]
    public void DisableConversation()
    {
        interactable = false;
    }
}
