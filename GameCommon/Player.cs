using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{

    [Serializable]
    public class Player: PlayerInfo
    {
       // public PlayerInfo PlayerInfo;

        float speed;
        Vector2D moveDirection;

        public Vector2D MoveDirection { get { return moveDirection; } }

        public Player(Vector2D position, int health, float speed)
        {
            //PlayerInfo = new PlayerInfo(position, health);
            Position = position;
            Health = health;
            this.speed = speed;
            moveDirection = new Vector2D(0, 0);
        }

        public Player()
        {
            Position = new Vector2D(0, 0);
            Health = 100;
            speed = 0.01f;
            moveDirection = new Vector2D(0, 0);
        }

        public void ChangeDirection(Vector2D direction)
        {
            moveDirection = direction.Normalize();
        }



        public void Move(float time)
        {
            Position.X += moveDirection.X * time * speed;
            Position.Y += moveDirection.Y * time * speed;
            Console.WriteLine("Positon: (" + Position.X + ": " + Position.Y + ")");
        }
    }
}
