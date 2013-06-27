using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.LocalMapGeneratorObjects;
using DivineRightGame.MapFactory;
using DivineRightGame.Managers.HelperObjects.HelperEnums;
using DivineRightGame.Managers.HelperObjects;

namespace DivineRightGame.Managers
{
    public class LocalMapGenerator
    {
        /// <summary>
        /// Generates a map based on the maplet assigned
        /// </summary>
        /// <param name="maplet">The maplet to generate</param>
        /// <param name="parentWallID">The wall that the parent has</param>
        /// <param name="parentTileID">The ID of the tiles used in the parent maplet item</param>
        /// <returns></returns>
        public MapBlock[,] GenerateMap(int parentTileID,int? parentWallID, Maplet maplet)
        {
            PlanningMapItemType[,] planningMap = new PlanningMapItemType[maplet.SizeX, maplet.SizeY];
            Random random = new Random(DateTime.UtcNow.Millisecond);

            //Step 1: Plan how the map will look

            //Step 1a: Set all tiles to available, and set the frame to walls if there's a wall
            planningMap = this.CreateBlueprint(maplet);

            //Step 1b: Put in the tiles in the actual map, and the walls if they are present
            MapBlock[,] generatedMap = new MapBlock[maplet.SizeX, maplet.SizeY];
            ItemFactory.ItemFactory factory = new ItemFactory.ItemFactory();

            int tileID = 0;

            if (maplet.Tiled)
            {
                tileID = maplet.TileID;
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
                    tile.Coordinate = new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

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
                    generatedMap[x, 0].PutItemOnBlock(factory.CreateItem("mundaneitems",wallID.Value));
                    generatedMap[x, maplet.SizeY - 1].PutItemOnBlock(factory.CreateItem("mundaneitems",wallID.Value));
                }

                for (int y = 0; y < maplet.SizeY; y++)
                {
                    generatedMap[0, y].PutItemOnBlock(factory.CreateItem("mundaneitems", wallID.Value));
                    generatedMap[maplet.SizeX -1, y].PutItemOnBlock(factory.CreateItem("mundaneitems",wallID.Value));
                }

                //Now we need to see where to put the doors

                //We'll check the planning map, if there is an edge which goes outside, then we put a door there
                
            }

            //Step 1c: Determine where we'll put the maplets
            foreach (MapletContentsMaplet childMaplet in maplet.MapletContents.Where(mc => (mc is MapletContentsMaplet)).OrderByDescending(mc => mc.ProbabilityPercentage))
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


                        if (Fits(planningMap, childMapletBlueprint, out x, out y, out newMap))
                        {
                            //it fits, generate it - <3 Recursion
                            MapBlock[,] childMap = this.GenerateMap(tileID,wallID.Value, childMaplet.Maplet);

                            //Join the two maps together
                            generatedMap = this.JoinMaps(generatedMap, childMap, x, y);
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

            //go through the maplet contents

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
                            itemPlaced = factory.CreateItem(mapletContent.Category,mapletContent.Tag,out tempInt);
                        }

