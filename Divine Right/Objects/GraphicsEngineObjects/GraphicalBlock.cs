using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

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
        public string[] TileGraphics { get; set; }

        /// <summary>
        /// The Graphics of the Item to show
        /// </summary>
        public string[] ItemGraphics {get;set; }

        #region Overridden functions

        /// <summary>
        /// The overlay graphic when this graphical block was obtained when requesting an overlay
        /// </summary>
        public string OverlayGraphic { get; set; }

        public override string ToString()
        {
            return "GB at:" + this.MapCoordinate + " " + "Items: " + ItemGraphics.Length;
        }

        #endregion
    }
}
