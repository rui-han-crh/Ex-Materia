using System;
using System.Collections;
using System.Collections.Generic;
using Transitions;
using UnityEngine;
using Yarn.Unity;

public class TutorialManager : MonoBehaviour //should be able to interact with yarn also
{

    //public event Action OnEnded = delegate { };

    [SerializeField]
    public string startNode;

    //this int is basically what thing it's on now

    private int currentStage;

    //stages are basically dialgoueNodes

    private string[] stageNames = new string[] { "Start", "IntroTutorial", "MoveTutorial", "ShootTutorial", "DuckTutorial" };


    ////icon positions to serialize
    //[SerializeField]
    //public Button MovementIcon;
    //[SerializeField]
    //public Button CombatIcon;
    //[SerializeField]
    //public Button OverwatchIcon;

    [SerializeField]
    public DialogueRunner dr;

    [SerializeField]
    public GameObject DialogueCanvas;

    [SerializeField]
    public GameObject[] buttonActions;

    [SerializeField]
    public GameObject actionUI;


    public void Start()
    {
        YarnManager.Instance.AddDialogueManager(dr);
        BeginConvo("OliviaIntro");
        //1) Disable all buttons 
        foreach (GameObject button in buttonActions)
        {
            button.SetActive(false); //shouldn't be able to click a single button 
        }
        //actionUI.SetActive(false);
        //startButton.enabled = true;
        //DialogueCanvas.SetActive(true);
        //YarnManager.Instance.StartConvoAuto("WrongOption");
        //DialogueCanvas.SetActive(false);
        StartStage(currentStage);
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

    public void BeginConvo(string startNode)
    {
        DialogueCanvas.gameObject.SetActive(true);
        YarnManager.Instance.StartConvoAuto(startNode);
    }


    public voi





}
