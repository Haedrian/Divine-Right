using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Divine_Right.HelperFunctions
{
    [Flags]
    public enum Alignment 
    { 
        Center = 1, 
        Left = 2, 
        Right = 4, 
        Top = 8,
        Bottom = 16 
    }
}
