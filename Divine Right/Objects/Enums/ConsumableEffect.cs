using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Enums
{
    /// <summary>
    /// The effects a particular item has when it is consumed
    /// </summary>
    [Flags]
    public enum ConsumableEffect
    {
        FEED = 1 << 0,

    }
}
