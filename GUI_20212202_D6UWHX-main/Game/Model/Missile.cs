using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GUI_20212202_D6UWHX.Model
{
    internal class Missile : Sprite
    {
        public Missile(string graphics, Brush foreground, Size pixelSize, Point position)
            : base(graphics, foreground, pixelSize, position)
        {
        }

        public bool CheckExplosion(Bomb bomb)
        {
            return this.Bounds.IntersectsWith(bomb.Bounds);
        }
    }
}
