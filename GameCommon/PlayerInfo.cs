using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameCommon
{
    public enum PlayerState {None, Shoot };
    [Serializable]
    public class PlayerInfo: MovableGameObject  
    {

        public int Health;
        public Vector2D ViewDirection;
        public double ViewAngle;
        public int RotateDirection;
        public double RotateSpeed = Math.PI / 1000;
        public PlayerState PlayerState = PlayerState.None;
        public PlayerInfo(Vector2D position, int health)
        {
            Position = position;
            Health = health;
            ViewDirection = new Vector2D(0, 0);
            ViewAngle = 0;
        }

        public PlayerInfo()
        {
            Position = new Vector2D(0, 0);
            Health = 100;
            ViewDirection = new Vector2D(0, 0);
            ViewAngle = 0;
        }

    }
}
