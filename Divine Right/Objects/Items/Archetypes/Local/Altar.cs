using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Local
{
    public class Altar
        : MapItem
    {
        /// <summary>
        /// Whether it's a cursed altar or not
        /// </summary>
        private bool IsCursed { get; set; }
        /// <summary>
        /// Whether the altar has been used or not
        /// </summary>
        private bool IsUsed { get; set; }
    }
}
