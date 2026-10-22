namespace DiabloClassRandomizer.Forms
{
    partial class GameForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.DiabloTile = new DiabloClassRandomizer.UI.GameTile();
            this.Diablo2Tile = new DiabloClassRandomizer.UI.GameTile();
            this.Diablo3Tile = new DiabloClassRandomizer.UI.GameTile();
            this.Diablo4Tile = new DiabloClassRandomizer.UI.GameTile();
            this.DiabloImmortalTile = new DiabloClassRandomizer.UI.GameTile();
            this.SuspendLayout();
            // 
            // DiabloTile
            // 
            this.DiabloTile.Image = global::DiabloClassRandomizer.Properties.Resources.diablo1;
            this.DiabloTile.Location = new System.Drawing.Point(24, 48);
            this.DiabloTile.Name = "DiabloTile";
            this.DiabloTile.Size = new System.Drawing.Size(336, 113);
            this.DiabloTile.TabIndex = 0;
            this.DiabloTile.Tag = "Diablo";
            this.DiabloTile.Click += new System.EventHandler(this.GameTile_Click);
            // 
            // Diablo2Tile
            // 
            this.Diablo2Tile.Image = global::DiabloClassRandomizer.Properties.Resources.diablo2;
            this.Diablo2Tile.Location = new System.Drawing.Point(24, 166);
            this.Diablo2Tile.Name = "Diablo2Tile";
            this.Diablo2Tile.Size = new System.Drawing.Size(336, 113);
            this.Diablo2Tile.TabIndex = 1;
            this.Diablo2Tile.Tag = "Diablo II: Resurrected";
            this.Diablo2Tile.Click += new System.EventHandler(this.GameTile_Click);
            // 
            // Diablo3Tile
            // 
            this.Diablo3Tile.Image = global::DiabloClassRandomizer.Properties.Resources.diablo3;
            this.Diablo3Tile.Location = new System.Drawing.Point(24, 284);
            this.Diablo3Tile.Name = "Diablo3Tile";
            this.Diablo3Tile.Size = new System.Drawing.Size(336, 113);
            this.Diablo3Tile.TabIndex = 2;
            this.Diablo3Tile.Tag = "Diablo III";
            this.Diablo3Tile.Click += new System.EventHandler(this.GameTile_Click);
            // 
            // Diablo4Tile
            // 
            this.Diablo4Tile.Image = global::DiabloClassRandomizer.Properties.Resources.diablo4;
            this.Diablo4Tile.Location = new System.Drawing.Point(24, 402);
            this.Diablo4Tile.Name = "Diablo4Tile";
            this.Diablo4Tile.Size = new System.Drawing.Size(336, 113);
            this.Diablo4Tile.TabIndex = 3;
            this.Diablo4Tile.Tag = "Diablo IV";
            this.Diablo4Tile.Click += new System.EventHandler(this.GameTile_Click);
            // 
            // DiabloImmortalTile
            // 
            this.DiabloImmortalTile.Image = global::DiabloClassRandomizer.Properties.Resources.diabloimmortal;
            this.DiabloImmortalTile.Location = new System.Drawing.Point(24, 520);
            this.DiabloImmortalTile.Name = "DiabloImmortalTile";
            this.DiabloImmortalTile.Size = new System.Drawing.Size(336, 113);
            this.DiabloImmortalTile.TabIndex = 4;
            this.DiabloImmortalTile.Tag = "Diablo Immortal";
            this.DiabloImmortalTile.Click += new System.EventHandler(this.GameTile_Click);
            // 
            // GameForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.Black;
            this.BackgroundImage = global::DiabloClassRandomizer.Properties.Resources.gamepage;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(384, 661);
            this.Controls.Add(this.DiabloImmortalTile);
            this.Controls.Add(this.Diablo4Tile);
            this.Controls.Add(this.Diablo3Tile);
            this.Controls.Add(this.Diablo2Tile);
            this.Controls.Add(this.DiabloTile);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "GameForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Game";
            this.ResumeLayout(false);

        }

        #endregion

        private DiabloClassRandomizer.UI.GameTile DiabloTile;
        private DiabloClassRandomizer.UI.GameTile Diablo2Tile;
        private DiabloClassRandomizer.UI.GameTile Diablo3Tile;
        private DiabloClassRandomizer.UI.GameTile Diablo4Tile;
        private DiabloClassRandomizer.UI.GameTile DiabloImmortalTile;
    }
}
