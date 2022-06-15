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
            Vector3 sourceWorldPosition = CombatSceneManager.Instance.CellToWorld(request.SourcePosition);

            Vector3 targetWorldPosition = CombatSceneManager.Instance.CellToWorld(map[request.TargetUnit]);

            IEnumerator[] subroutines = new IEnumerator[]
            {
                MoveUnitToPosition(attackerGameObject, sourceWorldPosition),
                PlayAttackAction(attackerGameObject, targetWorldPosition),
                MoveUnitToPosition(attackerGameObject, originalWorldPosition)
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

        public static IEnumerator PlayAttackAction(GameObject attacker, Vector3 worldTargetPosition)
        {
            Vector3 direction = (worldTargetPosition - attacker.transform.position).Rotate(FORTY_FIVE_DEGREES);

            PerformAnimation(attacker.GetComponent<Animator>(), "isShooting", true, direction.x, direction.y);

            AudioManager.Instance.PlayTrack("RifleFire");

            yield return new WaitForSeconds(ANIMATION_SPEED);

            PerformAnimation(attacker.GetComponent<Animator>(), "isShooting", false, direction.x, direction.y);

            yield return null;
        }

        public static IEnumerator PerformAnimation(Animator animator, string booleanFlag, bool state, float? xDirection = null, float? yDirection = null)
        {
            yield return null;

            if (animator == null)
            {
                throw new ArgumentException("No animator was passed to perform the animation on");
            }

            if (xDirection != null && yDirection != null)
            {
                animator.SetFloat("xDirection", xDirection.Value);
                animator.SetFloat("yDirection", yDirection.Value);
            }

            animator.SetBool(booleanFlag, state);
        }
    }
}
