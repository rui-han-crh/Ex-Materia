using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Collections;

public partial class GameManager
{
    /// <summary>
    /// Moves the given gameObject from its current position along a specified path of checkpoints.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="checkpoints"></param>
    /// <param name="actionPointsUsed"></param>
    /// <param name="setAction"> Optional to choose if this action affect the data of the underlying GameMap</param>
    /// <returns>A coroutine to perform the linear interpolation</returns>
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
            MovementRequest movementRequest = new MovementRequest(currentMap, CurrentUnitPosition, checkpoints.ToArray(), actionPointsUsed);
            currentMap = currentMap.DoAction(movementRequest);
            gameState = GameState.TurnEnded;
        }
    }

    private IEnumerator DoAttackAction(Vector3Int targetPosition, Vector3Int[] tilesHit, int cost, bool endsTurn = true)
    {
        yield return new WaitForSeconds(0.25f);
        AttackRequest attackRequest = new AttackRequest(currentMap, CurrentUnitPosition, targetPosition, AttackStatus.Success, tilesHit, cost);
        currentMap = currentMap.DoAction(attackRequest);

        if (endsTurn)
        {
            gameState = GameState.TurnEnded;
        }

        yield return null;
    }

    private IEnumerator ApplyAttackAction(AttackRequest attackRequest, bool endsTurn = true)
    {
        yield return new WaitForSeconds(0.25f);
        currentMap = currentMap.DoAction(attackRequest);

        if (endsTurn)
        {
            gameState = GameState.TurnEnded;
        }

        yield return null;
    }

    private IEnumerator CurrentUnitRecoverAP(WaitRequest request, bool endsTurn = true)
    {
        yield return new WaitForSeconds(0.25f);
        currentMap = currentMap.DoAction(request);

        if (endsTurn)
        {
            gameState = GameState.TurnEnded;
        }

        yield return null;
    }

    private IEnumerator CurrentUnitOverwatch(OverwatchRequest request, bool endsTurn = true)
    {
        yield return new WaitForSeconds(0.25f);
        currentMap = currentMap.DoAction(request);

        if (endsTurn)
        {
            gameState = GameState.TurnEnded;
        }

        yield return null;
    }

    private IEnumerator RemoveStatusEffectFromUnit(Unit unit, UnitStatusEffects.Status effect)
    {
        yield return new WaitForSeconds(0.25f);
        currentMap = currentMap.RemoveStatusEffectFromUnit(unit, effect);

        yield return null;
    }

    private IEnumerator DestroyUnitDelayed(string name, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);

        GameObject unitGO = nameUnitGameObjectMapping[name];
        GameObject unitHealthBar = unitToHealthbarMapping[unitGO];

        nameUnitGameObjectMapping.Remove(name);
        unitToHealthbarMapping.Remove(unitGO);

        Destroy(unitGO);
        Destroy(unitHealthBar);
    }

    private IEnumerator Wait(float second)
    {
        yield return new WaitForSeconds(second);
    }
}