using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;
using UnityEngine.UI;

/*
 * Singleton Dialogue manager class with access to a database / single YarnInteractable that should be tied to a game object somewhere 
 */

public class YarnManager : MonoBehaviour
{
    private static YarnManager instance;
    public static YarnManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<YarnManager>();
            }
            return instance;
        }
    }
    public YarnInteractable YI;

    [SerializeField]
    public CharacterDatabase CharacterDB;

    [SerializeField]
    public Button eventButton;

    //everybody can acceess this!
    public Dictionary<string, FigureCharacter> characterMap = new Dictionary<string, FigureCharacter>();
    public bool isActive = false; //I guess this can be used to signpost to everyone when a dialogue scene is active.



    void Awake()
    {
        YI = FindObjectOfType<YarnInteractable>();
        foreach (FigureCharacter c in CharacterDB.characterList)
        {
            characterMap.Add(c.CharName, c); //each character can switch expressions
        }

        foreach (FigureCharacter character in characterMap.Values)
        {
            character.InitializeDict();
        }

    }


    //Just displays the UI, and can only be accessed within YarnManager 
    //This can be used if there's more UI elements, but if there's only the dialogue 
    // +  a picture, there's actually no need for this
    private void DisplayUI()
    {
        isActive = true;

    }


    /// <summary>
    /// THERE ARE 2 ENTRY POINTS TO ACTIVATE A THE DIALOGUE, IT IS IN YARNMANAGER!
    /// (A) StartConvoButton --> Takes in the starting node, and awaits for a trigger to start the dialogue (Interact).
    /// Basically, it's awaiting for an event to kickstart the dialogue
    /// (B) StartConvoAuto --> Immediately starts the convo, and you can click anywhere to continue
    /// </summary>
    /// <param name="startingNode"></param>

    /*
     * Needs a button press on the character to start
     */

    
    public void StartConvoButton(string startingNode)
    {
        if (startingNode != null) //TODO: probably need to check for legit startNode for the scene somewhere, probably in the DB?
        {
            //any event related stuff, in this case enabling a button 
            isActive = true;
            YI.Rewake();
            YI.SetInterctable(startingNode);
        }
    }

    //when you wanna start immediately

    public void StartConvoAuto(string nextNode)
    {
        if (nextNode != null) //TODO: probably need to check for legit startNode for the scene somewhere, probably in the DB?
        {
            isActive = true;
            YI.Rewake();
            YI.StartImmediate(nextNode);
        }
    }

    public void EndConvoSequence()
    {
        isActive = false;
        DialogueManager.Instance.DisableContinue();
    }

}