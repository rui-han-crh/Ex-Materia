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

        Debug.Log($"{bestRequest} Safety: {bestRequest.GetNextMap().EvaluateCurrentPositionSafety()}");
        if (bestRequest.ActionType == MapActionType.Movement)
        {
            Debug.Log(((MovementRequest)bestRequest).GetAttackRating());
        }

        ParseRequest(bestRequest);
        autoPlayQueued = false;
    }


}
