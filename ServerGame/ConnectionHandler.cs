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

namespace ServerGame
{
    class ConnectionHandler
    {
        public delegate void DisconnectClient();
        public event DisconnectClient EventDisconnectClient;
        public string Name;
        Serializer messageSerializer;
        Socket socketClientHandler;
        Thread threadHandleClient;
        public bool IsConnected;
        PlayerHandler playerHandler;
        public ConnectionHandler(Socket socketClientHandler)
        {
            Name = "";
            playerHandler = new PlayerHandler(new Player(new Vector2D(0, 0), 100, 0.001f));
            playerHandler.PlayerUpdateEvent += OnPlayerUpdate;
            messageSerializer = new Serializer();
            IsConnected = true;
            EventDisconnectClient += RemoveClient;
            this.socketClientHandler = socketClientHandler;
            socketClientHandler.ReceiveTimeout = 1000;
            socketClientHandler.SendTimeout = 1000;
            Server.clients.Add(socketClientHandler.RemoteEndPoint.GetHashCode(), socketClientHandler);
            threadHandleClient = new Thread(HandleClient);
            threadHandleClient.Start();
        }

        public void RemoveClient()
        {
            Console.WriteLine(Server.clientNames[socketClientHandler.RemoteEndPoint.GetHashCode()] + " has left the game");
            Server.RemoveClient(socketClientHandler.RemoteEndPoint.GetHashCode());
            socketClientHandler.Close();
        }

        public void OnPlayerUpdate(IPlayerInfo playerInfo)
        {
            PlayerInfo message = new PlayerInfo(playerInfo.GetPosition(), playerInfo.GetHealth());
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

                socketClientHandler.Send(messageSerializer.Serialize(new PlayerInfo()));
                if (!socketClientHandler.Connected)
                    IsConnected = false;
            }
            catch
            {
                IsConnected = false;
            }
            return IsConnected;
        }


        void HandleMessage(PlayerInfo message)
        {
            playerHandler.ChangePlayerDirection(new Vector2D(0, 0));
            
        }

        public void ReceiveMessages()
        {
            byte[] data = new byte[1024];
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
                    PlayerInfo recievedMessage = messageSerializer.Deserialize(messageContainer.GetBuffer(),
                        messageContainer.GetBuffer().Length);
                    HandleMessage(recievedMessage);
                }
                catch
                {
                    if (!IsClientConnected())
                    {
                        IsConnected = false;
                    }
                }
            } while (IsConnected);
        }

        public void HandleClient()
        {
            ReceiveMessages();
            OnDisconnectClient();
        }
    }
}
