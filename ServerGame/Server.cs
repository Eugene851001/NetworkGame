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
using SerializeHandler;

namespace ServerGame
{
    static class TcpGameServer
    {
        public const int chatDialogId = 0;
        const int port = 8005;
        const string address = "127.0.0.1";
        const int MaxUsersAmount = 10;

        const int timeStep = 100;

        public static Dictionary<int, Socket> playersSockets = new Dictionary<int, Socket>();
      //  public static Dictionary<int, PlayerInfo> playersInfo = new Dictionary<int, PlayerInfo>(); 

        static ISerialize messageSerializer = new SerializerBinary();

        static ServerGameLogic gameLogic = new ServerGameLogic();

        public static void ListenTcp()
        {
            Socket socketListener;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            socketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketListener.Bind(endPoint);
            socketListener.Listen(MaxUsersAmount);
            Console.WriteLine("The server is avaible (TCP)");
            int playerCounter = 0;

            string map = "";
            map += "########";
            map += "#......#";
            map += "#....#.#";
            map += "########";
            gameLogic.Map = new TileMap(8, 4, map);

            Thread threadSendUpdatedInfo = new Thread(SendUpdatedPlayersInfo);
            threadSendUpdatedInfo.IsBackground = true;
            threadSendUpdatedInfo.Start();

            Thread threadUpdateGame = new Thread(UpdateGame);
            threadUpdateGame.IsBackground = true;
            threadUpdateGame.Start();

            while (true)
            {
                Socket socketClientHandler = socketListener.Accept();
                ConnectionHandler connection = new ConnectionHandler(socketClientHandler, playerCounter);
                connection.EventOnMessageReceive += gameLogic.AddMessage;
                Player newPlayer = new Player(new Vector2D(2, 2), 100, 0.5 / 1000);
                newPlayer.Size = 0.5;
                gameLogic.Players.Add(playerCounter, newPlayer);
                playersSockets.Add(playerCounter, socketClientHandler);

                SendMessage(new MessagePersonalAddPlayer() 
                    { PlayerID = playerCounter, Map = gameLogic.Map }, socketClientHandler);
                SendAddPlayersInfo(socketClientHandler);
                gameLogic.AddMessage(new MessageAddPlayer() { PlayerID = playerCounter, PlayerInfo = newPlayer });
                SendToAll(new MessageAddPlayer() { PlayerID = playerCounter, PlayerInfo = newPlayer });
          //      SendPlayersInfo(socketClientHandler);

                playerCounter++;
            }
        }

        public static void AddCheckedMessage(GameMessage message)
        {

        }

        public static void UpdateGame()
        {
            while (true)
            {
                gameLogic.ProceedMessages();
                gameLogic.UpdateGame(timeStep);
                Thread.Sleep(timeStep);
            }
        }

        public static void SendUpdatedPlayersInfo()
        {
            while (true)
            {
                foreach (int playerID in playersSockets.Keys)
                {
                    SendPlayersInfo(playersSockets[playerID]);
                }
                while (gameLogic.ChatMessages.Count > 0)
                    SendToAll(gameLogic.ChatMessages.Dequeue());
                gameLogic.RemoveShootState();
                Thread.Sleep(timeStep);
            }
        }

        public static void SendPlayersInfo(Socket socketClient)
        {
            foreach (int playerID in gameLogic.Players.Keys)
            {
                SendMessage(new MessagePlayerInfo()
                {
                    PlayerID = playerID,
                    PlayerInfo = gameLogic.Players[playerID]
                }, socketClient) ;
            }
        }

        public static void SendAddPlayersInfo(Socket socketClient)
        {
            foreach(int playerID in gameLogic.Players.Keys)
            {
                SendMessage(new MessageAddPlayer() {PlayerID = playerID, 
                    PlayerInfo = gameLogic.Players[playerID]}, socketClient);
            }
        }

        public static void StartListen()
        {
            Thread handleTcp = new Thread(ListenTcp);
            handleTcp.Start();
        }
         
        public static void SendMessage(GameMessage message, Socket socketClient)
        {
            try
            {
                socketClient.Send(messageSerializer.Serialize(message, message.GetType(),
               new Type[] { typeof(Player) }));
            }
            catch
            {
                gameLogic.AddMessage(new MessageChat()
                {
                    PlayerID = -1,
                    Content = message.PlayerID.ToString() + " покинул нас"
                });
                gameLogic.AddMessage(new MessageDeletePlayer() { PlayerID = message.PlayerID });
            }
            
        }

        public static void RemoveClient(int key)
        {
            playersSockets.Remove(key);
            gameLogic.Players.Remove(key);
        }

        public static void SendToAll(GameMessage message)
        {
            foreach (Socket socket in playersSockets.Values)
            {
                SendMessage(message, socket);
            }
        }

        public static bool CheckMessageSender(int playerID, EndPoint playerAddress)
        {
            if (playersSockets.ContainsKey(playerID))
                return false;
            return playersSockets[playerID].RemoteEndPoint.Equals(playerAddress);
        }
    }
}