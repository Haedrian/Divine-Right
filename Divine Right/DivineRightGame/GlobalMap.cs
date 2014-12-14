using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Compare;
using DRObjects.Items.Tiles.Global;
using DRObjects.Enums;
using DRObjects.Items.Archetypes.Global;

namespace DivineRightGame
{
    [Serializable]
    /// <summary>
    /// Represents a Global Map and the items it contains
    /// </summary>
    public class GlobalMap
    {
        #region Members
        public MapBlock[,] globalGameMap;
        private List<Actor> parties;
        private int worldSize;
        public List<SettlementItem> WorldSettlements { get; set; }

        /// <summary>
        /// Holds a reference to all map site items, so we can check regarding ownership change and such
        /// </summary>
        public List<MapSiteItem> MapSiteItems { get; set; }
        /// <summary>
        /// This lock is to be used during world generation to prevent race conditions
        /// </summary>
        public static readonly object lockMe = new object();
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new global map of a particular size
        /// </summary>
        /// <param name="size"></param>
        public GlobalMap(int size)
        {
            this.worldSize = size;
            this.globalGameMap = new MapBlock[size,size];
            this.parties = new List<Actor>();
        }
        /// <summary>
        /// Add a block to a global map
        /// </summary>
        /// <param name="block"></param>
        public void AddToGlobalMap(MapBlock block)
        {
            //Does it belong on the global map?
            MapCoordinate coord = block.Tile.Coordinate;

            if (coord.MapType != DRObjects.Enums.MapType.GLOBAL)
            {
                //Error
                throw new Exception("The map block is not for a global map");
            }
            else
                if (coord.X >= worldSize || coord.Y >= worldSize)
                {
                    //error
                    throw new Exception("The map block is outside the bounds of the world");

                }
                else 
            {
                try 
                {
                    globalGameMap[coord.X,coord.Y] = block ;
                }
                catch
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

            if (coordinate.X < 0 || coordinate.Y < 0)
            {
                //out of range, send an airtile
                MapBlock airBlock = new MapBlock();
                airBlock.Tile = new MapItem();
                airBlock.Tile.Coordinate = coordinate;

                return airBlock;

            }

            if (this.globalGameMap == null || coordinate.X >= worldSize || coordinate.Y >= worldSize)
            {
                //out of range, send an airtile
                //doesn't exist, send an air block.
                MapBlock airBlock = new MapBlock();
                airBlock.Tile = new MapItem();
                airBlock.Tile.Coordinate = coordinate;

                return airBlock;

            }

            if (this.globalGameMap[coordinate.X, coordinate.Y] != null)
            {

                return this.globalGameMap[coordinate.X, coordinate.Y];
            }
            else
            {
                //doesn't exist, send an air block.
                MapBlock airBlock = new MapBlock();
                airBlock.Tile = new DRObjects.Items.Tiles.Air(coordinate);
                airBlock.Tile.Coordinate = coordinate;

                return airBlock;
            }
        }

        #endregion

        #region Functions

        #endregion

    }
}
