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
        private Random random = new Random(DateTime.UtcNow.Millisecond);

        /// <summary>
        /// Generates a map based on the maplet assigned
        /// </summary>
        /// <param name="maplet">The maplet to generate</param>
        /// <param name="parentWallID">The wall that the parent has</param>
        /// <param name="parentTileID">The ID of the tiles used in the parent maplet item</param>
        /// <returns></returns>
        public MapBlock[,] GenerateMap(int parentTileID, int? parentWallID, Maplet maplet, bool preferSides)
        {
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
                    generatedMap[maplet.SizeX -1, y].PutItemOnBlock(factory.CreateItem("mundaneitems",wallID.Value));

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

                //Shall we put a few winows as well?


                
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


                        if (Fits(planningMap, childMapletBlueprint, childMaplet.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES, out x, out y, out newMap))
                        {
                            //it fits, generate it - <3 Recursion
                            MapBlock[,] childMap = this.GenerateMap(tileID,wallID.Value, childMaplet.Maplet,childMaplet.Position==DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES);

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
            List<MapBlock> edgeBlocks = new List<MapBlock>();

            //Lets also get the edge mapblocks - for those who prefer being on the edge
            for (int x = 0; x < planningMap.GetLength(0); x++)
            {
                if (!maplet.Walled)
                {
                    if (planningMap[x, 0] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[x,0]);
                    }

                    if (planningMap[x, planningMap.GetLength(1) - 1] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[x,planningMap.GetLength(1) - 1]);
                    }
                }
                else 
                {
                    if (planningMap[x, 1] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[x,1]);
                    }

                    if (planningMap[x, planningMap.GetLength(1) - 2] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[x,planningMap.GetLength(1) - 2]);
                    }
                }
                
            }

            //Doing the y parts
            for (int y=0; y < planningMap.GetLength(1); y++)
            {
                if (!maplet.Walled)
                {
                    if (planningMap[0,y] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[0,y]);
                    }

                    if (planningMap[planningMap.GetLength(0) -1,y] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[planningMap.GetLength(0) - 1,y]);
                    }
                }
                else 
                {
                    if (planningMap[1,y] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[1,y]);
                    }

                    if (planningMap[planningMap.GetLength(0) -2,y] == PlanningMapItemType.FREE)
                    {
                        edgeBlocks.Add(generatedMap[planningMap.GetLength(0) - 2,y]);
                    }

                }


            }

            //go through the maplet contents

            //Get the smallest x and y coordinate in the candidate blocks so we can use it for fixed things

            int smallestX = candidateBlocks.Select(b => b.Tile.Coordinate.X).Min();
            int smallestY = candidateBlocks.Select(b => b.Tile.Coordinate.Y).Min();

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
                            //Lets decide where to put it

                            if (contents.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.SIDES && edgeBlocks.Count != 0)
                            {
                                //pick a place at random and add it to the maplet
                                int position = random.Next(edgeBlocks.Count);
                                
                                edgeBlocks[position].PutItemOnBlock(itemPlaced);

                                //remove it from both
                                candidateBlocks.Remove(edgeBlocks[position]);
                                edgeBlocks.RemoveAt(position);

                            }

                            if (contents.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.MIDDLE && candidateBlocks.Except(edgeBlocks).Count() != 0)
                            {
                                //pick a place at random and add it to the maplet
                                int position = random.Next(candidateBlocks.Except(edgeBlocks).Count());

                                MapBlock block = candidateBlocks.Except(edgeBlocks).ToArray()[position]; 

                                block.PutItemOnBlock(itemPlaced);

                                //remove it from both
                                candidateBlocks.Remove(block);
                                edgeBlocks.Remove(block);
                            }

                            if (contents.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.ANYWHERE)
                            {
                                //pick a place at random and add it to the maplet
                                int position = random.Next(candidateBlocks.Count);

                                candidateBlocks[position].PutItemOnBlock(itemPlaced);

                                //remove it from both
                                edgeBlocks.Remove(candidateBlocks[position]);
                                candidateBlocks.RemoveAt(position);
                            }

                            if (contents.Position == DRObjects.LocalMapGeneratorObjects.Enums.PositionAffinity.FIXED)
                            {
                                //Fix it in a particular position.
                                MapCoordinate coordinate = new MapCoordinate(smallestX + contents.x.Value, smallestY + contents.y.Value, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

                                var selectedBlock = candidateBlocks.Where(cb => cb.Tile.Coordinate.Equals(coordinate)).FirstOrDefault();

                                if (selectedBlock != null)
                                { //maybe someone put something there already
                                    selectedBlock.PutItemOnBlock(itemPlaced);
                                }

                                //and remoev it from both
                                candidateBlocks.Remove(selectedBlock);
                                edgeBlocks.Remove(selectedBlock);
                            }
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
                    List<int> possibleYs = new List<int>();

                    int smallerY = Math.Min(segment.Start.Y,segment.End.Y);
                    int largerY = Math.Max(segment.Start.Y,segment.End.Y);

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
                        //nothing to do - take the smallest one
                        block = generatedMap[segment.Start.X, segment.Start.Y + 1];
                    }
                }
                else 
                {
                    List<int> possibleXs = new List<int>();

                    int smallerX = Math.Min(segment.Start.X,segment.End.X);
                    int largerX = Math.Max(segment.Start.X,segment.End.X);

                    //Check in the real map whether the tile next to it is free for walking in
                    for (int x = smallerX + 1; x <= largerX; x++)
                    {
                        if (generatedMap[x, segment.Start.Y -1].MayContainItems && generatedMap[x, segment.Start.Y +1].MayContainItems)
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
                        block = generatedMap[segment.Start.X+1, segment.Start.Y];
                    }


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
        /// <param name="edge">If this is set to true, will try to put them on the edge. Otherwise will try to be compeltly random</param>
        /// <param name="startX">Where the map will fit on the X coordinate</param>
        /// <param name="startY">Where the map will fit on the Y coordinate</param>
        /// <param name="newMap">What the map will look like with the maplet inside it. Will be equivalent to map if there is no fit</param>
        /// <returns></returns>
        public bool Fits(PlanningMapItemType[,] map, PlanningMapItemType[,] maplet, bool edge, out int startX, out int startY, out PlanningMapItemType[,] newMap)
        {

            if (edge)
            {
                //Brute force, nothing to be done
                for (int mapX = 0; mapX < map.GetLength(0); mapX++)
                {
                    for (int mapY = 0; mapY < map.GetLength(1); mapY++)
                    {
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
            else
            {
                //Lets try for a maximum 50 times
                int attemptCount = 0;

                while (attemptCount < 50)
                {
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
