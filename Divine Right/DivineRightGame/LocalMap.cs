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
using DRObjects.Extensions;
using DRObjects.ActorHandling;
using DRObjects.Graphics;
using DivineRightGame.RayTracing;

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
        /// A list of temporary graphics.
        /// </summary>
        public List<TemporaryGraphic> TemporaryGraphics { get; set; }

        /// <summary>
        /// A list of active effects. This is here to make going through them easier.
        /// </summary>
        public List<Effect> ActiveEffects { get; set; }

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

        public bool IsUnderground { get; set; }

        /// <summary>
        /// The Location the player is at
        /// </summary>
        public Location Location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;

                //Then subscribe to the event
                GameState.MinuteValueChanged += MinuteChanged;
            }
        }

        private Location _location;

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
            this.IsUnderground = false;
            this.ActiveEffects = new List<Effect>();
            this.TemporaryGraphics = new List<TemporaryGraphic>();
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

        public MapBlock[] GetBlocksAroundPoint(MapCoordinate centre, int radius)
        {
            int minY = centre.Y - Math.Abs(radius);
            int maxY = centre.Y + Math.Abs(radius);

            int minX = centre.X - Math.Abs(radius);
            int maxX = centre.X + Math.Abs(radius);

            List<MapBlock> returnList = new List<MapBlock>();

            //go through all of them

            for (int yLoop = maxY; yLoop >= minY; yLoop--)
            {
                for (int xLoop = minX; xLoop <= maxX; xLoop++)
                {
                    MapCoordinate coord = new MapCoordinate(xLoop, yLoop, 0, MapType.GLOBAL);

                    if (xLoop >= 0 && xLoop < this.localGameMap.GetLength(0) && yLoop >= 0 && yLoop < this.localGameMap.GetLength(1))
                    { //make sure they're in the map
                        returnList.Add(this.GetBlockAtCoordinate(coord));
                    }
                }
            }

            return returnList.ToArray();
        }

        /// <summary>
        /// Performs the summoning of creatures for Dungeons, as well as handles events
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MinuteChanged(object sender, EventArgs e)
        {
            //Have we got any effects to handle ?
            for (int i = 0; i < this.ActiveEffects.Count; i++)
            {
                var effect = this.ActiveEffects[i];

                effect.MinutesLeft -= 1;

                if (effect.MinutesLeft <= 0)
                {
                    //Remove it!
                    EffectsManager.RemoveEffect(effect);

                    i--;

                    //Display the message!
                    GameState.NewLog.Add(effect.EffectDisappeared);
                }
            }

            if (GameState.PlayerCharacter != null)
            {
                //If the player character (AND VIP CHARACTERS IN THE FUTURE) have defences which aren't full, then increase it
                if (GameState.PlayerCharacter.CurrentDefences < GameState.PlayerCharacter.MaximumDefences)
                {
                    GameState.PlayerCharacter.CurrentDefences++;
                }
                else if (GameState.PlayerCharacter.CurrentDefences > GameState.PlayerCharacter.MaximumDefences)
                {
                    GameState.PlayerCharacter.CurrentDefences = GameState.PlayerCharacter.MaximumDefences;
                }
            }

            if (this.Location is Dungeon)
            {
                Dungeon dungeon = this.Location as Dungeon;

                int maximumActors = dungeon.DifficultyLevel * dungeon.Rooms.Count;

                //How many actors do we have?
                int activeActors = this.Actors.Count(a => a.IsActive && a.IsAlive);

                if (maximumActors > activeActors)
                {
                    //Plop him on the map, and make him wander into a particular room
                    var summoningCircle = dungeon.SummoningCircles.Where(sc => sc.IsSummoning && sc.IsActive).ToList().GetRandom();

                    if (summoningCircle == null)
                    {
                        return; //They're all disabled
                    }

                    //Generate new actor
                    var gennedActor = ActorGeneration.CreateActor(OwningFactions.UNDEAD, 3 * dungeon.DifficultyLevel);

                    //Pick a room at random
                    Rectangle room = dungeon.Rooms.GetRandom();

                    gennedActor.MissionStack = new Stack<ActorMission>();

                    gennedActor.MissionStack.Push(new WanderMission()
                    {
                        LoiterPercentage = 5,
                        WanderPoint = new MapCoordinate(room.Center, MapType.LOCAL),
                        WanderRectangle = room
                    });

                    this.Actors.Add(gennedActor);

                    //Put it on the block
                    this.GetBlockAtCoordinate(summoningCircle.Coordinate).PutItemOnBlock(gennedActor.MapCharacter);
                }
            }
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
                        if (localGameMap[i,j,0] == null)
                        {
                            PathfindingMap[i, j] = Byte.MaxValue;
                        }
                        else if (localGameMap[i,j,0].MayContainItems)
                        {
                            //Then 1
                            PathfindingMap[i,j] = (byte)1;
                        }
                        else  if (localGameMap[i,j,0].GetTopItem() != null && localGameMap[i,j,0].GetTopItem().GetType().Equals(typeof(LocalCharacter)))
                        {
                            //It's another actor. Avoid going through, but it's an option
                            PathfindingMap[i, j] = (byte)5;
                        }
                        else
                        {
                            PathfindingMap[i, j] = Byte.MaxValue; //You shall not PASS
                        }

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

            //Age the temporary graphics by 1. If they are too old, delete them now
            foreach (var graphic in TemporaryGraphics)
            {
                graphic.LifeTime--;
            }
            //Bye bye
            TemporaryGraphics = TemporaryGraphics.Where(tg => tg.LifeTime > 0).ToList();

            //Any special attacks get timeout reduced
            foreach(var attack in GameState.PlayerCharacter.SpecialAttacks)
            {
                if (attack == null)
                {
                    continue;
                }

                if (attack.TimeOutLeft > 0)
                {
                    attack.TimeOutLeft--;
                }
            }

            return feedback.ToArray();
        }

        /// <summary>
        /// Saves the Local Map into the folder as per its guid
        /// </summary>
        public void SerialiseLocalMap()
        {
            //Grab the unique id of the settlement or dungeon

            Guid uniqueGuid = Guid.Empty;

            if (this.Location != null)
            {
                uniqueGuid = this.Location.UniqueGUID;
            }

            if (uniqueGuid == Guid.Empty)
            {
                return;
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

            //Clear the effects
            obj.ActiveEffects = new List<Effect>();

            return obj;
        }

        /// <summary>
        /// Determines whether a map has been generated already or not
        /// </summary>
        /// <param name="uniqueGuid"></param>
        /// <returns></returns>
        public static bool MapGenerated(Guid uniqueGuid)
        {
            return File.Exists(GameState.SAVEPATH + uniqueGuid + ".dvd");
        }


        /// <summary>
        /// Returns true if there is a direct path between the source and the target
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public bool HasDirectPath(MapCoordinate source, MapCoordinate target)
        {

            var points = RayTracingHelper.BresenhamLine(source.X, source.Y, target.X, target.Y).ToList();

            //Remove first and last point from the trace
            if (points.Count > 0)
            {
                points.RemoveAt(points.Count - 1);
            }
            if (points.Count > 0)
            {
                points.RemoveAt(0);
            }

            foreach(var point in points)
            {

                try
                {
                    if (!this.localGameMap[point.X, point.Y, 0].IsSeeThrough)
                    {
                        return false;
                    }
                }
                catch
                {
                    return false; //underflow or overflow
                }
            }

            return true;

        }

    }
}
