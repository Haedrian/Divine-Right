using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Enums
{
    /// <summary>
    /// The Factions which own this object
    /// </summary>
    [Flags]
    [Serializable]
    public enum OwningFactions
    {
        ABANDONED = 1 << 0,
        HUMANS = 1 << 1,
        BANDITS = 1 << 2,
        ORCS = 1 << 3,
        UNDEAD = 1 << 4
    }
}
