using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;
using System.Linq;
using UnityEngine.Extensions;
using Algorithms.Rasterisers;
using CombatSystem.Censuses;

namespace CombatSystem.Consultants
{
    public static class CombatConsultant
    {
        public static readonly int ATTACK_COST = 0;
        public static readonly int ATTACK_TIME_SPENT = 250;

        public static readonly float ONE_HUNDRED_PERCENT = 1;
        public static readonly float FIFTY_PERCENT = 0.5f;
        public static readonly int MINIMUM_DAMAGE_DEALT = 2;
        public static readonly Vector3 OFFSET = Vector2.one / 2;

        public static AttackRequest SimulateAttack(Unit attacker, Unit defender, GameMapData gameMapData, bool considerActionPoints = false)
        {
            if (considerActionPoints && attacker.CurrentActionPoints < ATTACK_COST)
            {
                return AttackRequest.CreateFailedRequest(attacker, defender, AttackRequest.Outcome.NotEnoughActionPoints);
            }

            bool HasLineOfSight(IEnumerable<Vector3Int> lineRepresentation)
            {
                bool hasDirectLineOfSight = true;
                for (int i = 1; i < lineRepresentation.Count() - 1; i++)
                {
                    Vector3Int point = lineRepresentation.ElementAt(i);
                    if (!gameMapData.IsVacant(point) && !gameMapData.HasHalfCover(point))
                    {
                        hasDirectLineOfSight = false;
                        break;
                    }
                }
                return hasDirectLineOfSight;
            }

            int potentialDamage = Mathf.Max(MINIMUM_DAMAGE_DEALT, attacker.Attack - defender.Defence);

            if (!IsInRangeOf(attacker, defender, gameMapData))
            {
                return AttackRequest.CreateFailedRequest(attacker, defender, AttackRequest.Outcome.OutOfRange);
            }

            Vector3Int attackerPosition = gameMapData[attacker];
            Vector3Int defenderPosition = gameMapData[defender];

            IEnumerable<Vector3Int> raytracedLine = Rasteriser.AdaptedBresenham(attackerPosition + OFFSET, defenderPosition + OFFSET, gridSize: 1);

            if (HasLineOfSight(raytracedLine))
            {
                return new AttackRequest(
                    attacker,
                    defender,
                    potentialDamage,
                    GetChanceToHit(raytracedLine, gameMapData),
                    raytracedLine,
                    ATTACK_COST,
                    ATTACK_TIME_SPENT,
                    AttackRequest.Outcome.Successful);
            }

            Vector3Int positionAhead = raytracedLine.ElementAt(1);
            Vector3Int vectorToWall = positionAhead - attackerPosition;

            if (!gameMapData.HasFullCover(positionAhead) || Vector3.Magnitude(vectorToWall) > 1)
            {
                return AttackRequest.CreateFailedRequest(attacker, defender, raytracedLine, AttackRequest.Outcome.NoLineOfSight);
            }

            for (float angle = -Mathf.PI / 2; angle <= Mathf.PI / 2; angle += Mathf.PI)
            {
                Vector3Int peekToPosition = attackerPosition + vectorToWall.Rotate(angle);

                if (!gameMapData.IsVacant(peekToPosition))
                {
                    continue;
                }
                IEnumerable<Vector3Int> peekingLine = Rasteriser.AdaptedBresenham(peekToPosition, defenderPosition, gridSize: 1);

                if (HasLineOfSight(peekingLine))
                {
                    return new AttackRequest(
                        attacker,
                        defender,
                        potentialDamage,
                        GetChanceToHit(raytracedLine, gameMapData),
                        peekingLine,
                        ATTACK_COST,
                        ATTACK_TIME_SPENT,
                        AttackRequest.Outcome.Successful);
                }
            }

            return AttackRequest.CreateFailedRequest(attacker, defender, raytracedLine, AttackRequest.Outcome.NoLineOfSight);
        }

        public static float GetChanceToHit(IEnumerable<Vector3Int> pathSourceToTarget, GameMapData gameMapData)
        {
            if (pathSourceToTarget.Count() == 2)
            {
                return 1;
            }

            IEnumerator tilesBetween = pathSourceToTarget.GetEnumerator();
            Unit attacker = gameMapData[pathSourceToTarget.First()];

            tilesBetween.MoveNext();
            int count = 0;
            int totalCover = 0;
            int tileDistance = 1;
            while (tilesBetween.MoveNext())
            {
                Vector3Int currentTile = (Vector3Int)tilesBetween.Current;
                
                if (gameMapData.HasHalfCover(currentTile))
                {
                    totalCover += count;
                }
                tileDistance += count;
                count++;
            }
            tileDistance -= --count;

            return (1 - ((float)totalCover / tileDistance))
                * (ONE_HUNDRED_PERCENT 
                    - (FIFTY_PERCENT * Vector3.Distance(pathSourceToTarget.First(), pathSourceToTarget.Last()) / attacker.Range));
        }

        public static IEnumerable<Vector3Int> GetAllRangePositions(GameMapData gameMapData, Unit unit)
        {
            int maxX = gameMapData[unit].x + unit.Range;
            int minX = gameMapData[unit].x - unit.Range;

            int maxY = gameMapData[unit].y + unit.Range;
            int minY = gameMapData[unit].y - unit.Range;

            List<Vector3Int> positions = new List<Vector3Int>();

            for (int x = minX ; x <= maxX; x++)
            {
                for (int y = minY ; y <= maxY; y++)
                {
                    Vector3Int position = new Vector3Int(x, y, 0);
                    if (Vector3.Distance(position, gameMapData[unit]) <= unit.Range)
                    {
                        positions.Add(position);
                    }
                }
            }
            return positions;
        }


        public static bool IsInRangeOf(Unit attacker, Unit defender, GameMapData gameMapData)
        {
            Vector3Int attackerPosition = gameMapData[attacker];
            Vector3Int defenderPosition = gameMapData[defender];

            return Vector3.Distance(attackerPosition, defenderPosition) <= attacker.Range;
        }

        public static IEnumerable<AttackRequest> GetIncomingAttacks(GameMapData gameMapData, Unit defender)
        {
            List<AttackRequest> validIncomingAttacks = new List<AttackRequest>();

            IEnumerable<Unit> rivals = gameMapData.UnitsInPlay.Where(other => !other.Faction.Equals(defender.Faction));
            foreach (Unit rival in rivals)
            {
                AttackRequest request = SimulateAttack(rival, defender, gameMapData);
                if (request.Successful)
                {
                    validIncomingAttacks.Add(request);
                }
            }

            return validIncomingAttacks;
        }


        public static IEnumerable<MapActionRequest> GetAllAttacks(GameMapData gameMapData, Unit attacker)
        {
            List<AttackRequest> attacks = new List<AttackRequest>();

            foreach (Unit opponent in gameMapData.UnitsInPlay.Where(unit => unit.Faction != attacker.Faction))
            {
                AttackRequest generatedRequest = SimulateAttack(attacker, opponent, gameMapData, considerActionPoints: true);

                if (generatedRequest.Successful)
                {
                    attacks.Add(generatedRequest);
                }
            }

            return attacks;
        }
    }
}
