using System;
using UnityEngine;

public class Node : IComparable<Node>
{
    private int value = -1;
    private readonly int weight;
    private readonly Vector3Int coordinates;
    private bool isReachable = false;

    public Node(Vector3Int coordinates)
    {
        this.coordinates = coordinates;
    }

    public Node(Vector3Int coordinates, int weight)
    {
        this.coordinates = coordinates;
        this.weight = weight;
    }

    public int Value 
    { 
        get { return value; }  
        set { this.value = value; } 
    }

    public bool IsReachable
    {
        get { return isReachable; }
        set { isReachable = value; }
    }

    public int Weight => weight;


    public Vector3Int Coordinates => coordinates;

    public Node[] GetNeighbours()
    {
        return new Node[0];
    }

    public int DistanceTo(Node other)
    {
        return (int) (Vector3.Distance(this.Coordinates, other.Coordinates) * Pathfinder2D.DECIMAL_PLACE_MULTIPLIER);
    }

    public int CompareTo(Node other)
    {
        return this.value - other.value;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Node))
        {
           return false;
        }
        return this.Coordinates.Equals(((Node) obj).Coordinates);
    }

    public override int GetHashCode()
    {
        return this.Coordinates.GetHashCode();
    }

    public override string ToString()
    {
        return this.Coordinates + " " + this.value.ToString();
    }
}