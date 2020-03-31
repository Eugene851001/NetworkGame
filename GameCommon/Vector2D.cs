using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
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
            X = X / length;
            Y = Y / length;
            return this;
        }

        public Vector2D()
        {
            X = Y = 0;
        }
    }
}
