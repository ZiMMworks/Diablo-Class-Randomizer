using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace DiabloClassRandomizer.UI
{
    // Shared colors and drawing helpers so every screen gets the same Diablo look:
    // outlined red text, soft red glows, dark framed panels.
    public static class DiabloPainter
    {
        public static readonly Color BloodRed = Color.FromArgb(192, 0, 0);
        public static readonly Color BrightRed = Color.FromArgb(255, 60, 30);
        public static readonly Color DarkRed = Color.FromArgb(110, 0, 0);
        public static readonly Color Ember = Color.FromArgb(255, 140, 60);
        public static readonly Color Bone = Color.FromArgb(220, 205, 180);
        public static readonly Color DimBone = Color.FromArgb(130, 115, 95);
        public static readonly Color FaintBone = Color.FromArgb(90, 80, 65);
        public static readonly Color Bronze = Color.FromArgb(120, 95, 70);

        public static void SetHighQuality(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        }

        // Draws outlined text filled with a vertical gradient (top color to bottom color), with an optional red glow.
        // The text is centered on the actual letters (not the font's line spacing), never wraps,
        // and shrinks to fit if it's too wide for the box.
        public static void DrawText(Graphics g, string text, float sizePx, RectangleF box, Color top, Color bottom, float glow = 0f)
        {
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            FontFamily family = Assets.DiabloFont;
            int style = (int)Assets.StyleFor(family);

            using (var path = new GraphicsPath())
            {
                path.AddString(text, family, style, sizePx, new PointF(0, 0), StringFormat.GenericTypographic);
                RectangleF bounds = path.GetBounds();

                float maxWidth = box.Width - 8;
                if (bounds.Width > maxWidth)
                {
                    float scale = maxWidth / bounds.Width;
                    path.Reset();
                    path.AddString(text, family, style, sizePx * scale, new PointF(0, 0), StringFormat.GenericTypographic);
                    bounds = path.GetBounds();
                }

                // Move the letters so their center sits on the box's center.
                using (var move = new Matrix())
                {
                    move.Translate(box.X + box.Width / 2f - (bounds.X + bounds.Width / 2f),
                                   box.Y + box.Height / 2f - (bounds.Y + bounds.Height / 2f));
                    path.Transform(move);
                }
                bounds = path.GetBounds();

                if (glow > 0f)
                {
                    for (int width = 12; width >= 4; width -= 4)
                    {
                        using (var glowPen = new Pen(Color.FromArgb((int)(40 * glow), BrightRed), width) { LineJoin = LineJoin.Round })
                        {
                            g.DrawPath(glowPen, path);
                        }
                    }
                }

                using (var outline = new Pen(Color.FromArgb(230, 0, 0, 0), 4) { LineJoin = LineJoin.Round })
                {
                    g.DrawPath(outline, path);
                }

                var gradientRect = new RectangleF(bounds.X, bounds.Y - 1, Math.Max(1, bounds.Width), Math.Max(2, bounds.Height + 2));
                using (var fill = new LinearGradientBrush(gradientRect, top, bottom, LinearGradientMode.Vertical))
                {
                    g.FillPath(fill, path);
                }
            }
        }

        // Soft red glow around a rectangle; strength 0..1.
        public static void DrawGlow(Graphics g, Rectangle rect, float strength)
        {
            if (strength <= 0.01f)
            {
                return;
            }

            for (int i = 8; i > 0; i -= 2)
            {
                int alpha = Math.Min(255, (int)(strength * 22 * (9 - i) / 4f));
                using (var brush = new SolidBrush(Color.FromArgb(alpha, BrightRed)))
                {
                    g.FillRectangle(brush, Rectangle.Inflate(rect, i, i));
                }
            }
        }

        // Dark see-through panel with a red outer border and a thin bronze inner border.
        public static void DrawPanel(Graphics g, Rectangle rect)
        {
            using (var fill = new SolidBrush(Color.FromArgb(185, 10, 4, 4)))
            using (var outer = new Pen(DarkRed, 2))
            using (var inner = new Pen(Color.FromArgb(70, 50, 30), 1))
            {
                g.FillRectangle(fill, rect);
                g.DrawRectangle(outer, rect);
                g.DrawRectangle(inner, Rectangle.Inflate(rect, -3, -3));
            }
        }

        // Draws the part of the parent's (stretched) background image that sits behind a control.
        // Custom controls use this instead of a Transparent BackColor, which can flicker.
        public static void PaintParentBackground(Control control, Graphics g)
        {
            Control parent = control.Parent;
            if (parent == null)
            {
                return;
            }

            using (var brush = new SolidBrush(parent.BackColor))
            {
                g.FillRectangle(brush, control.ClientRectangle);
            }

            Image background = parent.BackgroundImage;
            if (background == null)
            {
                return;
            }

            float scaleX = (float)background.Width / parent.ClientSize.Width;
            float scaleY = (float)background.Height / parent.ClientSize.Height;
            var source = new RectangleF(control.Left * scaleX, control.Top * scaleY, control.Width * scaleX, control.Height * scaleY);
            g.DrawImage(background, new RectangleF(0, 0, control.Width, control.Height), source, GraphicsUnit.Pixel);
        }
    }
}
