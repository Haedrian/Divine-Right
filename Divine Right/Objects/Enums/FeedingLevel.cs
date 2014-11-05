using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Enums
{
    [Serializable]
    public enum FeedingLevel
    {
        DEAD = 0,
        STARVING = 1,
        HUNGRY = 2,
        SATIATED = 3,
        FULL = 4,
        STUFFED = 5
    }
}
