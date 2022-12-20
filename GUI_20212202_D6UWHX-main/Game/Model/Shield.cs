using GUI_20212202_D6UWHX.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GUI_20212202_D6UWHX.Model
{
    internal class Shield : Sprite , IShield
    {
        public Shield(string graphics, Brush foreground, Size pixelSize, Point position)
            : base(graphics, foreground, pixelSize, position)
        {
        }

        public bool CheckBurning(Missile missile)
        {
            bool isBurnt = false;

            Point posMissile = missile.GetPosition();
            double halfWidthMissile = missile.Bounds.Width / 2;

            Point posShield = this.GetPosition();
            double halfWidthShield = this.Bounds.Width / 2;
            double halfHeightShield = this.Bounds.Height / 2;

            if (this.Bounds.IntersectsWith(missile.Bounds))
            {
                int column = (int)((posMissile.X - (posShield.X - halfWidthShield)) / this.CellSize.Width);
                if (column >= 0)
                {
                    for (int row = this.Patterns[Sequence[this.SequenceIndex]].Length - 1; row >= 0; row--)
                    {
                        string spriteData = this.Patterns[Sequence[this.SequenceIndex]][row];

                        if ((column < spriteData.Length) && (spriteData[column] == '1'))
                        {
                            this.Patterns[Sequence[this.SequenceIndex]][row] = spriteData.Substring(0, column) + '0' + spriteData.Substring(column + 1);
                            isBurnt = true;
                            break;
                        }
                    }

                }

                if (isBurnt)
                    this.Bitmaps[Sequence[this.SequenceIndex]] = GetBitmap(this.Patterns[Sequence[this.SequenceIndex]]);
            }

            return isBurnt;
        }

        public bool CheckExplosion(Bomb bomb)
        {
            bool isExploded = false;

            Point posBomb = bomb.GetPosition();
            double halfWidthBomb = bomb.Bounds.Width / 2;

            Point posShield = this.GetPosition();
            double halfWidthShield = this.Bounds.Width / 2;
            double halfHeightShield = this.Bounds.Height / 2;

            if (this.Bounds.IntersectsWith(bomb.Bounds))
            {
                int column = (int)((posBomb.X - (posShield.X - halfWidthShield)) / this.CellSize.Width) - 1;

                int damageCount = 0;
                for (int damageCol = 0; damageCol < 3; damageCol++)
                {
                    if (column + damageCol >= 0)
                    {
                        int damageRow = 0;

                        for (int row = 0; row < this.Patterns[Sequence[this.SequenceIndex]].Length; row++)
                        {
                            string spriteData = this.Patterns[Sequence[this.SequenceIndex]][row];

                            if (((column + damageCol) < spriteData.Length) && (spriteData[column + damageCol] == '1'))
                            {
                                isExploded = true;
                                damageRow++;
                                damageCount++;
                                this.Patterns[Sequence[this.SequenceIndex]][row] = spriteData.Substring(0, column + damageCol) + '0' + spriteData.Substring(column + damageCol + 1);

                                if (damageRow >= 2)
                                    break;

                                if (damageCount >= 5)
                                    break;
                            }
                        }
                    }

                    if (damageCount >= 6)
                        break;
                }

                if (damageCount > 0)
                    this.Bitmaps[Sequence[this.SequenceIndex]] = GetBitmap(this.Patterns[Sequence[this.SequenceIndex]]);
            }

            return isExploded;
        }
    }
}
