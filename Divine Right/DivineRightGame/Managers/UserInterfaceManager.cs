﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects;
using DRObjects;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;
using System.Diagnostics;
using DivineRightGame.CombatHandling;
using Microsoft.Xna.Framework;
using DRObjects.Graphics;
using DRObjects.ActorHandling.SpecialAttacks;
using DRObjects.Items.Archetypes.Local;

namespace DivineRightGame.Managers
{
    /// <summary>
    /// Used to communicate between the User Interface and the Game State
    /// </summary>
    public static class UserInterfaceManager
    {
        /// <summary>
        /// Performs an action on a particular or a particular item.
        /// If item is not null, will use the item. Otherwise will use the coordinate
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="actionType"></param>
        /// <param name="args"></param>
        public static ActionFeedback[] PerformAction(MapCoordinate coordinate,MapItem item, ActionType actionType, object[] args)
        {
            List<ActionFeedback> feedback = new List<ActionFeedback>();

            bool validAttack = false;

            if (actionType == ActionType.ATTACK)
            {
                //Handle this seperatly
                //Argument 0 - attacker
                //Argument 1 - target
                //Argument 2 - Body part to attack
                //Argument 3 - Special attack if there

                //Are we in the right place?
                Actor attacker = args[0] as Actor;
                Actor defender = args[1] as Actor;
                AttackLocation location = (AttackLocation) args[2];

                int distance = attacker.MapCharacter.Coordinate - defender.MapCharacter.Coordinate;

                if (defender.MapCharacter == null)
                {
                    //Something went wrong
                    validAttack = false;

                }

                if ( distance < 2)
                {
                    //Hand to hand

                    if (args.Length > 3)
                    {
                        //Special attack!
                        feedback.AddRange(CombatManager.PerformSpecialAttack(attacker, defender, args[3] as SpecialAttack));
                    }
                    else
                    {
                        feedback.AddRange(CombatManager.Attack(attacker, defender, location));
                    }
                    validAttack = true; //perform the tick
                }
                else
                {
                    //Is the attacker armed properly?
                    if (attacker.Inventory.EquippedItems.ContainsKey(EquipmentLocation.BOW))
                    {
                        //Do they have line of sight?
                        if (GameState.LocalMap.HasDirectPath(attacker.MapCharacter.Coordinate, defender.MapCharacter.Coordinate))
                        {
                            //Are they within a reasonable distance?
                            if (distance <= attacker.LineOfSight)
                            {
                                //Yes!
                                if (args.Length > 3)
                                {
                                    //Special attack!
                                    feedback.AddRange(CombatManager.PerformSpecialAttack(attacker, defender, args[3] as SpecialAttack));
                                }
                                else
                                {
                                    feedback.AddRange(CombatManager.Attack(attacker, defender, location));
                                }
                                validAttack = true;
                            }
                            else
                            {
                                validAttack = false;
                                return new ActionFeedback[] { new LogFeedback(InterfaceSpriteName.PERC,Color.Black, "This target is outside of your range") };
                            }
                        }
                        else
                        {
                            validAttack = false;
                            return new ActionFeedback[] { new LogFeedback(null, Color.Black, "You don't have a clear line of sight to the target") };
                        }
                        

                    }
                    else
                    {
                        validAttack = false;
                        //Invalid - no tick
                        return new ActionFeedback[] { new LogFeedback(null, Color.Black, "You are too far away to hit your target") };
                    }
                }
            }
            else if (actionType == ActionType.THROW_ITEM)
            {
                //Throw it
                Potion potion = args[2] as Potion;
                Actor attacker = args[0] as Actor;
                List<Actor> victims = args[1] as List<Actor>;

                //Also create a temporary icon to show what was done
                foreach(var victim in victims)
                {
                    TemporaryGraphic tg = new TemporaryGraphic()
                    {
                        Coord = victim.MapCharacter.Coordinate,
                        Graphic = SpriteManager.GetSprite(InterfaceSpriteName.POTION_BREAK),
                        LifeTime = 2
                    };

                    GameState.LocalMap.TemporaryGraphics.Add(tg);
                }

                feedback.AddRange(potion.ThrowUpon(attacker, victims));
            }
            else if (actionType == ActionType.IDLE)
            {
                //Do nothing
            }
            else
            {
                if (item == null)
                {
                    switch (coordinate.MapType)
                    {
                        case MapType.LOCAL:
                            feedback.AddRange(GameState.LocalMap.GetBlockAtCoordinate(coordinate).PerformAction(actionType, GameState.PlayerCharacter, args));
                            break;
                        case MapType.GLOBAL:
                            feedback.AddRange(GameState.GlobalMap.GetBlockAtCoordinate(coordinate).PerformAction(actionType, GameState.PlayerCharacter, args));
                            break;
                        default:
                            throw new NotImplementedException("There is no support for that particular maptype");
                    }
                }
                else
                {
                    //Perform it on the item
                    feedback.AddRange(item.PerformAction(actionType, GameState.PlayerCharacter, args));
                }
            }

            if (actionType == ActionType.EXAMINE || actionType == ActionType.THROW_ITEM || actionType == ActionType.MOVE || (actionType == ActionType.ATTACK && validAttack) || actionType == ActionType.IDLE)
            {
                //Perform a tick
                //Clear the Log
                GameState.NewLog.Clear();
                feedback.AddRange(UserInterfaceManager.PerformLocalTick());
            }

            //Is the player stunned?
            while (GameState.PlayerCharacter.IsStunned && GameState.PlayerCharacter.IsAlive)
            {
                feedback.AddRange(UserInterfaceManager.PerformLocalTick());
            }

            return feedback.ToArray();
        }

