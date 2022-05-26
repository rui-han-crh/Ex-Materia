using UnityEngine;
using System.Linq;

public class WaitRequest :MapActionRequest
{
    private int waitTime;

    public int WaitTime => waitTime;

    public WaitRequest(GameMap previousMap, Vector3Int sourcePosition, int waitTime) :
        base(previousMap, MapActionType.Wait, sourcePosition, 0)
    {
        this.waitTime = waitTime;
    }

    public override float GetUtility()
    {
        return PreviousMap.EvaluateCurrentPositionSafety();
    }
}