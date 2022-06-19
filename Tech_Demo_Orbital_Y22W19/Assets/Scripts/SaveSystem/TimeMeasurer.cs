using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeMeasurer : MonoBehaviour, IDataPersistence
{
    //just a test to record some 

    // In this case I don't load anything
    public void Awake()
    {
        DataPersistenceManager.Instance.SaveGame(); //need to update the current scene!
    }
    public void LoadData(GameData data)
    {
        Debug.Log("On load, the time spent so far is : " + data.totalTimeOnRun);
        data.sampleTest.printTest();

    }

    //
    public void SaveData(GameData data)
    {
        data.currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("The current scene is: " + data.currentScene);
        if (data.currentScene == "Battle_Substituted")
        {
            data.sampleTest = new TestA(369, 963);
        }
        
        //add time spent on this!
        data.totalTimeOnRun += Time.time;
        Debug.Log("On Save, the time spent total for this file: " + data.totalTimeOnRun);
    }
}
