using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{

    [Serializable]
    public class Player: PlayerInfo
    {

        public Player(Vector2D position, int health, double speed)
        {
            //PlayerInfo = new PlayerInfo(position, health);
            Position = position;
            Health = health;
            Speed = speed;
            ViewDirection = new Vector2D();
            Direction = new Vector2D(0, 0);
            Size = 10;
        }

        public Player()
        {
            Position = new Vector2D(0, 0);
            Health = 100;
            Speed = 0.01f;
            ViewDirection = new Vector2D();
            Direction = new Vector2D(0, 0);
        }

        public void ChangeDirection(Vector2D direction)
        {
            Direction = direction.Normalize();
        }

        public void Rotate(float time)
        {
            ViewAngle += RotateDirection * time * RotateSpeed;
            Direction.X = Math.Cos(ViewAngle);
            Direction.Y = Math.Sin(ViewAngle);
            RotateDirection = 0;
        }

        public void Move(float time)
        {
            Position.X += Direction.X * time * Speed;
            Position.Y += Direction.Y * time * Speed;
          //  Console.WriteLine("Positon: (" + Position.X + ": " + Position.Y + ")");
        }
    }
}
