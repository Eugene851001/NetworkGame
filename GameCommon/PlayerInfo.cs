using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCommon
{
    public class PlayerInfo : IPlayerInfo
    {
        public Vector2D pos;
        public int health;

        public PlayerInfo(Vector2D pos, int health)
        {
            this.pos = pos;
            this.health = health;
        }

        public PlayerInfo()
        {
            pos = new Vector2D(0, 0);
            health = 100;
        }

        public Vector2D GetPosition()
        {
            return pos;
        }

        public int GetHealth()
        {
            return health;
        }

    }
}
