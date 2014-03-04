using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.LocalMapGeneratorObjects;
using DivineRightGame.LocalMapGenerator.Objects;
using DRObjects;
using DRObjects.Enums;
using DRObjects.ActorHandling.ActorMissions;
using Microsoft.Xna.Framework;
using DRObjects.ActorHandling;
using DivineRightGame.ActorHandling;

namespace DivineRightGame.LocalMapGenerator
{
    public class DungeonGenerator
    {
        private const int WIDTH = 5;
        //Probability for the game to create 2..3..4 rooms in the same tier
        //private const int PROB_2 = 65;
        //private const int PROB_3 = 55;
        //private const int PROB_4 = 30;

        private const int PROB_2 = 95;
        private const int PROB_3 = 96;
        private const int PROB_4 = 96;

        /// <summary>
        /// Generates a dungeon having a particular amount of tiers, trap rooms, guard rooms and treasure rooms
        /// </summary>
        /// <param name="tiers">How many 'layers' the dungeon contains</param>
        /// <param name="enemyArray">A list of enemy actors</param>
        /// <param name="guardRooms">The maximum amount of guardrooms contained in the dungeon</param>
        /// <param name="maxOwnedPopulation">For each owned room which generates enemies, the maximum amount GENERATED in each room (does not preclude patrols from entering the same room)</param>
        /// <param name="maxWildPopulation">For each wild room which generates enemies, the maximum amount GENERATED in each room.</param>
        /// <param name="ownerType">For each owned room, the type of the enemies to create</param>
        /// <param name="percentageOwned">The percentage of the rooms which are owned as opposed to being wild. Bear in mind that wild rooms can spawn quite a bit of enemies</param>
        /// <param name="pointsOfInterest">The points of interest (ie guard and treasure rooms for instance) which have been generated. Used for patrols</param>
        /// <param name="startPoint">The entrance start point</param>
        /// <param name="trapRooms">The maximum amount of trap rooms to generate</param>
        /// <param name="treasureRooms">The maximum amount of teasure rooms to generate</param>
        /// <returns></returns>
        public MapBlock[,] GenerateDungeon(int tiers, int trapRooms, int guardRooms, int treasureRooms,string ownerType, 
            decimal percentageOwned, int maxWildPopulation, int maxOwnedPopulation,
            out MapCoordinate startPoint,out DRObjects.Actor[] enemyArray,out List<PointOfInterest> pointsOfInterest)
        {
            startPoint = new MapCoordinate(0, 0, 0, MapTypeEnum.LOCAL);
            pointsOfInterest = new List<PointOfInterest>();

            List<DRObjects.Actor> enemies = new List<DRObjects.Actor>();
            List<DungeonRoom> rooms = new List<DungeonRoom>();
            int uniqueID = 0;

            //Start with the root node
            DungeonRoom root = new DungeonRoom();
            root.SquareNumber = (int) Math.Ceiling((double)WIDTH / 2);
            root.TierNumber = 0;
            root.UniqueID = uniqueID++;
            root.Connections.Add(-1); //this is a special id. We'll use it to create a start point

            rooms.Add(root);

            int currentTier = 1;
            int square = (int)Math.Ceiling((double)WIDTH / 2);
            DungeonRoom focusNode = root;

            Random random = new Random(DateTime.UtcNow.Millisecond);

            while (currentTier < tiers)
            {
                //Create a new node
                DungeonRoom newNode = new DungeonRoom();

                newNode.SquareNumber = square;
                newNode.TierNumber = currentTier;
                newNode.UniqueID = uniqueID++;
                newNode.DungeonRoomType = DungeonRoomType.EMPTY_ROOM;
                //connect the focus node to this node
                focusNode.Connections.Add(newNode.UniqueID);
                newNode.Connections.Add(focusNode.UniqueID);

                //change the focus node
                focusNode = newNode;
                //aaaand add it to the list
                rooms.Add(newNode);

                //Now we decide whether to stay in the same tier - or increase the tier
                int randomNumber = random.Next(100);

                int siblings = rooms.Where(r => r.TierNumber.Equals(currentTier)).Count();
                int treshold = 0;

                switch(siblings)
                {
                    case 1: treshold = PROB_2; break;
                    case 2: treshold = PROB_3; break;
                    case 3: treshold = PROB_4; break;
                    default: treshold = 0; break; //NEVER
                }

                if (randomNumber < treshold)
                {
                    //then stay in the same place - go either left or right. Can we go in that direction?
                    bool canGoRight = !rooms.Any(r => (r.SquareNumber.Equals(square + 1) && r.TierNumber.Equals(currentTier)) || square + 1 > WIDTH);
                    bool canGoLeft = !rooms.Any(r => (r.SquareNumber.Equals(square - 1) && r.TierNumber.Equals(currentTier)) || square - 1 < 0);

                    if (canGoLeft && canGoRight)
                    {
                        //pick one at random
                        square += random.Next(2) == 1 ? 1 : -1;
                    }
                    else if (canGoLeft)
                    {
                        square -= 1;
                    }
                    else if (canGoRight)
                    {
                        square += 1;
                    }
                    else
                    {
                        //We've done it all
                        currentTier++;
                    }
                }
                else
                {
                    currentTier++;
                }
            }

            //Now that that part is done, lets add some more paths so we turn this into a graph
            foreach (DungeonRoom room in rooms)
            {
                //For each room, check who is a sibling or immediatly under him. There is a 50% chance of forming a link
                DungeonRoom[] potentialRooms = GetPathableRooms(rooms, room.TierNumber, room.SquareNumber);

                foreach (DungeonRoom potentialRoom in potentialRooms)
                {
                    //Is there a connection already?
                    if (!potentialRoom.Connections.Contains(room.UniqueID))
                    {
                        if (random.Next(2) == 1)
                        {
                            //add a connection
                            room.Connections.Add(potentialRoom.UniqueID);
                            potentialRoom.Connections.Add(room.UniqueID);
                        }
                    }
                }
            }

            //go through the rooms and set some as wild rooms already
            foreach (var room in rooms)
            {
                if (random.Next(100) > percentageOwned)
                {
                    //Wild room
                    room.DungeonRoomType = DungeonRoomType.WILD_ROOM;
                }
            }

            //Lets assign the rooms based on the maximum amount.

            //Some rooms have more probability in certain regions.

            //So lets divide the rooms in 3
            //Favoured - x3
            //Other - x2
            //Unfavoured - x1

            int lowerBoundary = rooms.Count/3;
            int upperBoundary = 2*rooms.Count/3;

            var orderedTrap = rooms.Where
                (o => o.DungeonRoomType == DungeonRoomType.EMPTY_ROOM).OrderByDescending(o => random.Next(100) *
                (o.UniqueID > upperBoundary ? 1 : o.UniqueID > lowerBoundary ? 2 : 3)).Where(r => r.DungeonRoomType == DungeonRoomType.EMPTY_ROOM
                    ).ToArray().Take(trapRooms);

            foreach (var room in orderedTrap)
            {
                room.DungeonRoomType = DungeonRoomType.TRAPPED_ROOM;
            }

            //Same thing for treasure rooms
            var orderedTreasure = rooms.Where
                (o => o.DungeonRoomType == DungeonRoomType.EMPTY_ROOM).OrderByDescending(o => random.Next(100) *
                (o.UniqueID > upperBoundary ? 3 : o.UniqueID > lowerBoundary ? 2 : 1)).Where(r => r.DungeonRoomType == DungeonRoomType.EMPTY_ROOM
                    ).Take(treasureRooms);

            foreach (var room in orderedTreasure)
            {
                room.DungeonRoomType = DungeonRoomType.TREASURE_ROOM;
            }
            
            //And guard rooms
            var orderedGuard = rooms.Where
                (o => o.DungeonRoomType == DungeonRoomType.EMPTY_ROOM).OrderByDescending(o => random.Next(100) *
                (o.UniqueID > upperBoundary ? 1 : o.UniqueID > lowerBoundary ? 3 : 2)).Where(r => r.DungeonRoomType == DungeonRoomType.EMPTY_ROOM
                    ).Take(guardRooms);

            foreach (var room in orderedGuard)
            {
                room.DungeonRoomType = DungeonRoomType.GUARD_ROOM;
            }

            //Now that that part is done, we put them on the actual grid.

            //We go for a 15x15 room and connect the items in it.

            //15x15 - with a gap of 7 between them for tunnels
            int mapWidth = ((WIDTH+7) * 20);
            int mapHeight = ((tiers+7) * 20);


            //Create new blocks
            MapBlock[,] map = new MapBlock[mapWidth, mapHeight];

            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    map[x, y] = new MapBlock()
                        {
                            Tile = new MapItem()
                            {
                                Coordinate = new MapCoordinate(x,y,0,DRObjects.Enums.MapTypeEnum.LOCAL),
                                MayContainItems = false
                            }
                        };
                }
            }

