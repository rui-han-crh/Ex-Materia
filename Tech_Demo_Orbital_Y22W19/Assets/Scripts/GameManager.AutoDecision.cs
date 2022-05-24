using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using AsyncTask = System.Threading.Tasks.Task;

// Autodecisions
public partial class GameManager
{

    private async void AutoPlay()
    {
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

        int cost;
        switch(bestRequest.ActionType)
        {
            case MapActionType.Movement:
                MovementRequest movementRequest = (MovementRequest)bestRequest;
                cost = movementRequest.PreviousMap.FindShortestPathTo(movementRequest.DestinationPosition, out IEnumerable<Vector3Int> path);
                Enqueue(LerpGameObject(nameUnitGameObjectMapping[CurrentUnit.Name], path, cost));
                break;

            case MapActionType.Attack:
                AttackRequest attackRequest = (AttackRequest)bestRequest;
                cost = attackRequest.ActionPointCost;

                Vector3Int initialUnitPosition = bestRequest.ActingUnitPosition;

                routineQueue.Enqueue(LerpGameObject(nameUnitGameObjectMapping[currentMap.CurrentUnit.Name],
                                        new Vector3Int[] { attackRequest.ShootFromPosition }, 0, false));

                routineQueue.Enqueue(DoAttackAction(attackRequest.TargetPosition, attackRequest.TilesHit, cost));

                routineQueue.Enqueue(LerpGameObject(nameUnitGameObjectMapping[currentMap.CurrentUnit.Name],
                                        new Vector3Int[] { initialUnitPosition }, 0, false));
                break;

            case MapActionType.Wait:
                WaitRequest waitRequest = (WaitRequest)bestRequest;
                routineQueue.Enqueue(CurrentUnitRecoverAP(waitRequest));
                break;

            case MapActionType.Overwatch:
                OverwatchRequest overwatchRequest = (OverwatchRequest)bestRequest;
                routineQueue.Enqueue(CurrentUnitOverwatch(overwatchRequest));
                break;

            default:
                break;
        }
        autoPlayQueued = false;
    }


}
