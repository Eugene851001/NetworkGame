using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCommon;

namespace ServerGame
{
    class ServerGameLogic
    {
        const int MaxQueueSize = 64;

        public Dictionary<int, Player> Players = new Dictionary<int, Player>();
        public List<Bullet> Bullets = new List<Bullet>();

        Queue<GameMessage> messages = new Queue<GameMessage>();

        public void AddMessage(GameMessage message)
        {
            if (messages.Count < MaxQueueSize)
            {
                messages.Enqueue(message);
            }
        }

        public double CountDistance(GameObject firstObject, GameObject secondObject)
        {
            return Math.Sqrt(Math.Pow(firstObject.Position.X - secondObject.Position.X, 2)
                + Math.Pow(secondObject.Position.Y - secondObject.Position.Y, 2));
        }

        bool isCollision(GameObject firstObject, GameObject secondObject)
        {
            return CountDistance(firstObject, secondObject) < (firstObject.Size + secondObject.Size);
        }

        void updatePlayers(int time)
        {
            foreach(var player in Players.Values)
            {
                player.Move(time);
                player.Rotate(time);
            }
        }

        void updateBullets(int time)
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Position.X += Bullets[i].Direction.X * Bullets[i].Speed * time;
                Bullets[i].Position.Y += Bullets[i].Direction.Y * Bullets[i].Speed * time;
                foreach (var playerID in Players.Keys)
                    if (isCollision(Bullets[i], Players[playerID]))
                    {
                        Players[playerID].Health -= Bullets[i].Damage;//
                        Bullets[i].IsDestroy = true;
                    }
            }
        }

        public void UpdateGame(int time)
        {

            updateBullets(time);
            updatePlayers(time);            

            foreach(var playerID in Players.Keys)
            {
                if (Players[playerID].Health <= 0)
                    Players[playerID].IsDestroy = true;
            }

            for(int i = 0; i < Bullets.Count; i++)
            {
                if (Bullets[i].IsDestroy)
                    Bullets.RemoveAt(i);
            }

            List<int> idArray = new List<int>(Players.Keys.AsEnumerable());
            for(int i = 0; i < idArray.Count; i++)
            {
                int playerID = idArray[i];
                if (Players[playerID].IsDestroy)
                {
                    idArray.RemoveAt(i);
                    Players.Remove(playerID);
                }
            }
        }

        void handleMove(MessagePlayerAction message)
        {
            if(Players.ContainsKey(message.PlayerID))
            {
                Players[message.PlayerID].ChangeDirection(message.Direction);
            }
        }

        void handleRotate(MessagePlayerAction message)
        {
            if (!Players.ContainsKey(message.PlayerID))
                return;
            message.Direction.Normalize();
            Players[message.PlayerID].RotateDirection = (int)message.Direction.X;
        }

        public void RemoveShootState()
        {
            foreach(int playerID in Players.Keys)
            {
                Players[playerID].PlayerState = PlayerState.None;
            }
        }

        void handleShoot(MessagePlayerAction message)
        {
            if (!Players.ContainsKey(message.PlayerID))
                return;
            Players[message.PlayerID].PlayerState = PlayerState.Shoot;
            Bullet bullet = new Bullet(message.PlayerID);
            bullet.Direction = Players[message.PlayerID].ViewDirection;
            bullet.Speed = Players[message.PlayerID].Speed * 2;
            bullet.Size = Players[message.PlayerID].Size / 4;
            Vector2D position = Players[message.PlayerID].Position;
            bullet.Position = new Vector2D(position.X + bullet.Size * 5, position.Y + bullet.Size * 5);
            Bullets.Add(bullet); 
        }

        public void ProceedMessages()
        {
            while(messages.Count > 0)
            {
                var message = messages.Dequeue();
                if(message.MessageType == MessageType.PlayerAction)
                {
                    var messageAction = (MessagePlayerAction)message;
                    switch(messageAction.Action)
                    {
                        case PlayerActionType.Move:
                            handleMove(messageAction);
                            break;
                        case PlayerActionType.Rotate:
                            handleRotate(messageAction);
                            break;
                        case PlayerActionType.Shoot:
                            handleShoot(messageAction);
                            break;
                    }
                }
            }
        }

    }
}
