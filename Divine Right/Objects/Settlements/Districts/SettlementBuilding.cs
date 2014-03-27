using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects.Settlements.Districts;

namespace DRObjects.Settlements.Districts
{
    /// <summary>
    /// Stores information about a particular Settlement Building located in a particular Settlement
    /// </summary>
    public class SettlementBuilding
    {
      
        /// <summary>
        /// The location Number
        /// XX00XX01XX02XX
        /// 11XXIPLAZAXX03
        /// 10XXIPLAZAXX04
        /// 09XXIPLAZAXX05
        /// XX08XX07XX06XX
        /// </summary>
        public int LocationNumber { get; set; }
        /// <summary>
        /// The district located at this place. might be null
        /// </summary>
        public District District { get; set; }
    }
}
