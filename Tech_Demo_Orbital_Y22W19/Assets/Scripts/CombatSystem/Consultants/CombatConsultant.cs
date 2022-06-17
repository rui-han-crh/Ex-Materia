using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CombatSystem.Entities;
using System.Linq;
using UnityEngine.Extensions;
using Algorithms.Rasterisers;

namespace CombatSystem.Consultants
{
    public static class CombatConsultant
    {
        public static readonly int ATTACK_AP_COST = 75;
        public static readonly int MINIMUM_DAMAGE_DEALT = 2;
        public static readonly Vector3 OFFSET = Vector2.one / 2;

        public static AttackRequest SimulateAttack(Unit attacker, Unit defender, GameMapData gameMapData, bool considerActionPoints = false)
        {
            if (considerActionPoints && attacker.CurrentActionPoints < ATTACK_AP_COST)
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

            int potentialDamage = Mathf.Max(0, attacker.Attack - defender.Defence);

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
                    1,
                    raytracedLine,
                    ATTACK_AP_COST,
                    ATTACK_AP_COST,
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
                        1,
                        peekingLine,
                        ATTACK_AP_COST,
                        ATTACK_AP_COST,
                        AttackRequest.Outcome.Successful);
                }
            }

            return AttackRequest.CreateFailedRequest(attacker, defender, raytracedLine, AttackRequest.Outcome.NoLineOfSight);
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
