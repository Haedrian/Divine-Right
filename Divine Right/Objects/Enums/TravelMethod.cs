using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Enums
{
    [Serializable]
    /// <summary>
    /// The way the player is travelling. Hunting, Sneaking or Normally
    /// </summary>
    public enum TravelMethod
    {
        SNEAKING,
        WALKING,
        HUNTING
    }
}
