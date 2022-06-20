using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GameData 
{
    /*
     * This GameData class is actually not very useful, 
     * Since we're not storing any states in between scenes 
     * E.g: Save Scumming for combat systems 
     * However, we might need to store some collectables in the future 
     * So this is just here for the time being for completeness 
     */
    public float totalTimeOnRun;
    public string currentScene;
    public TestA sampleTest;


    /*
     * so far, arrays are okay 
     * any subclass need to be tagged as serializable 
     * any subclasses using primatives are okay 
     * Dictionaries are NOT (will need a custom implementation for this)
     */

    

    public GameData()
    {
        totalTimeOnRun = 0;
        currentScene = "";
        sampleTest = new TestA(4, 20);
    }
    
}
