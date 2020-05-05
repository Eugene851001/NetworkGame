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
        const int TitleSize = 50;
        PlayerInfo playerInfo;
        GameClient client;

        int timeStep = 100;
        int prevTickCount = 0;
        int thisPlayerID = 0;//should receive value from server

        delegate void MessageHandler(GameMessage message);

        Dictionary<MessageType, MessageHandler> messageHandlers;

        Dictionary<int, PlayerInfo> players;
        List<Bullet> bullets;
        public Form1()
        {
            InitializeComponent();

            messageHandlers = new Dictionary<MessageType, MessageHandler>();
            messageHandlers.Add(MessageType.AddPlayer, HandleMessageAdd);
            messageHandlers.Add(MessageType.PlayerInfo, HandleMessagePayerInfo);
            messageHandlers.Add(MessageType.BulletInfo, HandleMessageBulletInfo);

            players = new Dictionary<int, PlayerInfo>();
            bullets = new List<Bullet>();
            client = new GameClient();
            client.ReceiveMessageHandler += HandleMessage;
            playerInfo = null;
            client.ConnectToServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005));

            tmDraw.Enabled = true;
            playerInfo = new PlayerInfo(new Vector2D(100, 100), 100);
        }

        void HandleMessage(GameMessage message)
        {
            if(messageHandlers.ContainsKey(message.MessageType))
            {
                messageHandlers[message.MessageType](message);
            }
        }

        void HandleMessageBulletInfo(GameMessage message)
        {
            if (!(message is MessageBulletInfo))
                return;
            MessageBulletInfo messageBullet = message as MessageBulletInfo;
            bullets.Add(messageBullet.Bullet);
        }
            
        void HandleMessageAdd(GameMessage message)
        {
            if (!(message is MessageAddPlayer))
                return;
            MessageAddPlayer messageAdd = message as MessageAddPlayer;
            if (players.ContainsKey(messageAdd.PlayerID))
                return;
            players.Add(messageAdd.PlayerID, messageAdd.PlayerInfo);
        }

        void HandleMessagePayerInfo(GameMessage message)
        {
            if (!(message is MessagePlayerInfo))
                return;
            MessagePlayerInfo messageInfo = message as MessagePlayerInfo;
            if(players.Keys.Contains(messageInfo.PlayerID))
            {
                players[messageInfo.PlayerID] = messageInfo.PlayerInfo;
                if(messageInfo.PlayerInfo.PlayerState == PlayerState.Shoot)
                {
                    Bullet bullet = new Bullet(message.PlayerID);
                    bullet.Direction = players[message.PlayerID].ViewDirection;
                    bullet.Speed = players[message.PlayerID].Speed * 2;
                    bullet.Size = players[message.PlayerID].Size / 4;
                    Vector2D position = players[message.PlayerID].Position;
                    bullet.Position = new Vector2D(position.X + bullet.Size * 5, position.Y + bullet.Size * 5);
                    bullets.Add(bullet);
                }
            }
        }

        void UpdateView()
        {
            DrawPlayer(this.CreateGraphics(), playerInfo);
        }

        void DrawPlayer(Graphics g, PlayerInfo playerInfo)
        {
            Vector2D position = playerInfo.Position;
            Vector2D viewDirection = new Vector2D(Math.Cos(playerInfo.ViewAngle), 
                Math.Sin(playerInfo.ViewAngle));
            if (players[thisPlayerID].ViewAngle > Math.PI && players[thisPlayerID].ViewAngle < Math.PI * 2)
                viewDirection.Y = -viewDirection.Y;
            float playerSize = 30;
            g.FillEllipse(new SolidBrush(Color.Red), (float)position.X - playerSize / 2, (float)position.Y - playerSize / 2,
                 playerSize,  playerSize);
            g.DrawLine(new Pen(Color.Blue, 3), (float)position.X, (float)position.Y, (float)(position.X +
                viewDirection.X * (playerSize / 2)), (float)(position.Y + viewDirection.Y * (playerSize / 2)));

        }

        void DrawBullets(Graphics g)
        {
            int bulletSize = 5;
            List<Bullet> saveBullets = new List<Bullet>(bullets);
            foreach(var bullet in saveBullets)
            {
                g.FillEllipse(new SolidBrush(Color.Brown), (float)bullet.Position.X - bulletSize / 2,
                    (float)bullet.Position.Y - bulletSize / 2, bulletSize, bulletSize);
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, 100, 100);
            List<PlayerInfo> savePlayersInfo = new List<PlayerInfo>(players.Values.AsEnumerable());
            foreach (PlayerInfo playerInfo in savePlayersInfo)
            {
                DrawPlayer(g, playerInfo);
            }
            DrawBullets(g);
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
                    message = new MessagePlayerAction() { Action = PlayerActionType.Move,
                        Direction = new Vector2D(0, -1)};
                    isSendMessage = true;
                    break;
                case Keys.Down:
                    message = new MessagePlayerAction() { Action = PlayerActionType.Move, 
                        Direction = new Vector2D(0, 1) };
                    isSendMessage = true;
                    break;
                case Keys.Left:
                    message = new MessagePlayerAction() { Action = PlayerActionType.Rotate, 
                        Direction = new Vector2D(-1, 0)};
                    isSendMessage = true;
                    break;
                case Keys.Right:
                    message = new MessagePlayerAction() { Action = PlayerActionType.Rotate,
                        Direction = new Vector2D(1, 0)};
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
            for(int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Position.X += bullets[i].Direction.X * bullets[i].Speed * this.tmUpdate.Interval;
                bullets[i].Position.Y += bullets[i].Direction.Y * bullets[i].Speed * this.tmUpdate.Interval;
            }
        }
    }
}
