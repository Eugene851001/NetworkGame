namespace ClientGame
{
    partial class FormRespawn
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
            this.btRise = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btRise
            // 
            this.btRise.Location = new System.Drawing.Point(101, 179);
            this.btRise.Name = "btRise";
            this.btRise.Size = new System.Drawing.Size(113, 36);
            this.btRise.TabIndex = 0;
            this.btRise.Text = "Возродиться";
            this.btRise.UseVisualStyleBackColor = true;
            this.btRise.Click += new System.EventHandler(this.btRise_Click);
            // 
            // FormRespawn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(639, 269);
            this.Controls.Add(this.btRise);
            this.Name = "FormRespawn";
            this.Text = "FormRespawn";
            this.Load += new System.EventHandler(this.FormRespawn_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btRise;
    }
}