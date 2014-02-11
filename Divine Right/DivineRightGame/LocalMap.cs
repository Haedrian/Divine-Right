﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Enums;
using DRObjects.ActorHandling.ActorMissions;
using DivineRightGame.Pathfinding;

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
                        PathfindingMap[i, j] = localGameMap[i, j, 0] != null ? localGameMap[i, j, 0].MayContainItems ? (byte)1 : (byte)150 : (byte)150;
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
        /// Updates the pathfinding map for that particular coordinate
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void UpdatePathfindingMap(int x, int y)
        {
            PathfindingMap[x, y] = localGameMap[x, y, 0].MayContainItems ? (byte)1 : (byte)150;
        }

        /// <summary>
        /// Perform a tick. Checks all actors and allow them an action
        /// </summary>
        public void Tick()
        {
            MapCoordinate playerLocation = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault().MapCharacter.Coordinate;

            foreach (Actor actor in actors.Where(a => !a.IsPlayerCharacter))
            {
                //First check whether they have a mission
                if (actor.CurrentMission == null)
                {
                    //Try to pop a new one
                    actor.CurrentMission = actor.MissionStack.Pop();

                    if (actor.CurrentMission == null)
                    {
                        //Just idle then
                        actor.CurrentMission = new IdleMission();
                    }

                }

                //Update the graphic
                switch (actor.CurrentMission.MissionType)
                {
                    case DRObjects.ActorHandling.ActorMissionType.ATTACK:
                        (actor.MapCharacter as LocalEnemy).EnemyThought = EnemyThought.ATTACK;
                        break;
                    case DRObjects.ActorHandling.ActorMissionType.IDLE:
                        (actor.MapCharacter as LocalEnemy).EnemyThought = EnemyThought.WAIT;
                        break;
                    case DRObjects.ActorHandling.ActorMissionType.PATROL:
                        (actor.MapCharacter as LocalEnemy).EnemyThought = EnemyThought.WALK;
                        break;
                    case DRObjects.ActorHandling.ActorMissionType.HUNTDOWN:
                        (actor.MapCharacter as LocalEnemy).EnemyThought = EnemyThought.ATTACK;
                        break;
                    default:
                        throw new NotImplementedException("No graphic exists for mission type " + actor.CurrentMission.MissionType);
                }

                if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.PATROL)
                {
                    PatrolMission mission = actor.CurrentMission as PatrolMission;

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
                    else if (Math.Abs(actor.MapCharacter.Coordinate - mission.PatrolPoint) > mission.PatrolRange)
                    {
                        //Send him back. TODO later. For now idle like a drooling idiot
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
                        if (this.GetBlockAtCoordinate(newCoord).MayContainItems && Math.Abs(newCoord - mission.PatrolPoint) < mission.PatrolRange)
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
                    //TODO - ACTUAL HUNT DOWN
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

                    //Otherwise, check whether the target is still where he was before, and whether we have anywhere to go to
                    if (mission.Coordinates == null || mission.Target.MapCharacter.Coordinate != mission.TargetCoordinate)
                    {
                        //Regenerate the path
                        GeneratePathfindingMap();
                        mission.TargetCoordinate = mission.Target.MapCharacter.Coordinate;
                        mission.Coordinates = PathfinderInterface.GetPath(actor.MapCharacter.Coordinate, mission.TargetCoordinate);
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


            }
        }

        #endregion

    }
}
