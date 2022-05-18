using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Collections;

public partial class GameManager
{
    private IEnumerator LerpGameObject(GameObject gameObject, IEnumerable<Vector3Int> checkpoints, int actionPointsUsed, bool setAction = true)
    {
        ClearAllHighlights();

        if (checkpoints.Count() == 0)
        {
            yield break;
        }

        Queue<Vector3Int> positions = new Queue<Vector3Int>(checkpoints);

        Vector3 currentWorldDestination = gameObject.transform.position;
        Animator unitAnimator = gameObject.GetComponentInChildren<Animator>();

        while (positions.Count > 0)
        {
            currentWorldDestination = groundTilemap.CellToWorld(positions.Dequeue());
            float startTime = Time.time;
            Vector3 source = gameObject.transform.position;

            float journeyLength = Vector3.Distance(source, currentWorldDestination);

            Vector3 directionVector = currentWorldDestination - source;
            unitAnimator?.SetInteger("xDirection", Math.Sign(directionVector.x));
            unitAnimator?.SetInteger("yDirection", Math.Sign(directionVector.y));

            while (Vector3.Distance(gameObject.transform.position, currentWorldDestination) > Mathf.Epsilon)
            {
                float distanceCovered = (Time.time - startTime) * interpolationSpeed;
                float fractionOfJourney = Mathf.Min(1, distanceCovered / journeyLength);
                gameObject.transform.position = Vector3.Lerp(source, currentWorldDestination, fractionOfJourney);
                yield return null;
            }
            gameObject.transform.position = currentWorldDestination;
        }

        if (setAction)
        {
            MovementRequest movementRequest = new MovementRequest(currentMap, CurrentUnitPosition, checkpoints.Last(), actionPointsUsed);
            currentMap = currentMap.DoAction(movementRequest);
            gameState = GameState.Selection;
        }
    }

    private IEnumerator DoAttackAction(Vector3Int targetPosition, Vector3Int[] tilesHit, int cost)
    {
        AttackRequest attackRequest = new AttackRequest(currentMap, CurrentUnitPosition, targetPosition, AttackStatus.Success, tilesHit, cost);
        currentMap = currentMap.DoAction(attackRequest);
        gameState = GameState.Selection;
        Debug.Log(currentMap);
        yield return null;
    }

    private IEnumerator CurrentUnitRecoverAP(WaitRequest request)
    {
        currentMap = currentMap.DoAction(request);
        gameState = GameState.Selection;
        Debug.Log(currentMap);
        yield return null;
    }
}