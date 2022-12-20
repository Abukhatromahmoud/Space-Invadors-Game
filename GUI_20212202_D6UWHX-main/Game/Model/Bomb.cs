using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GUI_20212202_D6UWHX.Model
{
    internal class Bomb : Sprite
    {
        public Bomb(string graphics, Brush foreground, Size pixelSize, Point position, int[] sequence)
            : base(graphics, foreground, pixelSize, position, sequence)
        {
        }
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
