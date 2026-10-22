using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DiabloClassRandomizer.UI;

namespace DiabloClassRandomizer.Forms
{
    // Title screen: Diablo logo, app name, START button and floating embers.
    public partial class MainForm : Form
    {
        public const string AppVersion = "v1.4";

        private const int EmberCount = 35;

        // One glowing ember that drifts up the screen.
        private class Ember
        {
            public float X;
            public float Y;
            public float Speed;
            public float Size;
            public float Drift;   // phase of the side-to-side sway
        }

        private readonly List<Ember> _embers = new List<Ember>();
        private readonly Random _random = new Random();
        private readonly Image _titleImage = Properties.Resources.diablotitle;

        public MainForm()
        {
            InitializeComponent();

            for (int i = 0; i < EmberCount; i++)
            {
                _embers.Add(CreateEmber(_random.Next(ClientSize.Height)));
            }

            EmberTimer.Start();
        }

        private Ember CreateEmber(float startY)
        {
            return new Ember
            {
                X = _random.Next(ClientSize.Width),
                Y = startY,
                Speed = 0.4f + (float)_random.NextDouble() * 1.1f,
                Size = 1.5f + (float)_random.NextDouble() * 2.5f,
                Drift = (float)(_random.NextDouble() * Math.PI * 2)
            };
        }

        #region Buttons, timer & input

        private void StartButton_Click(object sender, EventArgs e)
        {
            EmberTimer.Stop();
            Navigator.GoTo(this, new GameForm());
        }

        private void EmberTimer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < _embers.Count; i++)
            {
                Ember ember = _embers[i];
                ember.Y -= ember.Speed;
                ember.Drift += 0.03f;
                ember.X += (float)Math.Sin(ember.Drift) * 0.3f;

                // Once an ember floats off the top, start a new one at the bottom.
                if (ember.Y < -5)
                {
                    _embers[i] = CreateEmber(ClientSize.Height + 5);
                }
            }

            Invalidate();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // Enter or Space starts the app, just like clicking START.
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Space)
            {
                e.SuppressKeyPress = true;
                StartButton_Click(this, EventArgs.Empty);
            }

            base.OnKeyDown(e);
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            EmberTimer.Stop();
            base.OnFormClosed(e);
        }

        #endregion

        #region Drawing

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            DiabloPainter.SetHighQuality(g);

            DrawLogoGlow(g);
            DrawEmbers(g);

            g.DrawImage(_titleImage, new Rectangle(12, 120, 360, 236));

            DiabloPainter.DrawText(g, "CLASS RANDOMIZER", 22f, new RectangleF(0, 360, ClientSize.Width, 36),
                DiabloPainter.BrightRed, DiabloPainter.BloodRed, 0.5f);
            DiabloPainter.DrawText(g, AppVersion, 12f, new RectangleF(0, 394, ClientSize.Width, 22),
                DiabloPainter.DimBone, DiabloPainter.FaintBone);

            // Footer
            int footerLineY = ClientSize.Height - 46;
            using (var line = new Pen(Color.FromArgb(140, 0, 0), 1))
            {
                g.DrawLine(line, 102, footerLineY, ClientSize.Width - 102, footerLineY);
            }
            DiabloPainter.DrawText(g, "ZIMMEDIA LABS", 14f, new RectangleF(0, footerLineY + 4, ClientSize.Width, 26),
                DiabloPainter.Bone, DiabloPainter.DimBone);
        }

        // Soft red glow behind the logo.
        private void DrawLogoGlow(Graphics g)
        {
            using (var glowPath = new GraphicsPath())
            {
                glowPath.AddEllipse(-60, 90, ClientSize.Width + 120, 380);
                using (var glow = new PathGradientBrush(glowPath))
                {
                    glow.CenterColor = Color.FromArgb(90, 120, 0, 0);
                    glow.SurroundColors = new[] { Color.FromArgb(0, 0, 0, 0) };
                    g.FillPath(glow, glowPath);
                }
            }
        }

        // Embers fade out as they rise toward the top of the window.
        private void DrawEmbers(Graphics g)
        {
            foreach (Ember ember in _embers)
            {
                int alpha = (int)Math.Max(0, Math.Min(220, ember.Y / ClientSize.Height * 260));
                using (var brush = new SolidBrush(Color.FromArgb(alpha, 255, 110 + (int)(ember.Size * 20), 30)))
                {
                    g.FillEllipse(brush, ember.X, ember.Y, ember.Size, ember.Size);
                }
            }
        }

        #endregion
    }
}
