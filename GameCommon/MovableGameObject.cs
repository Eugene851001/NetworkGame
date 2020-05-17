using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class MovableGameObject: GameObject
    {
        public Vector2D Direction;
        public double Speed;

        public MovableGameObject()
        {
        }

        public void Move(int time)
        {
            Position.X += Direction.X * Speed * time;
            Position.Y += Direction.Y * Speed * time;
        }
    }
}
