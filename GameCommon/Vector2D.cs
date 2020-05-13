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
            double crossProduct = firstVector.X * secondVector.X + secondVector.Y * secondVector.Y;
            double lengthFirst = firstVector.GetLength();
            double lengthSecond = secondVector.GetLength();
            if (lengthFirst == 0 || lengthSecond == 0)
                return 0;
            return Math.Acos(crossProduct / (lengthFirst * lengthSecond));
        }

        public Vector2D()
        {
            X = Y = 0;
        }
    }
}
