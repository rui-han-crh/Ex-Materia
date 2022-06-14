using UnityEngine;
using System;
using MathNet.Numerics.LinearAlgebra.Factorization;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

namespace Math.Geometry.Shapes
{
    public class Line
    {
        // Lines are represented in the form start + magnitude * direction

        private readonly Vector3 origin;
        private readonly Vector3 direction;

        public Vector3 Origin => origin;
        public Vector3 Direction => direction;


        public Line(Vector2 startingVector, Vector2 directionVector)
        {
            this.origin = startingVector;
            this.direction = directionVector;
        }

        public LineIntersection IntersectionWithLine(Line otherLine)
        {
            Matrix<float> A = DenseMatrix.OfArray(new float[,]
            {
            {this.direction.x, -otherLine.direction.x},
            {this.direction.y, -otherLine.direction.y},
            {this.direction.z, -otherLine.direction.z}
            });

            Vector<float> B = DenseVector.OfArray(new float[]
            {
            otherLine.origin.x - this.origin.x,
            otherLine.origin.y - this.origin.y,
            otherLine.origin.z - this.origin.z
            });

            if (A.Rank() < A.Append(B.ToColumnMatrix()).Rank()) // System is inconsistent if rank(A) < rank(A | B)
            {
                return LineIntersection.NoIntersection();
            }

            if (A.Rank() < LineIntersection.NUM_OF_VARIABLES)   // System is consistent but has no unique soln
            {
                return LineIntersection.OverlappingLines();
            }

            // Ax = B
            Vector<float> x = A.Solve(B);

            return new LineIntersection(this, otherLine, x[0], x[1]);
        }

        public Vector3 GetPointOnLine(float magnitude)
        {
            return origin + magnitude * direction;
        }


        public override string ToString()
        {
            // scalar multiples to all variables are the same line
            return $"{origin} + α{direction}";
        }
    }
}