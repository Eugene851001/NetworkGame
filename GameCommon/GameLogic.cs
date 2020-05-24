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
            return CountDistance(firstObject, secondObject) < (firstObject.Size / 2 + secondObject.Size / 2);
        }

        bool isPlayersCollision(Player player, int time)
        {
            bool result = false;
            foreach(var checkPlayer in Players.Values.ToArray())
            {
                if(checkPlayer.PlayerID != player.PlayerID)
                {
                    if (isCollision(player, checkPlayer))
                        result = true;
                }
            }
            return false;//result;
        }

        protected bool isWallCollision(MovableGameObject gameObject, int time)
        {
            Vector2D newPosition = new Vector2D();
            newPosition.X = gameObject.Position.X + gameObject.Direction.X * gameObject.Speed * time;
            newPosition.Y = gameObject.Position.Y + gameObject.Direction.Y * gameObject.Speed * time;
            newPosition.X += gameObject.Direction.X * gameObject.Size / 2;
            newPosition.Y += gameObject.Direction.Y * gameObject.Size / 2;
            return Map.IsSolid((int)newPosition.X, (int)newPosition.Y);
            /*return Map.IsSolid((int)(newPosition.X + gameObject.Size), (int)(newPosition.Y + gameObject.Size)) 
                || Map.IsSolid((int)(newPosition.X + gameObject.Size), (int)(newPosition.Y - gameObject.Size)) 
                || Map.IsSolid((int)(newPosition.X - gameObject.Size), (int)(newPosition.Y + gameObject.Size))
                || Map.IsSolid((int)(newPosition.X - gameObject.Size), (int)(newPosition.Y - gameObject.Size))*/;
        }

        protected virtual void updatePlayers(int time)
        {
            foreach (var player in Players.Values.ToArray())
            {
                updatePhysicsPlayer(player, time);
            }
        }

        protected void applyInput(Player player, PlayerActionType action)
        {
            if ((action & PlayerActionType.MoveFront) != 0)
                player.PlayerState |= PlayerState.MoveFront;
            if ((action & PlayerActionType.MoveBack) != 0)
                player.PlayerState |= PlayerState.MoveBack;
            if ((action & PlayerActionType.RotateLeft) != 0)
                player.PlayerState |= PlayerState.RotateLeft;
            if ((action & PlayerActionType.RotateRight) != 0)
                player.PlayerState |= PlayerState.RotateRight;
            if ((action & PlayerActionType.Shoot) != 0)
                player.PlayerState |= PlayerState.Shoot;
        }

        protected Player updatePhysicsPlayer(Player player, int time)
        {
            if ((player.PlayerState & PlayerState.RotateRight) != 0)
                player.RotateRight(time);
            if ((player.PlayerState & PlayerState.RotateLeft) != 0)
                player.RotateLeft(time);
            if ((player.PlayerState & PlayerState.MoveFront) != 0 && !isWallCollision(player, time) 
                && !isPlayersCollision(player, time))
            {
                player.MoveFront(time);
            }
            if ((player.PlayerState & PlayerState.MoveBack) != 0)
            {
                var testPlayer = new Player(player.PlayerID)
                {
                    Position = player.Position,
                    Direction = new Vector2D(-player.Direction.X, -player.Direction.Y)
                };
                if(!isWallCollision(testPlayer, time) && !isPlayersCollision(testPlayer, time))
                    player.MoveBack(time);
            }
            return player;
        }

        protected virtual void updateBullets(int time)
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
             //   time = Environment.TickCount -  Bullets[i].LastUpdateTime;
                if(isWallCollision(Bullets[i], time))
                { 
                    Bullets[i].IsDestroy = true;
                    continue;
                }
                Bullets[i].Move(time);
                List<int> playersIDs = new List<int>(Players.Keys.AsEnumerable());
                foreach (var playerID in playersIDs)
                    if (isCollision(Bullets[i], Players[playerID]))
                    {
                        if(EventPlayerShooted.GetInvocationList().Count() > 0)
                            EventPlayerShooted(Bullets[i], playerID);
                    }
            }
        }

        public virtual void UpdateGame(int time)
        {

            updateBullets(time);
            updatePlayers(time);

            List<int> idArray = new List<int>(Players.Keys.ToArray());
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

        public void RemoveGeneralStates()
        {
            foreach (int playerID in Players.Keys.ToArray())
            {
                Players[playerID].PlayerState = Players[playerID].PlayerState &
                    (PlayerState.Killed | PlayerState.Shoot);
            }
        }

        public void RemoveShootState()
        {
            foreach (int playerID in Players.Keys.ToArray())
            {
                Players[playerID].PlayerState = Players[playerID].PlayerState & (~PlayerState.Shoot);
            }
        }
    }
}
