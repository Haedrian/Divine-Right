using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Divine_Right.HelperFunctions
{
    [Flags]
    public enum Alignment 
    { 
        Center = 0, 
        Left = 1, 
        Right = 2, 
        Top = 4,
        Bottom = 8 
    }
}
