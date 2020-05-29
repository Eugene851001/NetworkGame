namespace ClientGame
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tmDraw = new System.Windows.Forms.Timer(this.components);
            this.tmUpdate = new System.Windows.Forms.Timer(this.components);
            this.pbScreen = new System.Windows.Forms.PictureBox();
            this.pbStatusBar = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatusBar)).BeginInit();
            this.SuspendLayout();
            // 
            // tmDraw
            // 
            this.tmDraw.Tick += new System.EventHandler(this.tmDraw_Tick);
            // 
            // tmUpdate
            // 
            this.tmUpdate.Interval = 50;
            this.tmUpdate.Tick += new System.EventHandler(this.tmUpdate_Tick);
            // 
            // pbScreen
            // 
            this.pbScreen.Location = new System.Drawing.Point(339, 1);
            this.pbScreen.Name = "pbScreen";
            this.pbScreen.Size = new System.Drawing.Size(668, 480);
            this.pbScreen.TabIndex = 0;
            this.pbScreen.TabStop = false;
            this.pbScreen.Click += new System.EventHandler(this.pbScreen_Click);
            // 
            // pbStatusBar
            // 
            this.pbStatusBar.Location = new System.Drawing.Point(339, 476);
            this.pbStatusBar.Name = "pbStatusBar";
            this.pbStatusBar.Size = new System.Drawing.Size(668, 56);
            this.pbStatusBar.TabIndex = 1;
            this.pbStatusBar.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1012, 533);
            this.Controls.Add(this.pbStatusBar);
            this.Controls.Add(this.pbScreen);
            this.DoubleBuffered = true;
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this.pbScreen)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatusBar)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmDraw;
        private System.Windows.Forms.Timer tmUpdate;
        private System.Windows.Forms.PictureBox pbScreen;
        private System.Windows.Forms.PictureBox pbStatusBar;
    }
}

