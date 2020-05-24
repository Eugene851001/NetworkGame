using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameCommon;

namespace ServerGame
{
    class ServerGameLogic: GameLogic
    {
        const int MaxQueueSize = 64;
        const int ServerID = -1;

        const int ScoreForShoot = 10;

        Queue<GameMessage> messages = new Queue<GameMessage>();

        public Queue<MessageChat> ChatMessages = new Queue<MessageChat>();

        delegate void HandleMessage(GameMessage message);
        Dictionary<MessageType, HandleMessage> messageHandlers;
        public Dictionary<int, int> PlayersInputNumbers = new Dictionary<int, int>();
        public Dictionary<int, string> PlayersNames = new Dictionary<int, string>();

        public ServerGameLogic()
        {
            messageHandlers = new Dictionary<MessageType, HandleMessage>();
            messageHandlers.Add(MessageType.PlayerAction, handleMessageAction);
            messageHandlers.Add(MessageType.AddPlayer, handleMessageAdd);
            messageHandlers.Add(MessageType.DeletePlayer, handleMessageDelete);

            EventPlayerShooted += proceedPlayerShooted;
        }

        protected override void updatePlayers(int time)
        {
            foreach(var player in Players.Values.ToArray())
            {
                updatePhysicsPlayer(player, time);
                if ((player.PlayerState & PlayerState.Shoot) != 0)
                    Bullets.Add(player.Shoot(Environment.TickCount));
            }
        }

        void proceedPlayerShooted(Bullet bullet, int playerID)
        {
            Players[playerID].Health -= bullet.Damage;
            bullet.IsDestroy = true;
            string content;
            if (Players.ContainsKey(bullet.OwnerID))
            {
                Players[bullet.OwnerID].Score += ScoreForShoot;
                content = Players[bullet.OwnerID].Name;
            }
            else
                content = bullet.OwnerID.ToString();

            if (Players[playerID].Health <= 0)
                content += " убил ";
            else
                content += " подстрелил ";

            if (Players.ContainsKey(playerID))
                content += Players[playerID].Name;
            else
                content += playerID.ToString();
            ChatMessages.Enqueue(new MessageChat()
            {
                PlayerID = ServerID,
                Content = content
            });
        }

        public void AddMessage(GameMessage message)
        {
            if (messages.Count < MaxQueueSize)
            {
                messages.Enqueue(message);
            }
        }

        public void AddPlayer(int playerID, Player player)
        {
            Players.Add(playerID, player);
        }

        void handleMove(MessagePlayerAction message)
        {
            if(Players.ContainsKey(message.PlayerID))
            {
                Players[message.PlayerID].PlayerState |= (message.Action == 
                    PlayerActionType.MoveFront) ? PlayerState.MoveFront : PlayerState.MoveBack;
            }
        }

        void handleRotate(MessagePlayerAction message)
        {
            if (!Players.ContainsKey(message.PlayerID))
                return;
            Players[message.PlayerID].PlayerState |= ((message.Action &
                PlayerActionType.RotateLeft) != 0) ? PlayerState.RotateLeft : PlayerState.RotateRight;
        }

        void handleShoot(MessagePlayerAction message)
        {
             if (!Players.ContainsKey(message.PlayerID))
                return;
            var bullet = Players[message.PlayerID].Shoot(Environment.TickCount);
            if (bullet != null)
            {
                Players[message.PlayerID].PlayerState |= PlayerState.Shoot;
                Bullets.Add(bullet);
            }
        }

        void handleMessageAction(GameMessage message)
        {
            
            var messageAction = (MessagePlayerAction)message;
            if (!PlayersInputNumbers.ContainsKey(message.PlayerID))
                PlayersInputNumbers.Add(message.PlayerID, messageAction.InputNumber);
            else
                PlayersInputNumbers[message.PlayerID] = messageAction.InputNumber;
            if(Players.ContainsKey(message.PlayerID))
                applyInput(Players[message.PlayerID], messageAction.Action);
        }

        void handleMessageAdd(GameMessage message)
        {
            ChatMessages.Enqueue(new MessageChat()
            {
                PlayerID = ServerID,
                Content =
                message.PlayerID.ToString() + " присоединился к игре"
            });
        }

        void handleMessageDelete(GameMessage message)
        {
            if (ChatMessages.Count > 1200)
                return;
            ChatMessages.Enqueue(new MessageChat()
            {
                PlayerID = ServerID,
                Content = message.PlayerID.ToString() + " покинул игру"
            }); 
            Players.Remove(message.PlayerID);
        }


        public void ProceedMessages()
        {
            while(messages.Count > 0)
            {
                var message = messages.Dequeue();
                if (messageHandlers.ContainsKey(message.MessageType))
                    messageHandlers[message.MessageType](message);
            }
        }

    }
}
