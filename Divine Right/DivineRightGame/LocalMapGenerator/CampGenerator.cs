using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Enums;
using Microsoft.Xna.Framework;
using DRObjects.LocalMapGeneratorObjects;
using DRObjects.Items.Archetypes.Local;
using DivineRightGame.ActorHandling;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.ActorHandling;

namespace DivineRightGame.LocalMapGenerator
{
    public class CampGenerator
    {
        /// <summary>
        /// The size of each edge of the map
        /// </summary>
        private const int MAP_EDGE = 25;
        /// <summary>
        /// The size of each edge of the fortified part of the map
        /// </summary>
        private const int FORTIFICATION_EDGE = 15;

        private const int EASY_LEVEL = 2;
        private const int MEDIUM_LEVEL = 3;
        private const int HARD_LEVEL = 5;

        private const int EASY_MONEY = 100;
        private const int MEDIUM_MONEY = 200;
        private const int HARD_MONEY = 300;

        /// <summary>
        /// Generates a camp
        /// </summary>
        /// <returns></returns>
        public static MapBlock[,] GenerateCamp(int enemies, out MapCoordinate startPoint, out DRObjects.Actor[] enemyArray)
        {
            MapBlock[,] map = new MapBlock[MAP_EDGE, MAP_EDGE];

            Random random = new Random();

            ItemFactory.ItemFactory factory = new ItemFactory.ItemFactory();

            int grassTileID = 0;

            factory.CreateItem(Archetype.TILES, "grass", out grassTileID);

            //Create a new map which is edge X edge in dimensions and made of grass
            for (int x = 0; x < MAP_EDGE; x++)
            {
                for (int y = 0; y < MAP_EDGE; y++)
                {
                    MapBlock block = new MapBlock();
                    map[x, y] = block;
                    block.Tile = factory.CreateItem("tile", grassTileID);
                    block.Tile.Coordinate = new MapCoordinate(x, y, 0, MapType.LOCAL);
                }
            }

            //Now created a wall
            int pallisadeID = 0;

            factory.CreateItem(Archetype.MUNDANEITEMS, "pallisade wall lr", out pallisadeID);

            //Create a square of pallisade wall

            int startCoord = (MAP_EDGE - FORTIFICATION_EDGE) / 2;
            int endCoord = MAP_EDGE - ((MAP_EDGE - FORTIFICATION_EDGE) / 2);

            for (int x = startCoord + 1; x < endCoord; x++)
            {
                MapBlock block = map[x, startCoord];
                MapItem item = factory.CreateItem("mundaneitems", pallisadeID);

                block.ForcePutItemOnBlock(item);

                block = map[x, endCoord];
                item = factory.CreateItem("mundaneitems", pallisadeID);

                block.ForcePutItemOnBlock(item);
            }

            pallisadeID = 0;

            factory.CreateItem(Archetype.MUNDANEITEMS, "pallisade wall tb", out pallisadeID);

            for (int y = startCoord + 1; y < endCoord; y++)
            {
                MapBlock block = map[startCoord, y];
                MapItem item = factory.CreateItem("mundaneitems", pallisadeID);

                block.ForcePutItemOnBlock(item);

                block = map[endCoord, y];
                item = factory.CreateItem("mundaneitems", pallisadeID);

                block.ForcePutItemOnBlock(item);
            }

            //We need to poke a hole in wall as an entrance
            //Let's poke one at the top and one at the bottom
            int center = MAP_EDGE / 2;

            for (int x = -1; x < 2; x++)
            {
                MapBlock block = map[center + x, startCoord];

                block.RemoveTopItem();

                block = map[center + x, endCoord];

                block.RemoveTopItem();
            }

            for (int y = -1; y < 2; y++)
            {
                MapBlock block = map[startCoord, y + center];
                block.RemoveTopItem();

                block = map[endCoord, y + center];
                block.RemoveTopItem();
            }

            //Now, let's create some maplets in there

            //There's a single maplet containing the other maplets - let's get it
            LocalMapXMLParser lm = new LocalMapXMLParser();
            Maplet maplet = lm.ParseMapletFromTag("camp");

            LocalMapGenerator gen = new LocalMapGenerator();
            var gennedMap = gen.GenerateMap(grassTileID, null, maplet, false, "", out enemyArray);

            gen.JoinMaps(map, gennedMap, startCoord + 1, startCoord + 1);


            //Let's add some trees and stuff
            int decorCount = (int)(map.GetLength(1) * 0.50);

            //Just find as many random points and if it happens to be grass, drop them
            int itemID = 0;

            for (int i = 0; i < decorCount; i++)
            {
                //Just trees
                MapItem decorItem = factory.CreateItem(Archetype.MUNDANEITEMS, "tree", out itemID);

                //Pick a random point
                MapBlock randomBlock = map[random.Next(map.GetLength(0)), random.Next(map.GetLength(1))];

                //Make sure its not inside the camp
                if (randomBlock.Tile.Coordinate.X >= startCoord && randomBlock.Tile.Coordinate.X <= endCoord && randomBlock.Tile.Coordinate.Y >= startCoord && randomBlock.Tile.Coordinate.Y <= endCoord)
                {
                    //Not within the camp
                    i--;
                    continue; //try again
                }

                if (randomBlock.MayContainItems && randomBlock.Tile.Name == "Grass")
                {
                    //Yes, can support it
                    randomBlock.ForcePutItemOnBlock(decorItem);
                }
                //Otherwise forget all about it
            }


            //Now select all the border tiles and put in a "Exit here" border
            for (int x = 0; x < map.GetLength(0); x++)
            {
                MapCoordinate coo = new MapCoordinate(x, 0, 0, MapType.LOCAL);

                LeaveTownItem lti = new LeaveTownItem();
                lti.Coordinate = coo;
                lti.Description = "path outside the town";
                lti.Name = "Leave Town";

                lti.Coordinate = coo;

                map[x, 0].ForcePutItemOnBlock(lti);

                coo = new MapCoordinate(x, map.GetLength(1) - 1, 0, MapType.LOCAL);

                lti = new LeaveTownItem();
                lti.Coordinate = coo;
                lti.Description = "path outside the town";
                lti.Name = "Leave Town";

                lti.Coordinate = coo;

                map[x, map.GetLength(1) - 1].ForcePutItemOnBlock(lti);

            }

            for (int y = 0; y < map.GetLength(1); y++)
            {
                MapCoordinate coo = new MapCoordinate(0, y, 0, MapType.LOCAL);

                LeaveTownItem lti = new LeaveTownItem();
                lti.Coordinate = coo;
                lti.Description = "path outside the town";
                lti.Name = "Leave Town";

                lti.Coordinate = coo;

                map[0, y].ForcePutItemOnBlock(lti);

                coo = new MapCoordinate(map.GetLength(0) - 1, y, 0, MapType.LOCAL);

                lti = new LeaveTownItem();
                lti.Coordinate = coo;
                lti.Description = "path outside the town";
                lti.Name = "Town Borders";

                lti.Coordinate = coo;

                map[map.GetLength(0) - 1, y].ForcePutItemOnBlock(lti);
            }

            #region Patrol Points

            //Now let's collect the patrol points. We're going to have two possible patrols - one around each of the entrances - and another on the outside corners of the map

            List<MapCoordinate> outsidePatrol = new List<MapCoordinate>();

            outsidePatrol.Add(new MapCoordinate(startCoord - 2, startCoord, 0, MapType.LOCAL));
            outsidePatrol.Add(new MapCoordinate(endCoord + 2, startCoord, 0, MapType.LOCAL));
            outsidePatrol.Add(new MapCoordinate(endCoord + 2, endCoord, 0, MapType.LOCAL));
            outsidePatrol.Add(new MapCoordinate(startCoord - 2, endCoord , 0, MapType.LOCAL));

            List<MapCoordinate> insidePatrol = new List<MapCoordinate>();

            insidePatrol.Add(new MapCoordinate(center, startCoord,0,MapType.LOCAL));
            insidePatrol.Add(new MapCoordinate(center, endCoord, 0, MapType.LOCAL));
            insidePatrol.Add(new MapCoordinate(startCoord, center, 0, MapType.LOCAL));
            insidePatrol.Add(new MapCoordinate(endCoord, center, 0, MapType.LOCAL));

            //Go through all of those and make sure they're clear of anything that wouldn't let them walk upon them
            foreach (var coordinate in outsidePatrol)
            {
                map[coordinate.X, coordinate.Y].RemoveTopItem();
            }

            foreach (var coordinate in insidePatrol)
            {
                map[coordinate.X, coordinate.Y].RemoveTopItem();
            }

            #endregion

            #region Actors

            enemyArray = CreateBandits(enemies, outsidePatrol, insidePatrol);

            ConformEnemies(enemyArray.ToList());

            int tries = 0;

            //Put them on the mappity map
            for (int i = 0; i < enemyArray.Length; i++)
            {
                Actor actor = enemyArray[i];

                int randomX = random.Next(map.GetLength(0));
                int randomY = random.Next(map.GetLength(1));

                if (map[randomX, randomY].MayContainItems)
                {
                    //Plop it on there
                    actor.MapCharacter.Coordinate = new MapCoordinate(randomX, randomY, 0, MapType.LOCAL);
                    map[randomX, randomY].ForcePutItemOnBlock(actor.MapCharacter);
                    tries = 0;

                    //If they are wandering, make them wander in the right place
                    var mission = actor.MissionStack.Peek();

                    if (mission.MissionType == ActorMissionType.WANDER)
                    {
                        var wander = mission as WanderMission;

                        wander.WanderPoint = new MapCoordinate(actor.MapCharacter.Coordinate);
                        wander.WanderRectangle = new Rectangle(startCoord, startCoord, FORTIFICATION_EDGE, FORTIFICATION_EDGE);
                    }

                }
                else
                {
                    tries++;
                    i--;
                }

                if (tries >= 50)
                {
                    //give up
                    continue;
                }
            }

            #endregion


            startPoint = new MapCoordinate(map.GetLength(0) / 2, 0, 0, MapType.LOCAL);

            return map;


        }

