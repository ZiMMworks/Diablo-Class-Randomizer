using System;
using System.Windows.Forms;
using DiabloClassRandomizer.Data;
using DiabloClassRandomizer.UI;

namespace DiabloClassRandomizer.Forms
{
    // Pick a game. Clicking a game banner goes straight to the results screen,
    // which rolls a random class for that game.
    public partial class GameForm : Form
    {
        public GameForm()
        {
            InitializeComponent();
        }

        // All five game tiles share this handler - each tile's Tag holds its game's name.
        private void GameTile_Click(object sender, EventArgs e)
        {
            var tile = (GameTile)sender;
            DiabloGame game = GameData.GetGame(tile.Tag.ToString());

            Assets.PlaySound("freshmeat.wav");
            Navigator.GoTo(this, new ResultForm(game));
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Esc goes back to the title screen.
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                Navigator.GoTo(this, new MainForm());
            }

            base.OnKeyDown(e);
        }
    }
}
