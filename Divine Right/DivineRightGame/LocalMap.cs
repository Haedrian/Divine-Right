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
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using DRObjects.DataStructures.Enum;
using DRObjects.DataStructures;

namespace DivineRightGame
{
    [Serializable]
    /// <summary>
    /// Represents a local map and the items it contains
    /// </summary>
    public class LocalMap
    {
        #region Members
        public MapBlock[, ,] localGameMap;
        private List<Actor> actors;
        private Random random = new Random();

        /// <summary>
        /// Only used for serialisation and deserialisation
        /// </summary>
        private List<MapBlock> collapsedMap = new List<MapBlock>();

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

        /// <summary>
        /// Represents the dungeon the player is currently at
        /// </summary>
        public Dungeon Dungeon { get; set; }

        /// <summary>
        /// Represents the bandit camp the player is currently at
        /// </summary>
        public BanditCamp Camp { get; set; }

        public MapSite Site { get; set; }

        /// <summary>
        /// Whether the character is currently on the global map or not
        /// </summary>
        public bool IsGlobalMap { get; set; }

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
        /// Perform a tick. Checks all actors and allow them an action
        /// </summary>
        public ActionFeedback[] Tick()
        {
            List<ActionFeedback> feedback = new List<ActionFeedback>();

            MapCoordinate playerLocation = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault().MapCharacter.Coordinate;

            //Check if we have any dead actors to clean up
            foreach (Actor deadActor in actors.Where(a => !a.IsAlive && !a.IsPlayerCharacter && a.MapCharacter != null))
            {
                this.GetBlockAtCoordinate(deadActor.MapCharacter.Coordinate).RemoveItem(deadActor.MapCharacter); //This is much cleaner due to the shove mechanism

                deadActor.MapCharacter = null; //detatch it
            }

            foreach (Actor actor in actors.Where(a => !a.IsPlayerCharacter && a.IsAlive && a.MapCharacter.IsActive))
            {
                feedback.AddRange(ActorAIManager.PerformActions(actor, actors, playerLocation));
            }

            //Health check
            foreach (Actor actor in actors.Where(a => a.IsAlive && a.MapCharacter.IsActive))
            {
                feedback.AddRange(HealthCheckManager.CheckHealth(actor));
            }

            //Make time pass - 10 seconds for a tick
            GameState.IncrementGameTime(DRTimeComponent.SECOND, 10);

            return feedback.ToArray();
        }

        /// <summary>
        /// Saves the Local Map into the folder as per its guid
        /// </summary>
        public void SerialiseLocalMap()
        {
            //Grab the unique id of the settlement or dungeon

            Guid uniqueGuid = Guid.Empty;

            if (this.Settlement != null)
            {
                uniqueGuid = this.Settlement.UniqueGUID;
            }
            else if (this.Dungeon != null)
            {
                uniqueGuid = this.Dungeon.UniqueGUID;
            }
            else if (this.Camp != null)
            {
                uniqueGuid = this.Camp.UniqueGUID;
            }
            else if (this.Site != null)
            {
                uniqueGuid = this.Site.UniqueGUID;
            }

            if (uniqueGuid == Guid.Empty)
            {
                throw new NotImplementedException("Can't get the GUID for this map type");
            }

            IFormatter formatter = new BinaryFormatter();

            SurrogateSelector ss = new SurrogateSelector();

            ss.AddSurrogate(typeof(Rectangle), new StreamingContext(StreamingContextStates.All), new RectSerializationSurrogate());
            ss.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), new ColorSerializationSurrogate());

            formatter.SurrogateSelector = ss;

            Stream stream = new FileStream(GameState.SAVEPATH + uniqueGuid + ".dvd", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, this);
            stream.Close();
        }

        /// <summary>
        /// Loads a local map from the file it was saved in. Will not load it into the gamestate thoughs
        /// </summary>
        /// <param name="uniqueGuid"></param>
        /// <returns></returns>
        public static LocalMap DeserialiseLocalMap(Guid uniqueGuid)
        {
            IFormatter formatter = new BinaryFormatter();

            SurrogateSelector ss = new SurrogateSelector();

            ss.AddSurrogate(typeof(Rectangle), new StreamingContext(StreamingContextStates.All), new RectSerializationSurrogate());
            ss.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), new ColorSerializationSurrogate());

            formatter.SurrogateSelector = ss;

            Stream stream = new FileStream(GameState.SAVEPATH + uniqueGuid + ".dvd", FileMode.Open, FileAccess.Read, FileShare.Read);
            LocalMap obj = (LocalMap)formatter.Deserialize(stream);
            stream.Close();

            return obj;
        }

        /// <summary>
        /// Determines whether a map has been generated already or not
        /// </summary>
        /// <param name="uniqueGuid"></param>
        /// <returns></returns>
        public static bool MapGenerated(Guid uniqueGuid)
        {
            return File.Exists(GameState.SAVEPATH + uniqueGuid +".dvd");
        }
    }
}
