using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class Vector2D
    {
        public double X;
        public double Y;

        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2D Normalize()
        {
            double length = Math.Sqrt(X * X + Y * Y);
            if (length != 0)
            {
                X = X / length;
                Y = Y / length;
            }
            return this;
        }

        public double GetLength()
        {
            return Math.Sqrt(X * X + Y * Y);
        }

        public Vector2D(Vector2D vector)
        {
            X = vector.X;
            Y = vector.Y;
        }

        public static double GetAngle(Vector2D firstVector, Vector2D secondVector)
        {
            double dotProduct = firstVector.X * secondVector.X + firstVector.Y * secondVector.Y;
            double determinant = firstVector.X * secondVector.Y - firstVector.Y * secondVector.X;
            return Math.Atan2(determinant , dotProduct);
        }

        public Vector2D()
        {
            X = Y = 0;
        }
    }
}
