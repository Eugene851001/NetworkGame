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
    public partial class FormPause : Form
    {

        GameState gameState;
        Player[] players;

        public GameState GameState { get { return gameState; } }

        public FormPause(Player[] players)
        {
            InitializeComponent();
            this.players = players;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            gameState = GameState.Exit;
            this.Close();
        }

        private void btContinue_Click(object sender, EventArgs e)
        {
            gameState = GameState.Run;
            this.Close();
        }

        private void FormPause_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FormShowPlayers formPlayres = new FormShowPlayers(players);
            formPlayres.Show();

        }
    }
}
