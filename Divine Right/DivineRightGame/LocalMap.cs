using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Enums;
using DRObjects.ActorHandling.ActorMissions;
using DivineRightGame.Pathfinding;
using DRObjects.LocalMapGeneratorObjects;
using Microsoft.Xna.Framework;
using DivineRightGame.CombatHandling;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using DivineRightGame.ActorHandling;
using DRObjects.Items.Archetypes.Global;

namespace DivineRightGame
{
    /// <summary>
    /// Represents a local map and the items it contains
    /// </summary>
    public class LocalMap
    {
        #region Members
        private MapBlock[, ,] localGameMap;
        private List<Actor> actors;
        private Random random = new Random();

        private int groundLevel;
        #endregion

        #region Properties
        /// <summary>
        /// The actors in the local map
        /// </summary>
        public List<Actor> Actors
        {
            get { return actors; }
            set { actors = value; }
        }
        public List<PointOfInterest> PointsOfInterest { get; set; }
        public Byte[,] PathfindingMap { get; set; }

        /// <summary>
        /// Represents the settlement the player is currently at
        /// </summary>
        public Settlement Settlement { get; set; }

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
            this.localGameMap = new MapBlock[sizeX, sizeY, sizeZ];
            this.groundLevel = groundLevel;
            this.actors = new List<Actor>();
        }

        #endregion
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
        public void LoadLocalMap(MapBlock[, ,] map, int groundLevel)
        {
            this.localGameMap = map;
            this.groundLevel = groundLevel;
            this.actors = new List<Actor>(); //clear actors
        }

        /// <summary>
        /// Generates the map required for pathfinding, and assign it to the Interface
        /// </summary>
        public void GeneratePathfindingMap()
        {
            //Generate a byte map of x and y
            int squareSize = PathfinderInterface.CeilToPower2(Math.Max(localGameMap.GetLength(0), localGameMap.GetLength(1)));

            PathfindingMap = new byte[squareSize, squareSize];

            for (int i = 0; i < localGameMap.GetLength(0); i++)
            {
                for (int j = 0; j < localGameMap.GetLength(1); j++)
                {
                    if (i < localGameMap.GetLength(0) - 1 && j < localGameMap.GetLength(1) - 1)
                    {
                        //Copyable - if it may contain items, put a weight of 1, otherwise an essagerated one
                        PathfindingMap[i, j] = localGameMap[i, j, 0] != null ? localGameMap[i, j, 0].MayContainItems ? (byte)1 : Byte.MaxValue : Byte.MaxValue;
                    }
                    else
                    {
                        //Put in the largest possible weight
                        PathfindingMap[i, j] = Byte.MaxValue;
                    }
                }
            }

            PathfinderInterface.Nodes = PathfindingMap;
        }

        /// <summary>
        /// Updates the pathfinding map for that particular coordinate. and for the 9 tiles around it
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void UpdatePathfindingMap(int x, int y)
        {
            PathfindingMap[x, y] = localGameMap[x, y, 0].MayContainItems ? (byte)1 : Byte.MaxValue;

            for (int xLoop = -1; xLoop >= 0 && xLoop < localGameMap.GetLength(0) && xLoop < 2; xLoop++)
            {
                for (int yLoop = -1; yLoop >= 0 && yLoop < localGameMap.GetLength(1) && yLoop < 2; yLoop++)
                {
                    PathfindingMap[x + xLoop, y + yLoop] = localGameMap[x + xLoop, y + yLoop, 0].MayContainItems ? (byte)1 : Byte.MaxValue;
                }
            }
        }

        /// <summary>
        /// Perform a tick. Checks all actors and allow them an action
        /// </summary>
        public PlayerFeedback[] Tick()
        {
            List<PlayerFeedback> feedback = new List<PlayerFeedback>();

            MapCoordinate playerLocation = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault().MapCharacter.Coordinate;

            //Check if we have any dead actors to clean up
            foreach (Actor deadActor in actors.Where(a => !a.IsAlive && !a.IsPlayerCharacter && a.MapCharacter != null))
            {
                this.GetBlockAtCoordinate(deadActor.MapCharacter.Coordinate).RemoveTopItem(); //remove the character

                //TODO: PUT A CORPSE OF SOME SORT
                deadActor.MapCharacter = null; //detatch it
            }

            foreach (Actor actor in actors.Where(a => !a.IsPlayerCharacter && a.IsAlive))
            {
                feedback.AddRange(ActorAIManager.PerformActions(actor, actors, playerLocation));
            }

            //Health check
            foreach (Actor actor in actors.Where(a => a.IsAlive))
            {
                feedback.AddRange(HealthCheckManager.CheckHealth(actor));
            }

            return feedback.ToArray();
        }

    }
}
