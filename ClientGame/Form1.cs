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

        delegate void MessageHandler(GameMessage message);

        Dictionary<MessageType, MessageHandler> messageHandlers;

        Dictionary<int, PlayerInfo> players;
        public Form1()
        {
            InitializeComponent();

            messageHandlers = new Dictionary<MessageType, MessageHandler>();
            messageHandlers.Add(MessageType.AddPlayer, HandleMessageAdd);
            messageHandlers.Add(MessageType.PlayerInfo, HandleMessagePayerInfo);

            players = new Dictionary<int, PlayerInfo>();
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

        void HandleMessageAdd(GameMessage message)
        {
            if (!(message is MessageAddPlayer))
                return;
            MessageAddPlayer messageAdd = message as MessageAddPlayer;
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
            }
        }

        void UpdateView()
        {
            DrawPlayer(this.CreateGraphics(), playerInfo);
        }

        void DrawPlayer(Graphics g, PlayerInfo playerInfo)
        {
            Vector2D position = playerInfo.Position;
            float playerSize = 10;
            g.FillEllipse(new SolidBrush(Color.Red), (float)position.X - playerSize, (float)position.Y - playerSize,
                 playerSize,  playerSize);

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(new SolidBrush(Color.Blue), 0, 0, 100, 100);
            foreach(PlayerInfo playerInfo in players.Values)
            {
                DrawPlayer(g, playerInfo);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            bool isSendMessage = false;
            MessagePlayerAction message = null;
            switch (e.KeyCode)
            {
                case Keys.Up:
                    message = new MessagePlayerAction() { Direction = new Vector2D(0, -1)};
                    isSendMessage = true;
                    break;
                case Keys.Down:
                    message = new MessagePlayerAction() { Direction = new Vector2D(0, 1) };
                    isSendMessage = true;
                    break;
                case Keys.Left:
                    message = new MessagePlayerAction() { Direction = new Vector2D(-1, 0) };
                    isSendMessage = true;
                    break;
                case Keys.Right:
                    message = new MessagePlayerAction() { Direction = new Vector2D(1, 0) };
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
    }
}
