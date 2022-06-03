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
    public static YarnManager Instance;
    public YarnInteractable YI;
    //Not sure if DialogueRunner should end up here 

    [SerializeField]
    public CharacterDatabase CharacterDB;

    //everybody can acceess this!
    public Dictionary<string, FigureCharacter> characterMap = new Dictionary<string, FigureCharacter>();

   void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        YI = FindObjectOfType<YarnInteractable>();
        foreach (FigureCharacter c in CharacterDB.characterList)
        {
            characterMap.Add(c.CharName, c); //each character can switch expressions
        }
    }



    //2 choices --> start immediately, or start with an await to a button press!

    /*
     * Needs a button press on the character to start
     */

    public void StartConvoButton(string startingNode)
    {
        if (startingNode != null) //TODO: probably need to check for legit startNode for the scene somewhere, probably in the DB?
        {
            YI.SetInterctable(startingNode);
        }
    }
    
    //when you wanna start immediately

    public void StartConvoAuto(string nextNode)
    {
        if (nextNode != null) //TODO: probably need to check for legit startNode for the scene somewhere, probably in the DB?
        {
            YI.StartImmediate(nextNode);
        }
    }

}
