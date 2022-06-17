using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeMeasurer : MonoBehaviour, IDataPersistence
{
    //just a test to record some stuff 


    // In this case I don't load anything
    public void LoadData(GameData data)
    { 

    }

    //
    public void SaveData(GameData data)
    {
        Debug.Log("Time spent total for this file: " + data.totalTimeOnRun);
        //add time spent on this!
        data.totalTimeOnRun += Time.timeSinceLevelLoad; 
    }
}
