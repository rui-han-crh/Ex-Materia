using System;
using UnityEngine;

public class LineIntersection
{
    private readonly Line lineAlpha;
    private readonly Line lineBeta;
    private readonly float intersectionAlpha;
    private readonly float intersectionBeta;

    private readonly bool noIntersection;
    private readonly bool overlappingLines;
    public static int NUM_OF_VARIABLES = 2;

    public LineIntersection(Line lineAlpha, Line lineBeta, float alpha, float beta)
    {
        this.lineAlpha = lineAlpha;
        this.lineBeta = lineBeta;
        this.intersectionAlpha = alpha;
        this.intersectionBeta = beta;
    }

    public LineIntersection(LineIntersection lineIntersection)
    {
        this.lineAlpha = lineIntersection.lineAlpha;
        this.lineBeta = lineIntersection.lineBeta;
        this.intersectionAlpha = lineIntersection.intersectionAlpha;
        this.lineBeta = lineIntersection.lineBeta;
        this.noIntersection = lineIntersection.noIntersection;
        this.overlappingLines = lineIntersection.overlappingLines;
    }

    private LineIntersection(bool noIntersection, bool overlappingLines)
    {
        this.noIntersection = noIntersection;
        this.overlappingLines = overlappingLines;
    }

    public static LineIntersection NoIntersection()
    {
        return new LineIntersection(true, false);
    }

    public static LineIntersection OverlappingLines()
    {
        return new LineIntersection(false, true);
    }

    public bool HasIntersection => !noIntersection && !overlappingLines;

    public Line LineAlpha => lineAlpha;

    public Line LineBeta => lineBeta;

    public float IntersectionAlpha => intersectionAlpha;

    public float IntersectionBeta => intersectionBeta;

    public Vector3 PointOfIntersection => (Vector3)(lineAlpha?.GetPointOnLine(IntersectionAlpha));
}