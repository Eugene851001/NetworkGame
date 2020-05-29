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
            messageHandlers.Add(MessageType.RespawnRequest, handleMessageRespawn);
            messageHandlers.Add(MessageType.Regitsration, handleMessageRegistration);

            EventPlayerShooted += proceedPlayerShooted;
        }

        protected override void updatePlayers(int time)
        {
            foreach(var player in Players.Values.ToArray())
            {
                if ((player.PlayerState & PlayerState.Killed) == 0)
                {
                    updatePhysicsPlayer(player, time);
                    if ((player.PlayerState & PlayerState.Shoot) != 0)
                        Bullets.Add(player.Shoot(Environment.TickCount));
                }
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

        void handleMessageRegistration(GameMessage message)
        {
            if (message.MessageType != MessageType.Regitsration)
                return;
            if (!PlayersNames.ContainsKey(message.PlayerID))
            {
                PlayersNames.Add(message.PlayerID, ((MessageRegistration)message).Name);
                Players[message.PlayerID].Name = ((MessageRegistration)message).Name;
            }
            MessageRegistration messageRegistration = message as MessageRegistration;
            ChatMessages.Enqueue(new MessageChat()
            {
                PlayerID = ServerID,
                Content =
                messageRegistration.Name + " присоединился к игре"
            });
        }

        void handleMessageAdd(GameMessage message)
        {
            string name = PlayersNames.ContainsKey(message.PlayerID) 
                ? PlayersNames[message.PlayerID] : message.PlayerID.ToString();
            ChatMessages.Enqueue(new MessageChat()
            {
                PlayerID = ServerID,
                Content =
                name + " присоединился к игре"
            });
        }

        void handleMessageDelete(GameMessage message)
        {
            if (ChatMessages.Count > 1200)
                return;
            string name = PlayersNames.ContainsKey(message.PlayerID)
               ? PlayersNames[message.PlayerID] : message.PlayerID.ToString();
            ChatMessages.Enqueue(new MessageChat()
            {
                PlayerID = ServerID,
                Content = name + " покинул игру"
            }) ; 
            Players.Remove(message.PlayerID);
        }

        Vector2D getSpawnPoint()
        {
            Random random = new Random(Environment.TickCount);
            int pointNumber = random.Next(0, Map.SpawnPoints.Length - 1);
            Vector2D result = Map.SpawnPoints[pointNumber];
            return result;
        }

        void handleMessageRespawn(GameMessage message)
        {
            if (message.MessageType != MessageType.RespawnRequest)
                return;
            var player = Players[message.PlayerID];
            if((player.PlayerState & PlayerState.Killed) != 0 && player.Lives > 0)
            {
                player.Lives--;
                player.Position = getSpawnPoint();
                player.PlayerState = PlayerState.None;
                player.Health = 100;
                ChatMessages.Enqueue(new MessageChat()
                {
                    Content = PlayersNames[player.PlayerID] + " возродился"
                });
            }
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
