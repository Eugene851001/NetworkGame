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
    }
}
