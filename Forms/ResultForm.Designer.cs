namespace DiabloClassRandomizer.Forms
{
    partial class ResultForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ResultForm));
            this.RerollButton = new DiabloClassRandomizer.UI.DiabloButton();
            this.BackButton = new DiabloClassRandomizer.UI.DiabloButton();
            this.SpinTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // RerollButton
            // 
            this.RerollButton.Location = new System.Drawing.Point(112, 470);
            this.RerollButton.Name = "RerollButton";
            this.RerollButton.Size = new System.Drawing.Size(160, 54);
            this.RerollButton.TabIndex = 0;
            this.RerollButton.Text = "REROLL";
            this.RerollButton.TextSizePx = 18F;
            this.RerollButton.Click += new System.EventHandler(this.RerollButton_Click);
            // 
            // BackButton
            // 
            this.BackButton.Location = new System.Drawing.Point(122, 528);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(140, 50);
            this.BackButton.TabIndex = 1;
            this.BackButton.Text = "BACK";
            this.BackButton.TextSizePx = 16F;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // SpinTimer
            // 
            this.SpinTimer.Interval = 16;
            this.SpinTimer.Tick += new System.EventHandler(this.SpinTimer_Tick);
            // 
            // ResultForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(384, 661);
            this.Controls.Add(this.BackButton);
            this.Controls.Add(this.RerollButton);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ResultForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Results";
            this.ResumeLayout(false);

        }

        #endregion

        private DiabloClassRandomizer.UI.DiabloButton RerollButton;
        private DiabloClassRandomizer.UI.DiabloButton BackButton;
        private System.Windows.Forms.Timer SpinTimer;
    }
}
