using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Items.Tiles.Global;

namespace DivineRightGame.Managers.HelperObjects
{
    /// <summary>
    /// Represents a region on the world map
    /// </summary>
    public class Region
    {
        public List<MapBlock> Blocks { get; set; }
        public MapCoordinate Center { get; set; }

        public Region()
        {
            this.Blocks = new List<MapBlock>();
        }

        /// <summary>
        /// Returns true if the current block is in the region
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool IsInRegion(MapBlock block)
        {
            return Blocks.Any(b => b.Tile.Coordinate.Equals(block.Tile.Coordinate));
        }

        /// <summary>
        /// Returns true if this block is a distance of 1 away from any of the blocks in this region
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool IsNeighbour(MapBlock block)
        {
            return Blocks.Any(b => Math.Abs(b.Tile.Coordinate - block.Tile.Coordinate) <= 1);
        }

        /// <summary>
        /// Returns true if this region is adjacent to this region. That is they have at least one pair of adjacent tiles
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        public bool IsNeighbour(Region region)
        {
            return region.Blocks.Any(b => IsNeighbour(b));
        }

    }
}
