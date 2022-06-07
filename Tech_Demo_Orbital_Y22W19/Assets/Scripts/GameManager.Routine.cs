using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using ExtensionMethods;

public partial class GameManager
{
    private static float ANIMATION_SPEED = 0.667f;
    private static float FORTY_FIVE_DEGREES = Mathf.PI / 4;

    /// <summary>
    /// Selects the appropriate unit gameObject to move based on a given identifier,
    /// additionally also check to ensure that the unit is not marked for deletion
    /// before moving
    /// </summary>
    /// <param name="identifier"></param>
    /// <param name="checkpoints"></param>
    /// <param name="actionPointsUsed"></param>
    /// <param name="setAction"></param>
    /// <returns>A coroutine to move the unit gameObject</returns>
    private IEnumerator LerpGameObjectByUnitID(string identifier, IEnumerable<Vector3Int> checkpoints, int actionPointsUsed, bool setAction = true) 
    {
        if (!nameUnitGameObjectMapping.ContainsKey(identifier) || markedForDeletion.Contains(nameUnitGameObjectMapping[identifier]))
        {
            yield break;
        }

        IEnumerator lerp = LerpGameObject(nameUnitGameObjectMapping[identifier], checkpoints, actionPointsUsed, setAction);
        while (lerp.MoveNext())
        {
            yield return lerp.Current;
        }
    }


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
        Animator unitAnimator = gameObject.GetComponentInChildren<Animator>();
        UnitAnimatorPerform(unitAnimator, "isMoving", true);
        Vector3 currentWorldDestination = gameObject.transform.position;

        while (positions.Count > 0)
        {
            Vector3Int cellPosition = positions.Dequeue();
            currentWorldDestination = groundTilemap.CellToWorld(cellPosition);
            float startTime = Time.time;
            Vector3 source = gameObject.transform.position;

            float journeyLength = Vector3.Distance(source, currentWorldDestination);

            Vector3 directionVector = (cellPosition - groundTilemap.WorldToCell(source)).RotateVector(FORTY_FIVE_DEGREES);

            // Animator value setter --> Maybe move this somewhere else
            UnitAnimatorPerform(unitAnimator, "isMoving", true, directionVector.x, directionVector.y);

            while (Vector3.Distance(gameObject.transform.position, currentWorldDestination) > Mathf.Epsilon)
            {
                float distanceCovered = (Time.time - startTime) * UNIT_INTERPOLATION_SPEED;
                float fractionOfJourney = Mathf.Min(1, distanceCovered / journeyLength);
                gameObject.transform.position = Vector3.Lerp(source, currentWorldDestination, fractionOfJourney);
                yield return null;
            }
            gameObject.transform.position = currentWorldDestination;
        }

        UnitAnimatorPerform(unitAnimator, "isMoving", false);

        if (setAction)
        {
            MovementRequest movementRequest = new MovementRequest(currentMap, CurrentUnitPosition, checkpoints.ToArray(), actionPointsUsed);
            currentMap = currentMap.DoAction(movementRequest);
            gameState = GameState.TurnEnded;
        }
    }

    /// <summary>
    /// Moves the attack, according to the given AttackRequest, to the firing position
    /// and attacks the target.
    /// </summary>
    /// <param name="attackRequest"></param>
    /// <returns>A coroutine consisting of movement, then attack, then movement back to
    ///         starting position</returns>
    private IEnumerator MoveToPositionAndAttack(AttackRequest attackRequest)
    {
        if (!currentMap.AllUnitPositions.Contains(attackRequest.TargetPosition))
        {
            yield break;
        }

        string attackerIdentifier = attackRequest.ActingUnit.Name;
        Vector3Int originalPosition = attackRequest.ActingUnitPosition;

        GameObject attackerGameObject = nameUnitGameObjectMapping[attackerIdentifier];

        IEnumerator[] subroutines = new IEnumerator[]
        {
            LerpGameObject(attackerGameObject, new Vector3Int[] { attackRequest.ShootFromPosition }, 0, false),
            ApplyAttackAction(attackRequest, false),
            RemoveStatusEffectFromUnit(attackRequest.ActingUnitPosition, UnitStatusEffects.Status.Overwatch),
            LerpGameObject(attackerGameObject, new Vector3Int[] { originalPosition }, 0, false)
        };

        foreach (IEnumerator subroutine in subroutines)
        {
            while (subroutine.MoveNext())
            {
                yield return subroutine.Current;
            }
        }
        gameState = GameState.TurnEnded;

        yield return null;
    }

    private IEnumerator DoAttackAction(Vector3Int targetPosition, Vector3Int[] tilesHit, int cost, bool endsTurn = true)
    {
        AttackRequest attackRequest = new AttackRequest(currentMap, CurrentUnitPosition, targetPosition, AttackStatus.Success, tilesHit, cost);

        IEnumerator rout = ApplyAttackAction(attackRequest, endsTurn);
        while (rout.MoveNext())
        {
            yield return rout.Current;
        }

        yield return null;
    }

    private IEnumerator ApplyAttackAction(AttackRequest attackRequest, bool endsTurn = true)
    {
        Vector3 direction = (attackRequest.TargetPosition - attackRequest.ShootFromPosition).RotateVector(FORTY_FIVE_DEGREES);

        UnitAnimatorPerform(attackRequest.ActingUnit.Name, "isShooting", true, direction.x, direction.y);

        currentMap = currentMap.DoAction(attackRequest);
        AudioManager.Instance.PlayTrack("RifleFire");

        yield return new WaitForSeconds(ANIMATION_SPEED);

        if (endsTurn)
        {
            gameState = GameState.TurnEnded;
        }
        UnitAnimatorPerform(attackRequest.ActingUnit.Name, "isShooting", false);

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

    private IEnumerator RemoveStatusEffectFromUnit(Vector3Int unitPosition, UnitStatusEffects.Status effect)
    {
        yield return new WaitForSeconds(0.25f);
        currentMap = currentMap.RemoveStatusEffectFromUnitAtPosition(unitPosition, effect);

        yield return null;
    }

    private IEnumerator DestroyUnitDelayed(string name, float delayTime)
    {
        UnitAnimatorPerform(name, "isHurt", true);

        yield return new WaitForSeconds(ANIMATION_SPEED);
        if (!nameUnitGameObjectMapping.ContainsKey(name))
        {
            yield break;
        }
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