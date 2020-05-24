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
        const int ScreenWidth = 480;
        const int ScreenHeight = 640;
        GameClient client;
        string playerName;

        GameState gameState;


        ClientGameLogic gameLogic;
        Render3D render3D;

        // const int RenderInterval = 40;
        const int PhysicsUpdateInterval = 100;

        Bitmap textureBullet;
        Bitmap texturePlayer;
        Bitmap textureInterface;

        Dictionary<int, Bitmap> wallTextures;

        MessagePlayerAction messageToSend;
        int fps = 0;
        bool isConnected;

        Dictionary<int, AnimatedPlayer> animatedPlayers = new Dictionary<int, AnimatedPlayer>();
        Animation animationPlayerMoveFront;
        Animation animationPlayerMoveBack;
        Animation animationPlayerNoneFront;
        Animation animationPlayerNoneBack;
        Animation animationShootFront;

        Bitmap statusBar;

        FirstPersonAnimation firstPerson = new FirstPersonAnimation();

        GameSoundPlayer soundPlayer = new GameSoundPlayer();

        public Form1(string playerName)
        {
            InitializeComponent();
            this.playerName = playerName;

            wallTextures = new Dictionary<int, Bitmap>();
            wallTextures.Add(1, new Bitmap("textures/BrickWall.bmp"));

            statusBar = new Bitmap("textures/STATUSBARPIC.BMP");
            statusBar = new Bitmap(statusBar, ScreenWidth + 20, 50);

            textureBullet = new Bitmap("textures/Bullet.jpg");
            texturePlayer = new Bitmap("textures/Player.jpg");

            animationPlayerMoveFront = new Animation(500);
            animationPlayerMoveFront.Frames = new Bitmap[] { new Bitmap("textures/direction_front/SPR_BJ_M1.BMP"),
                new Bitmap("textures/direction_front/SPR_BJ_M2.BMP"),
                new Bitmap("textures/direction_front/SPR_BJ_M3.BMP"),
                new Bitmap("textures/direction_front/SPR_BJ_M4.BMP")};

            animationPlayerNoneFront = new Animation(1000);
            animationPlayerNoneFront.Frames = new Bitmap[] {
                new Bitmap("textures/direction_front/SPR_BJ_MACHINEGUNREADY.BMP")
            };

            animationPlayerNoneBack = new Animation(1000);
            animationPlayerNoneBack.Frames = new Bitmap[] {
                new Bitmap("textures/direction_back/SPR_BJ_MACHINEGUNREADY.BMP")
            };

            animationPlayerMoveBack = new Animation(500);
            animationPlayerMoveBack.Frames = new Bitmap[] {new Bitmap("textures/direction_back/SPR_BJ_M1.BMP"),
                new Bitmap("textures/direction_back/SPR_BJ_M2.BMP"),
                new Bitmap("textures/direction_back/SPR_BJ_M3.BMP"),
                new Bitmap("textures/direction_back/SPR_BJ_M4.BMP")};

            animationShootFront = new Animation(500);
            animationShootFront.Frames = new Bitmap[] { new Bitmap("textures/direction_front/SPR_BJ_MACHINEGUNATK1.BMP"),
                new Bitmap("textures/direction_front/SPR_BJ_MACHINEGUNATK2.BMP"),
                new Bitmap("textures/direction_front/SPR_BJ_MACHINEGUNATK3.BMP"),
                new Bitmap("textures/direction_front/SPR_BJ_MACHINEGUNATK4.BMP")};

            textureInterface = new Bitmap("textures/first_person/SPR_MACHINEGUNREADY.BMP");

            firstPerson.AnimationReady = new Animation(1000);
            firstPerson.AnimationReady.Frames = new Bitmap[] { new Bitmap("textures/first_person/SPR_MACHINEGUNREADY.BMP") };

            firstPerson.AnimationShoot = new Animation(500);
            firstPerson.AnimationShoot.Frames = new Bitmap[] { new Bitmap("textures/first_person/SPR_MACHINEGUNATK1.BMP"),
                new Bitmap("textures/first_person/SPR_MACHINEGUNATK2.BMP"),
                new Bitmap("textures/first_person/SPR_MACHINEGUNATK3.BMP"),
                new Bitmap("textures/first_person/SPR_MACHINEGUNATK4.BMP")};


            soundPlayer.LoadSound("shoot", "audio/gun-gunshot-01.wav");

            client = new GameClient();
            string map = "";
            map += "########";
            map += "#......#";
            map += "#....#.#";
            map += "########";
            gameLogic = new ClientGameLogic();
            gameLogic.Map = new TileMap(8, 4, map);
            gameLogic.EventChangeState += (Player playerToDraw, int playerID)
                => { if (animatedPlayers.ContainsKey(playerID))
                        animatedPlayers[playerID].UpdatePlayer(playerToDraw, Environment.TickCount);
                };
            gameLogic.EventChangeState += (Player playerToDraw, int playerID)
                => { if (playerID == gameLogic.ThisPlayerID)
                        firstPerson.UpdatePlayer(playerToDraw, Environment.TickCount);
                };
            gameLogic.EventChangeState += (Player playerToDraw, int playerID)
                => { if (playerID == gameLogic.ThisPlayerID)
                        updateStatusView();
                  };
            gameLogic.EventDeletePlayer += (int playerID)
                => { if (animatedPlayers.ContainsKey(playerID))
                        animatedPlayers.Remove(playerID);
                };
            render3D = new Render3D(gameLogic.Map, wallTextures);
            client.ReceiveMessageHandler += HandleMessage;
            isConnected = client.ConnectToServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005));
            client.SendMessage(new MessageRegistration() { Name = playerName });
            messageToSend = new MessagePlayerAction();

            Thread threadGameLoop = new Thread(gameLoop);
            threadGameLoop.Priority = ThreadPriority.Highest;
            threadGameLoop.Start();

        }


        PlayerActionType fetchInput()
        {
            PlayerActionType result = PlayerActionType.None;
            if (KeyboardState.IsKeyDown((int)Keys.Left))
                result |= PlayerActionType.RotateLeft;
            if (KeyboardState.IsKeyDown((int)Keys.Right))
                result |= PlayerActionType.RotateRight;
            if (KeyboardState.IsKeyDown((int)Keys.Up))
                result |= PlayerActionType.MoveFront;
            if (KeyboardState.IsKeyDown((int)Keys.Down))
                result |= PlayerActionType.MoveBack;
            if (KeyboardState.IsKeyDown((int)Keys.Space))
            {
                soundPlayer.PlaySound("shoot", false);
                result |= PlayerActionType.Shoot;
            }
            if (KeyboardState.IsKeyDown((int)Keys.Escape))
                result |= PlayerActionType.Pause;
            return result;
        }

        void gameLoop()
        {
            Stopwatch watch = Stopwatch.StartNew();
            int lastTime = 0;
            int accumulatedTime = 0;
            int messageCounter = 0;
            // client.SendMessage(new MessageRegistration() { Name = playerName });
            gameState = GameState.Run;
            while (isConnected)
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
                    if ((action & PlayerActionType.Pause) != 0)
                    {
                        gameState = GameState.Pause;
                        FormPause formPause = null;
                        Action actionForm = delegate {
                            formPause = new FormPause(gameLogic.Players.Values.ToArray());
                            formPause.ShowDialog();
                        };
                        Invoke(actionForm);
                        gameState = formPause.GameState;
                        if (gameState == GameState.Exit)
                        {
                            actionForm = delegate
                            {
                                this.Close();
                            };
                            Invoke(actionForm);
                        }
                    }
                    if (action != PlayerActionType.None)
                    {
                        MessagePlayerAction messageAction = new MessagePlayerAction()
                        { Action = action, InputNumber = messageCounter };
                        messageCounter++;
                        client.SendMessage(messageAction);
                        gameLogic.LastMessage = messageAction;
                        gameLogic.UnacknowledgedInputs.Enqueue(messageAction);
                    }
                    gameLogic.LastMessage = new MessagePlayerAction() { Action = action };
                    accumulatedTime -= PhysicsUpdateInterval;
                    gameLogic.UpdateGame(PhysicsUpdateInterval);
                }
                updateAnimatedPlayers(elapsedTime);
                firstPerson.UpdateAnimation(elapsedTime);
           //     updateStatusView();
                updateView();
                // Invalidate();
            }
        }

        void updateAnimatedPlayers(int time)
        {
            foreach (var playerID in gameLogic.Players.Keys.ToArray())
            {
                if (!animatedPlayers.ContainsKey(playerID) && playerID != gameLogic.ThisPlayerID)
                {
                    AnimatedPlayer newPlayer = new AnimatedPlayer(PhysicsUpdateInterval);

                    newPlayer.AnimationWalkFront = new Animation(animationPlayerMoveFront.TimeForFrame);
                    newPlayer.AnimationWalkFront.Frames = animationPlayerMoveFront.Frames;

                    newPlayer.AnimationNoneFront = new Animation(animationPlayerNoneFront.TimeForFrame);
                    newPlayer.AnimationNoneFront.Frames = animationPlayerNoneFront.Frames;

                    newPlayer.AnimationNoneBack = new Animation(animationPlayerNoneBack.TimeForFrame);
                    newPlayer.AnimationNoneBack.Frames = animationPlayerNoneBack.Frames;

                    newPlayer.AnimationWalkBack = new Animation(animationPlayerMoveBack.TimeForFrame);
                    newPlayer.AnimationWalkBack.Frames = animationPlayerMoveBack.Frames;

                    newPlayer.AnimationAttackFront = new Animation(animationShootFront.TimeForFrame);
                    newPlayer.AnimationAttackFront.Frames = animationShootFront.Frames;
                    // newPlayer.AnimationDie = 
                    var logicPlayer = gameLogic.Players[playerID];
                    newPlayer.UpdatePlayer(logicPlayer, Environment.TickCount);
                    newPlayer.PlayerID = playerID;
                    animatedPlayers.Add(playerID, newPlayer);
                }
            }
            foreach (AnimatedPlayer animatedPlayer in animatedPlayers.Values)
            {
                animatedPlayer.UpdateAnimation(gameLogic.GetThisPlayer(), time);
            }
        }

        void HandleMessage(GameMessage message)
        {
            if (message.MessageType == MessageType.PlayerInfo &&
                !gameLogic.Players.ContainsKey(message.PlayerID))
            {
                if (gameLogic.ThisPlayerID == -1)
                    client.SendMessage(new MessagePersonalAddPlayer());
                else
                    client.SendMessage(new MessageAddPlayer());
            }
            else if (message.MessageType == MessageType.Regitsration)
            {
                client.SendMessage(new MessageRegistration() { Name = playerName });
            }
            else
            {
                if (message.MessageType == MessageType.PersonalAddPlayer)
                    render3D.SetMap(((MessagePersonalAddPlayer)message).Map);
                gameLogic.HandleMessage(message);
            }
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
                - playerSize / 2, playerSize, playerSize);
            g.DrawLine(new Pen(Color.Blue, 3), (float)drawPosition.X, (float)drawPosition.Y,
                (float)(drawPosition.X + viewDirection.X * (playerSize / 2)),
                (float)(drawPosition.Y + viewDirection.Y * (playerSize / 2)));
        }

        void DrawBullets(Graphics g)
        {
            Vector2D drawPosition = new Vector2D();
            List<Bullet> saveBullets = new List<Bullet>(gameLogic.Bullets);
            foreach (var bullet in saveBullets)
            {
                drawPosition.X = bullet.Position.X * TileSize;
                drawPosition.Y = bullet.Position.Y * TileSize;
                int drawSize = (int)(bullet.Size * TileSize);
                g.FillEllipse(new SolidBrush(Color.Black), (float)drawPosition.X - drawSize / 2,
                   (float)drawPosition.Y - drawSize / 2, drawSize, drawSize);
            }
            //  g.FillEllipse(new SolidBrush(Color.Black), 0, 0, 200, 200);
        }

        void DrawMap(Graphics g, TileMap map)
        {
            for (int i = 0; i < map.Height; i++)
                for (int j = 0; j < map.Width; j++)
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
            Bitmap frame = getFrame(ScreenHeight, ScreenWidth);
            if (frame != null)
            {
                frame.Palette = wallTextures[1].Palette;
                pbScreen.Image = frame;
            }
        }

        void updateStatusView()
        {
            if (gameLogic.GetThisPlayer() != null)
                pbStatusBar.Image = drawCurrentStatus(statusBar, gameLogic.GetThisPlayer());
        }

        Bitmap drawCurrentStatus(Bitmap statusBar, Player player)
        {
            Bitmap canvas = new Bitmap(statusBar);
            int width = statusBar.Width;
            const int StartY = 20;
            const double LevelStarts = 0.045;
            const double ScoreStarts = 0.305;
            const double LiveStarts = 0.50;
            const double HealthStarts = 0.825;
            SolidBrush brushString = new SolidBrush(Color.White);
            Graphics g = Graphics.FromImage(canvas);
            Font drawFont = new Font("System", 12);
            g.DrawString("Level 1", drawFont, brushString, (int)(LevelStarts * width), StartY);
            g.DrawString(player.Score.ToString(), drawFont, brushString, (int)(ScoreStarts * width), StartY);
            g.DrawString(player.Lives.ToString(), drawFont, brushString, (int)(LiveStarts * width), StartY);
            g.DrawString(player.Health.ToString(), drawFont, brushString, (int)(HealthStarts * width), StartY);
            return canvas;
        }

        Bitmap getFrame(int width, int height)
        {
            Bitmap frame = new Bitmap(300, 300, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);
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
            Bitmap animationTexture = null;
            foreach (var animatedPlayer in animatedPlayers.Values.ToArray())
            {
                animationTexture = animatedPlayer.GetTexture(gameLogic.GetThisPlayer());
                if (animatedPlayer.PlayerID != gameLogic.ThisPlayerID)
                {
                    frame = render3D.DrawGameObject(gameLogic.Players[animatedPlayer.
                        PlayerID], gameLogic.GetThisPlayer(), frame, animationTexture, true);
                }
            }
            Bitmap textureInterface = firstPerson.GetTexture();
            frame = render3D.DrawInterface(frame, textureInterface);
            frame = render3D.GetScaledImage(frame, width, height);
            return frame;
        }


        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {     
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
            isConnected = false;
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
