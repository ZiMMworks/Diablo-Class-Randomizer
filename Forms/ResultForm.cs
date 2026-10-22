using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DiabloClassRandomizer.Data;
using DiabloClassRandomizer.Logic;
using DiabloClassRandomizer.UI;

namespace DiabloClassRandomizer.Forms
{
    // Slot-machine style reveal: the class name spins through the game's classes,
    // slows down, then locks in. Click anywhere during the spin to skip straight to the result.
    public partial class ResultForm : Form
    {
        // Timing (milliseconds)
        private const int SpinDuration = 1600;
        private const int GlowDuration = 800;

        // Layout
        private static readonly Rectangle LogoRect = new Rectangle(42, 48, 300, 94);
        private static readonly Rectangle BannerRect = new Rectangle(12, 184, 360, 76);
        private static readonly Rectangle BannerSlot = new Rectangle(104, 200, 222, 24);   // dark strip between the flag and the star
        private const int ListTop = 282;
        private const int ListRowHeight = 22;

        private readonly DiabloGame _game;
        private readonly Image _banner = Properties.Resources.randomclass;
        private readonly Stopwatch _clock = new Stopwatch();

        private string _pickedClass;   // the real result, decided before the spin starts
        private string _shownClass;    // what the banner is showing right now
        private bool _isSpinning;
        private long _nextSwapAt;
        private long _lockedAt = -1;

        public ResultForm(DiabloGame game)
        {
            InitializeComponent();
            _game = game;

            // Each game has its own background, tinted to match its logo.
            BackgroundImage = Assets.GetImage(game.BackgroundFile);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            StartSpin();
        }

        #region Rolling

        private void StartSpin()
        {
            // Result is decided up front; the animation only reveals it.
            _pickedClass = ClassRandomizer.Roll(_game);
            _isSpinning = true;
            _lockedAt = -1;
            _nextSwapAt = 0;

            RerollButton.Enabled = false;
            RefreshButtons();

            _clock.Restart();
            SpinTimer.Start();
        }

        private void SpinTimer_Tick(object sender, EventArgs e)
        {
            long elapsed = _clock.ElapsedMilliseconds;

            if (_isSpinning)
            {
                if (elapsed >= SpinDuration)
                {
                    LockIn();
                }
                else if (elapsed >= _nextSwapAt)
                {
                    _shownClass = PickDifferentClass(_shownClass);
                    _nextSwapAt = elapsed + SwapDelay(elapsed);
                }
            }
            else if (elapsed - _lockedAt > GlowDuration)
            {
                // Keep ticking until the lock-in glow has faded, then stop.
                SpinTimer.Stop();
            }

            Invalidate();
        }

        // Name swaps slow down near the end of the spin.
        private static long SwapDelay(long elapsed)
        {
            double progress = (double)elapsed / SpinDuration;
            return (long)(50 + 300 * progress * progress * progress);
        }

        private string PickDifferentClass(string current)
        {
            if (_game.Classes.Count < 2)
            {
                return _game.Classes[0];
            }

            string next;
            do
            {
                next = ClassRandomizer.PickAny(_game.Classes);
            } while (next == current);

            return next;
        }

        private void LockIn()
        {
            _isSpinning = false;
            _shownClass = _pickedClass;
            _lockedAt = _clock.ElapsedMilliseconds;

            RerollButton.Enabled = true;
            RefreshButtons();
            Assets.PlaySound("legendary.wav");
        }

        // Repaints the buttons right away, since the spin keeps the form busy repainting itself.
        private void RefreshButtons()
        {
            RerollButton.Refresh();
            BackButton.Refresh();
        }

        // Bright flash when the class locks in, then a soft steady glow.
        private float GlowStrength()
        {
            if (_lockedAt < 0)
            {
                return 0f;
            }

            float t = (_clock.ElapsedMilliseconds - _lockedAt) / (float)GlowDuration;
            return t >= 1f ? 0.35f : 1f - 0.65f * t;
        }

        #endregion

        #region Buttons & input

