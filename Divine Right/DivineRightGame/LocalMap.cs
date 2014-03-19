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

                //First check whether they have a mission
                if (actor.CurrentMission == null)
                {
                    //Try to pop a new one 
                    if (actor.MissionStack.Peek() == null)
                    {
                        //no mission. ah well
                        continue;
                    }

                    actor.CurrentMission = actor.MissionStack.Pop();

                    if (actor.CurrentMission == null)
                    {
                        //Just idle then
                        actor.CurrentMission = new IdleMission();
                    }

                }

                //Update the graphic
                (actor.MapCharacter as LocalCharacter).EnemyThought = actor.CurrentMission.EnemyThought;

                if (actor.IsStunned)
                {
                    continue; //do nothing
                }

                //if (actor.IsStunned)
                //{
                //    actor.IsStunned = false;
                //    continue;
                //}
                //else
                //{
                //    actor.IsStunned = true;
                //}

                if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.IDLE)
                {
                    if (Math.Abs(actor.MapCharacter.Coordinate - playerLocation) < actor.LineOfSight)
                    {
                        //He's there. Push the current mission into the stack and follow him
                        actor.MissionStack.Push(actor.CurrentMission);
                        actor.CurrentMission = new HuntDownMission()
                        {
                            Target = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault(),
                            TargetCoordinate = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault().MapCharacter.Coordinate
                        };

                        continue;
                    }
                }
                else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.WANDER)
                {
                    WanderMission mission = actor.CurrentMission as WanderMission;

                    //Is he seeing the player character?
                    if (Math.Abs(actor.MapCharacter.Coordinate - playerLocation) <= 1)
                    {
                        //He's there. Push the current mission into the stack and go on the attack
                        actor.MissionStack.Push(actor.CurrentMission);
                        actor.CurrentMission = new AttackMission(actors.Where(a => a.IsPlayerCharacter).FirstOrDefault());
                        continue;
                    }
                    else if (Math.Abs(actor.MapCharacter.Coordinate - playerLocation) < actor.LineOfSight)
                    {
                        //He's there. Push the current mission into the stack and follow him
                        actor.MissionStack.Push(actor.CurrentMission);
                        actor.CurrentMission = new HuntDownMission()
                            {
                                Target = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault(),
                                TargetCoordinate = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault().MapCharacter.Coordinate
                            };

                        continue;
                    }

                    //Perform an action accordingly
                    //Is he outside of the patrol area?
                    else if (!mission.WanderRectangle.Contains(new Point(actor.MapCharacter.Coordinate.X, actor.MapCharacter.Coordinate.Y)))
                    {
                        //Send him back.
                        WalkToMission walkMission = new WalkToMission();
                        walkMission.TargetCoordinate = mission.WanderPoint;

                        //Push it
                        actor.MissionStack.Push(mission);
                        actor.CurrentMission = walkMission;
                    }
                    else
                    {

                        //Walk somewhere randomly
                        int direction = GameState.Random.Next(4);

                        MapCoordinate coord = actor.MapCharacter.Coordinate;
                        //Copy it
                        MapCoordinate newCoord = new MapCoordinate(coord.X, coord.Y, coord.Z, coord.MapType);

                        switch (direction)
                        {
                            case 0: //Top
                                newCoord.Y++;
                                break;
                            case 1: //Bottom
                                newCoord.Y--;
                                break;
                            case 2: //Right
                                newCoord.X++;
                                break;
                            case 3: //Left
                                newCoord.X--;
                                break;
                        }

                        //Can we go there?
                        if (this.GetBlockAtCoordinate(newCoord).MayContainItems && mission.WanderRectangle.Contains(new Point(newCoord.X, newCoord.Y)))
                        {
                            //Do it
                            this.GetBlockAtCoordinate(newCoord).PutItemOnBlock(actor.MapCharacter);
                            actor.MapCharacter.Coordinate = newCoord;

                            //And that's done
                        }
                        //Otherwise do nothing. Stay there
                    }
                }
                else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.ATTACK)
                {
                    //Do they still see the character?
                    AttackMission mission = (actor.CurrentMission as AttackMission);

                    if (Math.Abs(mission.AttackTarget.MapCharacter.Coordinate - actor.MapCharacter.Coordinate) > actor.LineOfSight)
                    {
                        //Ran away - cancel the mission
                        actor.CurrentMission = null;
                    }
                    else if (Math.Abs(mission.AttackTarget.MapCharacter.Coordinate - actor.MapCharacter.Coordinate) > 1)
                    {
                        //Hunt him down!
                        actor.CurrentMission = new HuntDownMission()
                        {
                            Target = mission.AttackTarget,
                            TargetCoordinate = mission.AttackTarget.MapCharacter.Coordinate
                        };
                    }
                    else
                    {
                        if (CombatManager.GetRandomAttackLocation(actor, mission.AttackTarget).HasValue) //we can hit them
                        {
                            //Attack!
                            var logMessages = CombatManager.Attack(actor, mission.AttackTarget, CombatManager.GetRandomAttackLocation(actor, mission.AttackTarget).Value);

                            feedback.AddRange(logMessages);
                            
                        }
                        else
                        {
                            //TODO: Run away!
                        }

                    }
                }
                else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.HUNTDOWN)
                {
                    HuntDownMission mission = (actor.CurrentMission as HuntDownMission);

                    //Are we near the mission point?
                    if (Math.Abs(actor.MapCharacter.Coordinate - mission.Target.MapCharacter.Coordinate) <= 1)
                    {
                        //MIssion is done. Attack!
                        actor.CurrentMission = new AttackMission(mission.Target);
                    }

                    if (Math.Abs(mission.Target.MapCharacter.Coordinate - actor.MapCharacter.Coordinate) > actor.LineOfSight)
                    {
                        //Ran away - cancel the mission
                        actor.CurrentMission = null;
                    }

                    //Otherwise, check whether the target is still where he was before, and whether we have anywhere to go to
                    if (mission.Coordinates == null || mission.Target.MapCharacter.Coordinate != mission.TargetCoordinate)
                    {
                        //Regenerate the path
                        GeneratePathfindingMap();
                        mission.TargetCoordinate = mission.Target.MapCharacter.Coordinate;
                        mission.Coordinates = PathfinderInterface.GetPath(actor.MapCharacter.Coordinate, mission.TargetCoordinate);
                        continue;
                    }

                    //Okay, now...advance
                    if (mission.Coordinates.Count == 0)
                    {
                        //Where did he go? Take a turn to figure it out
                        mission.Coordinates = null;
                        continue;
                    }

                    MapCoordinate nextStep = mission.Coordinates.Pop();

                    //Do it
                    if (this.GetBlockAtCoordinate(nextStep).MayContainItems)
                    {
                        this.GetBlockAtCoordinate(nextStep).PutItemOnBlock(actor.MapCharacter);
                        actor.MapCharacter.Coordinate = nextStep;
                    }
                    //And that's done

                }
                else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.WALKTO)
                {
                    WalkToMission mission = actor.CurrentMission as WalkToMission;

                    if (Math.Abs(actor.MapCharacter.Coordinate - playerLocation) < actor.LineOfSight)
                    {
                        //Can we see the character?
                        //He's there. Push the current mission into the stack and follow him
                        actor.MissionStack.Push(actor.CurrentMission);
                        actor.CurrentMission = new HuntDownMission()
                        {
                            Target = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault(),
                            TargetCoordinate = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault().MapCharacter.Coordinate
                        };

                        continue;
                    }

                    //Are we near the mission point?
                    if (Math.Abs(actor.MapCharacter.Coordinate - mission.TargetCoordinate) <= 1)
                    {
                        //MIssion is done.
                        actor.CurrentMission = null;
                    }
                    else if (mission.Coordinates == null)
                    {
                        //(Re)generate the path
                        GeneratePathfindingMap();
                        mission.Coordinates = PathfinderInterface.GetPath(actor.MapCharacter.Coordinate, mission.TargetCoordinate);

                        //Is the new first coordinate valid?
                        if (mission.Coordinates == null)
                        {
                            //No path
                            actor.CurrentMission = null; //lose the mission
                        }
                        else
                            if (!GetBlockAtCoordinate(mission.Coordinates.Peek()).MayContainItems)
                            {
                                //Invalid path
                                Console.WriteLine("Invalid Path");
                                //lose the mission
                                actor.CurrentMission = null;
                            }
                    }
                    else
                    {
                        //Walk
                        //Okay, now...advance
                        if (mission.Coordinates.Count == 0)
                        {
                            //Where did he go? Take a turn to figure it out
                            mission.Coordinates = null;
                            continue;
                        }

                        MapCoordinate nextStep = mission.Coordinates.Pop();

                        //Are we close enough to move?
                        if (Math.Abs(nextStep - actor.MapCharacter.Coordinate) > 1)
                        {
                            //We moved off the path. Regenerate it
                            mission.Coordinates = null;
                            continue;
                        }

                        //Do it
                        if (this.GetBlockAtCoordinate(nextStep).MayContainItems)
                        {
                            this.GetBlockAtCoordinate(nextStep).PutItemOnBlock(actor.MapCharacter);
                            actor.MapCharacter.Coordinate = nextStep;
                        }
                        else
                        {
                            //Something changed. Let's regenerate the map and try again
                            mission.Coordinates = null;
                            continue;
                        }
                    }
                }
                else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.PATROL)
                {
                    PatrolMission mission = actor.CurrentMission as PatrolMission;

                    //Create a move to mission which moves the actor to the point of interest
                    PointOfInterest poi = this.PointsOfInterest[random.Next(this.PointsOfInterest.Count - 1)];

                    //Push the patrol on the stack
                    actor.MissionStack.Push(mission);

                    //Log it
                    Console.WriteLine("Actor " + actor.ToString() + " is going to" + poi.Coordinate.ToString());

                    actor.CurrentMission = new WalkToMission()
                    {
                        TargetCoordinate = poi.Coordinate
                    };
                }

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
