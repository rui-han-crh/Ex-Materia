using CombatSystem.Entities;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Extensions;

namespace CoroutineGenerators
{
    public static class CombatCoroutineGenerator
    {
        public static readonly float ANIMATION_SPEED = 0.667f;
        public static readonly int FORTY_FIVE_DEGREES = 45;

        public static IEnumerator MoveUnitToPosition(GameObject unitGameObject, Vector3 worldSpacePosition)
        {
            float startTime = Time.time;
            Vector3 source = unitGameObject.transform.position;

            float journeyLength = Vector3.Distance(source, worldSpacePosition);


            while (Vector3.Distance(unitGameObject.transform.position, worldSpacePosition) > Mathf.Epsilon)
            {
                float distanceCovered = (Time.time - startTime) * 2f;
                float fractionOfJourney = Mathf.Min(1, distanceCovered / journeyLength);
                unitGameObject.transform.position = Vector3.Lerp(source, worldSpacePosition, fractionOfJourney);
                yield return null;
            }
            unitGameObject.transform.position = worldSpacePosition;
        }

        public static IEnumerator EnactAttackRequest(GameMap map, AttackRequest request)
        {
            Vector3Int originalPosition = map[request.ActingUnit];
            Vector3 originalWorldPosition = CombatSceneManager.Instance.CellToWorld(originalPosition);

            GameObject attackerGameObject = UnitManager.Instance.GetGameObjectOfUnit(request.ActingUnit);
            Animator attackerAnimator = attackerGameObject.GetComponentInChildren<Animator>();
            Vector3 sourceWorldPosition = CombatSceneManager.Instance.CellToWorld(request.SourcePosition);

            Vector3Int targetPosition = map[request.TargetUnit];


            Vector3Int movingDirection = (request.SourcePosition - originalPosition).Rotate(FORTY_FIVE_DEGREES);
            Vector3Int returningDirection = (originalPosition - request.SourcePosition).Rotate(FORTY_FIVE_DEGREES);

            IEnumerator[] subroutines = new IEnumerator[]
            {
                PerformAnimation(attackerAnimator, "isMoving", true, movingDirection.x, movingDirection.y),
                MoveUnitToPosition(attackerGameObject, sourceWorldPosition),
                PerformAnimation(attackerAnimator, "isMoving", false),
                PlayAttackAction(attackerAnimator, request.SourcePosition, targetPosition),
                PerformAnimation(attackerAnimator, "isMoving", true, returningDirection.x, returningDirection.y),
                MoveUnitToPosition(attackerGameObject, originalWorldPosition),
                PerformAnimation(attackerAnimator, "isMoving", false, movingDirection.x, movingDirection.y)
            };

            foreach (IEnumerator subroutine in subroutines)
            {
                while (subroutine.MoveNext())
                {
                    yield return subroutine.Current;
                }
            }

            yield return null;
        }

        public static IEnumerator PlayAttackAction(Animator attackerAnimator, Vector3Int gridSourcePosition, Vector3Int gridTargetPosition)
        {
            yield return new WaitForEndOfFrame();

            Vector3Int direction = (gridTargetPosition - gridSourcePosition).Rotate(FORTY_FIVE_DEGREES);

            IEnumerator routine = PerformAnimation(attackerAnimator, "isShooting", true, direction.x, direction.y);

            while (routine.MoveNext()) yield return routine.Current;

            AudioManager.Instance.PlayTrack("RifleFire");

            yield return new WaitForSeconds(ANIMATION_SPEED);

            routine = PerformAnimation(attackerAnimator, "isShooting", false);

            while (routine.MoveNext()) yield return routine.Current;

            yield return null;
        }

        public static IEnumerator PerformAnimation(
            Animator animator, string booleanFlag, bool state, float xDirection = 0, float yDirection = 0)
        {
            yield return null;

            if (animator == null)
            {
                throw new ArgumentException("No animator was passed to perform the animation on");
            }

            if (!(xDirection == 0 && yDirection == 0))
            {
                animator.SetFloat("xDirection", xDirection);
                animator.SetFloat("yDirection", yDirection);
            }

            animator.SetBool(booleanFlag, state);
        }

        public static IEnumerator DisableDeadUnits(GameMap currentMap)
        {
            yield return null;
            IEnumerable<Unit> deadUnits = currentMap.GetUnits(x => x.CurrentHealth <= 0);
            foreach (Unit unit in deadUnits)
            {
                UnitManager.Instance.RemoveUnit(unit, delay: 1);
            }
        }
    }
}
