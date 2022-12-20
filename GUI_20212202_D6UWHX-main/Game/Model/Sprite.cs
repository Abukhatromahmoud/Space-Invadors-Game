using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GUI_20212202_D6UWHX.Model
{
    internal class Sprite
    {
        private FormattedText text;
        private Point initialPosition;

        protected BitmapSource[] Bitmaps;
        protected string[][] Patterns;
        protected int[] Sequence;

        public Brush Foreground { get; private set; }
        public Size CellSize { get; private set; }
        public Rect Bounds { get; private set; }
        protected int SequenceIndex { get; set; }
        public Point Position { get; private set; }

        public bool IsAnimated { get; set; }
        public Sprite(string patterns, Brush foreground, Size pixelSize, Point position, int[] spriteSequence)
        {
            text = null;
            initialPosition = new Point(position.X, position.Y);
            this.Position = new Point(position.X, position.Y);
            this.Foreground = foreground;
            this.CellSize = pixelSize;

            this.IsAnimated = (spriteSequence != null) || (spriteSequence.Length == 1);

            SequenceIndex = 0;
            if ((spriteSequence != null) && (spriteSequence.Length > 0))
            {
                Sequence = spriteSequence;
            }
            else
            {
                Sequence = new int[] { 0 };
            }
            this.IsAnimated = Sequence.Length > 1;

            LoadPatterns(patterns.Split(','));
            Select(Sequence[SequenceIndex]);
        }
        public Sprite(string patterns, Brush foreground, Size pixelSize, Point position, int sequenceIndex = 0)
            : this(patterns, foreground, pixelSize, position, new int[] { sequenceIndex })
        {
        }
        public Sprite(string text, Brush foreground, Point position)
        {
            this.text = new FormattedText(text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, Game.TextTypeFace, Game.TextFontSize, foreground);
            initialPosition = position;
            this.Position = initialPosition;
            this.Foreground = foreground;
            this.IsAnimated = false;
            this.Bounds = new Rect(new Point(this.Position.X - this.text.Width / 2.0, this.Position.Y - this.text.Height / 2.0), new Size(this.text.Width, this.text.Height));
        }
        private void LoadPatterns(string[] spritePatterns)
        {
            int spriteCount = spritePatterns.Length;
            if (spriteCount == 0) return;

            this.Bitmaps = new BitmapSource[spriteCount];
            this.Patterns = new string[spriteCount][];

            for (int i = 0; i < spriteCount; i++)
            {
                string[] spriteLines = spritePatterns[i].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                this.Patterns[i] = spriteLines;
                this.Bitmaps[i] = GetBitmap(spriteLines);
            }
        }
        protected RenderTargetBitmap GetBitmap(string[] spriteLines)
        {
            DrawingVisual drawingVisual = new DrawingVisual();
            int columnCount = 0;
            int rowCount = spriteLines.Length;

            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                for (int r = 0; r < rowCount; r++)
                {
                    if (columnCount < spriteLines[r].Length)
                        columnCount = spriteLines[r].Length;

                    for (int c = 0; c < columnCount; c++)
                    {
                        if (spriteLines[r][c] == '1')
                            drawingContext.DrawRectangle(this.Foreground, null, new Rect(
                                c * this.CellSize.Width,
                                r * this.CellSize.Height, this.CellSize.Width, this.CellSize.Height));
                    }
                }
            }

            RenderTargetBitmap bitmap = new RenderTargetBitmap((int)(columnCount * this.CellSize.Width), (int)(rowCount * this.CellSize.Height), 96, 96, PixelFormats.Default);
            bitmap.Render(drawingVisual);
            bitmap.Freeze();

            return  bitmap;
        }
        protected void Select(int p_sequenceIndex)
        {
            this.SequenceIndex = p_sequenceIndex;
            int width = Bitmaps[Sequence[this.SequenceIndex]].PixelWidth;
            int height = Bitmaps[Sequence[this.SequenceIndex]].PixelHeight;
            this.Bounds = new Rect(new Point(this.Position.X - width / 2.0, this.Position.Y - height / 2.0), new Size(width, height));
        }
        public virtual void Draw(DrawingContext dc)
        {
            if (text != null)
            {
                dc.DrawText(text, new Point(Position.X - this.Bounds.Width / 2, Position.Y - this.Bounds.Height / 2));
                return;
            }

            dc.DrawImage(Bitmaps[Sequence[this.SequenceIndex]], this.Bounds);
        }

        public virtual void Draw(DrawingContext dc, bool forced) { }
        public Point GetPosition()
        {
            return Position;
        }

        public Point GetPosition(double offsetX, double offsetY)
        {
            return new Point(Position.X + offsetX, Position.Y + offsetY);
        }
        public void SetPosition(Point position)
        {
            this.Position = position;
            this.Bounds = new Rect(new Point(position.X - this.Bounds.Width / 2.0, position.Y - this.Bounds.Height / 2.0), this.Bounds.Size);
        }

        public void SetPosition(double x, double y)
        {
            this.Position = new Point(x, y);
            this.Bounds = new Rect(new Point(x - this.Bounds.Width / 2.0, y - this.Bounds.Height / 2.0), this.Bounds.Size);
        }

        public void ResetPosition()
        {
            this.Position = initialPosition;
            this.Bounds = new Rect(new Point(initialPosition.X - this.Bounds.Width / 2.0, initialPosition.Y - this.Bounds.Height / 2.0), this.Bounds.Size);
        }
    }
}
