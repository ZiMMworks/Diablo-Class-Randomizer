using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace DiabloClassRandomizer.UI
{
    // Dark red button with a glowing border on hover.
    // Has normal / hover / pressed / disabled looks and draws its text in the Diablo font.
    public class DiabloButton : Control
    {
        private bool _isHovered;
        private bool _isPressed;

        public DiabloButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.ResizeRedraw
                     | ControlStyles.UserPaint, true);

            Cursor = Cursors.Hand;
            Size = new Size(150, 46);
        }

        [Category("Appearance"), DefaultValue(16f)]
        public float TextSizePx { get; set; } = 16f;

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            _isPressed = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                _isPressed = true;
                Invalidate();
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _isPressed = false;
            Invalidate();
            base.OnMouseUp(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            Cursor = Enabled ? Cursors.Hand : Cursors.Default;
            Invalidate();
            base.OnEnabledChanged(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            Invalidate();
            base.OnTextChanged(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            DiabloPainter.PaintParentBackground(this, e.Graphics);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DiabloPainter.SetHighQuality(g);

            // Leave a 6px margin around the button face for the hover glow.
            var face = new Rectangle(6, 6, Width - 13, Height - 13);

            if (_isHovered && Enabled)
            {
                DiabloPainter.DrawGlow(g, face, 1f);
            }

            Color topColor;
            Color bottomColor;
            if (!Enabled)
            {
                topColor = Color.FromArgb(35, 30, 30);
                bottomColor = Color.FromArgb(15, 12, 12);
            }
            else if (_isPressed)
            {
                topColor = Color.FromArgb(25, 0, 0);
                bottomColor = Color.FromArgb(55, 4, 4);
            }
            else
            {
                topColor = Color.FromArgb(70, 5, 5);
                bottomColor = Color.FromArgb(15, 0, 0);
            }

            using (var fill = new LinearGradientBrush(face, topColor, bottomColor, LinearGradientMode.Vertical))
            {
                g.FillRectangle(fill, face);
            }

            Color borderColor = !Enabled ? Color.FromArgb(70, 60, 60)
                              : _isHovered ? DiabloPainter.BrightRed
                              : DiabloPainter.BloodRed;
            using (var border = new Pen(borderColor, 2))
            using (var innerBorder = new Pen(Color.FromArgb(90, 70, 45), 1))
            {
                g.DrawRectangle(border, face);
                g.DrawRectangle(innerBorder, Rectangle.Inflate(face, -3, -3));
            }

            // Text sits slightly lower when pressed, like a real button.
            int offset = _isPressed ? 1 : 0;
            var textRect = new RectangleF(face.X, face.Y + offset + 1, face.Width, face.Height);

            if (!Enabled)
            {
                DiabloPainter.DrawText(g, Text, TextSizePx, textRect, Color.FromArgb(110, 100, 100), Color.FromArgb(70, 60, 60));
            }
            else if (_isHovered)
            {
                DiabloPainter.DrawText(g, Text, TextSizePx, textRect, DiabloPainter.Ember, DiabloPainter.BrightRed, 0.6f);
            }
            else
            {
                DiabloPainter.DrawText(g, Text, TextSizePx, textRect, DiabloPainter.BrightRed, DiabloPainter.BloodRed);
            }
        }
    }
}
