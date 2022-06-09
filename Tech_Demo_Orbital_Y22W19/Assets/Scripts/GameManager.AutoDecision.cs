using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using ExtensionMethods;
using AsyncTask = System.Threading.Tasks.Task;

// Autodecisions
public partial class GameManager
{

    private async void AutoPlay()
    {
        autoPlayQueued = true;
        if (currentMap.CurrentUnit.Faction == Faction.Friendly)
        {
            autoPlayQueued = false;
            return;
        }
        
        MapActionRequest bestRequest = await AsyncTask.Run(() =>
        {
            MapActionRequest bestRequest = currentMap.GetOrderedMapActions().Last();
            return bestRequest;
        }, 
        tokenSource.Token);

        Debug.Log($"{bestRequest} Utility: {bestRequest.GetUtility()}");
        

        ParseRequest(bestRequest);
        autoPlayQueued = false;
    }


}
