using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimeMeasurer : MonoBehaviour, IDataPersistence
{
    //just a test to record some 

    // In this case I don't load anything
    public void LoadData(GameData data)
    {
        Debug.Log("On load, the time spent so far is : " + data.totalTimeOnRun);

    }

    //
    public void SaveData(GameData data)
    {
        data.currentScene = SceneManager.GetActiveScene().name;
        Debug.Log("On Save, the time spent total for this file: " + data.totalTimeOnRun);
        //add time spent on this!
        data.totalTimeOnRun += Time.time;
    }
}
