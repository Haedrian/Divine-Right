using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects;
using Microsoft.Xna.Framework;

namespace Divine_Right.GraphicalObjects
{
    /// <summary>
    /// A graphical block with some additional information such as the interface tile its standing on
    /// </summary>
    class InterfaceBlock: 
        GraphicalBlock
    {
        /// <summary>
        /// The location of the tile in the interface
        /// </summary>
        public int InterfaceX { get; set; }
        /// <summary>
        /// The location of the tile in the interface
        /// </summary>
        public int InterfaceY { get; set; }

        public InterfaceBlock(GraphicalBlock block)
        {
            this.ItemGraphics = block.ItemGraphics;
            this.MapCoordinate = block.MapCoordinate;
            this.TileGraphics = block.TileGraphics;
            this.ActorGraphics = block.ActorGraphics;
            this.OverlayGraphic = block.OverlayGraphic;
            this.WasVisited = block.WasVisited;
            this.IsOld = block.IsOld;
        }
    }
}
