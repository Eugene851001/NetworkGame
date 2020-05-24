namespace ClientGame
{
    partial class FormPause
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
            this.btContinue = new System.Windows.Forms.Button();
            this.btQuit = new System.Windows.Forms.Button();
            this.btShowPlayres = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btContinue
            // 
            this.btContinue.Location = new System.Drawing.Point(199, 82);
            this.btContinue.Name = "btContinue";
            this.btContinue.Size = new System.Drawing.Size(124, 38);
            this.btContinue.TabIndex = 0;
            this.btContinue.Text = "Продолжить";
            this.btContinue.UseVisualStyleBackColor = true;
            this.btContinue.Click += new System.EventHandler(this.btContinue_Click);
            // 
            // btQuit
            // 
            this.btQuit.Location = new System.Drawing.Point(199, 211);
            this.btQuit.Name = "btQuit";
            this.btQuit.Size = new System.Drawing.Size(124, 38);
            this.btQuit.TabIndex = 1;
            this.btQuit.Text = "Выйти";
            this.btQuit.UseVisualStyleBackColor = true;
            this.btQuit.Click += new System.EventHandler(this.button2_Click);
            // 
            // btShowPlayres
            // 
            this.btShowPlayres.Location = new System.Drawing.Point(199, 147);
            this.btShowPlayres.Name = "btShowPlayres";
            this.btShowPlayres.Size = new System.Drawing.Size(124, 38);
            this.btShowPlayres.TabIndex = 2;
            this.btShowPlayres.Text = "Игроки";
            this.btShowPlayres.UseVisualStyleBackColor = true;
            this.btShowPlayres.Click += new System.EventHandler(this.button1_Click);
            // 
            // FormPause
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 348);
            this.Controls.Add(this.btShowPlayres);
            this.Controls.Add(this.btQuit);
            this.Controls.Add(this.btContinue);
            this.Name = "FormPause";
            this.Text = "Пауза";
            this.Load += new System.EventHandler(this.FormPause_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btContinue;
        private System.Windows.Forms.Button btQuit;
        private System.Windows.Forms.Button btShowPlayres;
    }
}