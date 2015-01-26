using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;

namespace DRObjects
{
    [Serializable]
    /// <summary>
    /// Represents a graphic which is to be displayed in a temporary manner
    /// </summary>
    public class TemporaryGraphic
    {
        /// <summary>
        /// The Coordinate of the Graphic to Draw
        /// </summary>
        public MapCoordinate Coord { get; set; }
        public SpriteData Graphic { get; set; }
        /// <summary>
        /// How many Ticks this graphic will survive before it is destroyed
        /// </summary>
        public int LifeTime { get; set; }
    }
}
