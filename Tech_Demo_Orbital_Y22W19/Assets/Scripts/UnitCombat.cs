using UnityEngine;
using System.Collections.Generic;
using ExtensionMethods;
using System.Linq;

public class UnitCombat
{
    public static readonly int ATTACK_COST = 75;
    public static readonly int MINIMUM_DAMAGE_DEALT = 1;

    private Unit selectedUnitToAttack;
    private GameMapData mapData;
    private Unit currentOffensiveUnit;

    // CONSTRUCTORS
    public UnitCombat(Unit currentOffensiveUnit, GameMapData mapData)
    {
        this.currentOffensiveUnit = currentOffensiveUnit;
        this.mapData = mapData;
    }

    // PUBLIC STATIC METHODS
    /// <summary>
    /// Determines if an attack initiated from an offensive position towards a
    /// defensive position is valid on the given game map, regardless if there exists a 
    /// unit at the defensive position. The returned attack request always has a cost of zero.
    /// </summary>
    /// <param name="gameMapRequesting"></param>
    /// <param name="mapData"></param>
    /// <param name="offensivePosition"></param>
    /// <param name="defensivePosition"></param>
    /// <param name="range"></param>
    /// <returns>An AttackRequest with zero cost</returns>
    public static AttackRequest QueryTargetAttackable(GameMap gameMapRequesting, 
                                                        Vector3Int offensivePosition, 
                                                        Vector3Int defensivePosition, 
                                                        int range)
    {
        GameMapData mapData = gameMapRequesting.MapData;

        Vector3Int sourcePosition = offensivePosition;
        Vector3Int[] tilesHit = new Vector3Int[] { offensivePosition, defensivePosition };

        HashSet<Vector3Int> tilesInRange = GetAllPositionsInRange(offensivePosition, range);

        if (!tilesInRange.Contains(defensivePosition))
        {
            return new AttackRequest(gameMapRequesting,
                                        sourcePosition,
                                        defensivePosition,
                                        AttackStatus.OutOfRange,
                                        tilesHit,
                                        0);
        }

        LineRaytracer raytracer = new LineRaytracer();
        bool hasDirectSight = raytracer.Trace(offensivePosition, defensivePosition, GameMap.UNIT_GRID_OFFSET, mapData);

        if (hasDirectSight)
        {
            sourcePosition = offensivePosition;
            tilesHit = raytracer.TilesHit;
            return new AttackRequest(gameMapRequesting,
                                        sourcePosition,
                                        defensivePosition,
                                        AttackStatus.Success,
                                        tilesHit,
                                        0);
        }

        Vector3Int tileAhead = raytracer.TilesHit[1];

        float unitVector = Vector3.Distance(tileAhead, offensivePosition);

        if (!mapData.HasFullCoverAt(tileAhead) || unitVector > 1)
        {
            // The unit is not against a wall
            sourcePosition = offensivePosition;
            tilesHit = raytracer.TilesHit;
            return new AttackRequest(gameMapRequesting,
                                        sourcePosition,
                                        defensivePosition,
                                        AttackStatus.NoLineOfSight,
                                        tilesHit,
                                        0);
        }

        Vector3Int vectorToWall = tileAhead - offensivePosition;
        Vector3Int peekCoordinates;
        bool canPeekAndShoot;

        for (float angle = -Mathf.PI / 2; angle <= Mathf.PI / 2; angle += Mathf.PI)
        {
            peekCoordinates = offensivePosition + vectorToWall.RotateVector(angle);
            if (mapData.HasFullCoverAt(peekCoordinates))
            {
                // There is an obstacle obstructing a peek
                continue;
            }

            canPeekAndShoot = raytracer.Trace(peekCoordinates, defensivePosition, GameMap.UNIT_GRID_OFFSET, mapData);

            if (canPeekAndShoot)
            {
                sourcePosition = peekCoordinates;
                tilesHit = raytracer.TilesHit;
                return new AttackRequest(gameMapRequesting,
                                        sourcePosition,
                                        defensivePosition,
                                        AttackStatus.Success,
                                        tilesHit,
                                        0);
            }
        }

        // Peeking and firing is also not possible
        return new AttackRequest(gameMapRequesting,
                                    sourcePosition,
                                    defensivePosition,
                                    AttackStatus.PeekUnsuccessful,
                                    tilesHit,
                                    0);

    }

