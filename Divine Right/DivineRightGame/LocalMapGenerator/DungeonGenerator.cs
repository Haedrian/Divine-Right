using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame.LocalMapGenerator.Objects;
using DivineRightGame.Pathfinding;
using DRObjects;
using DRObjects.Enums;
using DRObjects.Items.Archetypes;
using DRObjects.Items.Archetypes.Global;
using DRObjects.Items.Tiles;
using DRObjects.LocalMapGeneratorObjects;
using Microsoft.Xna.Framework;

namespace DivineRightGame.LocalMapGenerator
{
    /// <summary>
    /// For generation of dungeons
    /// </summary>
    public static class DungeonGenerator
    {
        private const int SIZE = 75;
        private const int AREA = SIZE * SIZE;
        private const int MINIMUM_AREA = 25;

        private const int MINIMUM_EDGE = 4;
        private const int MAXIMUM_EDGE = 15;

        public static MapBlock[,] GenerateDungeonLevel(int level, int percentCovered, out MapCoordinate startPoint, out List<Actor> enemies,out Dungeon dungeon)
        {
            startPoint = new MapCoordinate();
            enemies = new List<Actor>();
            List<Rectangle> rectangles = null;
            ItemFactory.ItemFactory fact = new ItemFactory.ItemFactory();
            List<SummoningCircle> summoningCircles = new List<SummoningCircle>();

            MapBlock[,] map = GenerateBaseMap(level, percentCovered, out rectangles);

            //Put the tiles
            int tileID = -1;
            var dummy = fact.CreateItem("tiles", "cave", out tileID);

            SummoningCircle dummyCircle = null;

            //Each rectangle is going to contain a room

            //First pick two to be the start and end rooms
            PutRoom(map, tileID, DungeonRoomType.ENTRANCE, rectangles[0],out dummyCircle);

            startPoint = new MapCoordinate(rectangles[0].Center.X, rectangles[0].Center.Y, 0, MapType.LOCAL);

            PutRoom(map, tileID, DungeonRoomType.EXIT, rectangles[1],out dummyCircle);

            //Then pick d6 + level as summoning rooms. Ensuring they are less than half the rooms
            int summoningRooms = GameState.Random.Next(1, 6) + level;

            summoningRooms = summoningRooms > rectangles.Count / 2 ? rectangles.Count / 2 : summoningRooms;

            for (int i = 0; i < summoningRooms; i++)
            {
                SummoningCircle circle = null;

                PutRoom(map, tileID, DungeonRoomType.SUMMONING, rectangles[2 + i],out circle); //Create them

                //Grab references to the summoning circle as we'll need them later

                summoningCircles.Add(circle);
            }


            //Then we can pick some of the rooms as being the other room types

            //Package it all into a Dungeon object
            dungeon = new Dungeon();
            dungeon.DifficultyLevel = level;
            dungeon.Rooms = rectangles;
            dungeon.SummoningCircles = summoningCircles;

            return map;
        }

        /// <summary>
        /// Part 1 - Generate the base map
        /// </summary>
        /// <param name="level"></param>
        /// <param name="percentCovered"></param>
        /// <returns></returns>
        private static MapBlock[,] GenerateBaseMap(int level, int percentCovered, out List<Rectangle> rectangles)
        {
            MapBlock[,] map = new MapBlock[SIZE, SIZE];

            ItemFactory.ItemFactory fact = new ItemFactory.ItemFactory();

            //Put the tiles
            int tileID = -1;
            var dummy = fact.CreateItem("tiles", "cave", out tileID);

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    map[x, y] = new MapBlock(); //Start with air :)
                    map[x, y].Tile = new Air();
                    map[x, y].Tile.Coordinate = (new MapCoordinate(x, y, 0, MapType.LOCAL));
                }
            }

            //Now we need to drop particular shapes on the map, with a minimum area of MINIMUM_AREA until we reach a particular percentage covered
            int totalCovered = 0;
            //Or until we run out of attempts
            int attempts = 0;

            rectangles = new List<Rectangle>();

            while (totalCovered < AREA && attempts < 50)
            {
                attempts++;
                //Pick a random spot to drop it in
                Random random = GameState.Random;

                Point p = new Point(random.Next(SIZE), random.Next(SIZE));

                //From this point, draw a rectangle with a random size from Edge to Edge
                Rectangle rect = new Rectangle(p.X, p.Y, random.Next(MINIMUM_EDGE, MAXIMUM_EDGE), random.Next(MINIMUM_EDGE, MAXIMUM_EDGE));

                //Does it have a valid area? Is it within the current map
                if ((rect.Width * rect.Height) >= MINIMUM_AREA && rect.Right < SIZE && rect.Bottom < SIZE)
                {
                    //Does it intersect or is contained within another rectangle ?
                    //(This will not preclude two rectangles next to each other, as long as they don't intersect)

                    if (rectangles.Any(r => r.Intersects(rect) || r.Contains(rect) || rect.Contains(r)))
                    {
                        //invalid
                        continue;
                    }

                    rectangles.Add(rect); //Valid
                    totalCovered += (rect.Width * rect.Height);
                    attempts = 0;
                }
                else
                {
                    //invalid
                    continue;
                }
            }


            //Now we should have a number of rectangles, let's turn those into actual rooms
            foreach (var rect in rectangles)
            {
                for (int x = 0; x < rect.Width; x++)
                {
                    for (int y = 0; y < rect.Height; y++)
                    {
                        MapCoordinate coord = new MapCoordinate(rect.X + x, rect.Y + y, 0, MapType.LOCAL);

                        MapBlock b = map[coord.X, coord.Y];
                        b.Tile = fact.CreateItem("tiles", tileID);
                        b.Tile.Coordinate = coord;
                    }
                }
            }
            int pathTile = -1;
            dummy = fact.CreateItem("tiles", "pavement", out pathTile);

