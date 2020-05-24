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
        public int ThisPlayerID  = -1;

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
            ThisPlayerID = -1;
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
            updatePhysicsPlayer(player, time);
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
            Players.Add(messageAdd.PlayerID, messageAdd.Player);
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
                    var bullet = newPlayer.Shoot(int.MaxValue);
                    Bullets.Add(bullet); 
                }
                if(EventChangeState.GetInvocationList().Count() > 0)
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
