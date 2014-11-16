using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.LocalMapGeneratorObjects;
using DivineRightGame.Managers.HelperObjects.HelperEnums;
using DivineRightGame.Managers.HelperObjects;
using DRObjects.Enums;
using DRObjects.ActorHandling.ActorMissions;
using Microsoft.Xna.Framework;
using DRObjects.ActorHandling;
using DivineRightGame.ActorHandling;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.LocalMapGeneratorObjects.Enums;
using DRObjects.Items.Archetypes.Local;
using DivineRightGame.ItemFactory.ItemFactoryManagers;
using DRObjects.DataStructures;

namespace DivineRightGame.LocalMapGenerator
{
    public class LocalMapGenerator
    {
        private Random random = new Random(DateTime.UtcNow.Millisecond);

        /// <summary>
        /// Generates a map based on the maplet assigned
        /// </summary>
        /// <param name="maplet">The maplet to generate</param>
        /// <param name="parentWallID">The wall that the parent has</param>
        /// <param name="parentTileID">The ID of the tiles used in the parent maplet item</param>
        /// <param name="enemyType">The type of actor which is dominant in this map</param>
        /// <param name="owner">The owner of the map. Any maplet items which don't belong will be hidden</param>
        /// <param name="actors">The actors which we have generated</param>
        /// <returns></returns>
        public MapBlock[,] GenerateMap(int parentTileID, int? parentWallID, Maplet maplet, bool preferSides, string actorType, OwningFactions owner, 
            out Actor[] actors,out MapletActorWanderArea[] wAreas,out MapletPatrolPoint[] patrolRoutes)
        {
            List<Actor> actorList = new List<Actor>();
            List<MapletActorWanderArea> wanderAreas = new List<MapletActorWanderArea>();
            List<MapletPatrolPoint> patrolRouteList = new List<MapletPatrolPoint>();

            PlanningMapItemType[,] planningMap = new PlanningMapItemType[maplet.SizeX, maplet.SizeY];

            //Step 1: Plan how the map will look

            //Step 1a: Set all tiles to available, and set the frame to walls if there's a wall
            planningMap = this.CreateBlueprint(maplet);

            //Step 1b: Put in the tiles in the actual map, and the walls if they are present
            MapBlock[,] generatedMap = new MapBlock[maplet.SizeX, maplet.SizeY];
            ItemFactory.ItemFactory factory = new ItemFactory.ItemFactory();

            int tileID = 0;

            if (maplet.Tiled)
            {
                if (maplet.TileID.HasValue)
                {
                    tileID = maplet.TileID.Value;
                }
                else
                {
                    //Load the tileID from the factory
                    factory.CreateItem(Archetype.TILES, maplet.TileTag, out tileID);
                }

            }
            else
            {
                tileID = parentTileID;
            }

            //That's the tiles done
            for (int x = 0; x < generatedMap.GetLength(0); x++)
            {
                for (int y = 0; y < generatedMap.GetLength(1); y++)
                {
                    MapItem tile = factory.CreateItem("tile", tileID);
                    tile.Coordinate = new MapCoordinate(x, y, 0, DRObjects.Enums.MapType.LOCAL);

                    MapBlock block = new MapBlock();
                    block.Tile = tile;

                    generatedMap[x, y] = block;
                }
            }

            //Do the walls now if they are required
            int? wallID = null;
            int tempWallID = -1;

            if (parentWallID.HasValue)
            {
                MapItem wall = factory.CreateItem("MUNDANEITEMS", parentWallID.Value);
                wallID = parentWallID;
            }
            else
            {
                MapItem wall = factory.CreateItem(DRObjects.Enums.Archetype.MUNDANEITEMS, "wall", out tempWallID);
                wallID = tempWallID;
            }

            if (maplet.Walled && wallID.HasValue)
            {
                //wall the edge tiles
                for (int x = 0; x < maplet.SizeX; x++)
                {
                    generatedMap[x, 0].PutItemOnBlock(factory.CreateItem("mundaneitems", wallID.Value));
                    generatedMap[x, maplet.SizeY - 1].PutItemOnBlock(factory.CreateItem("mundaneitems", wallID.Value));

                    if (maplet.WindowProbability.HasValue && maplet.WindowProbability.Value > 0)
                    {
                        if (random.Next(100) < maplet.WindowProbability.Value)
                        {
                            int itemID;
                            //Put a window :)
                            generatedMap[x, 0].ForcePutItemOnBlock(factory.CreateItem(DRObjects.Enums.Archetype.MUNDANEITEMS, "window", out itemID));
                        }
                        else if (random.Next(100) < maplet.WindowProbability.Value)
                        {
                            int itemID;
                            //Put a window :)
                            generatedMap[x, maplet.SizeY - 1].ForcePutItemOnBlock(factory.CreateItem(DRObjects.Enums.Archetype.MUNDANEITEMS, "window", out itemID));
                        }
                    }
                }

                for (int y = 0; y < maplet.SizeY; y++)
                {
                    generatedMap[0, y].PutItemOnBlock(factory.CreateItem("mundaneitems", wallID.Value));
                    generatedMap[maplet.SizeX - 1, y].PutItemOnBlock(factory.CreateItem("mundaneitems", wallID.Value));

                    if (maplet.WindowProbability.HasValue && maplet.WindowProbability.Value > 0)
                    {
                        if (random.Next(100) < maplet.WindowProbability.Value)
                        {
                            int itemID;
                            //Put a window :)
                            generatedMap[0, y].ForcePutItemOnBlock(factory.CreateItem(DRObjects.Enums.Archetype.MUNDANEITEMS, "window", out itemID));
                        }
                        else if (random.Next(100) < maplet.WindowProbability.Value)
                        {
                            int itemID;
                            //Put a window :)
                            generatedMap[maplet.SizeX - 1, y].ForcePutItemOnBlock(factory.CreateItem(DRObjects.Enums.Archetype.MUNDANEITEMS, "window", out itemID));
                        }
                    }
                }



            }

            //Step 1c: Determine where we'll put the maplets
            foreach (MapletContentsMaplet childMaplet in maplet.MapletContents.Where(mc => (mc is MapletContentsMaplet)).OrderByDescending(mc => mc.ProbabilityPercentage).ThenBy(mc => random.Next()))
            {
                //Calculate the probability of putting the item in, and how many items we're putting
                for (int i = 0; i < childMaplet.MaxAmount; i++)
                {
                    if (random.NextDouble() * 100 <= childMaplet.ProbabilityPercentage)
                    {
                        //Does it fit?
                        int x = -1;
                        int y = -1;
                        PlanningMapItemType[,] newMap;

                        //Convert the child maplet into a planning map
                        PlanningMapItemType[,] childMapletBlueprint = this.CreateBlueprint(childMaplet.Maplet);
                        //mark the areas covered by the blueprint as being held by that blueprint

                        if (childMaplet.Position == PositionAffinity.FIXED)
                        {
                            if (Fits(planningMap, childMapletBlueprint, childMaplet.x.Value, childMaplet.y.Value, out newMap))
                            {
                                //it fits, generate it - <3 Recursion
                                Actor[] childActors = null;

                                MapletActorWanderArea[] wanderA = null;
                                MapletPatrolPoint[] patrolPoints = null;

                                MapBlock[,] childMap = this.GenerateMap(tileID, wallID.Value, childMaplet.Maplet, childMaplet.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES, actorType,owner, out childActors,out wanderA,out patrolPoints);

                                //Add the child actors
                                actorList.AddRange(childActors);

                                //Update any actors's locations should they have any
                                foreach (var actor in childActors)
                                {
                                    if (actor.MissionStack.Count() > 0)
                                    {
                                        var wander = actor.MissionStack.Peek() as WanderMission;

                                        if (wander != null)
                                        {
                                            wander.WanderPoint.X += childMaplet.x.Value;
                                            wander.WanderPoint.Y += childMaplet.y.Value;

                                            wander.WanderRectangle = new Rectangle(wander.WanderRectangle.X + childMaplet.x.Value, wander.WanderRectangle.Y + childMaplet.y.Value, wander.WanderRectangle.Width, wander.WanderRectangle.Height);
                                        }

                                    }
                                }

                                //Update any wander areas too
                                foreach(var area in wanderA)
                                {
                                    area.WanderRect = new Rectangle(area.WanderRect.X + childMaplet.x.Value, area.WanderRect.Y + childMaplet.y.Value, area.WanderRect.Width, area.WanderRect.Height);
                                    area.WanderPoint.X += childMaplet.x.Value;
                                    area.WanderPoint.Y += childMaplet.y.Value;
                                }

                                //and patrol points
                                foreach(var point in patrolPoints)
                                {
                                    point.Point.X += childMaplet.x.Value;
                                    point.Point.Y += childMaplet.y.Value;
                                }

                                //And add them
                                wanderAreas.AddRange(wanderA);
                                patrolRouteList.AddRange(patrolPoints);

                                //Join the two maps together
                                generatedMap = this.JoinMaps(generatedMap, childMap, childMaplet.x.Value, childMaplet.y.Value);
                            }
                        }
                        else
                        {
                            if (Fits(planningMap, childMapletBlueprint, childMaplet.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES, childMaplet.FirstFit, childMaplet.Padding, out x, out y, out newMap))
                            {
                                //it fits, generate it - <3 Recursion
                                Actor[] childActors = null;
                                MapletActorWanderArea[] wanderA = null;
                                MapletPatrolPoint[] patrolPoints = null;

                                MapBlock[,] childMap = this.GenerateMap(tileID, wallID.Value, childMaplet.Maplet, childMaplet.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES, actorType,owner, out childActors,out wanderA,out patrolPoints);

                                //Add the child actors
                                actorList.AddRange(childActors);

                                //Update any actors's locations should they have any
                                foreach (var actor in childActors)
                                {
                                    if (actor.MissionStack.Count() > 0)
                                    {
                                        var wander = actor.MissionStack.Peek() as WanderMission;

                                        if (wander != null)
                                        {
                                            wander.WanderPoint.X += x;
                                            wander.WanderPoint.Y += y;

                                            wander.WanderRectangle = new Rectangle(wander.WanderRectangle.X + x, wander.WanderRectangle.Y + y, wander.WanderRectangle.Width, wander.WanderRectangle.Height);
                                        }

                                    }
                                }

                                //Update any wander areas too
                                foreach (var area in wanderA)
                                {
                                    area.WanderRect = new Rectangle(area.WanderRect.X + x, area.WanderRect.Y + y, area.WanderRect.Width, area.WanderRect.Height);
                                    area.WanderPoint.X += x;
                                    area.WanderPoint.Y += y;
                                }

                                //and patrol routes
                                foreach(var point in patrolPoints)
                                {
                                    point.Point.X += x;
                                    point.Point.Y += y;
                                }

                                //And add them
                                wanderAreas.AddRange(wanderA);
                                patrolRouteList.AddRange(patrolPoints);

                                //Join the two maps together
                                generatedMap = this.JoinMaps(generatedMap, childMap, x, y);
                            }
                        }
                    }
                }
            }

            //Step 2: Put the items into the map

            //Lets list places we can put it in
            List<MapBlock> candidateBlocks = new List<MapBlock>();

            for (int x = 0; x < planningMap.GetLength(0); x++)
            {
                for (int y = 0; y < planningMap.GetLength(1); y++)
                {
                    if (planningMap[x, y] == PlanningMapItemType.FREE)
                    {
                        candidateBlocks.Add(generatedMap[x, y]);
                    }

                }
            }
            List<MapBlock> edgeBlocks = new List<MapBlock>();

            //Lets also get the edge mapblocks - for those who prefer being on the edge
            for (int x = 0; x < planningMap.GetLength(0); x++)
            {
                if (!maplet.Walled)
                {
                    if (planningMap[x, 0] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[x, 0]);
                    }

                    if (planningMap[x, planningMap.GetLength(1) - 1] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[x, planningMap.GetLength(1) - 1]);
                    }
                }
                else
                {
                    if (planningMap[x, 1] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[x, 1]);
                    }

