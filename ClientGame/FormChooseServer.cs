using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GameCommon;

namespace ClientGame
{
    public partial class FormChooseServer : Form
    {
        GameClient client = new GameClient();
        bool isFoundServer = false;

        public FormChooseServer()
        {
            InitializeComponent();
            client.SetUdpEndPoint();
            client.ReceiveMessageHandler += ReceiveMessage;
        }

        void ReceiveMessage(GameMessage message)
        {
            if (!(message is MessageServerInfo))
                return;
            MessageServerInfo messageInfo = message as MessageServerInfo;
            Action action = delegate
            {
                tbIPAdress.Text = messageInfo.IPAdress;
                tbPort.Text = messageInfo.Port.ToString();
                tbParticipants.Text = messageInfo.CurrentPlayersAmount.ToString()
                    + "/" + messageInfo.MaxPlayersAmount.ToString();
            };
            isFoundServer = true;
            Invoke(action);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(isFoundServer)
            {
                Form1 mainForm = new Form1(tbPlayerName.Text);
                mainForm.Show();
                this.Hide();
            }
        }

        private void FormChooseServer_Load(object sender, EventArgs e)
        {

        }

        private void btFindServer_Click(object sender, EventArgs e)
        {
            client.UdpBroadcastRequest();
        }
    }
}
