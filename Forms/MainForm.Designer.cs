namespace DiabloClassRandomizer.Forms
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.StartButton = new DiabloClassRandomizer.UI.DiabloButton();
            this.EmberTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Location = new System.Drawing.Point(112, 505);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(160, 58);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "START";
            this.StartButton.TextSizePx = 20F;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // EmberTimer
            // 
            this.EmberTimer.Interval = 33;
            this.EmberTimer.Tick += new System.EventHandler(this.EmberTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(384, 661);
            this.Controls.Add(this.StartButton);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Diablo Class Randomizer";
            this.ResumeLayout(false);

        }

        #endregion

        private DiabloClassRandomizer.UI.DiabloButton StartButton;
        private System.Windows.Forms.Timer EmberTimer;
    }
}
