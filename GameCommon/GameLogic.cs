using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameCommon
{
    public class GameLogic
    {
        public Dictionary<int, Player> Players = new Dictionary<int, Player>();
        public List<Bullet> Bullets = new List<Bullet>();
        public TileMap Map;

        public delegate void OnPlayerShooted(Bullet bullet, int PlayerID);
        public event OnPlayerShooted EventPlayerShooted; 

        public double CountDistance(GameObject firstObject, GameObject secondObject)
        {
            return Math.Sqrt(Math.Pow(firstObject.Position.X - secondObject.Position.X, 2)
                + Math.Pow(secondObject.Position.Y - secondObject.Position.Y, 2));
        }

        protected bool isCollision(GameObject firstObject, GameObject secondObject)
        {
            return CountDistance(firstObject, secondObject) < (firstObject.Size + secondObject.Size);
        }

        bool isWallCollision(MovableGameObject gameObject, int time)
        {
            Vector2D newPosition = new Vector2D();
            newPosition.X = gameObject.Position.X + gameObject.Direction.X * gameObject.Speed * time;
            newPosition.Y = gameObject.Position.Y + gameObject.Direction.Y * gameObject.Speed * time;
            newPosition.X += gameObject.Direction.X * gameObject.Size / 2;
            newPosition.Y += gameObject.Direction.Y * gameObject.Size / 2;
            return Map.IsSolid((int)newPosition.X, (int)newPosition.Y);
        }

        protected virtual void updatePlayers(int time)
        {
            foreach (var player in Players.Values)
            {
                if (player.PlayerState == PlayerState.MoveFront || 
                    player.PlayerState == PlayerState.MoveBack)
                {
                    if(!isWallCollision(player, time))
                        player.Move(time);
                    player.PlayerState = PlayerState.None;
                }
                player.Rotate(time);
            }
        }

        protected virtual void updateBullets(int time)
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
                if(isWallCollision(Bullets[i], time))
                {
                    Bullets[i].IsDestroy = true;
                    continue;
                }
                Bullets[i].Position.X += Bullets[i].Direction.X * Bullets[i].Speed * time;
                Bullets[i].Position.Y += Bullets[i].Direction.Y * Bullets[i].Speed * time;
                List<int> playersIDs = new List<int>(Players.Keys.AsEnumerable());
                foreach (var playerID in playersIDs)
                    if (isCollision(Bullets[i], Players[playerID]))
                    {
                        // Players[playerID].Health -= Bullets[i].Damage;
                        //Bullets[i].IsDestroy = true;
                        if(EventPlayerShooted.GetInvocationList().Count() > 0)
                            EventPlayerShooted(Bullets[i], playerID);
                    }
            }
        }

        public virtual void UpdateGame(int time)
        {

            updateBullets(time);
            updatePlayers(time);

            List<int> idArray = new List<int>(Players.Keys.AsEnumerable());
            foreach (var playerID in idArray)
            {
                if (Players[playerID].Health <= 0)
                    Players[playerID].PlayerState = PlayerState.Killed;
            }

            for (int i = 0; i < Bullets.Count; i++)
            {
                if (Bullets[i].IsDestroy)
                    Bullets.RemoveAt(i);
            }

            for (int i = 0; i < idArray.Count; i++)
            {
                int playerID = idArray[i];
                if (Players[playerID].IsDestroy)
                {
                    idArray.RemoveAt(i);
                    Players.Remove(playerID);
                }
            }
        }
    }
}
