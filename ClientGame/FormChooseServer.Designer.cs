namespace ClientGame
{
    partial class FormChooseServer
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
            this.btFindServer = new System.Windows.Forms.Button();
            this.btConnectToServer = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lbStatus = new System.Windows.Forms.Label();
            this.tbIPAdress = new System.Windows.Forms.TextBox();
            this.tbPort = new System.Windows.Forms.TextBox();
            this.tbParticipants = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.tbPlayerName = new System.Windows.Forms.TextBox();
            this.lbPlayerName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btFindServer
            // 
            this.btFindServer.Location = new System.Drawing.Point(75, 247);
            this.btFindServer.Name = "btFindServer";
            this.btFindServer.Size = new System.Drawing.Size(125, 32);
            this.btFindServer.TabIndex = 0;
            this.btFindServer.Text = "Найти сервер";
            this.btFindServer.UseVisualStyleBackColor = true;
            this.btFindServer.Click += new System.EventHandler(this.btFindServer_Click);
            // 
            // btConnectToServer
            // 
            this.btConnectToServer.Location = new System.Drawing.Point(305, 247);
            this.btConnectToServer.Name = "btConnectToServer";
            this.btConnectToServer.Size = new System.Drawing.Size(125, 32);
            this.btConnectToServer.TabIndex = 1;
            this.btConnectToServer.Text = "Подключится";
            this.btConnectToServer.UseVisualStyleBackColor = true;
            this.btConnectToServer.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "IPAdress";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Порт";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(40, 135);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Участники";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(40, 164);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 17);
            this.label4.TabIndex = 5;
            this.label4.Text = "Карта";
            // 
            // lbStatus
            // 
            this.lbStatus.AutoSize = true;
            this.lbStatus.Location = new System.Drawing.Point(72, 21);
            this.lbStatus.Name = "lbStatus";
            this.lbStatus.Size = new System.Drawing.Size(70, 17);
            this.lbStatus.TabIndex = 6;
            this.lbStatus.Text = "Not found";
            // 
            // tbIPAdress
            // 
            this.tbIPAdress.Location = new System.Drawing.Point(126, 62);
            this.tbIPAdress.Name = "tbIPAdress";
            this.tbIPAdress.Size = new System.Drawing.Size(100, 22);
            this.tbIPAdress.TabIndex = 7;
            // 
            // tbPort
            // 
            this.tbPort.Location = new System.Drawing.Point(126, 98);
            this.tbPort.Name = "tbPort";
            this.tbPort.Size = new System.Drawing.Size(100, 22);
            this.tbPort.TabIndex = 8;
            // 
            // tbParticipants
            // 
            this.tbParticipants.Location = new System.Drawing.Point(126, 135);
            this.tbParticipants.Name = "tbParticipants";
            this.tbParticipants.Size = new System.Drawing.Size(100, 22);
            this.tbParticipants.TabIndex = 9;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(126, 164);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(100, 22);
            this.textBox4.TabIndex = 10;
            // 
            // tbPlayerName
            // 
            this.tbPlayerName.Location = new System.Drawing.Point(305, 164);
            this.tbPlayerName.Name = "tbPlayerName";
            this.tbPlayerName.Size = new System.Drawing.Size(100, 22);
            this.tbPlayerName.TabIndex = 11;
            // 
            // lbPlayerName
            // 
            this.lbPlayerName.AutoSize = true;
            this.lbPlayerName.Location = new System.Drawing.Point(302, 135);
            this.lbPlayerName.Name = "lbPlayerName";
            this.lbPlayerName.Size = new System.Drawing.Size(77, 17);
            this.lbPlayerName.TabIndex = 12;
            this.lbPlayerName.Text = "Ваше имя:";
            // 
            // FormChooseServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(513, 336);
            this.Controls.Add(this.lbPlayerName);
            this.Controls.Add(this.tbPlayerName);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.tbParticipants);
            this.Controls.Add(this.tbPort);
            this.Controls.Add(this.tbIPAdress);
            this.Controls.Add(this.lbStatus);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btConnectToServer);
            this.Controls.Add(this.btFindServer);
            this.Name = "FormChooseServer";
            this.Text = "FormChooseServer";
            this.Load += new System.EventHandler(this.FormChooseServer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btFindServer;
        private System.Windows.Forms.Button btConnectToServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lbStatus;
        private System.Windows.Forms.TextBox tbIPAdress;
        private System.Windows.Forms.TextBox tbPort;
        private System.Windows.Forms.TextBox tbParticipants;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox tbPlayerName;
        private System.Windows.Forms.Label lbPlayerName;
    }
}