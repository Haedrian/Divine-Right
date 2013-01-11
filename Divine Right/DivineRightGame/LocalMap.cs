using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;

namespace DivineRightGame
{
    /// <summary>
    /// Represents a local map and the items it contains
    /// </summary>
    public class LocalMap
    {
        #region Members
        private Dictionary<MapCoordinate, MapBlock> localGameMap;
        private List<Actor> actors;
        #endregion

        #region Constructors

        public LocalMap()
        {
            this.localGameMap = new Dictionary<MapCoordinate, MapBlock>(new DRObjects.Compare.MapCoordinateCompare());
            this.actors = new List<Actor>();
        }
        /// <summary>
        /// Add a block to a local map
        /// </summary>
        /// <param name="block"></param>
        public void AddToLocalMap(MapBlock block)
        {
            //Does it belong on the local map?
            MapCoordinate coord = block.Tile.Coordinate;

            if (coord.MapType != DRObjects.Enums.MapTypeEnum.LOCAL)
            {
                //Error
                throw new Exception("The map block is not for a local map");
            }
            else 
            {
                try 
                {
                    localGameMap.Add(block.Tile.Coordinate,block);
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
        public void AddToLocalMap(MapBlock[] blocks)
        {
            foreach (MapBlock block in blocks)
            {
                AddToLocalMap(block);
            }

        }
        /// <summary>
        /// Gets a block which is at a particular coordinate. If there is no block marked on the map, it will return an Air block.
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public MapBlock GetBlockAtCoordinate(MapCoordinate coordinate)
        {
            MapBlock ret;

            if (this.localGameMap.TryGetValue(coordinate,out ret))
            {
                return ret;
            }
            else
            {
                //doesn't exist, send a blank one
                MapBlock airBlock = new MapBlock();
                airBlock.Tile = new DRObjects.Items.Tiles.Air(coordinate);

                return airBlock;
            }
        }

        /// <summary>
        /// Loads a local map and clears actors
        /// MAKE SURE THAT THE RIGHT COMPARER FOR THE DICTIONARY IS BEING USED OR UNINTENDED ERRORS MAY OCCUR
        /// </summary>
        /// <param name="map"></param>
        public void LoadLocalMap(Dictionary<MapCoordinate, MapBlock> map)
        {
            this.localGameMap = map;
            this.actors = new List<Actor>(); //clear actors
        }

        #endregion

    }
}
