using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    [Serializable]
    public class GameObject
    {
        public Vector2D Position;
        public double Size;
        public bool IsDestroy = false;
    }
}
