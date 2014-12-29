using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    /// <summary>
    /// A Bandit Camp. A small map containing bandits.
    /// </summary>
    public class BanditCamp:
        Location
    {
        public int BanditTotal { get; set; }
    }
}
