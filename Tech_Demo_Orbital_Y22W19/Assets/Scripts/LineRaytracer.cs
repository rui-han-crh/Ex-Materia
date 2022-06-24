using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using static ExtensionMethods.MathExtensions;
public class LineRaytracer
{
    private static readonly float THRESHOLD = 0.01f;
    private Vector3Int[] tilesHit;

    /// <summary>
    /// Wrapper class for LineIntersection that implements IComparable to be used in a priority queue
    /// </summary>
    internal class LineIntersectionComparable : LineIntersection, IComparable<LineIntersectionComparable>
    {
        private readonly bool isHorizontal;
        public LineIntersectionComparable(Line lineAlpha, Line lineBeta, float alpha, float beta, bool isHorizontal) : 
            base(lineAlpha, lineBeta, alpha, beta)
        {
            this.isHorizontal = isHorizontal;
        }

        public LineIntersectionComparable(LineIntersection lineIntersection, bool isHorizontal) : base(lineIntersection)
        {
            this.isHorizontal = isHorizontal;
        }

        public bool IsHorizontal => isHorizontal;

        public int CompareTo(LineIntersectionComparable other)
        {
            return (int) Mathf.Sign(this.IntersectionAlpha - other.IntersectionAlpha);
        }

        public float Priority => IntersectionAlpha;
    }

    public bool Trace(Vector3 source, Vector3 destination, Vector3 offset, GameMapDataOld mapData)
    {
        return Trace(source + offset, destination + offset, mapData);
    }

    public bool Trace(Vector3 source, Vector3 destination, GameMapDataOld mapData)
    {
        int maxX = Mathf.FloorToInt(Mathf.Max(source.x, destination.x));
        int minX = Mathf.CeilToInt(Mathf.Min(source.x, destination.x));

        int maxY = Mathf.FloorToInt(Mathf.Max(source.y, destination.y));
        int minY = Mathf.CeilToInt(Mathf.Min(source.y,destination.y));


        IPriorityQueue<LineIntersectionComparable> priorityQueue = new MinHeap<LineIntersectionComparable>();

        Line sightLine = new Line(source, Vector3.Normalize(destination - source));

        for (int x = minX; x <= maxX; x++)
        {
            Line gridLine = new Line(new Vector2(x, 0), Vector2.up);
            priorityQueue.Add(new LineIntersectionComparable(sightLine.IntersectionWithLine(gridLine), false));
        }

        for (int y = minY; y <= maxY; y++)
        {
            Line gridLine = new Line(new Vector2(0, y), Vector2.right);
            priorityQueue.Add(new LineIntersectionComparable(sightLine.IntersectionWithLine(gridLine), true));
        }

        List<Vector3Int> tilesHit = new List<Vector3Int>();

        Vector3Int currentCoordinates = new Vector3Int(Mathf.FloorToInt(source.x), Mathf.FloorToInt(source.y), 0);
        tilesHit.Add(currentCoordinates);

        float lastPriority = -1;

        while (!priorityQueue.IsEmpty())
        {
            LineIntersectionComparable extractedIntersection = priorityQueue.Extract();
            if (Approximately(extractedIntersection.Priority, lastPriority, THRESHOLD))
            {
                tilesHit.RemoveAt(tilesHit.Count - 1);
            }

            lastPriority = extractedIntersection.Priority;

            if (extractedIntersection.IsHorizontal)
            {
                currentCoordinates += (int) Mathf.Sign(destination.y - source.y) * Vector3Int.up;
            } 
            else
            {
                currentCoordinates += (int) Mathf.Sign(destination.x - source.x) * Vector3Int.right;
            }
            tilesHit.Add(currentCoordinates);
        }

        this.tilesHit = tilesHit.ToArray();

        return !tilesHit.Any(x => mapData.HasFullCoverAt(x));
    }

    public Vector3Int[] TilesHit => this.tilesHit;
}