        /// <summary>
        /// Creates a number of bandits.
        /// They will be structured
        /// 9:3:1 in difficulty
        /// 33% will be assigned to a patrol. They will not be positioned on the map
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        public static Actor[] CreateBandits(int total, List<MapCoordinate> outsidePatrol, List<MapCoordinate> insidePatrol)
        {
            Random random = new Random();

            List<Actor> actors = new List<Actor>();

            ItemFactory.ItemFactory fact = new ItemFactory.ItemFactory();

            for (int i = 0; i < total; i++)
            {
                int enemyID = 0;
                Actor actor = ActorGeneration.CreateActor("human", "bandit easy", true, 10, 10, null, out enemyID, null);

                actors.Add(actor);

                actor.MapCharacter = fact.CreateItem("ENEMIES", enemyID);
                (actor.MapCharacter as LocalCharacter).Actor = actor;

                //Is this going to be one of the patroling ones ?

                if (random.Next(10) % 3 == 0)
                {
                    //Yes
                    actor.MissionStack.Push(new PatrolRouteMission() { PatrolRoute = random.Next(2) == 1 ? outsidePatrol : insidePatrol });
                }
                else
                {
                    //wander
                    actor.MissionStack.Push(new WanderMission());
                }
            }

            return actors.ToArray();
        }

