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
using DRObjects.ActorHandling;
using DRObjects.Enums;
using Microsoft.Xna.Framework;

namespace DivineRightGame.ActorHandling
{
    /// <summary>
    /// For handling the actions an actor performs
    /// </summary>
    public static class ActorAIManager
    {
        public static IEnumerable<ActionFeedback> PerformActions(Actor actor, IEnumerable<Actor> actors, MapCoordinate playerLocation)
        {
            if (GameState.LocalMap.PathfindingMap == null)
            {
                GameState.LocalMap.GeneratePathfindingMap();
            }

            List<ActionFeedback> feedback = new List<ActionFeedback>();

            //First check whether they have a mission
            while (actor.CurrentMission == null)
            {
                //Try to pop a new one 
                if (actor.MissionStack.Count() == 0)
                {
                    //no mission. ah well
                    return new ActionFeedback[] { };
                }

                actor.CurrentMission = actor.MissionStack.Pop();

                if (actor.CurrentMission == null)
                {
                    //Just idle then
                    actor.CurrentMission = new IdleMission();
                }

                if (!actor.CurrentMission.IsRetainable)
                {
                    //Discard!
                    actor.CurrentMission = null; //grab another
                }

            }

            //Update the graphic
            if (actor.IsProne)
            {
                (actor.MapCharacter as LocalCharacter).EnemyThought = EnemyThought.PRONE;
            }
            else
            {
                (actor.MapCharacter as LocalCharacter).EnemyThought = actor.CurrentMission.EnemyThought;
            }

            if (actor.IsStunned) //Do nothing
            {
                return new ActionFeedback[] { };
            }

            if (actor.IsProne)
            {
                //Can we get up?
                if (GameState.LocalMap.GetBlockAtCoordinate(actor.MapCharacter.Coordinate).MayContainItems)
                {
                    //Yes we can
                    actor.IsProne = false;
                }
                else
                {
                    return new ActionFeedback[] { }; //Do nothing
                }
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

                    return new ActionFeedback[] { };
                }
            }
            else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.WAIT)
            {
                return WaitMission(actor.CurrentMission as WaitMission, actor);
            }
            else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.WANDER)
            {
                return WanderMission(actor.CurrentMission as WanderMission, actor);
            }
            else if (actor.IsAggressive && actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.ATTACK)
            {
                return AttackMission(actor.CurrentMission as AttackMission, actor);
            }
            else if (actor.IsAggressive && actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.HUNTDOWN)
            {
                return HuntDownMission(actor.CurrentMission as HuntDownMission, actor);
            }
            else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.WALKTO)
            {
                return WalkToMission(actor.CurrentMission as WalkToMission, actor);
            }
            else if (actor.CurrentMission.MissionType == DRObjects.ActorHandling.ActorMissionType.PATROL)
            {
                return PatrolMission(actor.CurrentMission as PatrolMission, actor);
            }
            else if (actor.CurrentMission.MissionType == ActorMissionType.PATROL_ROUTE)
            {
                return PatrolRouteMission(actor.CurrentMission as PatrolRouteMission, actor);
            }

            return feedback;
        }

        public static ActionFeedback[] WaitMission(WaitMission mission, Actor actor)
        {
            mission.Wait();

            if (mission.WaitTime <= 0)
            {
                //Done waiting
                actor.CurrentMission = null;
            }

            return new ActionFeedback[] { };
        }

        public static ActionFeedback[] WanderMission(WanderMission mission, Actor actor)
        {
            MapCoordinate playerLocation = GameState.PlayerCharacter.MapCharacter.Coordinate;

            if (actor.UsesRanged)
            {
                //Is he a ranged user and seeing the player character
                if (actor.IsAggressive && actor.MapCharacter.Coordinate - playerLocation <= actor.LineOfSight)
                {
                    //Attack!
                    actor.MissionStack.Push(actor.CurrentMission);

                    actor.CurrentMission = new AttackMission(GameState.PlayerCharacter);
                }

            }
            else
            {
                //Is he seeing the player character? 
                if (actor.IsAggressive && Math.Abs(actor.MapCharacter.Coordinate - playerLocation) <= 1)
                {
                    //He's there. Push the current mission into the stack and go on the attack
                    actor.MissionStack.Push(actor.CurrentMission);
                    actor.CurrentMission = new AttackMission(GameState.PlayerCharacter);
                    return new ActionFeedback[] { };
                }
                else if (actor.IsAggressive && Math.Abs(actor.MapCharacter.Coordinate - playerLocation) < actor.LineOfSight)
                {
                    //He's there. Push the current mission into the stack and follow him
                    actor.MissionStack.Push(actor.CurrentMission);
                    actor.CurrentMission = new HuntDownMission()
                    {
                        Target = GameState.PlayerCharacter,
                        TargetCoordinate = GameState.PlayerCharacter.MapCharacter.Coordinate
                    };

                    return new ActionFeedback[] { };
                }
            }

            //Perform an action accordingly
            //Is he outside of the patrol area?
            if (!mission.WanderRectangle.Contains(actor.MapCharacter.Coordinate.X, actor.MapCharacter.Coordinate.Y))
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

            return new ActionFeedback[] { };
        }

        public static ActionFeedback[] AttackMission(AttackMission mission, Actor actor)
        {
            List<ActionFeedback> feedback = new List<ActionFeedback>();

            if (Math.Abs(mission.AttackTarget.MapCharacter.Coordinate - actor.MapCharacter.Coordinate) > actor.LineOfSight)
            {
                //Ran away - cancel the mission
                actor.CurrentMission = null;
            }
            else 
            {
                if (actor.UsesRanged)
                {
                    //Attack!
                    return CombatManager.Attack(actor, mission.AttackTarget, CombatManager.GetRandomAttackLocation(actor, mission.AttackTarget).Value);
                }
                else 
                {
                    if (Math.Abs(mission.AttackTarget.MapCharacter.Coordinate - actor.MapCharacter.Coordinate) > 1)
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
                            return CombatManager.Attack(actor, mission.AttackTarget, CombatManager.GetRandomAttackLocation(actor, mission.AttackTarget).Value);

                        }
                        else
                        {
                            //TODO: Run away!
                        }

                    }
                }
            }

            return new ActionFeedback[] { };
        }

        public static ActionFeedback[] HuntDownMission(HuntDownMission mission, Actor actor)
        {
            if (actor.UsesRanged)
            {
                //Are we within range?
                if (actor.MapCharacter.Coordinate - mission.Target.MapCharacter.Coordinate <= actor.LineOfSight)
                {
                    //Attack!
                    actor.CurrentMission = new AttackMission(mission.Target);
                    return new ActionFeedback[] { };
                }
            }
            else
            {
                //Are we near the mission point?
                if (Math.Abs(actor.MapCharacter.Coordinate - mission.Target.MapCharacter.Coordinate) <= 1)
                {
                    //MIssion is done. Attack!
                    actor.CurrentMission = new AttackMission(mission.Target);
                }
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
                mission.TargetCoordinate = mission.Target.MapCharacter.Coordinate;
                mission.Coordinates = PathfinderInterface.GetPath(actor.MapCharacter.Coordinate, mission.TargetCoordinate);
            }

            //Okay, now...advance
            if (mission.Coordinates == null || mission.Coordinates.Count == 0)
            {
                //Where did he go? Take a turn to figure it out
                mission.Coordinates = null;
                return new ActionFeedback[] { };
            }

            MapCoordinate nextStep = mission.Coordinates.Pop();

            //Do it
            if (GameState.LocalMap.GetBlockAtCoordinate(nextStep).MayContainItems)
            {
                GameState.LocalMap.GetBlockAtCoordinate(nextStep).PutItemOnBlock(actor.MapCharacter);
                actor.MapCharacter.Coordinate = nextStep;
            }
            //And that's done

            return new ActionFeedback[] { };
        }

        public static ActionFeedback[] WalkToMission(WalkToMission mission, Actor actor)
        {
            if (actor.IsAggressive && Math.Abs(actor.MapCharacter.Coordinate - GameState.PlayerCharacter.MapCharacter.Coordinate) < actor.LineOfSight)
            {
                if (actor.UsesRanged)
                {
                    //We can see him! Attack!
                    actor.CurrentMission = new AttackMission(GameState.PlayerCharacter);
                }
                else
                {
                    //Can we see the character?
                    //He's there. Push the current mission into the stack and follow him
                    actor.MissionStack.Push(actor.CurrentMission);
                    actor.CurrentMission = new HuntDownMission()
                    {
                        Target = GameState.PlayerCharacter,
                        TargetCoordinate = GameState.PlayerCharacter.MapCharacter.Coordinate
                    };
                }
                return new ActionFeedback[] { };
            }

            //Are we near the mission point?
            if (mission.TargetCoordinate == null || Math.Abs(actor.MapCharacter.Coordinate - mission.TargetCoordinate) <= mission.AcceptableRadius)
            {
                //MIssion is done.
                actor.CurrentMission = null;
                return new ActionFeedback[] { };
            }

            if (mission.Coordinates == null)
            {
                mission.Coordinates = PathfinderInterface.GetPath(actor.MapCharacter.Coordinate, mission.TargetCoordinate);

                //Is the new first coordinate valid?
                if (mission.Coordinates == null)
                {
                    //No path
                    actor.CurrentMission = null; //lose the mission

                    //Wander around where you are now
                    actor.CurrentMission = new WanderMission()
                    {
                        WanderPoint = new MapCoordinate(actor.MapCharacter.Coordinate),
                        WanderRectangle = new Rectangle(actor.MapCharacter.Coordinate.X - 5, actor.MapCharacter.Coordinate.Y - 5, 10, 10)
                    };

                    return new ActionFeedback[] { };
                }
            }

            if (!GameState.LocalMap.GetBlockAtCoordinate(mission.Coordinates.Peek()).MayContainItems)
            {
                //Is what is blocking us another actor? Force him to go prone
                var nonProneActors = GameState.LocalMap.GetBlockAtCoordinate(mission.Coordinates.Peek()).GetItems().Where(gi => gi.IsActive && gi.GetType().Equals(typeof(LocalCharacter))
                    && GameState.LocalMap.Actors.Any(a => a.MapCharacter == gi && a.Owners == actor.Owners && !a.IsProne));

                if (nonProneActors.Count() > 0)
                {
                    //Push them
                    foreach (var npa in nonProneActors)
                    {
                        GameState.LocalMap.Actors.First(a => a.MapCharacter == npa).IsProne = true;
                    }
                }

                //Can we walk now?
                if (!GameState.LocalMap.GetBlockAtCoordinate(mission.Coordinates.Peek()).MayContainItems)
                {
                    //Invalid path
                    Console.WriteLine("Invalid Path");

                    ////Increase the failure count
                    //mission.PathfindingFailureCount++;

                    ////Are we up to 5?
                    //if (mission.PathfindingFailureCount > 5)
                    //{
                    //    //Then just stop.
                        
                    //}

                    //lose the mission
                    actor.CurrentMission = new WaitMission(2); //wait for 2 turns
                    //Regenerate the path 
                    GameState.LocalMap.GeneratePathfindingMap();

                    return new ActionFeedback[0] { };
                }
            }

            //We can walk normally 

            //Walk
            //Okay, now...advance
            if (mission.Coordinates.Count == 0)
            {
                //Where did he go? Take a turn to figure it out
                mission.Coordinates = null;
                return new ActionFeedback[] { };
            }

            MapCoordinate nextStep = mission.Coordinates.Pop();

            //Are we close enough to move?
            if (Math.Abs(nextStep - actor.MapCharacter.Coordinate) > 1)
            {
                //We moved off the path. Regenerate it
                mission.Coordinates = null;
                return new ActionFeedback[] { };
            }

            //Do it
            if (GameState.LocalMap.GetBlockAtCoordinate(nextStep).MayContainItems)
            {
                GameState.LocalMap.GetBlockAtCoordinate(nextStep).PutItemOnBlock(actor.MapCharacter);
                actor.MapCharacter.Coordinate = nextStep;
            }

            return new ActionFeedback[] { };
        }

        public static ActionFeedback[] PatrolMission(PatrolMission mission, Actor actor)
        {
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

            return new ActionFeedback[] { };
        }

        public static ActionFeedback[] PatrolRouteMission(PatrolRouteMission mission, Actor actor)
        {
            //Push it back on the stack
            actor.MissionStack.Push(mission);

            var nextPoint = mission.GetNextPoint();

            //Create a move to mission which moves the actor to the next point
            WalkToMission wTM = new WalkToMission()
            {
                TargetCoordinate = nextPoint.Coordinate,
                AcceptableRadius = nextPoint.AcceptableRadius
            };

            actor.CurrentMission = wTM;

            //Log it
            Console.WriteLine("Actor " + actor.ToString() + " is going to" + wTM.TargetCoordinate.ToString() + " as part of his patrol route");

            return new ActionFeedback[] { };
        }
    }
}
