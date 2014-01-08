using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// This represents part of the map. It may be a room, or part of a room, or part of a part of a room etc...
    /// Each maplet may contain items, inclor other maplets
    /// </summary>
    public class Maplet
    {
        /// <summary>
        /// The name of the maplet
        /// </summary>
        public string MapletName{get;set;}
        /// <summary>
        /// Tags which represent the maplet
        /// </summary>
        public List<string> MapletTags { get; set; }

        /// <summary>
        /// The size of the maplet on the x axis
        /// </summary>
        public int SizeX { get; set; }

        /// <summary>
        /// The size of the maplet on the y axis
        /// </summary>
        public int SizeY { get; set; }

        /// <summary>
        /// Whether the maplet is surrounded by a wall or not
        /// </summary>
        public bool Walled { get; set; }

        /// <summary>
        /// If the maplet is walled, what's the probabilty of each single window appearing. 
        /// </summary>
        public int? WindowProbability { get; set; }

        /// <summary>
        /// Whether the maplet has its own tiles, or if it inherits them from its parent maplet
        /// </summary>
        public bool Tiled { get; set; }

        /// <summary>
        /// If the maplet is tiled, what tiles to put on it
        /// </summary>
        public int TileID { get; set; }

        /// <summary>
        /// The contents of the maplet
        /// </summary>
        public List<MapletContents> MapletContents {get;set;}
    }
}