    /// <summary>
    /// Finds all the positions in a grid of squares with sides one unit
    /// that are within a radius of the given range from the given source
    /// position.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public static HashSet<Vector3Int> GetAllPositionsInRange(Vector3Int source, int range)
    {
        HashSet<Vector3Int> rangeBorder = GetRangeBorder(source, range);

        Queue<Vector3Int> pollingQueue = new Queue<Vector3Int>();
        Queue<Vector3Int> offerQueue = new Queue<Vector3Int>();
        pollingQueue.Enqueue(source);

        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();
        visited.Add(source);

        int k = 0;
        while ((offerQueue.Count > 0 || pollingQueue.Count > 0) && k < 300)
        {
            k++;
            int j = 0;
            while (pollingQueue.Count > 0 && j < 25)
            {
                j++;
                Vector3Int current = pollingQueue.Dequeue();

                for (float angle = 0; angle < Mathf.PI * 2; angle += Mathf.PI / 2)
                {
                    Vector3Int directionVector = new Vector3Int((int)Mathf.Sin(angle), (int)Mathf.Cos(angle), 0);
                    Vector3Int neighbourVector = current + directionVector;

                    if (!visited.Contains(neighbourVector) && !rangeBorder.Contains(neighbourVector))
                    {
                        visited.Add(neighbourVector);
                        offerQueue.Enqueue(neighbourVector);
                    }
                }
            }
            Debug.Assert(j < 300, $"Failed 3 j = {j} {pollingQueue.Count}");
            Queue<Vector3Int> temp = pollingQueue;
            pollingQueue = offerQueue;
            offerQueue = temp;
        }
        Debug.Assert(k < 300, "failed 1");
        visited.UnionWith(rangeBorder);

        return visited;
    }

    /// <summary>
    /// Using Weisstein, Eric W.'s Circle-Line Intersection method, finds the border of
    /// the circle with a radius of the given range, centered at the given unit's position,
    /// superimposed onto a grid of squares with sides of one unit, as a hash set.
    /// <para>
    /// https://mathworld.wolfram.com/Circle-LineIntersection.html
    /// </para>
    /// </summary>
    /// <param name="unitPosition"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    private static HashSet<Vector3Int> GetRangeBorder(Vector3Int unitPosition, int range)
    {
        int radius = range;
        int maxX = range;
        int minX = -range;

        int maxY = range;
        int minY = -range;

        IPriorityQueue<CircleLineIntersection> priorityQueue = new MinHeap<CircleLineIntersection>();

        // Finds the continuous intersection points of the horizontal grid lines with the circle
        for (int x = minX; x <= maxX; x++)
        {
            Vector2Int startingVector = new Vector2Int(x, minY);
            Vector2Int endingVector = new Vector2Int(x, maxY);

            int dX = endingVector.x - startingVector.x;
            int dY = endingVector.y - startingVector.y;

            int dRSquared = (int)Mathf.Pow(dX, 2) + (int)Mathf.Pow(dY, 2);

            int determinant = startingVector.x * endingVector.y - endingVector.x * startingVector.y;

            if (Mathf.Pow(range, 2) * dRSquared - Mathf.Pow(determinant, 2) < 0)
            {
                continue;
            }

            float discriminant = Mathf.Pow(range, 2) * dRSquared - Mathf.Pow(determinant, 2);
            if (discriminant < 0)
            {
                continue;
            }

            Vector2 intersectionA = new Vector2(
                (determinant * dY + Mathf.Sign(dY) * dX * Mathf.Sqrt(Mathf.Pow(radius, 2) * dRSquared - Mathf.Pow(determinant, 2))) / dRSquared,
                (-determinant * dX + Mathf.Abs(dY) * Mathf.Sqrt(Mathf.Pow(radius, 2) * dRSquared - Mathf.Pow(determinant, 2))) / dRSquared
                );

            CircleLineIntersection circleLineIntersectionA = new CircleLineIntersection(intersectionA,
                                                                                        Mathf.Atan2(intersectionA.y,
                                                                                                    intersectionA.x));
            priorityQueue.Add(circleLineIntersectionA);

            if (Mathf.Approximately(discriminant, 0))
            {
                continue;
            }

            Vector2 intersectionB = new Vector2(
                (determinant * dY - Mathf.Sign(dY) * dX * Mathf.Sqrt(Mathf.Pow(radius, 2) * dRSquared - Mathf.Pow(determinant, 2))) / dRSquared,
                (-determinant * dX - Mathf.Abs(dY) * Mathf.Sqrt(Mathf.Pow(radius, 2) * dRSquared - Mathf.Pow(determinant, 2))) / dRSquared
                );

            CircleLineIntersection circleLineIntersectionB = new CircleLineIntersection(intersectionB,
                                                                                        Mathf.Atan2(intersectionB.y,
                                                                                                    intersectionB.x));
            priorityQueue.Add(circleLineIntersectionB);
        }

        // Finds the continuous intersection points of the vertical grid lines with the circle
        for (int y = minY; y <= maxY; y++)
        {
            Vector2Int startingVector = new Vector2Int(minX, y);
            Vector2Int endingVector = new Vector2Int(maxX, y);

            int dX = endingVector.x - startingVector.x;
            int dY = endingVector.y - startingVector.y;

            int dRSquared = (int)Mathf.Pow(dX, 2) + (int)Mathf.Pow(dY, 2);

            int determinant = startingVector.x * endingVector.y - endingVector.x * startingVector.y;

            float discriminant = Mathf.Pow(range, 2) * dRSquared - Mathf.Pow(determinant, 2);
            if (discriminant < 0)
            {
                continue;
            }

            Vector2 intersectionA = new Vector2(
                (determinant * dY + Mathf.Sign(dY) * dX * Mathf.Sqrt(Mathf.Pow(radius, 2) * dRSquared - Mathf.Pow(determinant, 2))) / dRSquared,
                (-determinant * dX + Mathf.Abs(dY) * Mathf.Sqrt(Mathf.Pow(radius, 2) * dRSquared - Mathf.Pow(determinant, 2))) / dRSquared
                );

            CircleLineIntersection circleLineIntersectionA = new CircleLineIntersection(intersectionA,
                                                                                        Mathf.Atan2(intersectionA.y,
                                                                                                    intersectionA.x));
            priorityQueue.Add(circleLineIntersectionA);

            if (Mathf.Approximately(discriminant, 0))
            {
                continue;
            }

            Vector2 intersectionB = new Vector2(
                (determinant * dY - Mathf.Sign(dY) * dX * Mathf.Sqrt(Mathf.Pow(radius, 2) * dRSquared - Mathf.Pow(determinant, 2))) / dRSquared,
                (-determinant * dX - Mathf.Abs(dY) * Mathf.Sqrt(Mathf.Pow(radius, 2) * dRSquared - Mathf.Pow(determinant, 2))) / dRSquared
                );

            CircleLineIntersection circleLineIntersectionB = new CircleLineIntersection(intersectionB,
                                                                                        Mathf.Atan2(intersectionB.y,
                                                                                                    intersectionB.x));
            priorityQueue.Add(circleLineIntersectionB);
        }

        HashSet<Vector3Int> visited = new HashSet<Vector3Int>();

        while (!priorityQueue.IsEmpty())
        {
            CircleLineIntersection current = priorityQueue.Extract();
            Vector2 coordinates = current.Coordinates;

            Vector3Int gridCoordinates = new Vector3Int(Mathf.RoundToInt(coordinates.x), Mathf.RoundToInt(coordinates.y), 0);
            if (!visited.Contains(gridCoordinates + unitPosition))
            {
                visited.Add(gridCoordinates + unitPosition);
            }
        }
        return visited;
    }

