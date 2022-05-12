using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class CircleLineIntersection : IComparable<CircleLineIntersection>
{
    private readonly float x;
    private readonly float y;
    private readonly float angle;

    public CircleLineIntersection(float x, float y, int angle)
    {
        this.x = x;
        this.y = y;
        this.angle = angle;
    }

    public CircleLineIntersection(Vector2 coordinates, float angle)
    {
        this.x = coordinates.x;
        this.y = coordinates.y;
        this.angle = angle;
    }

    public Vector2 Coordinates => new Vector2(x, y);

    public float Angle => angle;

    public int CompareTo(CircleLineIntersection other)
    {
        return (int) Mathf.Sign(this.angle - other.angle);
    }

    public override string ToString()
    {
        return Coordinates.ToString();
    }
}