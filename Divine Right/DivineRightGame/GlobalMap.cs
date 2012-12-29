using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Compare;

namespace DivineRightGame
{
    /// <summary>
    /// Represents a Global Map and the items it contains
    /// </summary>
    public class GlobalMap
    {
        #region Members
        private Dictionary<MapCoordinate, MapBlock> globalGameMap;
        private List<Actor> parties;
        #endregion

        #region Constructors

        public GlobalMap()
        {
            this.globalGameMap = new Dictionary<MapCoordinate, MapBlock>(new MapCoordinateCompare());
            this.parties = new List<Actor>();
        }
        /// <summary>
        /// Add a block to a global map
        /// </summary>
        /// <param name="block"></param>
        public void AddToGlobalMap(MapBlock block)
        {
            //Does it belong on the local map?
            MapCoordinate coord = block.Tile.Coordinate;

            if (coord.MapType != DRObjects.Enums.MapTypeEnum.GLOBAL)
            {
                //Error
                throw new Exception("The map block is not for a global map");
            }
            else 
            {
                try 
                {
                    globalGameMap.Add(block.Tile.Coordinate,block);
                }
                catch (Exception ex)
                {
                    //Error
                    throw new Exception("The map already has data at the coordinate " + block.Tile.Coordinate);
                }
            }

        }
        /// <summary>
        /// Adds a number of blocks to the local map
        /// </summary>
        /// <param name="block"></param>
        public void AddToGlobalMap(MapBlock[] blocks)
        {
            foreach (MapBlock block in blocks)
            {
                AddToGlobalMap(block);
            }

        }

        /// <summary>
        /// Gets a block which is at a particular coordinate. If there is no block marked on the map, it will return an Air block.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public MapBlock GetBlockAtCoordinate(MapCoordinate coordinate)
        {
            if (this.globalGameMap.ContainsKey(coordinate))
            {
                return this.globalGameMap[coordinate];
            }
            else
            {
                //doesn't exist, send an air block.
                MapBlock airBlock = new MapBlock();
                airBlock.Tile = new DRObjects.Items.Tiles.Air(coordinate);

                return airBlock;
            }
        }

        #endregion

    }
}
