using System;
using UnityEngine;

namespace Math.Geometry.Shapes
{
    public class LineIntersection
    {
        public enum Intersection
        {
            SinglePoint,
            Overlapping,
            NoIntersection
        }

        private readonly Line lineAlpha;
        private readonly Line lineBeta;
        private readonly float intersectionAlpha;
        private readonly float intersectionBeta;
        private readonly Intersection intersection;

        public static int NUM_OF_VARIABLES = 2;

        public LineIntersection(Line lineAlpha, Line lineBeta, float alpha, float beta)
        {
            this.lineAlpha = lineAlpha;
            this.lineBeta = lineBeta;
            this.intersectionAlpha = alpha;
            this.intersectionBeta = beta;
            this.intersection = Intersection.SinglePoint;
        }

        public LineIntersection(LineIntersection lineIntersection)
        {
            this.lineAlpha = lineIntersection.lineAlpha;
            this.lineBeta = lineIntersection.lineBeta;
            this.intersectionAlpha = lineIntersection.intersectionAlpha;
            this.lineBeta = lineIntersection.lineBeta;
            this.intersection = lineIntersection.intersection;
        }

        private LineIntersection(Intersection intersection)
        {
            this.intersection = intersection;
        }

        public static LineIntersection NoIntersection()
        {
            return new LineIntersection(Intersection.NoIntersection);
        }

        public static LineIntersection OverlappingLines()
        {
            return new LineIntersection(Intersection.Overlapping);
        }

        public bool HasDistinctIntersection => intersection == Intersection.SinglePoint;

        public Line LineAlpha => lineAlpha;

        public Line LineBeta => lineBeta;

        public float IntersectionAlpha => intersectionAlpha;

        public float IntersectionBeta => intersectionBeta;

        public Vector3 PointOfIntersection => (Vector3)(lineAlpha?.GetPointOnLine(IntersectionAlpha));
    }
}