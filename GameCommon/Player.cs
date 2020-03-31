using System;
using System.Collections.Generic;
using System.Text;

namespace GameCommon
{
    public class Player: IPlayerInfo
    {
        Vector2D position;
        int health;
        float speed;
        Vector2D moveDirection;

        public Vector2D GetPosition()
        {
            return position;
        }

        public int GetHealth()
        {
            return health;
        }
        public Player(Vector2D position, int health, float speed)
        {
            this.position = position;
            this.health = health;
            this.speed = speed;
            moveDirection = new Vector2D(0, 0);
        }

        public void ChangeDirection(Vector2D direction)
        {
            moveDirection = direction.Normalize();
        }

        public void Move(float time)
        {
            position.X = moveDirection.X * time * speed;
            position.Y = moveDirection.Y * time * speed;
        }
    }
}