    // PUBLIC METHODS

    /// <summary>
    /// Gives all the positions of tiles within the range of
    /// the current active unit.
    /// </summary>
    /// <returns>A hash set of all positions of tiles in range</returns>
    public HashSet<Vector3Int> GetAllPositionsInRange()
    {
        return GetAllPositionsInRange(mapData.GetPositionByUnit(currentOffensiveUnit), currentOffensiveUnit.Range);
    }

    /// <summary>
    /// From the given gameMap, retrieves a set of all possible attack positions
    /// that could be targetted by the current active unit.
    /// </summary>
    /// <param name="gameMap"></param>
    /// <returns>A hash set of attackable positions</returns>
    public HashSet<Vector3Int> GetAllPositionsAttackable(GameMap gameMap)
    {
        return new HashSet<Vector3Int> (mapData.PositionUnitMapping.Keys
                                    .Where(x => QueryTargetAttackable(gameMap, mapData.GetUnitByPosition(x)).Successful));
    }

    /// <summary>
    /// From the given gameMap, retrieves a set of all possible attacks 
    /// that could be potentially issued by the current active unit.
    /// </summary>
    /// <param name="gameMap"></param>
    /// <returns>A hash set of possible AttackRequests</returns>
    public HashSet<AttackRequest> GetAllPossibleAttacks(GameMap gameMap)
    {
        return new HashSet<AttackRequest>(mapData.PositionUnitMapping.Keys
                                            .Select(x => QueryTargetAttackable(gameMap, mapData.GetUnitByPosition(x)))
                                            .Where(x => x.Status == AttackStatus.Success));
    }


