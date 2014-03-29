using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Settlements.Districts;

namespace DivineRightGame.SettlementHandling.Objects
{
    /// <summary>
    /// Holds the location and map of the settlement building
    /// </summary>
    public class SettlementBuildingMap
    {
        /// <summary>
        /// X coordinate w.r.t. the main map
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Y coordinate w.r.t. the main map
        /// </summary>
        public int Y { get; set; }

        /// <summary>
        /// The actual map when it is generated
        /// </summary>
        public MapBlock[,] GeneratedMap { get; set; }

        /// <summary>
        /// The district in question
        /// </summary>
        public District District { get; set; }
    }
}
