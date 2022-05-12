using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ReachableTilesFinder : Pathfinder2D
{
    UnitManager unitManager;
    public ReachableTilesFinder(UnitManager unitManager, TileManager map, int initialPathCost = 0) 
        : base( unitManager.SelectedUnit, 
                new Node(map.Ground.WorldToCell(unitManager.SelectedUnit.WorldPosition)),
                null,
                map, 
                initialPathCost )
    {
        this.unitManager = unitManager;
    }

    public HashSet<Vector3Int> GetReachableTilePositions()
    {
        FindDirectedPath();
        return new HashSet<Vector3Int>(gScore
                                        .Where(pair => pair.Value <= unit.ActionPointsLeft 
                                                        && map.Ground.HasTile(pair.Key.Coordinates) 
                                                        && !map.Obstacles.HasTile(pair.Key.Coordinates) 
                                                        && !unitManager.PositionsOfUnits.ContainsKey(pair.Key.Coordinates))
                                        .Select(pair => pair.Key.Coordinates));
    }

    public static Vector3Int[] GetRangeBorder(Vector3Int source, int range)
    {
        Vector3Int unitPosition = source;

        int radius = range;
        int maxX =  range;
        int minX = -range;

        int maxY = range;
        int minY = -range;

        IPriorityQueue<CircleLineIntersection> priorityQueue = new MinHeap<CircleLineIntersection>();

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

        List<Vector3Int> resultBorder = new List<Vector3Int>();
        HashSet<Vector3Int> added = new HashSet<Vector3Int>();

        while (!priorityQueue.IsEmpty())
        {
            CircleLineIntersection current = priorityQueue.Extract();
            Vector2 coordinates = current.Coordinates;

            Vector3Int gridCoordinates = new Vector3Int(Mathf.RoundToInt(coordinates.x), Mathf.RoundToInt(coordinates.y), 0);
            if (!added.Contains(gridCoordinates))
            {
                added.Add(gridCoordinates);
                resultBorder.Add(gridCoordinates + unitPosition);
            }
        }
        return resultBorder.ToArray();
    }
}