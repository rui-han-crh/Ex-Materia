using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder2D
{
    private const int INITIAL_PQ_SIZE = 3;

    private readonly IPriorityQueue<Node> priorityQueue;
    private readonly Dictionary<Node, Node> childParentMap;

    private readonly Node source;
    private readonly Node destination;

    protected readonly Dictionary<Node, int> gScore;

    public readonly static int DECIMAL_PLACE_MULTIPLIER = 10;

    protected TileManager map;

    private int pathCost;
    private readonly int initialPathCost;

    protected Unit unit;


    public Pathfinder2D(Unit unit, Node source, Node destination, TileManager map, int initialPathCost = 0)
    {
        this.unit = unit;
        this.map = map;
        this.source = source;
        this.pathCost = initialPathCost;
        this.initialPathCost = initialPathCost;

        int mapSize = INITIAL_PQ_SIZE;
        childParentMap = new Dictionary<Node, Node>(mapSize);
        gScore = new Dictionary<Node, int>(mapSize);

        priorityQueue = new MinHeap<Node>(mapSize);

        this.destination = destination;

        gScore[source] = 0;
        source.Value = HScore(source);
        priorityQueue.Add(source);
        childParentMap.Add(source, null);
    }

    /// <summary>
    /// Gets the cost of the path calculated by latest called GetDirectedPath
    /// </summary>
    /// <returns></returns>
    public int PathCost => pathCost;

    protected int HScore(Node node)
    {
        if (destination == null)
        {
            return 0;
        }
        return (int) (Vector3.Distance(node.Coordinates, destination.Coordinates) * DECIMAL_PLACE_MULTIPLIER);
    }

    public Node[] FindDirectedPath()
    {
        if (destination != null 
            && (!map.Ground.HasTile(destination.Coordinates) || map.Obstacles.HasTile(destination.Coordinates)))
        {
            return new Node[0];
        }

        while (!priorityQueue.IsEmpty())
        {
            Node current = priorityQueue.Extract();

            if (gScore[current] > unit.ActionPointsLeft)
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

                Vector3Int nodeCoordinates = new Vector3Int(
                    Mathf.RoundToInt(current.Coordinates.x + Mathf.Sin(angle)),
                    Mathf.RoundToInt(current.Coordinates.y + Mathf.Cos(angle)),
                    0
                    );


                Node neighbour = new Node(nodeCoordinates, map.GetTileCost(map.Ground.GetTile<Tile>(nodeCoordinates)));

                if (map.Obstacles.HasTile(nodeCoordinates) || !map.Ground.HasTile(nodeCoordinates))
                {
                    continue;
                }


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
}
