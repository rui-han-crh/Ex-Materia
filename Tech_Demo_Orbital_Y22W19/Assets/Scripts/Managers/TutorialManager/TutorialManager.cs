using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour //should be able to interact with yarn also
{
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
    public GameObject DialogueCanvas;

    [SerializeField]
    public Button[] buttonActions;

    [SerializeField]
    public GameObject actionUI;


    public void Start()
    {
        //1) Disable all buttons 
        foreach (Button button in buttonActions)
        {
            button.enabled = false; //shouldn't be able to click a single button 
        }

        Button startButton = buttonActions[0];
        startButton.enabled = true; //thisworks!
        //actionUI.SetActive(false);
        //startButton.enabled = true;
        //YarnManager.Instance.StartConvoAuto("WrongOption");
        //DialogueCanvas.SetActive(false);
        
    }




}
