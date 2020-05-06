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

        Queue<GameMessage> messages = new Queue<GameMessage>();

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
            bullet.Direction = Players[message.PlayerID].Direction;
            bullet.Speed = Players[message.PlayerID].Speed * 2;
            bullet.Size = Players[message.PlayerID].Size / 4;
            Vector2D position = Players[message.PlayerID].Position;
            bullet.Position = new Vector2D(position.X + bullet.Direction.X * bullet.Size * 6, 
                position.Y + bullet.Direction.Y * bullet.Size * 6);
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
            }
        }

    }
}
