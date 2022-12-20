using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GUI_20212202_D6UWHX.Model
{
    internal class Explosion : Sprite
    {
        public int VisibilityCounter { get; set; }

        public Explosion(string graphics, Brush foreground, Size pixelSize, Point position, int visibilityCounter)
            : base(graphics, foreground, pixelSize, position)
        {
            this.VisibilityCounter = visibilityCounter;
        }

        public Explosion(int points, Brush foreground, Point position, int visibilityCounter)
            : base(points.ToString(), foreground, position)
        {
            this.VisibilityCounter = visibilityCounter;
        }
    }
}