            LocalMapGenerator gen = new LocalMapGenerator();
            LocalMapXMLParser xmlGen = new LocalMapXMLParser();

            //Start generating the maps and then stitch them upon the main map
            foreach (DungeonRoom room in rooms)
            {
                MapBlock[,] gennedMap = null;
                string tag = String.Empty;

                switch(room.DungeonRoomType)
                {
                    case DungeonRoomType.EMPTY_ROOM: tag = "Empty Dungeon"; break;
                    case DungeonRoomType.GUARD_ROOM: tag = "Guard Dungeon"; break;
                    case DungeonRoomType.TRAPPED_ROOM: tag = "Trap Dungeon"; break;
                    case DungeonRoomType.TREASURE_ROOM: tag = "Treasure Dungeon"; break;
                    case DungeonRoomType.WILD_ROOM: tag = "Empty Dungeon"; break;
                    default:
                        throw new NotImplementedException("Dungeon Room " + room.DungeonRoomType + " not planned for yet.");
                }

                //Generate it :)
                Maplet maplet = xmlGen.ParseMapletFromTag(tag);

                gennedMap = gen.GenerateMap(25, null, maplet, true);

                PointOfInterest mapletInterest = null;

                if (room.DungeonRoomType == DungeonRoomType.GUARD_ROOM || room.DungeonRoomType == DungeonRoomType.TREASURE_ROOM)
                {
                    //This will be a point of interest. Select a random walkable point in the room and mark the place as such
                    for (int tryAmount = 0; tryAmount < 50; tryAmount++)
                    {
                        //Try for a maximum of 50 times
                        int x = random.Next(gennedMap.GetLength(0));
                        int y = random.Next(gennedMap.GetLength(1));

                        if (gennedMap[x, y].Tile.MayContainItems)
                        {
                            //Put this as the point
                            PointOfInterest interest = new PointOfInterest();
                            interest.Coordinate = new MapCoordinate(x, y, 0, MapTypeEnum.LOCAL);

                            if (room.DungeonRoomType == DungeonRoomType.GUARD_ROOM)
                            {
                                interest.Type = PointOfInterestType.GUARD_ROOM;
                            }
                            else if (room.DungeonRoomType == DungeonRoomType.TREASURE_ROOM)
                            {
                                interest.Type = PointOfInterestType.TREASURE;
                            }

                            pointsOfInterest.Add(interest);
                            mapletInterest = interest;
                            break;
                        }
                    }
                }

                DRObjects.Actor[] roomEnemies = new DRObjects.Actor[] { };
                if (room.DungeonRoomType == DungeonRoomType.GUARD_ROOM || room.DungeonRoomType == DungeonRoomType.TREASURE_ROOM)
                {
                    //Create an amount of enemies
                    gennedMap = gen.GenerateEnemies(gennedMap, random.Next(maxOwnedPopulation), ownerType,out roomEnemies);

                    enemies.AddRange(roomEnemies);
                }

                if (room.DungeonRoomType == DungeonRoomType.WILD_ROOM)
                {
                    //Create an amount of wild enemies - let's get a random type for this room
                    string type = ActorGeneration.GetEnemyType(false);

                    gennedMap = gen.GenerateEnemies(gennedMap, random.Next(maxWildPopulation), type, out roomEnemies);

                    //go through all of room enemies and set them to idle
                    foreach (var enemy in roomEnemies)
                    {
                        enemy.MissionStack.Clear();
                        enemy.MissionStack.Push(new IdleMission());
                    }
                }

                //fit her onto the main map

                int xIncreaser = room.SquareNumber*20 ;
                int yIncreaser = (room.TierNumber*20) + 3;


                //Fix the patrol points of any enemies 
                foreach (Actor enemy in roomEnemies)
                {
                    if (enemy.MissionStack.Count != 0 && enemy.MissionStack.Peek().MissionType  == DRObjects.ActorHandling.ActorMissionType.WANDER)
                    {
                        //Change patrol point
                        MapCoordinate point = (enemy.MissionStack.Peek() as WanderMission).WanderPoint;
                        point.X += xIncreaser;
                        point.Y += yIncreaser;

                        //Change the rectangle x and y too
                        Rectangle rect = (enemy.MissionStack.Peek() as WanderMission).WanderRectangle;
                        rect.X = xIncreaser;
                        rect.Y = yIncreaser;

                        (enemy.MissionStack.Peek() as WanderMission).WanderRectangle = rect; //apparently rectangles are immutable or something
                    }
                }
                //Update the point of interest if there is one
                if (mapletInterest != null)
                {
                    mapletInterest.Coordinate.X += xIncreaser;
                    mapletInterest.Coordinate.Y += yIncreaser;
                }

                for (int x = 0; x < gennedMap.GetLength(0); x++)
                {
                    for (int y = 0; y < gennedMap.GetLength(1); y++)
                    {
                        map[x + xIncreaser,y + yIncreaser] = gennedMap[x,y];
                        map[x + xIncreaser, y + yIncreaser].Tile.Coordinate = new MapCoordinate(x + xIncreaser, y + yIncreaser, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

                        foreach (var item in map[x + xIncreaser, y + yIncreaser].GetItems())
                        {
                            item.Coordinate = new MapCoordinate(x + xIncreaser, y + yIncreaser, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                        }
                    }
                }

                //Lets draw the connections - only the ones who's rooms we've drawn yet
                
                ItemFactory.ItemFactory factory = new ItemFactory.ItemFactory();

                foreach (var connection in room.Connections.Where(c => c < room.UniqueID))
                {
                    if (connection == -1)
                    {
                        //Entrance hall!
                        //Create a line of 3 at the bottom and return the coordinates
                        int topEdgeY = yIncreaser;
                        int bottomEdgeXMin = 0 + xIncreaser;
                        int bottomEdgeXMax = gennedMap.GetLength(0) + xIncreaser;

                        //Find the start
                        int xStart = (bottomEdgeXMax - bottomEdgeXMin)/2 + bottomEdgeXMin;

                        //Set the start point
                        startPoint = new MapCoordinate(xStart+1, topEdgeY-2, 0, MapTypeEnum.LOCAL);

                        int x = xStart;
                        int y = topEdgeY;
                        
                        //go 3 steps down
                        for(int a = 0; a < 3; a++)
                        {
                            for (int x1 = 0; x1 < 3; x1++)
                            {
                                //Draw!
                                map[x + x1, y].Tile = factory.CreateItem("TILES", 25);
                                map[x + x1, y].Tile.Coordinate = new MapCoordinate(x + x1, y, 0, MapTypeEnum.LOCAL);
                            }

                            y--; //move down
                        }

                        y = topEdgeY;

                        //Walk back
                        do
                        {
                            for (int x1 = 0; x1 < 3; x1++)
                            {
                                //Draw!
                                map[x + x1, y].Tile = factory.CreateItem("TILES", 25);
                                map[x + x1, y].Tile.Coordinate = new MapCoordinate(x + x1, y, 0, MapTypeEnum.LOCAL);
                            }

                            y++; //move up
                        }while (x >= 0 && y < map.GetLength(1) && !map[x, y].Tile.MayContainItems);

                        //Put the spikes at the entrance
                        int dummy = -1;

                        map[xStart, topEdgeY - 2].ForcePutItemOnBlock(factory.CreateItem(Archetype.MUNDANEITEMS, "spikes", out dummy));
                        map[xStart+2, topEdgeY - 2].ForcePutItemOnBlock(factory.CreateItem(Archetype.MUNDANEITEMS, "spikes", out dummy));


                        continue;
                    }

                    //Identify the room to be connected with
                    var roomToBeConnected = rooms.Where(r => r.UniqueID.Equals(connection)).FirstOrDefault();

                    //Determine the direction relative to the current room
                    if (roomToBeConnected.SquareNumber > room.SquareNumber)
                    {
                        //RIGHT
                        //Find the rightmost edge of the room and start... somewhere

                        int rightEdgeX = gennedMap.GetLength(0) + xIncreaser;
                        int rightEdgeYMin = 0 + yIncreaser;
                        int rightEdgeYMax = gennedMap.GetLength(1) + yIncreaser;
 
                        //Pick a start at random
                        int yStart = random.Next(rightEdgeYMax - rightEdgeYMin-2) + rightEdgeYMin;

                        //Now 'walk' from ystart-ystart+3 until you hit on something which has a block in it

                        int x = rightEdgeX;
                        int y = yStart;

                        while ( x < map.GetLength(0) && y < map.GetLength(1) && !map[x, y].Tile.MayContainItems)
                        {
                            bool holed = false;
                            for (int y1 = 0; y1 < 3; y1++)
                            {
                                if (!holed)
                                {
                                    //Have a chance of putting in a hole
                                    if (random.Next(8) == 0)
                                    {
                                        holed = true;
                                        continue; //don't put in a tile
                                    }
                                }

                                //Draw!
                                map[x, y + y1].Tile = factory.CreateItem("TILES",25);
                                map[x,y+y1].Tile.Coordinate = new MapCoordinate(x,y+y1,0,MapTypeEnum.LOCAL);
                            }

                            x++; //increment x
                        }

                        x = rightEdgeX - 1;

                        //now lets walk backwards too
                        while (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && !map[x, y].Tile.MayContainItems)
                        {
                            for (int y1 = 0; y1 < 3; y1++)
                            {
                                //Draw!
                                map[x, y + y1].Tile = factory.CreateItem("TILES", 25);
                                map[x, y + y1].Tile.Coordinate = new MapCoordinate(x, y + y1, 0, MapTypeEnum.LOCAL);
                            }

                            x--; //walk back
                        }


                    }
                    else if (roomToBeConnected.SquareNumber < room.SquareNumber)
                    {
                        //LEFT
                        //Find the leftMose edge of the room and start... somewhere

                        int leftEdgeX = xIncreaser;
                        int leftEdgeYMin = 0 + yIncreaser;
                        int leftEdgeYMax = gennedMap.GetLength(1) + yIncreaser;

                        //Pick a start at random
                        int yStart = random.Next(leftEdgeYMax - leftEdgeYMin-2) + leftEdgeYMin;

                        //Now 'walk' from ystart-ystart+3 until you hit on something which has a block in it

                        int x = leftEdgeX;
                        int y = yStart;

                        do
                        {
                            bool holed = false;

                            for (int y1 = 0; y1 < 3; y1++)
                            {

                                if (!holed)
                                {
                                    //Have a chance of putting in a hole
                                    if (random.Next(8) == 0)
                                    {
                                        holed = true;
                                        continue; //don't put in a tile
                                    }
                                }

                                //Draw!
                                map[x, y + y1].Tile = factory.CreateItem("TILES", 25);
                                map[x, y + y1].Tile.Coordinate = new MapCoordinate(x, y + y1, 0, MapTypeEnum.LOCAL);
                            }

                            x--; //decrement x
                        } while (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && !map[x, y].Tile.MayContainItems);

                        x = leftEdgeX + 1;
                        //walk backwards
                        do
                        {
                            for (int y1 = 0; y1 < 3; y1++)
                            {
                                //Draw!
                                map[x, y + y1].Tile = factory.CreateItem("TILES", 25);
                                map[x, y + y1].Tile.Coordinate = new MapCoordinate(x, y + y1, 0, MapTypeEnum.LOCAL);
                            }

                            x++; //walk back
                        } while (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && !map[x, y].Tile.MayContainItems);



                    }
                    else if (roomToBeConnected.TierNumber < room.TierNumber)
                    {
                        //BOTTOM
                        //Find the bottommost edge of the room and start... somewhere

                        int bottomEdgeY = yIncreaser;
                        int bottomEdgeXMin = 0 + xIncreaser;
                        int bottomEdgeXMax = gennedMap.GetLength(0) + xIncreaser;

                        //Pick a start at random
                        int xStart = random.Next(bottomEdgeXMax - bottomEdgeXMin- 2) + bottomEdgeXMin;

                        //Now 'walk' from xstart-xstart+3 until you hit on something which has a block in it

                        int x = xStart;
                        int y = bottomEdgeY;

                        do
                        {
                            bool holed = false;

                            for (int x1 = 0; x1 < 3; x1++)
                            {

                                if (!holed)
                                {
                                    //Have a chance of putting in a hole
                                    if (random.Next(8) == 0)
                                    {
                                        holed = true;
                                        continue; //don't put in a tile
                                    }
                                }

                                //Draw!
                                map[x+x1, y].Tile = factory.CreateItem("TILES", 25);
                                map[x+x1, y].Tile.Coordinate = new MapCoordinate(x+x1, y, 0, MapTypeEnum.LOCAL);
                            }

                            y--; //decrement y
                        } while (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) &&!map[x, y].Tile.MayContainItems);

                        y = bottomEdgeY + 1;

                        //Walk backwards
                        do
                        {
                            for (int x1 = 0; x1 < 3; x1++)
                            {
                                //Draw!
                                map[x + x1, y].Tile = factory.CreateItem("TILES", 25);
                                map[x + x1, y].Tile.Coordinate = new MapCoordinate(x + x1, y, 0, MapTypeEnum.LOCAL);
                            }

                            y++; //walk back
                        } while (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && !map[x, y].Tile.MayContainItems);
                        
                    }
                    else if (roomToBeConnected.TierNumber > room.TierNumber)
                    {
                        //TOP - Won't ever happen - but sure
                        //Find the topmost edge of the room and start... somewhere

                        int topEdgeY = yIncreaser + gennedMap.GetLength(1);
                        int bottomEdgeXMin = 0 + xIncreaser;
                        int bottomEdgeXMax = gennedMap.GetLength(0) + xIncreaser;

                        //Pick a start at random
                        int xStart = random.Next(bottomEdgeXMax - bottomEdgeXMin - 2) + bottomEdgeXMin;

                        //Now 'walk' from xstart-xstart+3 until you hit on something which has a block in it

                        int x = xStart;
                        int y = topEdgeY;

                        do
                        {
                            bool holed = false;

                            for (int x1 = 0; x1 < 3; x1++)
                            {

                                if (!holed)
                                {
                                    //Have a chance of putting in a hole
                                    if (random.Next(8) == 0)
                                    {
                                        holed = true;
                                        continue; //don't put in a tile
                                    }
                                }

                                //Draw!
                                map[x + x1, y].Tile = factory.CreateItem("TILES", 25);
                                map[x + x1, y].Tile.Coordinate = new MapCoordinate(x + x1, y, 0, MapTypeEnum.LOCAL);
                            }

                            y++; //move up
                        } while (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && !map[x, y].Tile.MayContainItems);

                        //walk back
                        y = topEdgeY + 1;

                        do
                        {
                            for (int x1 = 0; x1 < 3; x1++)
                            {
                                //Draw!
                                map[x + x1, y].Tile = factory.CreateItem("TILES", 25);
                                map[x + x1, y].Tile.Coordinate = new MapCoordinate(x + x1, y, 0, MapTypeEnum.LOCAL);
                            }

                            y--; //move down
                        } while (x >= 0 && x < map.GetLength(0) && y >= 0 && y < map.GetLength(1) && !map[x, y].Tile.MayContainItems);
                    }
                }
            }

            enemyArray = enemies.ToArray();
            return map;

        }

        private DungeonRoom[] GetPathableRooms(List<DungeonRoom> sourceList, int tier, int square)
        {
            return sourceList.Where
                (
                sl => (sl.TierNumber.Equals(tier+1) && sl.SquareNumber.Equals(square)
                    )
                
                    || (sl.TierNumber.Equals(tier) && (sl.SquareNumber.Equals(square+1) || sl.SquareNumber.Equals(square-1 ) ))).ToArray();
        }
    }
}
