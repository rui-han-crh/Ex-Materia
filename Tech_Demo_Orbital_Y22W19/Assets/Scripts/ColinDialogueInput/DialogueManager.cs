using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

public class DialogueManager : MonoBehaviour
{
    public int sceneNumber = 0;
    public Dictionary<int, string> startingNodeDB;
    // Start is called before the first frame update
    void Start()
    {
        //initialize startingNode DB, if not already initialized 
        //increment dialogueNumber and play dialogue
        sceneNumber += 1;
        YarnManager.Instance.StartConvoAuto("StartEvelynAndOlivia"); //just to test if i can do this?
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
