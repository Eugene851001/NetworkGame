using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    public enum PlayerState { 
        None = 0, 
        Shoot = 1, 
        Killed = 2,
        MoveFront = 4,
        MoveBack = 8,
        RotateLeft = 16,
        RotateRight = 32,
        MoveFrontShoot = Shoot | MoveFront,
        MoveBackShoot = Shoot | MoveBack,
        MoveFrontRotateRight = MoveFront | RotateRight,
        MoveBackRotateRight = MoveBack | RotateRight,
        MoveFrontRotateLeft = MoveFront | RotateLeft,
        MoveBackRotateLeft = MoveBack | RotateLeft
    };

    [Serializable]
    public class Player: MovableGameObject
    {
        Weapon weapon;
        int playerID;
        public int Health;
        public double ViewAngle;
        public double RotateSpeed = Math.PI / 1000;
        public PlayerState PlayerState = PlayerState.None;

        public Player(Vector2D position, int health, double speed)
        {
            
            Position = position;
            Health = health;
            Speed = speed;
            Direction = new Vector2D(0, 0);
            Size = 10;
            weapon = new Weapon();
            playerID = 0;
        }

        public Player()
        {
            Position = new Vector2D(0, 0);
            Health = 100;
            Speed = 0.01f;
            Direction = new Vector2D(0, 0);
            weapon = new Weapon();
            playerID = 0;
        }

        public Bullet Shoot(int time)
        {
            return weapon.Shoot(this.Direction, this.Position, this.playerID, time);
        }

        public void ChangeDirection(Vector2D direction)
        {
            Direction = direction.Normalize();
        }

        public void RotateRight(float time)
        {
            ViewAngle += time * RotateSpeed;
            Direction.X = Math.Cos(ViewAngle);
            Direction.Y = Math.Sin(ViewAngle);
        }

        public void RotateLeft(float time)
        {
            ViewAngle -= time * RotateSpeed;
            Direction.X = Math.Cos(ViewAngle);
            Direction.Y = Math.Sin(ViewAngle);
        }

        public void MoveFront(float time)
        {
            Direction.X = Math.Cos(ViewAngle);
            Direction.Y = Math.Sin(ViewAngle);
            Position.X += Direction.X * time * Speed;
            Position.Y += Direction.Y * time * Speed;
        }

        public void MoveBack(float time)
        {
            Direction.X = Math.Cos(ViewAngle);
            Direction.Y = Math.Sin(ViewAngle);
            Position.X -= Direction.X * time * Speed;
            Position.Y -= Direction.Y * time * Speed;
        }
    }
}
