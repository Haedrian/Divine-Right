using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.Items.Archetypes.Local;
using DivineRightGame.CombatHandling;
using DivineRightGame.Pathfinding;
using DRObjects.LocalMapGeneratorObjects;

namespace DivineRightGame.ActorHandling
{
    /// <summary>
    /// For handling the actions an actor performs
    /// </summary>
    public static class ActorAIManager
    {
        public static IEnumerable<PlayerFeedback> PerformActions(Actor actor, IEnumerable<Actor> actors, MapCoordinate playerLocation)
        {
            if (GameState.LocalMap.PathfindingMap == null)
            {
                GameState.LocalMap.GeneratePathfindingMap();
            }

            List<PlayerFeedback> feedback = new List<PlayerFeedback>();

            //First check whether they have a mission
            if (actor.CurrentMission == null)
            {
                //Try to pop a new one 
                if (actor.MissionStack.Count() == 0)
                {
                    //no mission. ah well
                    return new PlayerFeedback[] { };
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

            if (actor.IsStunned) //Do nothing
            {
                return new PlayerFeedback[] { };
            }

            if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.IDLE)
            {
                if (actor.IsAggressive && Math.Abs(actor.MapCharacter.Coordinate - playerLocation) < actor.LineOfSight)
                {
                    //He's there. Push the current mission into the stack and follow him
                    actor.MissionStack.Push(actor.CurrentMission);
                    actor.CurrentMission = new HuntDownMission()
                    {
                        Target = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault(),
                        TargetCoordinate = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault().MapCharacter.Coordinate
                    };

                    return new PlayerFeedback[] { };
                }
            }
            else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.WAIT)
            {
                WaitMission wait = actor.CurrentMission as WaitMission;

                wait.Wait();

                if (wait.WaitTime <= 0)
                {
                    //Done waiting
                    actor.CurrentMission = null;
                }
            }
            else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.WANDER)
            {
                WanderMission mission = actor.CurrentMission as WanderMission;

                //Is he seeing the player character?
                if (actor.IsAggressive && Math.Abs(actor.MapCharacter.Coordinate - playerLocation) <= 1)
                {
                    //He's there. Push the current mission into the stack and go on the attack
                    actor.MissionStack.Push(actor.CurrentMission);
                    actor.CurrentMission = new AttackMission(actors.Where(a => a.IsPlayerCharacter).FirstOrDefault());
                    return new PlayerFeedback[] { };
                }
                else if (actor.IsAggressive && Math.Abs(actor.MapCharacter.Coordinate - playerLocation) < actor.LineOfSight)
                {
                    //He's there. Push the current mission into the stack and follow him
                    actor.MissionStack.Push(actor.CurrentMission);
                    actor.CurrentMission = new HuntDownMission()
                    {
                        Target = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault(),
                        TargetCoordinate = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault().MapCharacter.Coordinate
                    };

                    return new PlayerFeedback[] { };
                }

                //Perform an action accordingly
                //Is he outside of the patrol area?
                else if (!mission.WanderRectangle.Contains(actor.MapCharacter.Coordinate.X, actor.MapCharacter.Coordinate.Y))
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
                    int randomNumber = GameState.Random.Next(100);
                    //Do we wander?
                    if (mission.LoiterPercentage != 0 && randomNumber < mission.LoiterPercentage)
                    {
                        //Nope, we loiter
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
                        if (GameState.LocalMap.GetBlockAtCoordinate(newCoord).MayContainItems && mission.WanderRectangle.Contains(newCoord.X, newCoord.Y))
                        {
                            //Do it
                            GameState.LocalMap.GetBlockAtCoordinate(newCoord).PutItemOnBlock(actor.MapCharacter);
                            actor.MapCharacter.Coordinate = newCoord;

                            //And that's done
                        }
                    }
                    //Otherwise do nothing. Stay there
                }
            }
            else if (actor.IsAggressive && actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.ATTACK)
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
                    if (actor.IsAggressive && CombatManager.GetRandomAttackLocation(actor, mission.AttackTarget).HasValue) //we can hit them
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
            else if (actor.IsAggressive && actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.HUNTDOWN)
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
                    //GameState.LocalMap.GeneratePathfindingMap();
                    mission.TargetCoordinate = mission.Target.MapCharacter.Coordinate;
                    mission.Coordinates = PathfinderInterface.GetPath(actor.MapCharacter.Coordinate, mission.TargetCoordinate);
                    return feedback;
                }

                //Okay, now...advance
                if (mission.Coordinates.Count == 0)
                {
                    //Where did he go? Take a turn to figure it out
                    mission.Coordinates = null;
                    return feedback;
                }

                MapCoordinate nextStep = mission.Coordinates.Pop();

                //Do it
                if (GameState.LocalMap.GetBlockAtCoordinate(nextStep).MayContainItems)
                {
                    GameState.LocalMap.GetBlockAtCoordinate(nextStep).PutItemOnBlock(actor.MapCharacter);
                    actor.MapCharacter.Coordinate = nextStep;
                }
                //And that's done

            }
            else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.WALKTO)
            {
                WalkToMission mission = actor.CurrentMission as WalkToMission;

                if (actor.IsAggressive && Math.Abs(actor.MapCharacter.Coordinate - playerLocation) < actor.LineOfSight)
                {
                    //Can we see the character?
                    //He's there. Push the current mission into the stack and follow him
                    actor.MissionStack.Push(actor.CurrentMission);
                    actor.CurrentMission = new HuntDownMission()
                    {
                        Target = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault(),
                        TargetCoordinate = actors.Where(a => a.IsPlayerCharacter).FirstOrDefault().MapCharacter.Coordinate
                    };

                    return feedback;
                }

                //Are we near the mission point?
                if (Math.Abs(actor.MapCharacter.Coordinate - mission.TargetCoordinate) <= 1)
                {
                    //MIssion is done.
                    actor.CurrentMission = null;
                }
                else if (mission.Coordinates == null)
                {
                    mission.Coordinates = PathfinderInterface.GetPath(actor.MapCharacter.Coordinate, mission.TargetCoordinate);

                    //Is the new first coordinate valid?
                    if (mission.Coordinates == null)
                    {
                        //No path
                        actor.CurrentMission = null; //lose the mission
                    }
                    else if (!GameState.LocalMap.GetBlockAtCoordinate(mission.Coordinates.Peek()).MayContainItems)
                    {
                        //Invalid path
                        Console.WriteLine("Invalid Path");
                        //lose the mission
                        actor.CurrentMission = new WaitMission(2); //wait for 2 turns
                        //Regenerate the path 
                        GameState.LocalMap.GeneratePathfindingMap();
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
                        return feedback;
                    }

                    MapCoordinate nextStep = mission.Coordinates.Pop();

                    //Are we close enough to move?
                    if (Math.Abs(nextStep - actor.MapCharacter.Coordinate) > 1)
                    {
                        //We moved off the path. Regenerate it
                        mission.Coordinates = null;
                        return feedback;
                    }

                    //Do it
                    if (GameState.LocalMap.GetBlockAtCoordinate(nextStep).MayContainItems)
                    {
                        GameState.LocalMap.GetBlockAtCoordinate(nextStep).PutItemOnBlock(actor.MapCharacter);
                        actor.MapCharacter.Coordinate = nextStep;
                    }
                    else
                    {
                        //Something changed. Let's regenerate the map and try again
                        mission.Coordinates = null;
                        return feedback;
                    }
                }
            }
            else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.PATROL)
            {
                PatrolMission mission = actor.CurrentMission as PatrolMission;

                //Create a move to mission which moves the actor to the point of interest
                PointOfInterest poi = GameState.LocalMap.PointsOfInterest[GameState.Random.Next(GameState.LocalMap.PointsOfInterest.Count)];

                //Push the patrol on the stack
                actor.MissionStack.Push(mission);

                //Log it
                Console.WriteLine("Actor " + actor.ToString() + " is going to" + poi.Coordinate.ToString());

                actor.CurrentMission = new WalkToMission()
                {
                    TargetCoordinate = poi.Coordinate
                };
            }

            return feedback;
        }

    }
}
