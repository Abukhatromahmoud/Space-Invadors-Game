using GUI_20212202_D6UWHX.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GUI_20212202_D6UWHX.Model
{
    internal class Player : Sprite, IPlayer
    {
        private int invicibilityCounter;
        private bool wasDrawn;

        public bool IsInvincible { get { return invicibilityCounter > 0; } }

        public Player(string graphics, Brush foreground, Size pixelSize, Point position)
            : base(graphics, foreground, pixelSize, position)
        {
            invicibilityCounter = 0;
            wasDrawn = false;
        }
        public void BecomeInvincible()
        {
            invicibilityCounter = 200;
        }
        public void DecreaseInvincibility()
        {
            invicibilityCounter--;
        }

        public void BecomeNormal()
        {
            invicibilityCounter = 0;
        }

        public bool CheckExplosion(Bomb bomb)
        {
            return this.Bounds.IntersectsWith(bomb.Bounds);
        }

        public override void Draw(DrawingContext dc, bool forced)
        {
            if (forced)
                base.Draw(dc);
            else
                this.Draw(dc);
        }
        public override void Draw(DrawingContext dc)
        {
            if (invicibilityCounter == 0)
            {
                base.Draw(dc);
                return;
            }

            if (!wasDrawn)
            {
                base.Draw(dc);
            }

            wasDrawn = !wasDrawn;
        }
    }
}