                        if (candidateBlocks.Count != 0)
                        {
                            //pick a place at random and add it to the maplet
                            int position = random.Next(candidateBlocks.Count);

                            candidateBlocks[position].PutItemOnBlock(itemPlaced);

                            //remove the candidate block from the list
                            candidateBlocks.RemoveAt(position);
                        }
                    }
                }

            }

            //Step 3: Stripe through the map except for the current maplet's walls - work out where the walls are, and for each wall segment, put a door in
            #region Wall Segments
            List<Line> wallSegments = new List<Line>();

            for (int x = 1; x < planningMap.GetLength(0)-1; x++)
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
                            wallSegment = new Line(new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL), null);
                        }
                        else
                        {
                            //Continuation or end
                            //Check if there's an interesection
                            //Go up one and down one. If there is the maplet's walls there won't be a door - but then there'll be a double wall anyway which makes no sense
                            if (planningMap[x + 1, y] == PlanningMapItemType.WALL || planningMap[x - 1, y] == PlanningMapItemType.WALL)
                            {
                                //terminate the wall - and start a new one
                                wallSegment.End = new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                                wallSegments.Add(wallSegment);

                                wallSegment = new Line(new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL), null);
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
                            wallSegment.End = new MapCoordinate(x, y - 1, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                            wallSegments.Add(wallSegment);

                            wallSegment = null;
                        }

                    }


                }

                //Check if there's an active line - maybe it reaches till the end of the maplet
                if (wallSegment != null)
                {
                    wallSegment.End = new MapCoordinate(x, (planningMap.GetLength(1) -1), 0, DRObjects.Enums.MapTypeEnum.LOCAL);
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
                            wallSegment = new Line(new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL), null);
                        }
                        else
                        {
                            //Continuation or end
                            //Check if there's an interesection
                            //Go up one and down one. If there is the maplet's walls there won't be a door - but then there'll be a double wall anyway which makes no sense
                            if (planningMap[x, y+1] == PlanningMapItemType.WALL || planningMap[x, y-1] == PlanningMapItemType.WALL)
                            {
                                //terminate the wall - and start a new one
                                wallSegment.End = new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                                wallSegments.Add(wallSegment);

                                wallSegment = new Line(new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL), null);
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
                            wallSegment.End = new MapCoordinate(x -1, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                            wallSegments.Add(wallSegment);

                            wallSegment = null;
                        }

                    }


                }

                //Check if there's an active line - maybe it reaches till the end of the maplet
                if (wallSegment != null)
                {
                    wallSegment.End = new MapCoordinate(planningMap.GetLength(0) -1, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                    wallSegments.Add(wallSegment);
                    wallSegment = null;
                }
            }
            
            #endregion Wall Segments

            #region Doors

            //Get all wall segments larger than 0, and we can put a door there
 
            foreach (Line segment in wallSegments.Where(ws => ws.Length() > 0))
            {
                //Put a door somewhere, as long as its not the start or end
                //Oh and remove the wall

                MapBlock block = null;

                if (segment.Start.X == segment.End.X)
                {
                    //Get the entirety of the segment



                    int smallerY = Math.Min(segment.Start.Y,segment.End.Y);
                    int largerY = Math.Min(segment.Start.Y,segment.End.Y);

                    block = generatedMap[segment.Start.X,smallerY + 1 + random.Next(largerY - smallerY)]; 
                }
                else 
                {
                    int smallerX = Math.Min(segment.Start.X,segment.End.X);
                    int largerX = Math.Min(segment.Start.X,segment.End.X);

                    block = generatedMap[smallerX + 1 + random.Next(largerX - smallerX),segment.Start.Y]; 
                }

                try
                {
                    block.RemoveTopItem();
                }
                catch { }
                int doorID = -1;
                block.PutItemOnBlock(factory.CreateItem(DRObjects.Enums.Archetype.TOGGLEITEMS,"door",out doorID));
            }

            #endregion


            //we're done
            return generatedMap;
        }

        #region Helper Functions

        /// <summary>
        /// Determines whether a maplet will fit into a map, and if it does, where it fits
        /// </summary>
        /// <param name="map">The original map</param>
        /// <param name="maplet">The maplet we are trying to introduce</param>
        /// <param name="startX">Where the map will fit on the X coordinate</param>
        /// <param name="startY">Where the map will fit on the Y coordinate</param>
        /// <param name="newMap">What the map will look like with the maplet inside it. Will be equivalent to map if there is no fit</param>
        /// <returns></returns>
        public bool Fits(PlanningMapItemType[,] map, PlanningMapItemType[,] maplet, out int startX, out int startY, out PlanningMapItemType[,] newMap)
        {
            //Brute force, nothing to be done
            for (int mapX = 0; mapX < map.GetLength(0); mapX++)
            {
                for (int mapY = 0; mapY < map.GetLength(1); mapY++)
                {
                    //Do we have a starting point?
                    if (map[mapX, mapY] == PlanningMapItemType.FREE || (map[mapX,mapY] == PlanningMapItemType.WALL && maplet[0,0] == PlanningMapItemType.WALL))
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

                        if (fits)
                        {
                            //it fits, yaay
                            startX = mapX;
                            startY = mapY;

                            //overlap the map

                            newMap = FuseMaps(map,maplet,startX,startY);

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
                    blueprint[maplet.SizeX-1, y] = PlanningMapItemType.WALL;
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
                        baseMap[x + startX, y+startY] = mapletBlock;
                        //Fix the base coordinate
                        baseMap[x + startX, y + startY].Tile.Coordinate = new MapCoordinate(x + startX, y + startY, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

                        //also fix any items on the tile
                        foreach (MapItem item in baseMap[x + startX, y + startY].GetItems())
                        {
                            item.Coordinate = new MapCoordinate(x + startX, y + startY, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                        }
                    }

                }
            }

            return baseMap;
        }

        #endregion
    }
}
