using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GUI_20212202_D6UWHX.Helper
{
    internal interface IGame
    {
        void Reset();
        void Exit();
        void Draw(DrawingContext dc);
    }
}
