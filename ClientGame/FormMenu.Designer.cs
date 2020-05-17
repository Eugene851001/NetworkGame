namespace ClientGame
{
    partial class FormMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btStartGame = new System.Windows.Forms.Button();
            this.btQuit = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btStartGame
            // 
            this.btStartGame.Location = new System.Drawing.Point(287, 147);
            this.btStartGame.Name = "btStartGame";
            this.btStartGame.Size = new System.Drawing.Size(183, 48);
            this.btStartGame.TabIndex = 0;
            this.btStartGame.Text = "Играть";
            this.btStartGame.UseVisualStyleBackColor = true;
            this.btStartGame.Click += new System.EventHandler(this.btStartGame_Click);
            // 
            // btQuit
            // 
            this.btQuit.Location = new System.Drawing.Point(287, 218);
            this.btQuit.Name = "btQuit";
            this.btQuit.Size = new System.Drawing.Size(183, 48);
            this.btQuit.TabIndex = 1;
            this.btQuit.Text = "Выйти";
            this.btQuit.UseVisualStyleBackColor = true;
            this.btQuit.Click += new System.EventHandler(this.button2_Click);
            // 
            // FormMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btQuit);
            this.Controls.Add(this.btStartGame);
            this.Name = "FormMenu";
            this.Text = "FormMenu";
            this.Load += new System.EventHandler(this.FormMenu_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btStartGame;
        private System.Windows.Forms.Button btQuit;
    }
}