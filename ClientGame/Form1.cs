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

namespace ClientGame
{
    public partial class Form1 : Form
    {
        const int TitleSize = 50;
        PlayerInfo playerInfo;
        GameClient client;
        public Form1()
        {
            InitializeComponent();
            client = new GameClient();
            client.ReceiveMessageHandler += HandleMessage;
            playerInfo = null;
            if( client.ConnectToServer(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8005)))
            {
                cbIsConnected.Checked = true;
            }
            else
            {
                cbIsConnected.Checked = false;
            }
        }

        void HandleMessage(PlayerInfo message)
        {
            playerInfo = message;
            Invalidate();
        }

        void DrawPlayer(Graphics g, IPlayerInfo playerInfo)
        {
            Vector2D position = playerInfo.GetPosition();
            float playerSize = 10;
            g.FillEllipse(new SolidBrush(Color.Red), (float)position.X - playerSize, (float)position.Y - playerSize,
                (float)position.X + playerSize, (float)position.Y + playerSize);

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (playerInfo != null)
                DrawPlayer(e.Graphics, playerInfo);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    client.SendMessage(new PlayerInfo());
                    break;
                case Keys.Down:
                    break;
                case Keys.Left:
                    break;
                case Keys.Right:
                    break;
            }

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }
    }
}
