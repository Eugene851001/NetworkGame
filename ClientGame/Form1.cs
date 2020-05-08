using System;
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
using System.Threading;

namespace ClientGame
{
    public partial class Form1 : Form
    {
        const int TileSize = 60;
        PlayerInfo playerInfo;
        GameClient client;

        const int SeverID = -1;

        Vector2D camera;

        int timeStep = 100;
        int prevTickCount = 0;
        int thisPlayerID = 0;//should receive value from server
        ClientGameLogic gameLogic;
        Render3D render3D;

        Bitmap textureWall;
        Bitmap textureBullet;
        Bitmap texturePlayer;

        Dictionary<int, Bitmap> wallTextures;

        public Form1()
        {
            InitializeComponent();

            wallTextures = new Dictionary<int, Bitmap>();
            wallTextures.Add(1, new Bitmap("textures/BrickWall.jpg"));

            textureWall = new Bitmap("textures/BrickWall.jpg");
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
            playerInfo = null;
            client.ConnectToServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005));

            tmDraw.Enabled = true;
            playerInfo = new PlayerInfo(new Vector2D(100, 100), 100);
        }

        void HandleMessage(GameMessage message)
        {
            gameLogic.HandleMessage(message);
        }

        void DrawPlayer(Graphics g, PlayerInfo playerInfo)
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
            Font drawFont = new Font("Arial", 16);
            for (int i = 0; i < gameLogic.chatMessages.Count; i++)
            {
                g.DrawString(gameLogic.chatMessages[i].Content, drawFont, new SolidBrush(Color.Green), 0, i * 20);
            }
        }

        void drawRays(PlayerInfo player, Graphics g)
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

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, 100, 100);
            DrawMap(g, gameLogic.Map); 
            List<PlayerInfo> savePlayersInfo = new List<PlayerInfo>(gameLogic.Players.Values.AsEnumerable());
            foreach (PlayerInfo playerInfo in savePlayersInfo)
            {
                if(playerInfo.PlayerState != PlayerState.Killed)
                    DrawPlayer(g, playerInfo);
            }
            DrawBullets(g);
            DrawChat(g);
            if (gameLogic.GetThisPlayer() == null)
                return;
            drawRays(gameLogic.GetThisPlayer(), g);
            Bitmap frame = new Bitmap(300, 300);
            frame = render3D.DrawWalls(gameLogic.GetThisPlayer(), frame);
            List<GameObject> gameObjects = new List<GameObject>(gameLogic.Players.Values);
            gameObjects.Remove(gameLogic.GetThisPlayer());
            frame = render3D.DrawGameObjects(gameObjects, gameLogic.GetThisPlayer(), frame, texturePlayer);
            gameObjects = new List<GameObject>(gameLogic.Bullets);
            frame = render3D.DrawGameObjects(gameObjects, gameLogic.GetThisPlayer(), frame, textureBullet);
            pbScreen.Image = frame;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {     
            if (Environment.TickCount - prevTickCount < timeStep)
                return;
            prevTickCount = Environment.TickCount;
            bool isSendMessage = false;
            MessagePlayerAction message = null;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    message = new MessagePlayerAction() { Action = PlayerActionType.MoveFront};
                    isSendMessage = true;
                    break;
                case Keys.Down:
                    message = new MessagePlayerAction() { Action = PlayerActionType.MoveBack};
                    isSendMessage = true;
                    break;
                case Keys.Left:
                    message = new MessagePlayerAction() { Action = PlayerActionType.RotateLeft};
                    isSendMessage = true;
                    break;
                case Keys.Right:
                    message = new MessagePlayerAction() { Action = PlayerActionType.RotateRight};
                    isSendMessage = true;
                    break;
                case Keys.Space:
                    message = new MessagePlayerAction() { Action = PlayerActionType.Shoot };
                    isSendMessage = true;
                    break;
            }
            if(isSendMessage)
            {
                client.SendMessage(message);
            }
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
