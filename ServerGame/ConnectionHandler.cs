using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using GameCommon;
using System.Threading;
using SerializeHandler;

namespace ServerGame
{
    class ConnectionHandler
    {
        public delegate void DisconnectClient();
        public event DisconnectClient EventDisconnectClient;

        public string Name;
        ISerialize messageSerializer;
        Socket socketClientHandler;
        Thread threadHandleClient;
        public bool IsConnected;
        PlayerHandler playerHandler;
        public ConnectionHandler(Socket socketClientHandler, int playerID)
        {
            Name = "";
            messageSerializer = new SerializerBinary();
            IsConnected = true;
            EventDisconnectClient += RemoveClient;
            this.socketClientHandler = socketClientHandler;
            socketClientHandler.ReceiveTimeout = 1000;
            socketClientHandler.SendTimeout = 1000;
            Server.clients.Add(socketClientHandler.RemoteEndPoint.GetHashCode(), socketClientHandler);
            Player player = new Player(new Vector2D(100, 100), 100, 0.1f);

            playerHandler = new PlayerHandler(player, playerID);
            playerHandler.PlayerUpdateEvent += OnPlayerUpdate;

            Server.SendToAll(new MessageAddPlayer() { PlayerID = playerID, PlayerInfo = player });

            threadHandleClient = new Thread(HandleClient);
            threadHandleClient.IsBackground = true;
            threadHandleClient.Start();

            playerHandler.StartUpdatePlayer();
        }

        public void RemoveClient()
        {
      //      Console.WriteLine(Server.clientNames[socketClientHandler.RemoteEndPoint.GetHashCode()] + " has left the game");
        //    Server.RemoveClient(socketClientHandler.RemoteEndPoint.GetHashCode());
        //    socketClientHandler.Close();
        }

        public void OnPlayerUpdate(PlayerInfo playerInfo)
        {
            MessagePlayerInfo message = new MessagePlayerInfo()
            {
                PlayerID = playerHandler.PlayerID,
                PlayerInfo = playerHandler.player
            };
            Server.SendToAll(message);
        }
        public void OnDisconnectClient()
        {
            EventDisconnectClient();

        }
        bool IsClientConnected()
        {
            bool IsConnected = true;
            try
            {
                socketClientHandler.Send(messageSerializer.Serialize(new PlayerInfo(), typeof(PlayerInfo)));
                if (!socketClientHandler.Connected)
                    IsConnected = false;
            }
            catch
            {
                IsConnected = false;
            }
            return IsConnected;
        }


        void HandleMessage(GameMessage message)
        {
            Console.WriteLine("Handle message");
            switch(message.MessageType)
            {
                case MessageType.AddPlayer:

                    break;
                case MessageType.PlayerAction:
                    MessagePlayerAction messageAction = message as MessagePlayerAction;
                    Console.WriteLine("Get action");
                    playerHandler.ChangePlayerDirection(messageAction.Direction);
                    break;
            }
            //playerHandler.ChangePlayerDirection(message.pos);
        }

        public void ReceiveMessages()
        {
            byte[] data = new byte[1024];
            Console.WriteLine("Receive messages");
            IsConnected = true;
            int amount;
            do
            {
                MemoryStream messageContainer = new MemoryStream();
                try
               {
                    do
                    {
                        amount = socketClientHandler.Receive(data);
                        messageContainer.Write(data, 0, amount);
                    } while (socketClientHandler.Available > 0);
                    GameMessage recievedMessage = (GameMessage)messageSerializer.Deserialize(messageContainer, 
                        typeof(GameMessage), new Type[] { typeof(MessagePlayerInfo), 
                        typeof(MessagePlayerAction), typeof(MessageAddPlayer)});
                    Console.WriteLine("Get message");
                    HandleMessage(recievedMessage);
                }
                catch(SocketException)
                {
                   /* if (!IsClientConnected())
                    {
                        IsConnected = false;
                    }*/
                }
            } while (IsConnected);
        }

        public void HandleClient()
        {
            ReceiveMessages();
        //    OnDisconnectClient();
        }
    }
}
