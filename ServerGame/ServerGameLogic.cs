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
            if (Players[playerID].Health <= 0)
                content = " убил ";
            else
                content = " подстрелил ";
            ChatMessages.Enqueue(new MessageChat() { 
                PlayerID = ServerID, 
                Content = bullet.OwnerID.ToString() + content + playerID.ToString()
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
                Players[message.PlayerID].PlayerState = (message.Action == 
                    PlayerActionType.MoveFront) ? PlayerState.MoveFront : PlayerState.MoveBack;
            }
        }

        void handleRotate(MessagePlayerAction message)
        {
            if (!Players.ContainsKey(message.PlayerID))
                return;
            int RotateDirection = (message.Action == PlayerActionType.RotateLeft) ? -1 : 1;
            Players[message.PlayerID].RotateDirection = RotateDirection;
        }

        public void RemoveShootState()
        {
            foreach(int playerID in Players.Keys)
            {
                if(Players[playerID].PlayerState == PlayerState.Shoot)
                    Players[playerID].PlayerState = PlayerState.None;
            }
        }

        void handleShoot(MessagePlayerAction message)
        {
            if (!Players.ContainsKey(message.PlayerID))
                return;
            Players[message.PlayerID].PlayerState = PlayerState.Shoot;
            Bullet bullet = new Bullet(message.PlayerID);
            bullet.Direction = new Vector2D(Players[message.PlayerID].Direction);
            bullet.Speed = Players[message.PlayerID].Speed * 4;
            bullet.Size = Players[message.PlayerID].Size / 4;
            Vector2D position = Players[message.PlayerID].Position;

            bullet.Position = new Vector2D(position.X + bullet.Direction.X * bullet.Size * 6,
                position.Y + bullet.Direction.Y * bullet.Size * 6);
            Bullets.Add(bullet);
        }

        void handleMessageAction(GameMessage message)
        {
            var messageAction = (MessagePlayerAction)message;
            switch (messageAction.Action)
            {
                case PlayerActionType.MoveBack:
                case PlayerActionType.MoveFront:
                    handleMove(messageAction);
                    break;
                case PlayerActionType.RotateLeft:
                case PlayerActionType.RotateRight:
                    handleRotate(messageAction);
                    break;
                case PlayerActionType.Shoot:
                    handleShoot(messageAction);
                    break;
            }
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
            ChatMessages.Enqueue(new MessageChat()
            {
                PlayerID = ServerID,
                Content = message.PlayerID.ToString() + " покинул игру"
            }); ;
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
