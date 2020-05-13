﻿using System;
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
using System.Diagnostics;

namespace ServerGame
{
    static class TcpGameServer
    {
        public const int chatDialogId = 0;
        const int port = 8005;
        const string address = "127.0.0.1";
        const int MaxUsersAmount = 10;

        const int PhysicsUpdateInterval = 100;

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

            Thread threadUpdateGame = new Thread(GameLoop);
        //    threadUpdateGame.IsBackground = true;
            threadUpdateGame.Start();

            while (true)
            {
                Socket socketClientHandler = socketListener.Accept();
                ConnectionHandler connection = new ConnectionHandler(socketClientHandler, playerCounter);
                connection.EventOnMessageReceive += AddCheckedMessage;// gameLogic.AddMessage;
                Player newPlayer = new Player(new Vector2D(2, 2), 100, 0.5 / 1000);
                newPlayer.Size = 0.5;
                gameLogic.Players.Add(playerCounter, newPlayer);
                playersSockets.Add(playerCounter, socketClientHandler);

                SendMessage(new MessagePersonalAddPlayer() 
                    { PlayerID = playerCounter, Map = gameLogic.Map }, socketClientHandler);
                SendAddPlayersInfo(socketClientHandler);
                gameLogic.AddMessage(new MessageAddPlayer() { PlayerID = playerCounter, Player = newPlayer });
                SendToAll(new MessageAddPlayer() { PlayerID = playerCounter, Player = newPlayer });

                playerCounter++;
            }
        }

        public static void AddCheckedMessage(GameMessage message)
        {
            if (message.MessageType == MessageType.AddPlayer)
                SendAddPlayersInfo(playersSockets[message.PlayerID]);
            else
                gameLogic.AddMessage(message);
            if (message.MessageType == MessageType.DeletePlayer)
            {
                SendToAll(new MessageDeletePlayer() { PlayerID = message.PlayerID });
                playersSockets.Remove(message.PlayerID);
            }
        }

        public static void GameLoop()
        {
            Stopwatch watch = Stopwatch.StartNew();
            int lastTime = 0;
            int accumulatedTime = 0;
            while (true)
            {
                int thisTime = (int)watch.ElapsedMilliseconds;
                int elapsedTime = thisTime - lastTime;
                lastTime = thisTime;
                accumulatedTime += elapsedTime;

                if (accumulatedTime >= PhysicsUpdateInterval)
                {
                    gameLogic.ProceedMessages();
                    gameLogic.UpdateGame(PhysicsUpdateInterval);
                    SendUpdatedPlayersInfo();
                }
            }
        }

        public static void SendUpdatedPlayersInfo()
        {
            foreach (int playerID in playersSockets.Keys.ToArray())
            {
                SendPlayersInfo(playersSockets[playerID]);
            }
            while (gameLogic.ChatMessages.Count > 0)
                SendToAll(gameLogic.ChatMessages.Dequeue());
            gameLogic.RemoveGeneralStates();
            gameLogic.RemoveShootState();
        }

        public static void SendPlayersInfo(Socket socketClient)
        {
            foreach (int playerID in gameLogic.Players.Keys.ToArray())
            {
                int inputNumber = gameLogic.PlayersInputNumbers.ContainsKey(playerID) ?
                    gameLogic.PlayersInputNumbers[playerID] : 0;
                SendMessage(new MessagePlayerInfo()
                {
                    PlayerID = playerID,
                    Player = gameLogic.Players[playerID],
                    InputNumber = inputNumber
                }, socketClient) ;
            }
        }

        public static void SendAddPlayersInfo(Socket socketClient)
        {
            foreach(int playerID in gameLogic.Players.Keys)
            {
                SendMessage(new MessageAddPlayer() {PlayerID = playerID, 
                    Player = gameLogic.Players[playerID]}, socketClient);
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
            }
            
        }

        public static void RemoveClient(int key)
        {
            playersSockets.Remove(key);
            gameLogic.Players.Remove(key);
        }

        public static void SendToAll(GameMessage message)
        {
            foreach (Socket socket in playersSockets.Values.ToArray())
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