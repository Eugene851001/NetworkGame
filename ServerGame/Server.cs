using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Collections;
using System.Threading;
using GameCommon;

namespace ServerGame
{
    static class Server
    {
        public const int chatDialogId = 0;
        const int port = 8005;
        const string address = "127.0.0.1";
        const int MaxUsersAmount = 10;

        public static Dictionary<int, Socket> clients = new Dictionary<int, Socket>();
        public static Dictionary<int, string> clientNames = new Dictionary<int, string>();
        public static List<string> MessageHistory = new List<string>();

        static Serializer messageSerializer = new Serializer();

        public static void ListenTcp()
        {
            Socket socketListener;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketListener.Bind(endPoint);
            socketListener.Listen(MaxUsersAmount);
            Console.WriteLine("The server is avaible (TCP)");
            while (true)
            {
                Socket socketClientHandler = socketListener.Accept();
                ConnectionHandler connection = new ConnectionHandler(socketClientHandler);
            }
        }

        public static void StartListen()
        {
            Thread handleTcp = new Thread(ListenTcp);
            handleTcp.Start();
        }

        public static void SendMessage(PlayerInfo message, Socket socketClient)
        {
            socketClient.Send(messageSerializer.Serialize(message));
        }

        public static void RemoveClient(int key)
        {
            clientNames.Remove(key);
            clients.Remove(key);
        }
        public static void SendToAll(PlayerInfo message)
        {
            foreach (Socket socket in clients.Values)
            {
                SendMessage(message, socket);
            }
        }
    }
}