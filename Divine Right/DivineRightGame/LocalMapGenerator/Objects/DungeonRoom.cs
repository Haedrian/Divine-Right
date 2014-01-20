using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.LocalMapGeneratorObjects;

namespace DivineRightGame.LocalMapGenerator.Objects
{
    /// <summary>
    /// A Room in A Dungeon
    /// </summary>
    public class DungeonRoom
    {
        /// <summary>
        /// A Unique ID For the Dungeon
        /// </summary>
        public int UniqueID { get; set; }

        /// <summary>
        /// The Maplet that represents the room
        /// </summary>
        public Maplet Maplet { get; set; }

        /// <summary>
        /// The Tier under which the Dungeon Room falls under
        /// </summary>
        public int TierNumber { get; set; }

        /// <summary>
        /// The Number of the Square in which the room falls under
        /// </summary>
        public int SquareNumber { get; set; }

        /// <summary>
        /// The Connections made between this object and other Dungeon Rooms
        /// </summary>
        public List<int> Connections { get; set; }

         
    }
}
