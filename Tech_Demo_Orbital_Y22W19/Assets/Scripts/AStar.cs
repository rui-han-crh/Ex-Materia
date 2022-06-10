using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.TestTools;

public class AStar
{
    public struct Node
    {
        public readonly Vector3Int Coordinates;
        public readonly int Cost;

        public Node(Vector3Int coordinates, int cost)
        {
            Coordinates = coordinates;
            Cost = cost;
        }
    }

    private readonly Dictionary<Vector3Int, Node> nodes;
    private readonly Node source;
    private readonly Node destination;

    public AStar(Vector3Int source, Vector3Int destination, Dictionary<Vector3Int, int> positionToCostMapping)
    {
        this.source = new Node(source, positionToCostMapping[source]);
        this.destination = new Node(destination, positionToCostMapping[destination]);

        nodes.Add(source, this.source);
        nodes.Add(destination, this.destination);

        foreach (KeyValuePair<Vector3Int, int> tileToCost in positionToCostMapping)
        {
            nodes.Add(tileToCost.Key, new Node(tileToCost.Key, tileToCost.Value));
        }
    }

    public void Solve()
    {
        throw new NotImplementedException();
    
    }
    
}