            //Let's connect each room with each other room
            for (int i = 0; i < rectangles.Count; i++)
            {
                Rectangle curr = rectangles[i];
                Rectangle next = i == rectangles.Count - 1 ? rectangles[0] : rectangles[i + 1];  //Next rectangle is either the one in the list, or the first one

                PathfinderInterface.Nodes = GeneratePathfindingMapConnector(map);

                //Path from the center of the rectangles

                var path = PathfinderInterface.GetPath(new MapCoordinate(curr.Center.X, curr.Center.Y, 0, MapType.LOCAL), new MapCoordinate(next.Center.Y, next.Center.Y, 0, MapType.LOCAL));

                if (path != null)
                {
                    //Path it!
                    foreach (var p in path)
                    {
                        if (!map[p.X, p.Y].MayContainItems)
                        {
                            MapCoordinate coord = new MapCoordinate(p.X, p.Y, 0, MapType.LOCAL);

                            MapBlock b = map[coord.X, coord.Y];
                            b.Tile = fact.CreateItem("tiles", pathTile);
                            b.Tile.Coordinate = coord;
                        }
                    }
                }
            }

            //Now we can go through the blocks and put walls around the border

            int borderID = -1;
            dummy = fact.CreateItem("mundaneitems", "dungeon wall", out borderID);

            for (int x = 0; x < SIZE; x++)
            {
                for (int y = 0; y < SIZE; y++)
                {
                    MapBlock current = map[x, y];

                    if (!current.MayContainItems)
                    {
                        //Are we near one which can contain items?
                        if (GetBlocksAroundPoint(map, current.Tile.Coordinate, 1).Any(b => b.MayContainItems))
                        {
                            //Yep - put a wall there
                            current.ForcePutItemOnBlock(fact.CreateItem("mundaneitems", borderID));
                        }
                    }
                }
            }

            return map;
        }

        /// <summary>
        /// Generates a pathfinding map for connecting
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static byte[,] GeneratePathfindingMapConnector(MapBlock[,] map)
        {
            //Generate a byte map of x and y
            int squareSize = PathfinderInterface.CeilToPower2(Math.Max(map.GetLength(0), map.GetLength(1)));

            byte[,] pf = new byte[squareSize, squareSize];

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (i < map.GetLength(0) - 1 && j < map.GetLength(1) - 1)
                    {
                        //To promote path reuse - if it contains a path give it a weight of 1, otherwise 5, otherwise 50

                        if (map[i, j] == null)
                        {
                            pf[i, j] = Byte.MaxValue;
                        }
                        else if (map[i, j].MayContainItems)
                        {
                            if (map[i, j].Tile.InternalName.ToLower() == "pavement")
                            {
                                pf[i, j] = (byte)1;
                            }
                            else
                            {
                                pf[i, j] = (byte)5;
                            }
                        }
                        else
                        {
                            pf[i, j] = (byte)50;
                        }
                    }
                    else
                    {
                        //Put in the largest possible weight
                        pf[i, j] = Byte.MaxValue;
                    }
                }
            }

            return pf;
        }

        /// <summary>
        /// Creates and puts a particular room in a particular rectangle
        /// </summary>
        /// <param name="roomType"></param>
        /// <param name="rect"></param>
        /// <param name="circle">When the room is a Summoning Room, there's the circle</param>
        private static void PutRoom(MapBlock[,] map, int tileID, DungeonRoomType roomType, Rectangle rect,out SummoningCircle circle)
        {
            circle = null;

            string tagName = roomType.ToString().ToLower().Replace("_", " ") + " room";

            LocalMapXMLParser parser = new LocalMapXMLParser();
            var maplet = parser.ParseMapletFromTag(tagName);

            //Change the width and height to match the rectangle we're fitting it in
            maplet.SizeX = rect.Width;
            maplet.SizeY = rect.Height;

            LocalMapGenerator lmg = new LocalMapGenerator();

            Actor[] actors = null;
            MapletActorWanderArea[] areas = null;
            MapletPatrolPoint[] patrolRoutes = null;
            MapletFootpathNode[] footpathNodes = null;

            var gennedMap = lmg.GenerateMap(tileID, null, maplet, false, "", OwningFactions.UNDEAD, out actors, out areas, out patrolRoutes, out footpathNodes);

            //Is this a summoning room?
            if (roomType == DungeonRoomType.SUMMONING)
            {
                //Go through each map block and see if we find a summoning circle
                for(int x=0; x < gennedMap.GetLength(0); x++)
                {
                    for (int y=0; y < gennedMap.GetLength(1); y++)
                    {
                        var block = gennedMap[x, y];
                        if (block.GetTopMapItem() != null && block.GetTopMapItem().GetType().Equals(typeof(SummoningCircle)))
                        {
                            circle = block.GetTopMapItem() as SummoningCircle;
                        }
                    }
                }
            }

            //Now fit one into the other
            lmg.JoinMaps(map, gennedMap, rect.X, rect.Y);


        }

        public static MapBlock[] GetBlocksAroundPoint(MapBlock[,] map, MapCoordinate centre, int radius)
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

                    if (xLoop >= 0 && xLoop < map.GetLength(0) && yLoop >= 0 && yLoop < map.GetLength(1))
                    { //make sure they're in the map
                        returnList.Add(map[xLoop, yLoop]);
                    }
                }
            }

            return returnList.ToArray();
        }
    }
}
