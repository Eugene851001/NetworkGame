using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using System.IO;
using System.Threading;
using GameCommon;

namespace ClientGame
{
    public class GameClient
    { 
        const string BroadcastIP = "192.168.48.255";
        const int ServerPort = 8005;
        Socket socketServerHandler;
        Serializer messageSerializer;
        public bool IsConnected = false;

        public delegate void MessageHandler(PlayerInfo message);
        public event MessageHandler ReceiveMessageHandler;
        public GameClient()
        {
            messageSerializer = new Serializer();
            socketServerHandler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public bool Connect(IPEndPoint iPEnd)
        {
            try
            {
                socketServerHandler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketServerHandler.Connect(iPEnd);
            }
            catch
            {
                socketServerHandler.Close();
                return false;
            }
            IsConnected = true;
            return true;
        }

        public void Disconnect()
        {
            socketServerHandler.Close();
            IsConnected = false;
        }

        public void OnMessageReceive(PlayerInfo message)
        {
            ReceiveMessageHandler(message);
        }

        public bool ConnectToServer(IPEndPoint endPoint)
        {
            if (Connect(endPoint))
            {
                SendMessage(new PlayerInfo());
                Thread threadServerConnection = new Thread(ReceiveMessages);
                threadServerConnection.Start();
                return true;
            }
            else
                return false;
        }
        public void ReceiveMessages()
        {
            byte[] data = new byte[1024];
            StringBuilder receivedData = new StringBuilder();
            int amount;
            do
            {
                MemoryStream messageContainer = new MemoryStream();
                try
                {
                    do
                    {
                        amount = socketServerHandler.Receive(data);
                        messageContainer.Write(data, 0, amount);
                    } while (socketServerHandler.Available > 0);
                    PlayerInfo recievedMessage = messageSerializer.Deserialize(messageContainer.GetBuffer(),
                        messageContainer.GetBuffer().Length);
                    OnMessageReceive(recievedMessage);
                }
                catch
                {
                    Disconnect();
                    return;
                }
            } while (IsConnected);
        }

        public bool SendMessage(PlayerInfo message)
        {
            byte[] buffer = messageSerializer.Serialize(message);
            try
            {
                socketServerHandler.Send(buffer);
            }
            catch
            {
                Disconnect();
            }
            return true;
        }

        public override int GetHashCode()
        {
            return socketServerHandler.LocalEndPoint.GetHashCode();
        }
    }
}