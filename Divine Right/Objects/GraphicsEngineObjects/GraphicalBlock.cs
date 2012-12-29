using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.GraphicsEngineObjects
{
    /// <summary>
    /// A Block which is displayed on the interface
    /// </summary>
    public class GraphicalBlock
    {
        /// <summary>
        /// The Coordinate of the map item.
        /// </summary>
        public MapCoordinate MapCoordinate { get; set; }

        /// <summary>
        /// The Graphic of the Tile
        /// </summary>
        public string TileGraphic { get; set; }

        /// <summary>
        /// The Graphic of the Item to show
        /// </summary>
        public string ItemGraphic {get;set; }

        #region Overridden functions

        public override string ToString()
        {
            return "GB at:" + this.MapCoordinate + " " + "Item: " + ItemGraphic;
        }

        #endregion
    }
}
