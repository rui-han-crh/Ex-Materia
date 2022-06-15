using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using Math.Geometry.Shapes;
using static Math.Extensions.NumericExtensions;

namespace Algorithms.Rasterisers
{
    public static class Rasteriser
    {
        public static IEnumerable<Vector3Int> AdaptedBresenham(Vector3 source, Vector3 destination, int gridSize)
        {
            int maxX = Mathf.FloorToInt(Mathf.Max(source.x, destination.x));
            int minX = Mathf.CeilToInt(Mathf.Min(source.x, destination.x));

            int maxY = Mathf.FloorToInt(Mathf.Max(source.y, destination.y));
            int minY = Mathf.CeilToInt(Mathf.Min(source.y, destination.y));

            IPriorityQueue<LineIntersection, float> priorityQueue = new SimplePriorityQueue<LineIntersection, float>();

            Line sightLine = new Line(source, Vector3.Normalize(destination - source));

            for (int x = minX; x <= maxX; x += gridSize)
            {
                Line gridLine = new Line(new Vector2(x, 0), Vector2.up);
                LineIntersection intersection = sightLine.IntersectionWithLine(gridLine);
                priorityQueue.Enqueue(intersection, intersection.IntersectionAlpha);
            }

            for (int y = minY; y <= maxY; y += gridSize)
            {
                Line gridLine = new Line(new Vector2(0, y), Vector2.right);
                LineIntersection intersection = sightLine.IntersectionWithLine(gridLine);
                priorityQueue.Enqueue(intersection, intersection.IntersectionAlpha);
            }

            List<Vector3Int> tilesHit = new List<Vector3Int>();

            Vector3Int currentCoordinates = new Vector3Int(Mathf.FloorToInt(source.x), Mathf.FloorToInt(source.y), 0);
            tilesHit.Add(currentCoordinates);

            float lastPriority = -1;

            while (priorityQueue.Count > 0)
            {
                LineIntersection extractedIntersection = priorityQueue.Dequeue();
                if (Approximately(extractedIntersection.IntersectionAlpha, lastPriority, 0.05f))
                {
                    tilesHit.RemoveAt(tilesHit.Count - 1);
                }

                lastPriority = extractedIntersection.IntersectionAlpha;

                if (extractedIntersection.LineBeta.Direction.y == 0)
                {
                    currentCoordinates += (int)Mathf.Sign(destination.y - source.y) * Vector3Int.up;
                }
                else
                {
                    currentCoordinates += (int)Mathf.Sign(destination.x - source.x) * Vector3Int.right;
                }
                tilesHit.Add(currentCoordinates);
            }

            return tilesHit;
        }
    }
}
