using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.LocalMapGeneratorObjects;
using DivineRightGame.LocalMapGenerator.Objects;
using DRObjects;

namespace DivineRightGame.LocalMapGenerator
{
    public class DungeonGenerator
    {
        private const int WIDTH = 5;
        //Probability for the game to create 2..3..4 rooms in the same tier
        private const int PROB_2 = 75;
        private const int PROB_3 = 40;
        private const int PROB_4 = 10;

        /// <summary>
        /// Generates a dungeon having a particular amount of tiers, trap rooms, guard rooms and treasure rooms
        /// </summary>
        /// <param name="tiers"></param>
        /// <returns></returns>
        public MapBlock[,] GenerateDungeon(int tiers, int trapRooms, int guardRooms, int treasureRooms)
        {
            List<DungeonRoom> rooms = new List<DungeonRoom>();
            int uniqueID = 0;

            //Start with the root node
            DungeonRoom root = new DungeonRoom();
            root.SquareNumber = (int) Math.Ceiling((double)WIDTH / 2);
            root.TierNumber = 0;
            root.UniqueID = uniqueID++;

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

            //Lets assign the rooms based on the maximum amount.

            //Some rooms have more probability in certain regions.

            //So lets divide the rooms in 3
            //Favoured - x3
            //Other - x2
            //Unfavoured - x1

            int lowerBoundary = rooms.Count/3;
            int upperBoundary = 2*rooms.Count/3;

            //Start with trap rooms
            var orderedTrap = rooms.OrderByDescending(o => random.Next(100) *
                (o.UniqueID > upperBoundary ? 2 : o.UniqueID > lowerBoundary ? 3 : 1)).Where(r => r.DungeonRoomType == DungeonRoomType.EMPTY_ROOM
                    ).Take(trapRooms);

            foreach (var room in orderedTrap)
            {
                room.DungeonRoomType = DungeonRoomType.TRAPPED_ROOM;
            }

            //Same thing for treasure rooms
            var orderedTreasure = rooms.OrderByDescending(o => random.Next(100) *
                (o.UniqueID > upperBoundary ? 1 : o.UniqueID > lowerBoundary ? 2 : 3)).Where(r => r.DungeonRoomType == DungeonRoomType.EMPTY_ROOM
                    ).Take(treasureRooms);

            foreach (var room in orderedTreasure)
            {
                room.DungeonRoomType = DungeonRoomType.TREASURE_ROOM;
            }
            
            //And guard rooms
            var orderedGuard = rooms.OrderByDescending(o => random.Next(100) *
                (o.UniqueID > upperBoundary ? 3 : o.UniqueID > lowerBoundary ? 1 : 2)).Where(r => r.DungeonRoomType == DungeonRoomType.EMPTY_ROOM
                    ).Take(guardRooms);

            foreach (var room in orderedGuard)
            {
                room.DungeonRoomType = DungeonRoomType.GUARD_ROOM;
            }

            //Now that that part is done, we put them on the actual grid.

            //We go for a 15x15 room and connect the items in it.

            //15x15 - with a gap of 7 between them for tunnels
            int mapWidth = ((WIDTH+1) * 16);
            int mapHeight = ((tiers+1) * 16);


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
                                Coordinate = new MapCoordinate(x,y,0,DRObjects.Enums.MapTypeEnum.LOCAL)
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
                    default:
                        throw new NotImplementedException("Dungeon Room " + room.DungeonRoomType + " not planned for yet.");
                }

                //Generate it :)
                Maplet maplet = xmlGen.ParseMapletFromTag(tag);

                gennedMap = gen.GenerateMap(25, null, maplet, true);

                //fit her onto the main map

                int xIncreaser = room.SquareNumber*16;
                int yIncreaser = room.TierNumber*16;

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
            }


            return map;

        }

        private DungeonRoom[] GetPathableRooms(List<DungeonRoom> sourceList, int tier, int square)
        {
            return sourceList.Where(
                sl => (sl.TierNumber.Equals(tier+1) && sl.SquareNumber.Equals(square))
                || sl.TierNumber.Equals(tier+1) && (sl.SquareNumber.Equals(square+1) || sl.SquareNumber.Equals(square-1 ) )).ToArray();
        }
    }
}
