﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI_20212202_D6UWHX.Helper
{
    internal interface IEnemy
    {
        int Points { get; }

        void Animate();
    }
}