        /// <summary>
        /// Performs a local map tick
        /// </summary>
        public static ActionFeedback[] PerformLocalTick()
        {
            Stopwatch sWatch = new Stopwatch();
            sWatch.Start();
            var playerFeedback = GameState.LocalMap.Tick();
            sWatch.Stop();
            Console.WriteLine("Time taken for tick :" + sWatch.ElapsedMilliseconds);

            return playerFeedback;
        }

        /// <summary>
        /// Gets possible actions for a particular block
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        public static ActionType[] GetPossibleActions(MapCoordinate coordinate)
        {
            switch (coordinate.MapType)
            {
                case MapType.LOCAL:
                    return GameState.LocalMap.GetBlockAtCoordinate(coordinate).GetActions(GameState.PlayerCharacter);
                case MapType.GLOBAL:
                    return GameState.GlobalMap.GetBlockAtCoordinate(coordinate).GetActions(GameState.PlayerCharacter);
                default:
                    throw new NotImplementedException("There is no support for that particular maptype");
            }
        }

        /// <summary>
        /// Gets a number of graphical blocks around a particular point.
        /// The order will be as follows:
        /// first max z, then max x, then max y.
        /// </summary>
        /// <param name="centrePoint">The center point around which the tiles will be calculated</param>
        /// <param name="xRange">How many tiles away from the centre point on the x axis will be obtained</param>
        /// <param name="yCount">How many tiles away from the centre point on the y axis will be obtained</param>
        /// <param name="zCount">How many tiles away from the centre point on the y axis will be obtained</param>
        /// <param name="overlay">If its a global map, the overlay to apply on it</param>
        /// <returns></returns>
        public static GraphicalBlock[] GetBlocksAroundPoint(MapCoordinate centrePoint, int xRange, int yRange, int zRange,GlobalOverlay overlay = GlobalOverlay.NONE)
        {
            int minZ = centrePoint.Z - Math.Abs(zRange);
            int maxZ = centrePoint.Z + Math.Abs(zRange);

            int minY = centrePoint.Y - Math.Abs(yRange);
            int maxY = centrePoint.Y + Math.Abs(yRange);

            int minX = centrePoint.X - Math.Abs(xRange);
            int maxX = centrePoint.X + Math.Abs(xRange);

            List<GraphicalBlock> returnList = new List<GraphicalBlock>();

            //go through all of them


            for (int zLoop = maxZ; zLoop >= minZ; zLoop--)
            {
                for (int yLoop = maxY; yLoop >= minY; yLoop--)
                {
                    for (int xLoop = minX; xLoop <= maxX; xLoop++)
                    {
                        MapCoordinate coord = new MapCoordinate(xLoop, yLoop, zLoop, centrePoint.MapType);
                        if (overlay != GlobalOverlay.NONE)
                        {
                            returnList.Add(GetBlockAtPoint(coord, overlay));
                        }
                        else
                        {
                            returnList.Add(GetBlockAtPoint(coord));
                        }
                    }
                }

            }

            return returnList.ToArray();
        }

        /// <summary>
        /// Gets a number of graphical blocks around the player.
        /// The order will be as follows:
        /// first max z, then max x, then max y.
        /// </summary>
        /// <param name="xRange">How many tiles away from the centre point on the x axis will be obtained</param>
        /// <param name="yCount">How many tiles away from the centre point on the y axis will be obtained</param>
        /// <param name="zCount">How many tiles away from the centre point on the y axis will be obtained</param>
        /// <returns></returns>
        public static GraphicalBlock[] GetBlocksAroundPlayer(int xRange, int yRange, int zRange)
        {
                return GetBlocksAroundPoint(GameState.PlayerCharacter.MapCharacter.Coordinate, xRange, yRange, zRange);   
        }

        /// <summary>
        /// Gets a graphical block whcih exists on a particular point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static GraphicalBlock GetBlockAtPoint(MapCoordinate point)
        {
            switch (point.MapType)
            {
                case (DRObjects.Enums.MapType.GLOBAL)
                :
                    try
                    {
                        return GameState.GlobalMap.GetBlockAtCoordinate(point).ConvertToGraphicalBlock();
                    }
                    catch
                    {//send an empty one
                        GraphicalBlock block = new GraphicalBlock();
                        block.MapCoordinate = point;
                        return block;
                    }
                case (DRObjects.Enums.MapType.LOCAL)
                :
                        return GameState.LocalMap.GetBlockAtCoordinate(point).ConvertToGraphicalBlock();
                default:
                        throw new NotImplementedException("There is no map manager for that type");
            }
        }

        /// <summary>
        /// Gets a graphical block whch exists on a particular point, with a global overlay
        /// </summary>
        /// <param name="point"></param>
        /// <param name="globalOverlay"></param>
        /// <returns></returns>
        public static GraphicalBlock GetBlockAtPoint(MapCoordinate point, GlobalOverlay globalOverlay)
        {
            switch (point.MapType)
            {
                case (DRObjects.Enums.MapType.GLOBAL)
                :
                    try
                    {
                        return GameState.GlobalMap.GetBlockAtCoordinate(point).ConvertToGraphicalBlock(globalOverlay);
                    }
                    catch
                    {//send an empty one
                        GraphicalBlock block = new GraphicalBlock();
                        block.MapCoordinate = point;
                        return block;
                    }
                case (DRObjects.Enums.MapType.LOCAL)
                :
                    return GameState.LocalMap.GetBlockAtCoordinate(point).ConvertToGraphicalBlock();
                default:
                    throw new NotImplementedException("There is no map manager for that type");
            }

        }

        /// <summary>
        /// Gets the Actor object which represents the player
        /// </summary>
        /// <returns></returns>
        public static Actor GetPlayerActor()
        {
            return GameState.PlayerCharacter;
        }

    }
}
