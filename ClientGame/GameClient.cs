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
using SerializeHandler;

namespace ClientGame
{
    public class GameClient
    { 
        const string BroadcastIP = "192.168.48.255";
        const int ServerPort = 8005;
        Socket socketServerHandler;
        Socket socketUdpHandler;
        ISerialize messageSerializer;
        public bool IsConnected = false;

        public delegate void MessageHandler(GameMessage message);
        public event MessageHandler ReceiveMessageHandler;

        public GameClient()
        {
            messageSerializer = new SerializerBinary();
            socketServerHandler = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void SetUdpEndPoint()
        {
            socketUdpHandler = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 0);
            socketUdpHandler.Bind(localEndPoint);
        }

        public void ReceiveMessagesUdp()
        {

            byte[] data = new byte[1024];
            EndPoint endPoint = socketUdpHandler.LocalEndPoint;
            while (true)
            {
                int amount = socketUdpHandler.ReceiveFrom(data, ref endPoint);
                MemoryStream messageContainer = new MemoryStream(data, 0, data.Length);
                MessageServerInfo messageServerInfo = 
                    (MessageServerInfo)messageSerializer.Deserialize(messageContainer, 
                    typeof(MessageServerInfo), null);
                ReceiveMessageHandler(messageServerInfo);
                    return;
            }
        }

        public void UdpBroadcastRequest()
        {
            MessageSearchRequest message = new MessageSearchRequest();
            message.Port = ((IPEndPoint)socketUdpHandler.LocalEndPoint).Port;
            message.IPAdress = "127.0.0.1";
            IPEndPoint IPendPoint = new IPEndPoint(IPAddress.Parse(BroadcastIP), ServerPort);
            Socket sendRequest = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendRequest.SendTo(messageSerializer.Serialize(message, message.GetType()), IPendPoint);
            Thread threadReceiveUdp = new Thread(ReceiveMessagesUdp);
            threadReceiveUdp.IsBackground = true;
            threadReceiveUdp.Start();
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

        public void OnMessageReceive(GameMessage message)
        {
            ReceiveMessageHandler(message);
        }

        public bool ConnectToServer(IPEndPoint endPoint)
        {
            if (Connect(endPoint))
            {
                SendMessage(new MessageAddPlayer());
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
            Console.WriteLine("Start listening player");
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
                    messageContainer.Position = 0;
                    try
                    {
                        GameMessage recievedMessage = (GameMessage)messageSerializer.Deserialize(messageContainer,
                            typeof(GameMessage), new Type[] { typeof(MessagePlayerInfo), typeof(MessageAddPlayer),
                            typeof(MessageDeletePlayer), typeof(MessagePlayerAction), typeof(Player)});
                        if (recievedMessage != null)
                            OnMessageReceive(recievedMessage);
                    }
                    catch
                    {

                    }
                }    
                catch
                {
                    Disconnect();
                    return;
                }
            } while (IsConnected);
        }

        public bool SendMessage(GameMessage message)
        {
            byte[] buffer = messageSerializer.Serialize(message, message.GetType());
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