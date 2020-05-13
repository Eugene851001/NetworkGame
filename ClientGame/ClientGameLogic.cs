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
        const int ServerID = -1;

        //public int MyID;
        public int ThisPlayerID;

    //    const int timeStep = 100;
        const int PhysicalUpdateInterval = 100;

        bool isPrediction = true;
        bool isReconcilation = true;

        public MessagePlayerAction LastMessage;

        public delegate void OnChangeState(Player playerToDraw, int playerID);
        public event OnChangeState EventChangeState;

        public delegate void OnDeletePlayer(int playerID);
        public event OnDeletePlayer EventDeletePlayer;

        delegate void MessageHandler(GameMessage message);
        Dictionary<MessageType, MessageHandler> messageHandlers;
        public List<MessageChat> chatMessages;
        public Queue<MessagePlayerAction> UnacknowledgedInputs = new Queue<MessagePlayerAction>();
        public ClientGameLogic()
        {
            messageHandlers = new Dictionary<MessageType, MessageHandler>();
            messageHandlers.Add(MessageType.AddPlayer, handleMessageAdd);
            messageHandlers.Add(MessageType.PlayerInfo, handleMessagePlayerInfo);
            messageHandlers.Add(MessageType.Chat, handleMessageChat);
            messageHandlers.Add(MessageType.DeletePlayer, handleMessageDelete);
            messageHandlers.Add(MessageType.PersonalAddPlayer, handleMessagePersonalAdd);
            ThisPlayerID = 0;
        //    messageHandlers.Add()

            chatMessages = new List<MessageChat>();

            EventPlayerShooted += proceedPlayerShooted;
        }

        public Player GetThisPlayer()
        {
            if (Players.ContainsKey(ThisPlayerID))
                return Players[ThisPlayerID];
            else
                return null;
        }

        void proceedPlayerShooted(Bullet bullet, int playerID)
        {
       //     Players[playerID].Health -= bullet.Damage;
            bullet.IsDestroy = true;
        }

        void applyInput(Player player, PlayerActionType action)
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

        protected override void updatePlayers(int time)
        {
            if (!isPrediction)
                return; 
            if (UnacknowledgedInputs.Count == 0)
                return;
            MessagePlayerAction messageAction = LastMessage;
            if (!Players.ContainsKey(ThisPlayerID))
                return;
            applyInput(Players[ThisPlayerID], messageAction.Action);
            var player = GetThisPlayer();
            if (player == null)
                return;
            if ((player.PlayerState & PlayerState.MoveFront) != 0 && !isWallCollision(player, time))
            {
                player.MoveFront(time);
            }
            if ((player.PlayerState & PlayerState.MoveBack) != 0 && !isWallCollision(
                new MovableGameObject()
                {
                    Direction = new Vector2D(-player.Direction.X, -player.Direction.Y),
                    Position = player.Position
                }, time))
            {
                player.MoveBack(time);
            }
            if ((player.PlayerState & PlayerState.RotateRight) != 0)
                player.RotateRight(time);
            if ((player.PlayerState & PlayerState.RotateLeft) != 0)
                player.RotateLeft(time);
            player.PlayerState = player.PlayerState & (PlayerState.Killed | PlayerState.Shoot);
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
            Players.Add(messageAdd.PlayerID, (Player)messageAdd.Player);
        }

        void handleMessageDelete(GameMessage message)
        {
            if (!(message is MessageDeletePlayer))
                return;
            EventDeletePlayer(message.PlayerID);
            Players.Remove(message.PlayerID);
        }

        void handleMessageChat(GameMessage message)
        {
            if (!(message is MessageChat))
                return;
            chatMessages.Add((MessageChat)message);
        }

        void handleMessagePersonalAdd(GameMessage message)
        {
            if (!(message is MessagePersonalAddPlayer))
                return;
            MessagePersonalAddPlayer messageAdd = message as MessagePersonalAddPlayer;
            ThisPlayerID = messageAdd.PlayerID;
            Map = messageAdd.Map;
        }

        void handleMessagePlayerInfo(GameMessage message)
        {
            if (!(message is MessagePlayerInfo))
                return;
            MessagePlayerInfo messageInfo = message as MessagePlayerInfo;
            if(message.PlayerID == ThisPlayerID)
            {
                while (UnacknowledgedInputs.Count > 0 &&
                    UnacknowledgedInputs.Peek().InputNumber <= messageInfo.InputNumber)
                    UnacknowledgedInputs.Dequeue();
            }
            Player newPlayer = null;
            if (Players.Keys.Contains(messageInfo.PlayerID))
            {
                newPlayer = messageInfo.Player;
                if (messageInfo.Player.PlayerState == PlayerState.Shoot)
                {
                    /*Bullet bullet = new Bullet(message.PlayerID);
                    bullet.Direction = new Vector2D(Players[message.PlayerID].Direction);
                    bullet.Speed = Players[message.PlayerID].Speed * 4;
                    bullet.Size = Players[message.PlayerID].Size / 4;
                    Vector2D position = Players[message.PlayerID].Position;

                    bullet.Position = new Vector2D(position.X + bullet.Direction.X * bullet.Size * 6,
                        position.Y + bullet.Direction.Y * bullet.Size * 6);*/
                    var bullet = newPlayer.Shoot(int.MaxValue);
                    Bullets.Add(bullet); 
                }
                if(messageInfo.PlayerID != ThisPlayerID && EventChangeState.GetInvocationList().Count() > 0)
                {
                    if(newPlayer.PlayerState != PlayerState.None)
                        EventChangeState(newPlayer, message.PlayerID);
                }
            }
            else
            {
                return;
            }

            if (isReconcilation && message.PlayerID == ThisPlayerID)
            {
                foreach (var messageAction in UnacknowledgedInputs)
                {
                    applyInput(newPlayer, messageAction.Action);
                    updatePhysicsPlayer(newPlayer, PhysicalUpdateInterval);
                    newPlayer.PlayerState = newPlayer.PlayerState & (PlayerState.Killed | PlayerState.Shoot);
                }
            }

           /* if (Players[message.PlayerID].Position.X != newPlayer.Position.X
                || Players[message.PlayerID].Position.Y != newPlayer.Position.Y)
                return;*/

            Players[message.PlayerID] = newPlayer;

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
