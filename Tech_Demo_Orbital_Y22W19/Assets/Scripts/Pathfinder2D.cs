using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder2D
{
    public class ShortestPathTree
    {
        private readonly Dictionary<Vector3Int, Vector3Int> childParentMap;
        private readonly Dictionary<Vector3Int, int> costs;

        public IEnumerable<Vector3Int> Children => childParentMap.Keys;
        public Dictionary<Vector3Int, int> Costs => costs;

        public ShortestPathTree(Dictionary<Vector3Int, Vector3Int> childParentMap, Dictionary<Vector3Int, int> costs)
        {
            this.childParentMap = childParentMap;
            this.costs = costs;
        }

        public int GetCostToPosition(Vector3Int position)
        {
            return costs.ContainsKey(position) ? costs[position] : int.MaxValue;
        }

        public Vector3Int[] GetShortestPathToPosition(Vector3Int position)
        {
            List<Vector3Int> result = new List<Vector3Int>();

            Vector3Int current = position;

            while (childParentMap.ContainsKey(current) && childParentMap[current] != null)
            {
                result.Add(current);
                current = childParentMap[current];
            }

            result.Reverse();
            return result.ToArray();
        }
    }

    private const int INITIAL_PQ_SIZE = 3;

    private readonly IPriorityQueue<Node> priorityQueue;
    private readonly Dictionary<Node, Node> childParentMap;

    private readonly Node source;
    private readonly Node destination;

    protected readonly Dictionary<Node, int> gScore;

    public readonly static int DECIMAL_PLACE_MULTIPLIER = 10;

    protected GameMapData mapData;

    private int pathCost;
    private readonly int initialPathCost;

    protected UnitOld unit;
    public int PathCost => pathCost;


    // CONSTRUCTORS
    public Pathfinder2D(UnitOld unit, Vector3Int sourcePosition, Vector3Int destinationPosition, GameMapData mapData, int initialPathCost = 0)
    {
        this.unit = unit;
        this.mapData = mapData;
        this.source = new Node(sourcePosition);
        this.pathCost = initialPathCost;
        this.initialPathCost = initialPathCost;

        int mapSize = INITIAL_PQ_SIZE;
        childParentMap = new Dictionary<Node, Node>(mapSize);
        gScore = new Dictionary<Node, int>(mapSize);

        priorityQueue = new MinHeap<Node>(mapSize);

        this.destination = new Node(destinationPosition);

        gScore[source] = 0;
        source.Value = HScore(source);
        priorityQueue.Add(source);
        childParentMap.Add(source, null);
    }

    private Pathfinder2D(UnitOld unit, GameMapData mapData)
    {
        this.unit = unit;
        this.source = new Node(mapData.GetPositionByUnit(unit));
        this.mapData = mapData;
        destination = null;
        this.initialPathCost = 0;

        int mapSize = INITIAL_PQ_SIZE;
        childParentMap = new Dictionary<Node, Node>(mapSize);
        gScore = new Dictionary<Node, int>(mapSize);

        priorityQueue = new MinHeap<Node>(mapSize);

        gScore[source] = 0;
        source.Value = HScore(source);
        priorityQueue.Add(source);
        childParentMap.Add(source, null);
    }


    // PUBLIC STATIC METHODS
    public static Dictionary<Vector3Int, int> GetAllReachablePositions(UnitOld unit, GameMapData mapData)
    {
        Pathfinder2D pathfinder = new Pathfinder2D(unit, mapData);
        pathfinder.FindDirectedPath();
        return new Dictionary<Vector3Int, int>(pathfinder.gScore
                                        .Where(pair => pair.Value <= unit.ActionPointsLeft
                                                        && mapData.IsWalkableOn(pair.Key.Coordinates)
                                                        && !mapData.ExistsUnitAt(pair.Key.Coordinates))
                                        .ToDictionary(pair => pair.Key.Coordinates, pair => pathfinder.gScore[pair.Key]));
    }

    public static ShortestPathTree GetShortestPathTree(UnitOld unit, GameMapData mapData)
    {
        Pathfinder2D pathfinder = new Pathfinder2D(unit, mapData);
        pathfinder.FindDirectedPath();
        Dictionary<Vector3Int, int> allReachablePositions = new Dictionary<Vector3Int, int>(pathfinder.gScore
                                        .Where(pair => pair.Value <= unit.ActionPointsLeft
                                                        && mapData.IsWalkableOn(pair.Key.Coordinates)
                                                        && !mapData.ExistsUnitAt(pair.Key.Coordinates))
                                        .ToDictionary(pair => pair.Key.Coordinates, pair => pathfinder.gScore[pair.Key]));

        return new ShortestPathTree(pathfinder
                                    .childParentMap
                                    .Where(x => allReachablePositions.ContainsKey(x.Key.Coordinates))
                                    .ToDictionary(x => x.Key.Coordinates, x => x.Value.Coordinates),
                                    allReachablePositions);
    }

    // PUBLIC METHODS
    public Node[] FindDirectedPath(bool unboundedActionPoints = false)
    {
        if (destination != null && !mapData.IsWalkableOn(destination.Coordinates))
        {
            return new Node[0];
        }

        while (!priorityQueue.IsEmpty())
        {
            Node current = priorityQueue.Extract();

            if (!unboundedActionPoints && gScore[current] > unit.ActionPointsLeft)
            {
                return new Node[0];
            }

            if (current.Equals(destination))
            {
                return ConstructPath(current);
            }

            for (int i = 0; i < 8; i++) // potentially 8 neighbours surrounding
            {
                float angle = i * Mathf.PI / 4;

                Vector3Int neighbourCoordinates = new Vector3Int(
                    Mathf.RoundToInt(current.Coordinates.x + Mathf.Sin(angle)),
                    Mathf.RoundToInt(current.Coordinates.y + Mathf.Cos(angle)),
                    0
                    );

                if (!mapData.IsWalkableOn(neighbourCoordinates))
                {
                    continue;
                }

                Node neighbour = new Node(neighbourCoordinates, mapData.GetTileCost(neighbourCoordinates));

                int tentative_gScore = gScore[current] + current.DistanceTo(neighbour) + neighbour.Weight;

                if (!gScore.ContainsKey(neighbour) || tentative_gScore < gScore[neighbour])
                {
                    childParentMap[neighbour] = current;
                    gScore[neighbour] = tentative_gScore;

                    neighbour.Value = HScore(neighbour) + tentative_gScore;
                    
                    if (!priorityQueue.Contains(neighbour))
                    {
                        priorityQueue.Add(neighbour);
                    }
                    else
                    {
                        priorityQueue.DecreaseKey(neighbour);
                    }
                }
            }
        }
        return new Node[0]; // no path
    }

    // PRIVATE METHODS
    private Node[] ConstructPath(Node current)
    {
        List<Node> result = new List<Node>();
        int newPathCost = gScore[current];

        while (childParentMap.ContainsKey(current) && childParentMap[current] != null)
        {
            if (gScore[current] + pathCost <= unit.ActionPointsLeft)
            {
                current.IsReachable = true;
            }
            result.Add(current);
            current = childParentMap[current];
        }

        if (gScore[current] + pathCost <= unit.ActionPointsLeft)
        {
            current.IsReachable = true;
        }

        result.Reverse();

        pathCost = initialPathCost + newPathCost;
        return result.ToArray();
    }

    private int HScore(Node node)
    {
        if (destination == null)
        {
            return 0;
        }
        return (int)(Vector3.Distance(node.Coordinates, destination.Coordinates) * DECIMAL_PLACE_MULTIPLIER);
    }
}
