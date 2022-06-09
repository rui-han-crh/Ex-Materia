using UnityEngine;
using System;
using System.Collections.Generic;

public class AStar
{
    private struct Node
    {
        public readonly Vector3Int Coordinates;
        public readonly int Cost;
        public readonly HashSet<Node> Children;

        public Node(Vector3Int coordinates, int cost, HashSet<Node> children)
        {
            Coordinates = coordinates;
            Cost = cost;
            Children = children;
        }
    }

    private readonly Dictionary<Vector3Int, Node> nodes;
    private readonly Node source;
    private readonly Node destination;

    public AStar(Vector3Int source, Vector3Int destination, Dictionary<Vector3Int, int> permittedZone)
    {

    }

    /// Unfinished, unused :<
}