    /// <summary>
    /// Determines if the current active unit of the GameMap is able
    /// to issue an attack against a target unit. If an attack is possible,
    /// an attack cost is pegged to the AttackRequest.
    /// </summary>
    /// <param name="gameMapRequesting"></param>
    /// <param name="target"></param>
    /// <returns></returns>
    public AttackRequest QueryTargetAttackable(GameMap gameMapRequesting, Unit target)
    {
        Vector3Int offensiveUnitPosition = mapData.GetPositionByUnit(currentOffensiveUnit);
        Vector3Int targetUnitPosition = mapData.GetPositionByUnit(target);

        Vector3Int sourcePosition = offensiveUnitPosition;
        Vector3Int[] tilesHit = new Vector3Int[] { offensiveUnitPosition , targetUnitPosition };

        if (target.Faction == currentOffensiveUnit.Faction)
        {
            return new AttackRequest(gameMapRequesting, 
                                        sourcePosition,
                                        targetUnitPosition, 
                                        AttackStatus.IllegalTarget,
                                        tilesHit,
                                        0);
        }

        if (currentOffensiveUnit.ActionPointsLeft < ATTACK_COST)
        {
            return new AttackRequest(gameMapRequesting,
                                        sourcePosition,
                                        targetUnitPosition,
                                        AttackStatus.NotEnoughAP,
                                        tilesHit,
                                        0);
        }

        HashSet<Vector3Int> tilesInRange = GetAllPositionsInRange(offensiveUnitPosition, currentOffensiveUnit.Range);

        if (!tilesInRange.Contains(targetUnitPosition))
        {
            return new AttackRequest(gameMapRequesting,
                                        sourcePosition,
                                        targetUnitPosition,
                                        AttackStatus.OutOfRange,
                                        tilesHit,
                                        0);
        }

        LineRaytracer raytracer = new LineRaytracer();
        bool hasDirectSight = raytracer.Trace(offensiveUnitPosition, targetUnitPosition, GameMap.UNIT_GRID_OFFSET, mapData);

        if (hasDirectSight)
        {
            sourcePosition = offensiveUnitPosition;
            tilesHit = raytracer.TilesHit;
            return new AttackRequest(gameMapRequesting,
                                        sourcePosition,
                                        targetUnitPosition,
                                        AttackStatus.Success,
                                        tilesHit,
                                        ATTACK_COST);
        }

        Vector3Int tileAhead = raytracer.TilesHit[1];

        float unitVector = Vector3.Distance(tileAhead, offensiveUnitPosition);

        if (!mapData.HasFullCoverAt(tileAhead) || unitVector > 1)
        {
            // The unit is not against a wall
            sourcePosition = offensiveUnitPosition;
            tilesHit = raytracer.TilesHit;
            return new AttackRequest(gameMapRequesting,
                                        sourcePosition,
                                        targetUnitPosition,
                                        AttackStatus.NoLineOfSight,
                                        tilesHit,
                                        0);
        }

        Vector3Int vectorToWall = tileAhead - offensiveUnitPosition;
        Vector3Int peekCoordinates;
        bool canPeekAndShoot;

        for (float angle = -Mathf.PI / 2; angle <= Mathf.PI / 2; angle += Mathf.PI)
        {
            peekCoordinates = offensiveUnitPosition + vectorToWall.RotateVector(angle);
            if (mapData.HasFullCoverAt(peekCoordinates))
            {
                // There is an obstacle obstructing a peek
                continue;
            }

            canPeekAndShoot = raytracer.Trace(peekCoordinates, targetUnitPosition, GameMap.UNIT_GRID_OFFSET, mapData);

            if (canPeekAndShoot)
            {
                sourcePosition = peekCoordinates;
                tilesHit = raytracer.TilesHit;
                Debug.Log("Only peek");
                return new AttackRequest(gameMapRequesting,
                                        sourcePosition,
                                        targetUnitPosition,
                                        AttackStatus.Success,
                                        tilesHit,
                                        ATTACK_COST);
            }
        }

        raytracer.Trace(offensiveUnitPosition, targetUnitPosition, GameMap.UNIT_GRID_OFFSET, mapData);
        // Peeking and firing is also not possible
        return new AttackRequest(gameMapRequesting,
                                    sourcePosition,
                                    targetUnitPosition,
                                    AttackStatus.PeekUnsuccessful,
                                    raytracer.TilesHit,
                                    0);

    }

    // PRIVATE METHODS

    private bool HasLineOfSight(Vector3Int offensivePosition, Vector3Int targetPosition)
    {

        return new LineRaytracer().Trace(offensivePosition, targetPosition, mapData);
    }

}