                    if (planningMap[x, planningMap.GetLength(1) - 2] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[x, planningMap.GetLength(1) - 2]);
                    }
                }

            }

            //Doing the y parts
            for (int y = 0; y < planningMap.GetLength(1); y++)
            {
                if (!maplet.Walled)
                {
                    if (planningMap[0, y] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[0, y]);
                    }

                    if (planningMap[planningMap.GetLength(0) - 1, y] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[planningMap.GetLength(0) - 1, y]);
                    }
                }
                else
                {
                    if (planningMap[1, y] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[1, y]);
                    }

                    if (planningMap[planningMap.GetLength(0) - 2, y] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[planningMap.GetLength(0) - 2, y]);
                    }

                }


            }

            //go through the maplet contents

            //Get the smallest x and y coordinate in the candidate blocks so we can use it for fixed things

            int smallestX = -1;
            int smallestY = -1;

            try
            {
                smallestX = candidateBlocks.Select(b => b.Tile.Coordinate.X).Min();
                smallestY = candidateBlocks.Select(b => b.Tile.Coordinate.Y).Min();
            }
            catch
            {
                //No space :(

            }

            foreach (MapletContents contents in maplet.MapletContents.Where(mc => mc is MapletContentsItem || mc is MapletContentsItemTag).OrderByDescending(mc => mc.ProbabilityPercentage))
            {
                //We'll see if we even put this at all
                MapItem itemPlaced = null;

                for (int i = 0; i < contents.MaxAmount; i++)
                {
                    //lets see what the probability of putting it in is
                    if ((random.NextDouble() * 100) <= contents.ProbabilityPercentage)
                    {
                        //Put it in
                        if (contents is MapletContentsItem)
                        {
                            MapletContentsItem mapletContent = (MapletContentsItem)contents;
                            itemPlaced = factory.CreateItem(mapletContent.ItemCategory, mapletContent.ItemID);
                        }
                        if (contents is MapletContentsItemTag)
                        {
                            MapletContentsItemTag mapletContent = (MapletContentsItemTag)contents;
                            int tempInt;
                            itemPlaced = factory.CreateItem(mapletContent.Category, mapletContent.Tag, out tempInt);
                            //I CHANGED THIS
                            itemPlaced.OwnedBy = mapletContent.Factions;
                        }

                        if (candidateBlocks.Count != 0)
                        {
                            //Lets decide where to put it

                            if (contents.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES && edgeBlocks.Count != 0)
                            {
                                //pick a place at random and add it to the maplet
                                int position = random.Next(edgeBlocks.Count);

                                edgeBlocks[position].PutItemOnBlock(itemPlaced);

                                if (!contents.AllowItemsOnTop)
                                {
                                    //remove it from both
                                    candidateBlocks.Remove(edgeBlocks[position]);
                                    edgeBlocks.RemoveAt(position);
                                }
                            }

                            if (contents.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.MIDDLE && candidateBlocks.Except(edgeBlocks).Count() != 0)
                            {
                                //pick a place at random and add it to the maplet
                                int position = random.Next(candidateBlocks.Except(edgeBlocks).Count());

                                MapBlock block = candidateBlocks.Except(edgeBlocks).ToArray()[position];

                                block.PutItemOnBlock(itemPlaced);

                                if (!contents.AllowItemsOnTop)
                                {
                                    //remove it from both
                                    candidateBlocks.Remove(block);
                                    edgeBlocks.Remove(block);
                                }
                            }

                            if (contents.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.ANYWHERE)
                            {
                                //pick a place at random and add it to the maplet
                                int position = random.Next(candidateBlocks.Count);

                                candidateBlocks[position].PutItemOnBlock(itemPlaced);

                                if (!contents.AllowItemsOnTop)
                                {
                                    //remove it from both
                                    edgeBlocks.Remove(candidateBlocks[position]);
                                    candidateBlocks.RemoveAt(position);
                                }
                            }

                            if (contents.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.FIXED)
                            {
                                //Fix it in a particular position.
                                MapCoordinate coordinate = new MapCoordinate(smallestX + contents.x.Value, smallestY + contents.y.Value, 0, DRObjects.Enums.MapType.LOCAL);

                                var selectedBlock = candidateBlocks.Where(cb => cb.Tile.Coordinate.Equals(coordinate)).FirstOrDefault();

                                if (selectedBlock != null)
                                { //maybe someone put something there already
                                    selectedBlock.PutItemOnBlock(itemPlaced);
                                }

                                if (!contents.AllowItemsOnTop)
                                {
                                    //and remoev it from both
                                    candidateBlocks.Remove(selectedBlock);
                                    edgeBlocks.Remove(selectedBlock);
                                }
                            }
                        }
                    }
                }

            }

            //Step 3: Stripe through the map except for the current maplet's walls - work out where the walls are, and for each wall segment, put a door in
            #region Wall Segments
            List<Line> wallSegments = new List<Line>();

            for (int x = 1; x < planningMap.GetLength(0) - 1; x++)
            {
                //lets see if we find a wall Segment
                Line wallSegment = null;

                for (int y = 1; y < planningMap.GetLength(1) - 1; y++)
                {
                    if (planningMap[x, y] == PlanningMapItemType.WALL)
                    {
                        //Three possibilities exist. Either this is the start of a wall segment
                        //Or this is a continuation of a wall segment
                        //Or this is the end of a wall segment
                        // -> Because there is an intersection
                        // -> Because there was an active wall segment and there is no wall in this one
                        if (wallSegment == null)
                        {
                            //Its a start
                            wallSegment = new Line(new MapCoordinate(x, y, 0, DRObjects.Enums.MapType.LOCAL), null);
                        }
                        else
                        {
                            //Continuation or end
                            //Check if there's an interesection
                            //Go up one and down one. If there is the maplet's walls there won't be a door - but then there'll be a double wall anyway which makes no sense
                            if (planningMap[x + 1, y] == PlanningMapItemType.WALL || planningMap[x - 1, y] == PlanningMapItemType.WALL)
                            {
                                //terminate the wall - and start a new one
                                wallSegment.End = new MapCoordinate(x, y, 0, DRObjects.Enums.MapType.LOCAL);
                                wallSegments.Add(wallSegment);

                                wallSegment = new Line(new MapCoordinate(x, y, 0, DRObjects.Enums.MapType.LOCAL), null);
                            }
                            else
                            {
                                //do nothing, its a continuation
                            }
                        }

                    }
                    else
                    {
                        //Mayhaps a line has stopped?
                        if (wallSegment != null)
                        {
                            //It has - lets terminate it
                            wallSegment.End = new MapCoordinate(x, y - 1, 0, DRObjects.Enums.MapType.LOCAL);
                            wallSegments.Add(wallSegment);

                            wallSegment = null;
                        }

                    }


                }

                //Check if there's an active line - maybe it reaches till the end of the maplet
                if (wallSegment != null)
                {
                    wallSegment.End = new MapCoordinate(x, (planningMap.GetLength(1) - 1), 0, DRObjects.Enums.MapType.LOCAL);
                    wallSegments.Add(wallSegment);
                    wallSegment = null;
                }
            }

            //Now stripe in the other direction
            for (int y = 1; y < planningMap.GetLength(1) - 1; y++)
            {
                //lets see if we find a wall Segment
                Line wallSegment = null;

                for (int x = 1; x < planningMap.GetLength(0) - 1; x++)
                {
                    if (planningMap[x, y] == PlanningMapItemType.WALL)
                    {
                        //Three possibilities exist. Either this is the start of a wall segment
                        //Or this is a continuation of a wall segment
                        //Or this is the end of a wall segment
                        // -> Because there is an intersection
                        // -> Because there was an active wall segment and there is no wall in this one
                        if (wallSegment == null)
                        {
                            //Its a start
                            wallSegment = new Line(new MapCoordinate(x, y, 0, DRObjects.Enums.MapType.LOCAL), null);
                        }
                        else
                        {
                            //Continuation or end
                            //Check if there's an interesection
                            //Go up one and down one. If there is the maplet's walls there won't be a door - but then there'll be a double wall anyway which makes no sense
                            if (planningMap[x, y + 1] == PlanningMapItemType.WALL || planningMap[x, y - 1] == PlanningMapItemType.WALL)
                            {
                                //terminate the wall - and start a new one
                                wallSegment.End = new MapCoordinate(x, y, 0, DRObjects.Enums.MapType.LOCAL);
                                wallSegments.Add(wallSegment);

                                wallSegment = new Line(new MapCoordinate(x, y, 0, DRObjects.Enums.MapType.LOCAL), null);
                            }
                            else
                            {
                                //do nothing, its a continuation
                            }
                        }

                    }
                    else
                    {
                        //Mayhaps a line has stopped?
                        if (wallSegment != null)
                        {
                            //It has - lets terminate it
                            wallSegment.End = new MapCoordinate(x - 1, y, 0, DRObjects.Enums.MapType.LOCAL);
                            wallSegments.Add(wallSegment);

                            wallSegment = null;
                        }

                    }


                }

                //Check if there's an active line - maybe it reaches till the end of the maplet
                if (wallSegment != null)
                {
                    wallSegment.End = new MapCoordinate(planningMap.GetLength(0) - 1, y, 0, DRObjects.Enums.MapType.LOCAL);
                    wallSegments.Add(wallSegment);
                    wallSegment = null;
                }
            }

            #endregion Wall Segments

            #region Doors

            //Get all wall segments larger than 0, and we can put a door there

            foreach (Line segment in wallSegments.Where(ws => ws.Length() > 1))
            {
                //Put a door somewhere, as long as its not the start or end
                //Oh and remove the wall

                MapBlock block = null;

                if (segment.Start.X == segment.End.X)
                {
                    //Get the entirety of the segment
                    List<int> possibleYs = new List<int>();

                    int smallerY = Math.Min(segment.Start.Y, segment.End.Y);
                    int largerY = Math.Max(segment.Start.Y, segment.End.Y);

                    //Check in the real map whether the tile next to it is free for walking in
                    for (int y = smallerY + 1; y <= largerY; y++)
                    {
                        if (generatedMap[segment.Start.X - 1, y].MayContainItems && generatedMap[segment.Start.X + 1, y].MayContainItems)
                        {
                            possibleYs.Add(y);
                        }
                    }

                    //Now check whether there's a possible y, and pick a random one from it
                    if (possibleYs.Count != 0)
                    {
                        block = generatedMap[segment.Start.X, possibleYs[random.Next(possibleYs.Count - 1)]];
                    }
                    else
                    {
                        //nothing to do - take smallest
                        block = generatedMap[segment.Start.X, segment.Start.Y + 1];
                    }
                }
                else
                {
                    List<int> possibleXs = new List<int>();

                    int smallerX = Math.Min(segment.Start.X, segment.End.X);
                    int largerX = Math.Max(segment.Start.X, segment.End.X);

                    //Check in the real map whether the tile next to it is free for walking in
                    for (int x = smallerX + 1; x <= largerX; x++)
                    {
                        if (generatedMap[x, segment.Start.Y - 1].MayContainItems && generatedMap[x, segment.Start.Y + 1].MayContainItems)
                        {
                            possibleXs.Add(x);
                        }
                    }

                    //Now check whether there's a possible x, and pick a random one from it
                    if (possibleXs.Count != 0)
                    {
                        block = generatedMap[possibleXs[random.Next(possibleXs.Count - 1)], segment.Start.Y];
                    }
                    else
                    {
                        //nothing to do - take the smallest one
                        block = generatedMap[segment.Start.X + 1, segment.Start.Y];
                    }


                }

                try
                {
                    if (block != null)
                    {
                        block.RemoveTopItem();
                        int doorID = -1;
                        block.PutItemOnBlock(factory.CreateItem(DRObjects.Enums.Archetype.MUNDANEITEMS, "door", out doorID));
                    }
                }
                catch { }
            }

            #endregion

            #region Enemies

            //Now lets create enemies :)
            foreach (var mc in maplet.MapletContents.Where(mc => mc.GetType().Equals(typeof(MapletActor))).OrderByDescending(o => o.ProbabilityPercentage))
            {
                var actor = mc as MapletActor;

                for (int i = 0; i < actor.MaxAmount; i++)
                {
                    //Check the random
                    if (random.Next(100) < actor.ProbabilityPercentage)
                    {
                        int actorID = 0;

                        string enemyType = actor.UseLocalType ? actorType : actor.EnemyType;

                        //For now set gear cost to 0
                        Actor newActor = ActorGeneration.CreateActor(enemyType, actor.EnemyTag, null, 10, 0, null, out actorID);

                        if (actor.VendorType.HasValue)
                        {
                            GenerateVendor(newActor, actor);
                        }

                        //Generate the map character
                        var mapCharacter = factory.CreateItem("enemies", actorID);

                        newActor.MapCharacter = mapCharacter;

                        if (!actor.Factions.HasFlag(owner))
                        {
                            //inactive character
                            newActor.MapCharacter.IsActive = false;
                        }
                        
                        (mapCharacter as LocalCharacter).Actor = newActor;

                        //Lets position them randomly
                        for (int attempt = 0; attempt < 150; attempt++)
                        {
                            //Try 150 times
                            int x = random.Next(maplet.SizeX);
                            int y = random.Next(maplet.SizeY);

                            if (generatedMap[x, y].MayContainItems)
                            {
                                //Put it there
                                mapCharacter.Coordinate = new MapCoordinate(x, y, 0, MapType.LOCAL);
                                generatedMap[x, y].ForcePutItemOnBlock(mapCharacter);
                                actorList.Add(newActor);

                                //What mission does he have?
                                if (actor.EnemyMission == ActorMissionType.WANDER)
                                {
                                    newActor.MissionStack.Push(new WanderMission()
                                        {
                                            LoiterPercentage = 80,
                                            WanderPoint = new MapCoordinate(mapCharacter.Coordinate),
                                            WanderRectangle = new Rectangle(0, 0, generatedMap.GetLength(0), generatedMap.GetLength(1))
                                        });
                                }

                                break;
                            }
                        }
                    }
                }
            }

            #endregion

            #region Wander Areas

            foreach(var mc in maplet.MapletContents.Where(mc => mc.GetType().Equals(typeof(MapletActorWanderArea))))
            {
                var wander = mc as MapletActorWanderArea;

                //The area of this is going to be the entire maplet (so if we're in a submaplet, they'll wander in there - Awesome no ?)
                wander.WanderRect = new Rectangle(0, 0, generatedMap.GetLength(0), generatedMap.GetLength(1));

                //Pick the wander point to be the middle of the rectangle. If the point isn't valid we might have a problem
                wander.WanderPoint = new MapCoordinate(generatedMap.GetLength(0) / 2, generatedMap.GetLength(1) / 2,0,MapType.LOCAL);

                wanderAreas.Add(wander);
            }

            wAreas = wanderAreas.ToArray();

            #endregion

            #region Patrol Points

            foreach(var mc in maplet.MapletContents.Where(mc => mc.GetType().Equals(typeof(MapletPatrolPoint))))
            {
                var point = mc as MapletPatrolPoint;

                //The point is going to be in the middle of the entire maplet
                point.Point = new MapCoordinate(generatedMap.GetLength(0) / 2, generatedMap.GetLength(1) / 2, 0, MapType.LOCAL);

                patrolRouteList.Add(point);
            }

            patrolRoutes = patrolRouteList.ToArray();

            #endregion

            #region Aniamls

            //Now lets create enemies :)
            foreach (var mc in maplet.MapletContents.Where(mc => mc.GetType().Equals(typeof(MapletHerd))).OrderByDescending(o => o.ProbabilityPercentage))
            {
                var herd = mc as MapletHerd;

                for (int i = 0; i < herd.MaxAmount; i++)
                {
                    //Check the random
                    if (random.Next(100) < herd.ProbabilityPercentage)
                    {
                        var herds = ActorGeneration.CreateAnimalHerds(herd.Biome, herd.Domesticated, 1);

                        foreach (var animalHerd in herds)
                        {
                            foreach (var animal in animalHerd)
                            {
                                //Position them on the map
                                for (int attempt = 0; attempt < 150; attempt++)
                                {
                                    //Try 150 times
                                    int x = random.Next(maplet.SizeX);
                                    int y = random.Next(maplet.SizeY);

                                    if (generatedMap[x, y].MayContainItems)
                                    {
                                        //Put it there
                                        animal.MapCharacter.Coordinate = new MapCoordinate(x, y, 0, MapType.LOCAL);
                                        generatedMap[x, y].ForcePutItemOnBlock(animal.MapCharacter);
                                        actorList.Add(animal);

                                        //Wander around does he have?
                                        animal.MissionStack.Push(new WanderMission()
                                        {
                                            LoiterPercentage = 80,
                                            WanderPoint = new MapCoordinate(animal.MapCharacter.Coordinate),
                                            WanderRectangle = new Rectangle(0, 0, generatedMap.GetLength(0), generatedMap.GetLength(1))
                                        });


                                        break;
                                    }
                                }
                            }
                        }


                    }
                }
            }

            #endregion

            actors = actorList.ToArray();

            #region Ownership

            //Go through all map items - If they're not valid for this particular owner, make them inactive.
            foreach(var mapBlock in generatedMap)
            {
                foreach(var item in mapBlock.GetItems())
                {
                    if (!item.OwnedBy.HasFlag(owner))
                    {
                        item.IsActive = false;
                    }
                }
            }

            #endregion


            //we're done
            return generatedMap;
        }

        /// <summary>
        /// Generates a vendor, adds the vendor details to the actor being created
        /// </summary>
        /// <param name="newActor"></param>
        /// <param name="actor"></param>
        public void GenerateVendor(Actor newActor, MapletActor actor)
        {
            newActor.VendorDetails = new VendorDetails();
            newActor.VendorDetails.VendorType = actor.VendorType.Value;
            newActor.VendorDetails.VendorLevel = actor.VendorLevel ?? 1;
            newActor.VendorDetails.GenerationTime = new DivineRightDateTime(GameState.UniverseTime);

            //Generate the stuff
            InventoryItemManager iim = new InventoryItemManager();

            int maxCategorySize = 1000 * newActor.VendorDetails.VendorLevel;

            newActor.VendorDetails.Stock = new GroupedList<InventoryItem>();

            switch (newActor.VendorDetails.VendorType)
            {
                case VendorType.GENERAL:
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.SUPPLY.ToString(), (int) (maxCategorySize * 0.75)))
                    {
                        inv.InInventory = true;
                        newActor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.LOOT.ToString(), (int)(maxCategorySize * 0.75)))
                    {
                        inv.InInventory = true;
                        newActor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.ARMOUR.ToString(), (int)(maxCategorySize * 0.75)))
                    {
                        inv.InInventory = true;
                        newActor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.WEAPON.ToString(), (int) (maxCategorySize * 0.75)))
                    {
                        inv.InInventory = true;
                        newActor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    break;
                case VendorType.SMITH:
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.ARMOUR.ToString(), (int)(maxCategorySize * 1.5)))
                    {
                        inv.InInventory = true;
                        newActor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.WEAPON.ToString(), (int)(maxCategorySize * 1.5)))
                    {
                        inv.InInventory = true;
                        newActor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    break;
                case VendorType.TRADER:
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.LOOT.ToString(), maxCategorySize * 3))
                    {
                        inv.InInventory = true;
                        newActor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    break;
                case VendorType.TAVERN:
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.SUPPLY.ToString(), maxCategorySize))
                    {
                        inv.InInventory = true;
                        newActor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    break;
            }

            newActor.VendorDetails.Money = 1000;
        }

        /// <summary>
        /// Updates the Vendor's stock to brand new stock
        /// </summary>
        /// <param name="actor"></param>
        public void UpdateVendorStock(Actor actor)
        {
            //First see how much money they have to buy stuff with.
            //Money to buy stuff = SUM (Base Values of old stock Inventory Items) + Money on Hand - 1000

            int totalMoney = actor.VendorDetails.Stock.GetAllObjects().Sum(s => s.BaseValue) + actor.VendorDetails.Money - 1000;

            //Generate the stuff
            InventoryItemManager iim = new InventoryItemManager();

            actor.VendorDetails.Stock = new GroupedList<InventoryItem>();
            actor.VendorDetails.GenerationTime = new DivineRightDateTime(GameState.UniverseTime);

            switch (actor.VendorDetails.VendorType)
            {
                case VendorType.GENERAL:
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.ARMOUR.ToString(), totalMoney / 4))
                    {
                        inv.InInventory = true;
                        actor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.LOOT.ToString(), totalMoney / 4))
                    {
                        inv.InInventory = true;
                        actor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.WEAPON.ToString(), totalMoney / 4))
                    {
                        inv.InInventory = true;
                        actor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.SUPPLY.ToString(), totalMoney / 4))
                    {
                        inv.InInventory = true;
                        actor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    break;
                case VendorType.SMITH:
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.ARMOUR.ToString(), totalMoney / 2))
                    {
                        inv.InInventory = true;
                        actor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.WEAPON.ToString(), totalMoney / 2))
                    {
                        inv.InInventory = true;
                        actor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    break;
                case VendorType.TRADER:
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.LOOT.ToString(), totalMoney))
                    {
                        inv.InInventory = true;
                        actor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    break;
                case VendorType.TAVERN:
                    foreach (InventoryItem inv in iim.GetItemsWithAMaxValue(InventoryCategory.SUPPLY.ToString(), totalMoney))
                    {
                        inv.InInventory = true;
                        actor.VendorDetails.Stock.Add(inv.Category, inv);
                    }
                    break;
            }

            actor.VendorDetails.Money = 1000;

        }

        /// <summary>
        /// Generates enemies on the map.
        /// </summary>
        /// <param name="enemyCount"></param>
        /// <param name="enemyType"></param>
        /// <returns></returns>
        public MapBlock[,] GenerateEnemies(MapBlock[,] blocks, int enemyCount, string enemyType, out DRObjects.Actor[] actors, int level, int equipmentCost = 0)
        {
            ItemFactory.ItemFactory fact = new ItemFactory.ItemFactory();
            List<Actor> actorList = new List<Actor>();

            //We'll just pick blocks at random until we fail 50 times in a row or run out of enemies to place
            int failureCount = 0;

            for (int i = 0; i < enemyCount; i++)
            {
                if (failureCount == 50)
                {
                    break;
                }

                int x = random.Next(blocks.GetLength(0));
                int y = random.Next(blocks.GetLength(1));

                //Is the block free
                if (blocks[x, y].MayContainItems)
                {
                    int returnedID = -1;
                    //Put the enemy in there

                    //Get the basic Actor object

                    double multiplier = GameState.Random.Next(75, 125);

                    multiplier /= 100; //So we get a number between 0.75 and 1.25

                    Actor actor = ActorGeneration.CreateActor(enemyType, null, null, (int)(level * multiplier), (int)(equipmentCost * multiplier), null, out returnedID);

                    var mapObject = fact.CreateItem("enemies", returnedID);

                    (mapObject as LocalCharacter).Actor = actor;

                    //Create the Actor
                    actor.GlobalCoordinates = null; //useless for now
                    actor.IsPlayerCharacter = false;
                    actor.MapCharacter = mapObject;

                    actorList.Add(actor);

                    blocks[x, y].ForcePutItemOnBlock(mapObject);

                    int missionType = random.Next(3);

                    if (missionType == 0)
                    {
                        //33% of them will Wander - 
                        WanderMission mission = new WanderMission();
                        mission.WanderPoint = new MapCoordinate(x, y, 0, MapType.LOCAL);
                        mission.WanderRectangle = new Rectangle(0, 0, blocks.GetLength(0), blocks.GetLength(1));

                        actor.MissionStack.Push(mission);
                    }
                    else if (missionType == 1)
                    {
                        //33% will idle
                        IdleMission mission = new IdleMission();
                        actor.MissionStack.Push(mission);
                    }
                    else
                    {
                        //The rest will patrol
                        PatrolMission mission = new PatrolMission();
                        //Don't give them the point. We;ll choose it later
                        actor.MissionStack.Push(mission);
                    }



                    failureCount = 0;//reset
                }
                else
                {
                    failureCount++;
                    i--; //And again
                }
            }

            //return the map
            actors = actorList.ToArray();
            return blocks;

        }

        #region Helper Functions

        /// <summary>
        /// Determines whether a maplet will fit into a map, and if it does, where it fits
        /// </summary>
        /// <param name="map">The original map</param>
        /// <param name="maplet">The maplet we are trying to introduce</param>
        /// <param name="edge">If this is set to true, will try to put them on the edge. Otherwise will try to be compeltly random</param>
        /// <param name="firstFit">If this is set to true, and edge is set to true, it will try to fit the object in the first place it can find</param>
        /// <param name="startX">Where the map will fit on the X coordinate</param>
        /// <param name="startY">Where the map will fit on the Y coordinate</param>
        /// <param name="newMap">What the map will look like with the maplet inside it. Will be equivalent to map if there is no fit</param>
        /// <returns></returns>
        public bool Fits(PlanningMapItemType[,] map, PlanningMapItemType[,] maplet, bool edge, bool firstFit, int? padding, out int startX, out int startY, out PlanningMapItemType[,] newMap)
        {
            if (!padding.HasValue)
            {
                padding = 0;
            }
            if (edge && firstFit)
            {
                //Brute force, nothing to be done
                for (int mapX = 0; mapX < (map.GetLength(0)); mapX++)
                {
                    for (int mapY = 0; mapY < (map.GetLength(1)); mapY++)
                    {
                        //Do we have a starting point?
                        if (map[mapX, mapY] == PlanningMapItemType.FREE || (map[mapX, mapY] == PlanningMapItemType.WALL && maplet[0, 0] == PlanningMapItemType.WALL))
                        {
                            //Does it fit?
                            bool fits = true;

                            //Now that we have a starting point, check if we have a padding. If we have a padding, then we need to incremement x and y by the padding amount, and continue the loop
                            if (padding > 0)
                            {
                                mapX += padding.Value - 1;
                                mapY += padding.Value - 1;
                                //continue;

                                if (mapX >= map.GetLength(0))
                                {
                                    break;
                                }
                            }

                            for (int mapletX = 0; mapletX < maplet.GetLength(0); mapletX++)
                            {
                                for (int mapletY = 0; mapletY < maplet.GetLength(1); mapletY++)
                                {
                                    int mapTotalX = mapletX + mapX;
                                    int mapTotalY = mapletY + mapY;

                                    if (mapTotalX < map.GetLength(0) && mapTotalY < map.GetLength(1))
                                    {
                                        /* The allowable fits are:
                                         *  - Map is free and maplet is anything
                                         *  - Map is wall and maplet is wall
                                         */

                                        //will it fit
                                        if (map[mapTotalX, mapTotalY] == PlanningMapItemType.FREE)
                                        {
                                            continue;
                                        }

                                        if (map[mapTotalX, mapTotalY] == PlanningMapItemType.WALL)
                                        {
                                            if (maplet[mapletX, mapletY] == PlanningMapItemType.WALL)
                                            {
                                                continue;
                                            }
                                        }

                                        if (map[mapTotalX, mapTotalY] != PlanningMapItemType.FREE)
                                        {
                                            fits = false;
                                        }
                                    }
                                    else
                                    {
                                        fits = false;
                                    }

                                }
                            }

                            if (fits && maplet[0, 0] == PlanningMapItemType.WALL && !NoDoubleWall(map, maplet, mapX, mapY))
                            {
                                fits = false;
                            }

                            if (fits)
                            {
                                //it fits, yaay
                                startX = mapX;
                                startY = mapY;

                                //overlap the map

                                newMap = FuseMaps(map, maplet, startX, startY);

                                return true;
                            }
                        }
                    }
                }
            }
            else if (!edge)
            {
                //Lets try for a maximum 150 times
                int attemptCount = 0;

                while (attemptCount < 150)
                {
                    attemptCount++;

                    int mapX = random.Next(map.GetLength(0));
                    int mapY = random.Next(map.GetLength(1));

                    //Do we have a starting point?
                    if (map[mapX, mapY] == PlanningMapItemType.FREE || (map[mapX, mapY] == PlanningMapItemType.WALL && maplet[0, 0] == PlanningMapItemType.WALL))
                    {
                        //Does it fit?
                        bool fits = true;

                        for (int mapletX = 0; mapletX < maplet.GetLength(0); mapletX++)
                        {
                            for (int mapletY = 0; mapletY < maplet.GetLength(1); mapletY++)
                            {
                                int mapTotalX = mapletX + mapX;
                                int mapTotalY = mapletY + mapY;

                                if (mapTotalX < map.GetLength(0) && mapTotalY < map.GetLength(1))
                                {
                                    /* The allowable fits are:
                                     *  - Map is free and maplet is anything
                                     *  - Map is wall and maplet is wall
                                     */

                                    //will it fit
                                    if (map[mapTotalX, mapTotalY] == PlanningMapItemType.FREE)
                                    {
                                        continue;
                                    }

                                    if (map[mapTotalX, mapTotalY] == PlanningMapItemType.WALL)
                                    {
                                        if (maplet[mapletX, mapletY] == PlanningMapItemType.WALL)
                                        {
                                            continue;
                                        }
                                    }

                                    if (map[mapTotalX, mapTotalY] != PlanningMapItemType.FREE)
                                    {
                                        fits = false;
                                    }
                                }
                                else
                                {
                                    fits = false;
                                }

                            }
                        }

                        if (fits && maplet[0, 0] == PlanningMapItemType.WALL && !NoDoubleWall(map, maplet, mapX, mapY))
                        {
                            fits = false;
                        }

                        if (fits)
                        {
                            //it fits, yaay
                            startX = mapX;
                            startY = mapY;

                            //overlap the map

                            newMap = FuseMaps(map, maplet, startX, startY);

                            return true;
                        }
                    }
                }
            }
            else if (edge && !firstFit)
            {
                List<Tuple<int, int>> edgeBlocks = new List<Tuple<int, int>>();

                //Get all the edge blocks
                for (int x = 0; x < map.GetLength(0); x++)
                {

                    edgeBlocks.Add(new Tuple<int, int>(x, 0));
                    edgeBlocks.Add(new Tuple<int, int>(x, map.GetLength(1) - 1));
                }

                //Doing the y parts
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    edgeBlocks.Add(new Tuple<int, int>(0, y));
                    edgeBlocks.Add(new Tuple<int, int>(map.GetLength(0) - 1, y));
                }

                //Okay, now we randomly choose a block and see if it fits
                //Lets try for a maximum 150 times
                int attemptCount = 0;

                while (attemptCount < 150)
                {
                    if (edgeBlocks.Count == 0)
                    {
                        //Nothing to do
                        break;
                    }

                    attemptCount++;

                    var tuple = edgeBlocks[random.Next(edgeBlocks.Count - 1)];

                    int mapX = tuple.Item1;
                    int mapY = tuple.Item2;

                    edgeBlocks.Remove(tuple);

                    //Do we have a starting point?
                    if (map[mapX, mapY] == PlanningMapItemType.FREE || (map[mapX, mapY] == PlanningMapItemType.WALL && maplet[0, 0] == PlanningMapItemType.WALL))
                    {
                        //Does it fit?
                        bool fits = true;

                        for (int mapletX = 0; mapletX < maplet.GetLength(0); mapletX++)
                        {
                            for (int mapletY = 0; mapletY < maplet.GetLength(1); mapletY++)
                            {
                                int mapTotalX = mapletX + mapX;
                                int mapTotalY = mapletY + mapY;

                                if (mapTotalX < map.GetLength(0) && mapTotalY < map.GetLength(1))
                                {
                                    /* The allowable fits are:
                                     *  - Map is free and maplet is anything
                                     *  - Map is wall and maplet is wall
                                     */

                                    //will it fit
                                    if (map[mapTotalX, mapTotalY] == PlanningMapItemType.FREE)
                                    {
                                        continue;
                                    }

                                    if (map[mapTotalX, mapTotalY] == PlanningMapItemType.WALL)
                                    {
                                        if (maplet[mapletX, mapletY] == PlanningMapItemType.WALL)
                                        {
                                            continue;
                                        }
                                    }

                                    if (map[mapTotalX, mapTotalY] != PlanningMapItemType.FREE)
                                    {
                                        fits = false;
                                    }
                                }
                                else
                                {
                                    fits = false;
                                }

                            }
                        }

                        if (fits && maplet[0, 0] == PlanningMapItemType.WALL && !NoDoubleWall(map, maplet, mapX, mapY))
                        {
                            fits = false;
                        }

                        if (fits)
                        {
                            //it fits, yaay
                            startX = mapX;
                            startY = mapY;

                            //overlap the map

                            newMap = FuseMaps(map, maplet, startX, startY);

                            return true;
                        }
                    }
                }



            }

            //no fit
            startX = -1;
            startY = -1;
            newMap = map;
            return false;
        }

        /// <summary>
        /// Checks whether this positioning would create a double wall
        /// </summary>
        /// <param name="map"></param>
        /// <param name="maplet"></param>
        /// <param name="mapX"></param>
        /// <param name="mapY"></param>
        /// <returns></returns>
        private bool NoDoubleWall(PlanningMapItemType[,] map, PlanningMapItemType[,] maplet, int mapX, int mapY)
        {
            if (maplet[0, 0] != PlanningMapItemType.WALL)
            {
                return true; //No Wall. So no double wall
            }

            //Otherwise, lets ensure that its never the case that for any X & Y (which are not the corners) - there are no double walls ever

            //Don't underflow
            for (int y = 1; y < maplet.GetLength(1) - 1; y++)
            {
                //Start with Left line
                if (mapX != 0)
                {
                    if (map[mapX - 1, mapY + y] == PlanningMapItemType.WALL)
                    {
                        return false; //Double wall
                    }
                }

                //Also check the right line
                if (mapX + maplet.GetLength(0) < map.GetLength(0))
                {
                    if (map[mapX + maplet.GetLength(0), mapY + y] == PlanningMapItemType.WALL)
                    {
                        return false;
                    }
                }

            }

            //Still here? Lets try the top and bottom
            for (int x = 1; x < maplet.GetLength(0) - 1; x++)
            {
                if (mapY != 0)
                {
                    //Top line
                    if (map[mapX + x, mapY - 1] == PlanningMapItemType.WALL)
                    {
                        return false;
                    }
                }

                //Bottom line
                if (mapY + maplet.GetLength(1) < map.GetLength(1))
                {
                    if (map[mapX + x, mapY + maplet.GetLength(1)] == PlanningMapItemType.WALL)
                    {
                        return false;
                    }
                }

            }

            return true;
        }

        /// <summary>
        /// Determines whether a particular maplet with a fixed x and y coordiate will fit, and places it on the updated planning map if it does
        /// ONLY USE FOR ABSOLUTE POSITIONING
        /// </summary>
        /// <param name="map"></param>
        /// <param name="maplet"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="newMap"></param>
        /// <returns></returns>
        public bool Fits(PlanningMapItemType[,] map, PlanningMapItemType[,] maplet, int x, int y, out PlanningMapItemType[,] newMap)
        {
            newMap = FuseMaps(map, maplet, x, y);

            return true;
        }

        /// <summary>
        /// Fuses the two maps together such that the maplet will reserve space for itself in the blueprint. This function assumes that the maps will fit and does not perform any checking.
        /// It is greatly recommended that you use Fits to invoke this function
        /// </summary>
        /// <param name="map">The base map blueprint</param>
        /// <param name="maplet">The maplet blueprint</param>
        /// <param name="startX">Where on the x coordinate the maplet will begin</param>
        /// <param name="startY">Where on the Y coordinate the maplet will begin</param>
        /// <returns>A blueprint of the map with the maplet inside it</returns>
        public PlanningMapItemType[,] FuseMaps(PlanningMapItemType[,] map, PlanningMapItemType[,] maplet, int startX, int startY)
        {
            for (int x = 0; x < maplet.GetLength(0); x++)
            {
                for (int y = 0; y < maplet.GetLength(1); y++)
                {
                    if (maplet[x, y] == PlanningMapItemType.WALL)
                    {
                        map[startX + x, startY + y] = maplet[x, y];
                    }
                    else
                    {
                        //mark it as taken
                        map[startX + x, startY + y] = PlanningMapItemType.MAPLET;
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// Creates a Blueprint from a maplet
        /// </summary>
        /// <param name="maplet"></param>
        /// <returns></returns>
        public PlanningMapItemType[,] CreateBlueprint(Maplet maplet)
        {
            PlanningMapItemType[,] blueprint = new PlanningMapItemType[maplet.SizeX, maplet.SizeY];

            for (int x = 0; x < maplet.SizeX; x++)
            {
                for (int y = 0; y < maplet.SizeY; y++)
                {
                    blueprint[x, y] = PlanningMapItemType.FREE;
                }
            }

            if (maplet.Walled)
            {
                //set edge tiles to wall
                for (int x = 0; x < maplet.SizeX; x++)
                {
                    blueprint[x, 0] = PlanningMapItemType.WALL;
                    blueprint[x, maplet.SizeY - 1] = PlanningMapItemType.WALL;
                }

                for (int y = 0; y < maplet.SizeY; y++)
                {
                    blueprint[0, y] = PlanningMapItemType.WALL;
                    blueprint[maplet.SizeX - 1, y] = PlanningMapItemType.WALL;
                }
            }

            return blueprint;
        }

        /// <summary>
        /// Places the contents of the items in the maplet into the base map. Tiles must already have been set at this point, as this will only place mapitems.
        /// </summary>
        /// <param name="baseMap">The Base Map</param>
        /// <param name="maplet">The maplet items to place</param>
        /// <param name="startX">The x coordinate on the basemap where to start</param>
        /// <param name="startY">The y coordinate on the basemap where to start</param>
        /// <returns></returns>
        public MapBlock[,] JoinMaps(MapBlock[,] baseMap, MapBlock[,] maplet, int startX, int startY)
        {
            for (int x = 0; x < maplet.GetLength(0); x++)
            {
                for (int y = 0; y < maplet.GetLength(1); y++)
                {
                    MapBlock block = baseMap[x + startX, y + startY];
                    MapBlock mapletBlock = maplet[x, y];

                    if (block.GetItems().Length == 0)
                    {
                        //This means there is nothing on this block, so lets replace it
                        //The only time when this will be true is if there is a shared wall, in which case we'll keep the parent wall type
                        baseMap[x + startX, y + startY] = mapletBlock;
                        //Fix the base coordinate
                        baseMap[x + startX, y + startY].Tile.Coordinate = new MapCoordinate(x + startX, y + startY, 0, DRObjects.Enums.MapType.LOCAL);

                        //also fix any items on the tile
                        foreach (MapItem item in baseMap[x + startX, y + startY].GetItems())
                        {
                            item.Coordinate = new MapCoordinate(x + startX, y + startY, 0, DRObjects.Enums.MapType.LOCAL);
                        }
                    }

                }
            }

            return baseMap;
        }

        #endregion
    }
}
