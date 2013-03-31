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
        private MapBlock[,,] localGameMap;
        private List<Actor> actors;
        private int groundLevel;
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new Local map with x,y,z as size. The ground level determines the entry level for the map (generally 0)
        /// </summary>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="sizeZ"></param>
        /// <param name="groundLevel"></param>
        public LocalMap(int sizeX, int sizeY, int sizeZ, int groundLevel)
        {
            this.localGameMap = new MapBlock[sizeX,sizeY,sizeZ];
            this.groundLevel = groundLevel;
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
                    //Check whether the block is within the bounds of the map

                    if (block.Tile.Coordinate.X < this.localGameMap.GetLength(0) && block.Tile.Coordinate.X >= 0)
                    {
                        if (block.Tile.Coordinate.Y < this.localGameMap.GetLength(1) && block.Tile.Coordinate.Y >= 0)
                        {
                            if (block.Tile.Coordinate.Z < this.localGameMap.GetLength(2) && block.Tile.Coordinate.Z >= 0)
                            {
                                //write it
                                localGameMap[block.Tile.Coordinate.X, block.Tile.Coordinate.Y, block.Tile.Coordinate.Z] = block;
                            }
                        }
                    }
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

            if (coordinate.X < this.localGameMap.GetLength(0) && coordinate.X >= 0)
            {
                if (coordinate.Y < this.localGameMap.GetLength(1) && coordinate.Y >= 0)
                {
                    if (coordinate.Z < this.localGameMap.GetLength(2) && coordinate.Z >= 0)
                    {
                        if (this.localGameMap[coordinate.X, coordinate.Y, coordinate.Z] != null)
                        {
                            return this.localGameMap[coordinate.X, coordinate.Y, coordinate.Z];
                        }
                    }
                }
            }

                //doesn't exist, send a blank one
                MapBlock airBlock = new MapBlock();
                airBlock.Tile = new DRObjects.Items.Tiles.Air(coordinate);

                return airBlock;
            
        }

        /// <summary>
        /// Loads a local map and clears actors
        /// </summary>
        /// <param name="map"></param>
        public void LoadLocalMap(MapBlock[,,] map,int groundLevel)
        {
            this.localGameMap = map;
            this.groundLevel = groundLevel;
            this.actors = new List<Actor>(); //clear actors
        }

        #endregion

    }
}
