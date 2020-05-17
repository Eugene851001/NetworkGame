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

        void proceedPlayerShooted(Bullet bullet, int playerID)
        {
            Players[playerID].Health -= bullet.Damage;
            bullet.IsDestroy = true;
            string content;
            if (PlayersNames.ContainsKey(bullet.OwnerID))
                content = PlayersNames[bullet.OwnerID];
            else
                content = bullet.OwnerID.ToString();

            if (Players[playerID].Health <= 0)
                content += " убил ";
            else
                content += " подстрелил ";

            if (PlayersNames.ContainsKey(playerID))
                content += PlayersNames[playerID];
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
           /*  Bullet bullet = new Bullet(message.PlayerID);
             bullet.Direction = new Vector2D(Players[message.PlayerID].Direction);
             bullet.Speed = Players[message.PlayerID].Speed * 4;
             bullet.Size = Players[message.PlayerID].Size / 4;
             Vector2D position = Players[message.PlayerID].Position;

             bullet.Position = new Vector2D(position.X + bullet.Direction.X * bullet.Size * 6,
                 position.Y + bullet.Direction.Y * bullet.Size * 6);*/
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
            if ((messageAction.Action & PlayerActionType.MoveBack) != 0
                || (messageAction.Action & PlayerActionType.MoveFront) != 0)
                handleMove(messageAction);
            if ((messageAction.Action & PlayerActionType.RotateLeft) != 0
                || (messageAction.Action & PlayerActionType.RotateRight) != 0)
                handleRotate(messageAction);
            if ((messageAction.Action & PlayerActionType.Shoot) != 0)
                handleShoot(messageAction);
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
                if(messageHandlers.ContainsKey(message.MessageType))
                    messageHandlers[message.MessageType](message);
            }
        }

    }
}
