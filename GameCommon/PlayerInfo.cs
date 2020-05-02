using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCommon
{
    [Serializable]
    public class PlayerInfo
    {
        public Vector2D Position;
        public int Health;

        public PlayerInfo(Vector2D position, int health)
        {
            Position = position;
            Health = health;
        }

        public PlayerInfo()
        {
            Position = new Vector2D(0, 0);
            Health = 100;
        }

    }
}
