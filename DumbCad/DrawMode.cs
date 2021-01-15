﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DumbCad
{
    public enum DrawMode
    {
        Ready,
        Select,
        CircleStart,
        CircleFinish,
        PolylineStart,
        PolylineFinish,
        PanStart,
        PanFinish,
    }
}
