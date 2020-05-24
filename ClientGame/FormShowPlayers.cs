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
    public partial class FormShowPlayers : Form
    {

        const int ColumnWidth = 50;
        const int ColumnHeight = 20;
        const int StartX = 50;
        const int StartY = 10;

        Color tableColor = Color.Red;
        Color textColor = Color.Black;

        Player[] playres;
        
        public FormShowPlayers(Player[] playres)
        {
            InitializeComponent();
            this.playres = playres;
        }

        private void FormShowPlayers_Load(object sender, EventArgs e)
        { 
        }

        private void dgPlayers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void FormShowPlayers_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Font drawFont = new Font("Arial", 12);
            Pen tablePen = new Pen(tableColor, 3);
            g.DrawString("Имя", drawFont, new SolidBrush(textColor), StartX, StartY);
            g.DrawString("Очки", drawFont, new SolidBrush(textColor),
                ColumnWidth + StartX,  StartY);
            g.DrawRectangle(tablePen, StartX, ColumnHeight + StartY, ColumnWidth, ColumnHeight);
            g.DrawRectangle(tablePen, ColumnWidth + StartX, ColumnHeight + StartY, ColumnWidth, ColumnHeight);

            int i = 1;
            foreach (var player in playres)
            {
                g.DrawString(player.Name, drawFont, new SolidBrush(textColor), StartX, i * ColumnHeight + StartY);
                g.DrawString(player.Score.ToString(), drawFont, new SolidBrush(textColor),
                    ColumnWidth + StartX, i * ColumnHeight + StartY);
                g.DrawRectangle(tablePen, StartX, i * ColumnHeight + StartY, ColumnWidth, ColumnHeight);
                g.DrawRectangle(tablePen, ColumnWidth + StartX, i * ColumnHeight + StartY, ColumnWidth, ColumnHeight);
                i++;
            }
        }
    }
}
