﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using GameCommon;
using System.Diagnostics;
using System.Threading;

namespace ClientGame
{
    public partial class Form1 : Form
    {
        const int TileSize = 60;
        GameClient client;

        const int SeverID = -1;

        ClientGameLogic gameLogic;
        Render3D render3D;

       // const int RenderInterval = 40;
        const int PhysicsUpdateInterval = 100;

        Bitmap textureBullet;
        Bitmap texturePlayer;

        Dictionary<int, Bitmap> wallTextures;

        MessagePlayerAction messageToSend;
        int fps = 0;

        public Form1()
        {
            InitializeComponent();

            wallTextures = new Dictionary<int, Bitmap>();
            wallTextures.Add(1, new Bitmap("textures/BrickWall.bmp"));

            textureBullet = new Bitmap("textures/Bullet.jpg");
            texturePlayer = new Bitmap("textures/Player.jpg"); 

            client = new GameClient();
            string map = "";
            map += "########";
            map += "#......#";
            map += "#....#.#";
            map += "########";
            gameLogic = new ClientGameLogic();
            gameLogic.Map = new TileMap(8, 4, map);
            render3D = new Render3D(gameLogic.Map, wallTextures);
            client.ReceiveMessageHandler += HandleMessage;
            client.ConnectToServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005));
            messageToSend = new MessagePlayerAction();
            Thread threadGameLoop = new Thread(gameLoop);
            threadGameLoop.Priority = ThreadPriority.Highest;
            threadGameLoop.Start();

        }


        PlayerActionType fetchInput()
        {
            PlayerActionType result = PlayerActionType.None;
            if (KeyboardState.IsKeyDown((int)Keys.A))
                result |= PlayerActionType.RotateLeft;
            if (KeyboardState.IsKeyDown((int)Keys.D))
                result |= PlayerActionType.RotateRight;
            if (KeyboardState.IsKeyDown((int)Keys.W))
                result |= PlayerActionType.MoveFront;
            if (KeyboardState.IsKeyDown((int)Keys.S))
                result |= PlayerActionType.MoveBack;
            if (KeyboardState.IsKeyDown((int)Keys.Space))
                result |= PlayerActionType.Shoot;
            return result;
        }

        void gameLoop()
        {
            Stopwatch watch = Stopwatch.StartNew();
            int lastTime = 0;
            int accumulatedTime = 0;
            int messageCounter = 0;
            while(true)
            {
                int thisTime = (int)watch.ElapsedMilliseconds;
                int elapsedTime = thisTime - lastTime;
                lastTime = thisTime;
                accumulatedTime += elapsedTime;

                if (elapsedTime != 0)
                {
                    fps = 1000 / elapsedTime;
                }
                while (accumulatedTime > PhysicsUpdateInterval)
                {
                    PlayerActionType action = fetchInput();
                    if (action != PlayerActionType.None)
                    {
                        MessagePlayerAction messageAction = new MessagePlayerAction()
                            { Action = action, InputNumber = messageCounter};
                        messageCounter++;
                        client.SendMessage(messageAction);
                        gameLogic.LastMessage = messageAction;
                        gameLogic.UnacknowledgedInputs.Enqueue(messageAction);
                    }
                    gameLogic.LastMessage = new MessagePlayerAction() { Action = action };
                    accumulatedTime -= PhysicsUpdateInterval;
                    gameLogic.UpdateGame(PhysicsUpdateInterval);
                }
                updateView();
               // Invalidate();
            }
        }

        void HandleMessage(GameMessage message)
        {
            if (message.MessageType == MessageType.PlayerInfo && 
                !gameLogic.Players.ContainsKey(message.PlayerID))
                client.SendMessage(new MessageAddPlayer());
            else
                gameLogic.HandleMessage(message);
        }

        void DrawPlayer(Graphics g, Player playerInfo)
        {
            Vector2D drawPosition = new Vector2D();
            drawPosition.X = playerInfo.Position.X * TileSize;
            drawPosition.Y = playerInfo.Position.Y * TileSize;
            Vector2D viewDirection = new Vector2D(Math.Cos(playerInfo.ViewAngle), 
                Math.Sin(playerInfo.ViewAngle));
            float playerSize = (float)playerInfo.Size * TileSize;
            g.FillEllipse(new SolidBrush(Color.Red), (float)drawPosition.X - playerSize / 2, (float)drawPosition.Y 
                - playerSize / 2, playerSize,  playerSize);
            g.DrawLine(new Pen(Color.Blue, 3), (float)drawPosition.X, (float)drawPosition.Y, 
                (float)(drawPosition.X + viewDirection.X * (playerSize / 2)), 
                (float)(drawPosition.Y + viewDirection.Y * (playerSize / 2)));
        }

        void DrawBullets(Graphics g)
        {
            Vector2D drawPosition = new Vector2D();
            List<Bullet> saveBullets = new List<Bullet>(gameLogic.Bullets);
            foreach(var bullet in saveBullets)
            {
                drawPosition.X = bullet.Position.X * TileSize;
                drawPosition.Y = bullet.Position.Y * TileSize;
                int drawSize = (int)(bullet.Size * TileSize );
                g.FillEllipse(new SolidBrush(Color.Black), (float)drawPosition.X - drawSize / 2,
                   (float)drawPosition.Y - drawSize / 2, drawSize, drawSize);
            }
          //  g.FillEllipse(new SolidBrush(Color.Black), 0, 0, 200, 200);
        }

        void DrawMap(Graphics g, TileMap map)
        {
            for(int i = 0; i < map.Height; i++)
                for(int j = 0; j < map.Width; j++)
                {
                    SolidBrush brush;
                    if (map.IsSolid(j, i))
                        brush = new SolidBrush(Color.Black);
                    else
                        brush = new SolidBrush(Color.White);
                    g.FillRectangle(brush, j * TileSize, i * TileSize, TileSize, TileSize);
                    g.DrawRectangle(new Pen(Color.Red), j * TileSize, i * TileSize, TileSize, TileSize);
                }
        }

        void DrawChat(Graphics g)
        {
            Font drawFont = new Font("Arial", 12);
            for (int i = 0; i < gameLogic.chatMessages.Count; i++)
            {
                g.DrawString(gameLogic.chatMessages[i].Content, drawFont, new SolidBrush(Color.Green), 0, i * 20);
            }
            g.DrawString(this.fps.ToString(), drawFont, new SolidBrush(Color.Red), 200, 0);
        }

        void drawRays(Player player, Graphics g)
        {
            TileMap map = gameLogic.Map;
            double ViewAngle = Math.PI / 2;
            int VerticalSegmentsAmount = 50;
            double checkStep = 0.1;
            for (int x = 0; x < VerticalSegmentsAmount; x++)
            {
                //float fRayAngle = (player.fAngle - fViewAngle / 2.0f) + ((float)x / ScreenWidth) * fViewAngle;
                double rayAngle = player.ViewAngle - ViewAngle / 2 + ((double)x / VerticalSegmentsAmount) * ViewAngle;

                double wallDistance = 0;
                bool isWall = false;
                int testX, testY;
                while (!isWall)
                {
                    wallDistance += checkStep;
                    testX = (int)(player.Position.X + Math.Cos(rayAngle) * wallDistance);
                    testY = (int)(player.Position.Y + Math.Sin(rayAngle) * wallDistance);
                    if (testX >= map.Width || testX < 0 || testY >= map.Height || testY < 0)
                    {
                        isWall = true;
                    }
                    else
                    {
                        isWall = map.IsSolid(testX, testY);
                    }
                }
                Vector2D drawPosition = new Vector2D();
                drawPosition.X = player.Position.X * TileSize;
                drawPosition.Y = player.Position.Y * TileSize;
                Vector2D drawEndPosition = new Vector2D(drawPosition);
                drawEndPosition.X += wallDistance * Math.Cos(rayAngle) * TileSize;
                drawEndPosition.Y += wallDistance * Math.Sin(rayAngle) * TileSize;
                g.DrawLine(new Pen(Color.Red), (int)drawPosition.X, (int)drawPosition.Y, 
                    (int)drawEndPosition.X, (int)drawEndPosition.Y);
            }
        }

        void updateView()
        {
            Graphics g = this.CreateGraphics();
            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, 100, 100);
            DrawMap(g, gameLogic.Map);
            List<Player> savePlayersInfo = new List<Player>(gameLogic.Players.Values.AsEnumerable());
            foreach (var playerInfo in savePlayersInfo)
            {
                if (playerInfo.PlayerState != PlayerState.Killed)
                    DrawPlayer(g, playerInfo);
            }
            DrawBullets(g);
            DrawChat(g);
            if (gameLogic.GetThisPlayer() == null)
                return;
         //   drawRays(gameLogic.GetThisPlayer(), g);
            Bitmap frame = getFrame();
            if(frame != null)
                pbScreen.Image = frame;
        }

        Bitmap getFrame()
        {
            Bitmap frame = new Bitmap(300, 300);
            frame = render3D.DrawWalls(gameLogic.GetThisPlayer(), frame);
            List<GameObject> gameObjects;
            try
            {
                gameObjects = new List<GameObject>(gameLogic.Players.Values);
            }
            catch
            {
                return null;
            }
            gameObjects.Remove(gameLogic.GetThisPlayer());
            for (int i = 0; i < gameObjects.Count; i++)
            {
                if (((Player)gameObjects[i]).PlayerState == PlayerState.Killed)
                {
                    gameObjects.RemoveAt(i);
                    i--;
                }
            }
            frame = render3D.DrawGameObjects(gameObjects, gameLogic.GetThisPlayer(), frame, texturePlayer, true);
            gameObjects = new List<GameObject>(gameLogic.Bullets);
            frame = render3D.DrawGameObjects(gameObjects, gameLogic.GetThisPlayer(), frame, textureBullet, false);
            return frame;
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {     
          /*  var thisPlayer = gameLogic.GetThisPlayer();
            switch (e.KeyCode)
            {
                case Keys.Up:
                    messageToSend.Action |= PlayerActionType.MoveFront; ;//= new MessagePlayerAction() { Action = PlayerActionType.MoveFront};
                 //   thisPlayer.PlayerState = PlayerState.MoveFront;
                    isSendMessage = true;
                    break;
                case Keys.Down:
                    messageToSend.Action |= PlayerActionType.MoveBack;
               //     thisPlayer.PlayerState = PlayerState.MoveBack;
                    isSendMessage = true;
                    break;
                case Keys.Left:
                    messageToSend.Action |= PlayerActionType.RotateLeft;
                  //  thisPlayer.
                    isSendMessage = true;
                    break;
                case Keys.Right:
                    messageToSend.Action |= PlayerActionType.RotateRight;
                 //   thisPlayer.RotateDirection = 1;
                    isSendMessage = true;
                    break;
                case Keys.Space:
                    messageToSend.Action |= PlayerActionType.Shoot;
                   // thisPlayer.PlayerState = PlayerState.Shoot;
                    isSendMessage = true;
                    break;
            }
            if (Environment.TickCount - prevTickCount < timeStep)
                return;
            prevTickCount = Environment.TickCount;
            if (isSendMessage)
            {
                client.SendMessage(messageToSend);
                messageToSend.Action = 0;
                isSendMessage = false;
            }*/
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void tmDraw_Tick(object sender, EventArgs e)
        {
            Invalidate();
            //UpdateView();
        }

        private void tmUpdate_Tick(object sender, EventArgs e)
        {
            gameLogic.UpdateGame(this.tmUpdate.Interval);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            MessageDeletePlayer message = new MessageDeletePlayer();
            client.SendMessage(message);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
        //    MessageDeletePlayer message = new MessageDeletePlayer();
          //  client.SendMessage(message);
        }

        private void pbScreen_Click(object sender, EventArgs e)
        {

        }
    }
}
