using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCommon;

namespace ClientGame
{
    class ClientGameLogic: GameLogic
    {
        delegate void MessageHandler(GameMessage message);
        Dictionary<MessageType, MessageHandler> messageHandlers;

        public ClientGameLogic()
        {
            messageHandlers = new Dictionary<MessageType, MessageHandler>();
            messageHandlers.Add(MessageType.AddPlayer, handleMessageAdd);
            messageHandlers.Add(MessageType.PlayerInfo, handleMessagePlayerInfo);
            messageHandlers.Add(MessageType.BulletInfo, handleMessageBulletInfo);

        }

        protected override void updatePlayers(int time)
        {
            for (int i = 0; i < Bullets.Count; i++)
            {
                Bullets[i].Position.X += Bullets[i].Direction.X * Bullets[i].Speed * time;
                Bullets[i].Position.Y += Bullets[i].Direction.Y * Bullets[i].Speed * time;
                Dictionary<int, Player> savePlayers = new Dictionary<int, Player>(Players);
                foreach (var playerID in savePlayers.Keys)
                    if (isCollision(Bullets[i], Players[playerID]))
                    {
                        Bullets[i].IsDestroy = true;
                    }
            }
        }

        protected override void updateBullets(int time)
        {
            base.updateBullets(time);
        }


        void handleMessageAdd(GameMessage message)
        {
            if (!(message is MessageAddPlayer))
                return;
            MessageAddPlayer messageAdd = message as MessageAddPlayer;
            if (Players.ContainsKey(messageAdd.PlayerID))
                return;
            Players.Add(messageAdd.PlayerID, (Player)messageAdd.PlayerInfo);
        }

        void handleMessagePlayerInfo(GameMessage message)
        {
            if (!(message is MessagePlayerInfo))
                return;
            MessagePlayerInfo messageInfo = message as MessagePlayerInfo;
            if (Players.Keys.Contains(messageInfo.PlayerID))
            {
                Players[messageInfo.PlayerID] = (Player)messageInfo.PlayerInfo;
                if (messageInfo.PlayerInfo.PlayerState == PlayerState.Shoot)
                {
                    Bullet bullet = new Bullet(message.PlayerID);
                    bullet.Direction = Players[message.PlayerID].Direction;
                    bullet.Speed = Players[message.PlayerID].Speed * 2;
                    bullet.Size = Players[message.PlayerID].Size / 4;
                    Vector2D position = Players[message.PlayerID].Position;
                    bullet.Position = new Vector2D(position.X + bullet.Direction.X * bullet.Size * 6,
                        position.Y + bullet.Direction.Y * bullet.Size * 6);
                    Bullets.Add(bullet);
                }
            }

        }

        void handleMessageBulletInfo(GameMessage message)
        {

        }



        public void HandleMessage(GameMessage message)
        {
            if (messageHandlers.ContainsKey(message.MessageType))
            {
                messageHandlers[message.MessageType](message);
            }
        }
    }
}
