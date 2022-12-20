using GUI_20212202_D6UWHX.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GUI_20212202_D6UWHX.Model
{
    internal class Saucer : Sprite, IEnemy
    {
        private int points;
        public Saucer(string graphics, Brush foreground, Size pixelSize, Point position, int[] spriteSequence, int points)
            : base(graphics, foreground, pixelSize, position, spriteSequence)
        {
            this.points = points;
        }

        public int Points => points;
        public void Animate()
        {
            if (SequenceIndex < Sequence.Length - 1)
            {
                SequenceIndex++;
            }
            else
            {
                SequenceIndex = 0;
            }
            Select(SequenceIndex);
        }
    }
}