        private void RerollButton_Click(object sender, EventArgs e)
        {
            Assets.PlaySound("freshmeat.wav");
            StartSpin();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            SpinTimer.Stop();
            Navigator.GoTo(this, new GameForm());
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            // Click anywhere while it spins to skip to the result.
            if (_isSpinning)
            {
                LockIn();
                Invalidate();
            }

            base.OnMouseClick(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                e.SuppressKeyPress = true;
                BackButton_Click(this, EventArgs.Empty);
            }
            else if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
            {
                // Space/Enter skips the spin, or rerolls once it's done.
                e.SuppressKeyPress = true;
                if (_isSpinning)
                {
                    LockIn();
                }
                else
                {
                    RerollButton_Click(this, EventArgs.Empty);
                }
            }

            base.OnKeyDown(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            SpinTimer.Stop();
            base.OnFormClosed(e);
        }

        #endregion

        #region Drawing

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            DiabloPainter.SetHighQuality(g);

            // Game logo
            g.DrawImage(_game.Logo, LogoRect);
            using (var border = new Pen(DiabloPainter.Bronze, 2))
            {
                g.DrawRectangle(border, LogoRect);
            }

            string status = _isSpinning ? "ROLLING..." : "YOUR CLASS";
            DiabloPainter.DrawText(g, status, 14f, new RectangleF(0, 152, ClientSize.Width, 26), DiabloPainter.Bone, DiabloPainter.DimBone);

            DrawClassBanner(g);
            DrawClassList(g);
        }

        private void DrawClassBanner(Graphics g)
        {
            if (!_isSpinning)
            {
                DiabloPainter.DrawGlow(g, BannerSlot, GlowStrength());
            }
            g.DrawImage(_banner, BannerRect);

            // Center the name on the window. The box starts at the flag's edge,
            // so long names shrink to fit instead of running into the flag.
            float windowCenter = ClientSize.Width / 2f;
            float halfWidth = windowCenter - BannerSlot.X;
            var nameRect = new RectangleF(windowCenter - halfWidth, BannerSlot.Y, halfWidth * 2, BannerSlot.Height);

            string name = _shownClass?.ToUpper();
            if (_isSpinning)
            {
                DiabloPainter.DrawText(g, name, 15f, nameRect, DiabloPainter.Bone, DiabloPainter.DimBone);
            }
            else
            {
                DiabloPainter.DrawText(g, name, 16f, nameRect, DiabloPainter.Ember, DiabloPainter.BloodRed, GlowStrength());
            }
        }

        // Every class for this game in two columns, with the picked one highlighted.
        private void DrawClassList(Graphics g)
        {
            int rows = (_game.Classes.Count + 1) / 2;
            var panel = new Rectangle(42, ListTop, 300, 34 + rows * ListRowHeight);
            DiabloPainter.DrawPanel(g, panel);

            DiabloPainter.DrawText(g, _game.Name.ToUpper() + " CLASSES", 11f,
                new RectangleF(panel.X, panel.Y + 6, panel.Width, 20), DiabloPainter.DimBone, DiabloPainter.FaintBone);

            const int columnWidth = 142;
            for (int i = 0; i < _game.Classes.Count; i++)
            {
                string className = _game.Classes[i];
                int column = i / rows;
                int row = i % rows;
                var cell = new RectangleF(panel.X + 8 + column * columnWidth, panel.Y + 28 + row * ListRowHeight, columnWidth, ListRowHeight);

                bool isPicked = !_isSpinning && className == _pickedClass;
                if (isPicked)
                {
                    // Red highlight bar behind the picked class
                    var bar = Rectangle.Round(new RectangleF(cell.X + 4, cell.Y + 2, cell.Width - 8, cell.Height - 4));
                    using (var highlight = new SolidBrush(Color.FromArgb(120, 140, 0, 0)))
                    using (var edge = new Pen(Color.FromArgb(200, DiabloPainter.BrightRed), 1))
                    {
                        g.FillRectangle(highlight, bar);
                        g.DrawRectangle(edge, bar);
                    }
                    DiabloPainter.DrawText(g, className, 13f, cell, DiabloPainter.Ember, DiabloPainter.BrightRed, 0.5f);
                }
                else
                {
                    // While spinning, the class currently on the banner is lit up in the list too.
                    bool isShown = _isSpinning && className == _shownClass;
                    Color top = isShown ? DiabloPainter.Bone : Color.FromArgb(150, 135, 115);
                    DiabloPainter.DrawText(g, className, 12f, cell, top, Color.FromArgb(100, 88, 72));
                }
            }
        }

        #endregion
    }
}
