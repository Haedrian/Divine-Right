using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.LocalMapGeneratorObjects;
using DivineRightGame.LocalMapGenerator.Objects;

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
        public Maplet GenerateDungeon(int tiers, int trapRooms, int guardRooms, int treasureRooms)
        {
            List<DungeonRoom> rooms = new List<DungeonRoom>();
            int uniqueID = 0;

            //Start with the root node
            DungeonRoom root = new DungeonRoom();
            root.SquareNumber = (int) Math.Ceiling((double)WIDTH / 5);
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

            




            return null;

        }

        private DungeonRoom[] GetPathableRooms(List<DungeonRoom> sourceList, int tier, int square)
        {
            return sourceList.Where(
                sl => (sl.TierNumber.Equals(tier+1) && sl.SquareNumber.Equals(square))
                || sl.TierNumber.Equals(tier+1) && (sl.SquareNumber.Equals(square+1) || sl.SquareNumber.Equals(square-1 ) )).ToArray();
        }
    }
}
