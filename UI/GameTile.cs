using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace DiabloClassRandomizer.UI
{
    // A clickable game banner on the Select Game screen. Slightly dimmed until hovered,
    // then it brightens and glows red. The game's name is stored in the tile's Tag.
    public class GameTile : Control
    {
        private bool _isHovered;
        private Image _image;

        public GameTile()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                     | ControlStyles.OptimizedDoubleBuffer
                     | ControlStyles.ResizeRedraw
                     | ControlStyles.UserPaint, true);

            Cursor = Cursors.Hand;
            Size = new Size(336, 113);
        }

        // The game's logo banner.
        [Category("Appearance")]
        public Image Image
        {
            get { return _image; }
            set
            {
                _image = value;
                Invalidate();
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            _isHovered = true;
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            _isHovered = false;
            Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            DiabloPainter.PaintParentBackground(this, e.Graphics);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            DiabloPainter.SetHighQuality(g);

            // Leave a 6px margin around the banner for the hover glow.
            var art = new Rectangle(6, 6, Width - 12, Height - 12);

            if (_isHovered)
            {
                DiabloPainter.DrawGlow(g, art, 1f);
            }

            if (_image != null)
            {
                g.DrawImage(_image, art);
            }

            // Brighten on hover, darken slightly otherwise, so the hover really stands out.
            Color overlay = _isHovered ? Color.FromArgb(28, 255, 220, 180) : Color.FromArgb(50, 0, 0, 0);
            using (var brush = new SolidBrush(overlay))
            {
                g.FillRectangle(brush, art);
            }

            using (var border = new Pen(_isHovered ? DiabloPainter.BrightRed : DiabloPainter.Bronze, 2))
            {
                g.DrawRectangle(border, art);
            }
        }
    }
}
