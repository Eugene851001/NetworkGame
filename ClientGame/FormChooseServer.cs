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
                tbMapName.Text = messageInfo.MapName;
                tbParticipants.Text = messageInfo.CurrentPlayersAmount.ToString()
                    + "/" + messageInfo.MaxPlayersAmount.ToString();
                lbStatus.Text = "Found";
            };
            isFoundServer = true;
            Invoke(action);
        }

        bool isCorrectData()
        {
            bool result = true;
            try
            {
                int.Parse(tbPort.Text);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(isFoundServer)
            {
                if (isCorrectData())
                {
                    Form1 mainForm = new Form1(tbPlayerName.Text, tbIPAdress.Text, int.Parse(tbPort.Text));
                    mainForm.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Пожалуйста, проверьте введённые данные", "", 
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
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