        public static void ConformEnemies(List<Actor> enemies)
        {
            Random random = new Random();

            //Filter out the enemies which are the denizens of this dungeon
            List<Actor> ens = enemies.Where(e => e.EnemyData.Intelligent).ToList();

            //The rules for level are as follows
            //For each 3 'easys' - put in a medium
            //For each 3 mediums - put in a hard
            //Later we might have an even harder, but leave it like that for now

            int easyCount = 0;
            int mediumCount = 0;

            //Shuffle the ens - so we don't get hard batches. It'd be more logically to leave them unshuffled, but it'd be too hard.

            ens = ens.OrderBy(r => random.Next(100)).ToList();

            foreach (var enemy in ens)
            {
                if (mediumCount == 3)
                {
                    //Generate a hard
                    ActorGeneration.RegenerateBandit(enemy, HARD_LEVEL, HARD_MONEY);
                    mediumCount = 0;
                }
                else if (easyCount == 3)
                {
                    //Generate a medium
                    ActorGeneration.RegenerateBandit(enemy, MEDIUM_LEVEL, MEDIUM_MONEY);
                    easyCount = 0;
                    mediumCount++;
                }
                else
                {
                    //Generate an easy
                    ActorGeneration.RegenerateBandit(enemy, EASY_LEVEL, EASY_MONEY);
                    easyCount++;
                }
            }

            //Done
            return;

        }

    }
}
