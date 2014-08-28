using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects.Settlements.Districts;

namespace DRObjects.Settlements.Districts
{
    [Serializable]
    /// <summary>
    /// Stores information about a particular Settlement Building located in a particular Settlement
    /// </summary>
    public class SettlementBuilding
    {
      
        /// <summary>
        /// The location Number
        /// 00,01,02
        /// 03,PL,04
        /// 05,06,07
        /// </summary>
        public int LocationNumber { get; set; }
        /// <summary>
        /// The district located at this place. might be null
        /// </summary>
        public District District { get; set; }
    }
}
