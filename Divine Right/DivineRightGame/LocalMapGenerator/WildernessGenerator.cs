using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame.ActorHandling;
using DivineRightGame.LocalMapGenerator.Objects;
using DRObjects;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.Enums;
using DRObjects.Items.Archetypes.Local;
using Microsoft.Xna.Framework;

namespace DivineRightGame.LocalMapGenerator
{
    /// <summary>
    /// A class for generating patches of Wilderness with animals and stuff
    /// </summary>
    public static class WildernessGenerator
    {
        /// <summary>
        /// The size of the wilderness map
        /// </summary>
        private static int MAP_EDGE = 30;

        private static int TREE_AMOUNT_FOREST = 100;
        private static int TREE_AMOUNT_WOODLAND = 50;
        private static int ARID_DESERT_TREE_COUNT = 15;

        public static Dictionary<GlobalBiome, WildernessGenerationData> details = new Dictionary<GlobalBiome, WildernessGenerationData>();

        static WildernessGenerator()
        {
            details.Add(GlobalBiome.ARID_DESERT, new WildernessGenerationData() { BaseTileTag="sand",TreeCount=ARID_DESERT_TREE_COUNT,TreeTag="cactus" });
            details.Add(GlobalBiome.DENSE_FOREST, new WildernessGenerationData() { BaseTileTag = "grass", TreeCount = TREE_AMOUNT_FOREST, TreeTag = "tree" });
            details.Add(GlobalBiome.GRASSLAND, new WildernessGenerationData() { BaseTileTag = "grass", TreeCount = ARID_DESERT_TREE_COUNT, TreeTag = "tree" });
            details.Add(GlobalBiome.WOODLAND, new WildernessGenerationData() { BaseTileTag = "grass", TreeCount = TREE_AMOUNT_WOODLAND, TreeTag = "tree" });
            details.Add(GlobalBiome.RAINFOREST, new WildernessGenerationData() { BaseTileTag = "jungle", TreeCount = TREE_AMOUNT_FOREST, TreeTag = "jungle tree" });
            details.Add(GlobalBiome.POLAR_DESERT, new WildernessGenerationData() { BaseTileTag = "snow", TreeCount = ARID_DESERT_TREE_COUNT, TreeTag = "dead tree" });
            details.Add(GlobalBiome.POLAR_FOREST, new WildernessGenerationData() { BaseTileTag = "snow", TreeCount = TREE_AMOUNT_WOODLAND, TreeTag = "snow tree" });
            details.Add(GlobalBiome.WETLAND, new WildernessGenerationData() { BaseTileTag = "swamp", TreeCount = TREE_AMOUNT_WOODLAND, TreeTag = "jungle tree" });
            details.Add(GlobalBiome.GARIGUE, new WildernessGenerationData() { BaseTileTag = "rocky", TreeCount = ARID_DESERT_TREE_COUNT, TreeTag = "shrub" });
        }

        /// <summary>
        /// Generates a map with a particular biome
        /// </summary>
        /// <param name="herdAmount">The total amount of herds to generate</param>
        /// <param name="BanditAmount">The total amount of bandits to generate.</param>
        /// <param name="actors"></param>
        /// <returns></returns>
        public static MapBlock[,] GenerateMap(GlobalBiome biome, int herdAmount, int banditAmount, out Actor[] actors, out MapCoordinate startPoint)
        {
            MapBlock[,] map = new MapBlock[MAP_EDGE, MAP_EDGE];

            Random random = new Random();

            ItemFactory.ItemFactory factory = new ItemFactory.ItemFactory();

            int tileID = 0;

            factory.CreateItem(Archetype.TILES, details[biome].BaseTileTag, out tileID);

            //Create a new map which is edge X edge in dimensions and made of the base tile
            for (int x = 0; x < MAP_EDGE; x++)
            {
                for (int y = 0; y < MAP_EDGE; y++)
                {
                    MapBlock block = new MapBlock();
                    map[x, y] = block;
                    block.Tile = factory.CreateItem("tile", tileID);
                    block.Tile.Coordinate = new MapCoordinate(x, y, 0, MapType.LOCAL);
                }
            }

            #region Leave Town Item

            //Now select all the border tiles and put in a "Exit here" border
            for (int x = 0; x < map.GetLength(0); x++)
            {
                MapCoordinate coo = new MapCoordinate(x, 0, 0, MapType.LOCAL);

                LeaveTownItem lti = new LeaveTownItem();
                lti.Coordinate = coo;
                lti.Description = "continue on your journey";
                lti.Name = "Leave Area";

                lti.Coordinate = coo;

                map[x, 0].ForcePutItemOnBlock(lti);

                coo = new MapCoordinate(x, map.GetLength(1) - 1, 0, MapType.LOCAL);

                lti = new LeaveTownItem();
                lti.Coordinate = coo;
                lti.Description = "continue on your journey";
                lti.Name = "Leave Area";

                lti.Coordinate = coo;

                map[x, map.GetLength(1) - 1].ForcePutItemOnBlock(lti);

            }

            for (int y = 0; y < map.GetLength(1); y++)
            {
                MapCoordinate coo = new MapCoordinate(0, y, 0, MapType.LOCAL);

                LeaveTownItem lti = new LeaveTownItem();
                lti.Coordinate = coo;
                lti.Description = "continue on your journey";
                lti.Name = "Leave Area";

                lti.Coordinate = coo;

                map[0, y].ForcePutItemOnBlock(lti);

                coo = new MapCoordinate(map.GetLength(0) - 1, y, 0, MapType.LOCAL);

                lti = new LeaveTownItem();
                lti.Coordinate = coo;
                lti.Description = "continue on your journey";
                lti.Name = "Leave Area";

                lti.Coordinate = coo;

                map[map.GetLength(0) - 1, y].ForcePutItemOnBlock(lti);
            }

            #endregion

            #region Desert Oasis
            if (biome == GlobalBiome.ARID_DESERT)
            {
                //Let's create a pool of water towards one of the corners.
                int randomNumber = random.Next(2);

                int rXCoord = 0;

                if (randomNumber == 1)
                {
                    //Lower
                    rXCoord = random.Next(0, MAP_EDGE / 3);
                }
                else
                {
                    rXCoord = random.Next(2 * MAP_EDGE / 3, MAP_EDGE);
                }

                randomNumber = random.Next(2);

                int rYCoord = 0;

                if (randomNumber == 1)
                {
                    //Lower
                    rYCoord = random.Next(0, MAP_EDGE / 3);
                }
                else
                {
                    rYCoord = random.Next(2 * MAP_EDGE / 3, MAP_EDGE);
                }

                //The pool will have a radius of 3

                MapCoordinate coo = new MapCoordinate(rXCoord, rYCoord, 0, MapType.LOCAL);

                //Go through the blocks with a radius of 3
                var oasisBlocks = map.Cast<MapBlock>().ToArray().Where(b => Math.Abs(b.Tile.Coordinate - coo) <= 3).ToArray();

                int waterTile = -1;

                factory.CreateItem(Archetype.TILES, "water", out waterTile);

                foreach (var block in oasisBlocks)
                {
                    var coord = block.Tile.Coordinate;
                    block.Tile = factory.CreateItem("tile", waterTile);
                    block.Tile.Coordinate = coord;
                }

                var aroundOasis = map.Cast<MapBlock>().ToArray().Where(b => Math.Abs(b.Tile.Coordinate - coo) <= 4 && Math.Abs(b.Tile.Coordinate - coo) > 3).ToArray();

                int dummy;

                int grassTile = 0;

                factory.CreateItem(Archetype.TILES, "grass", out grassTile);

                foreach (var block in aroundOasis)
                {
                    var coord = block.Tile.Coordinate;
                    block.Tile = factory.CreateItem("tile", grassTile);
                    block.Tile.Coordinate = coord;
                }

                //Put in some trees around the pool
                for (int i = 0; i < 3; i++)
                {
                    MapItem tree = factory.CreateItem(Archetype.MUNDANEITEMS, "jungle tree", out dummy);

                    var block = aroundOasis[random.Next(aroundOasis.Length)];

                    if (block.MayContainItems)
                    {
                        //Drop it
                        block.ForcePutItemOnBlock(tree);
                    }
                }

            }
            #endregion

            #region Wetland Splotches

            if (biome == GlobalBiome.WETLAND)
            {
                int waterTile = -1;

                factory.CreateItem(Archetype.TILES, "water", out waterTile);

                for (int i=0; i < 7; i++)
                {
                    MapBlock rBlock = map[random.Next(map.GetLength(0)), random.Next(map.GetLength(1))];

                    Rectangle safeRect = new Rectangle( MAP_EDGE/2 - 5,MAP_EDGE/2 -5,10,10); 

                    if (safeRect.Contains(rBlock.Tile.Coordinate.X,rBlock.Tile.Coordinate.Y))
                    {
                        continue; //Not here!
                    }

                    int size = random.Next(1, 3);

                    //Get all the tiles around the block for a particular size
                    var pool = map.Cast<MapBlock>().ToArray().Where(b => Math.Abs(b.Tile.Coordinate - rBlock.Tile.Coordinate) <= size).ToArray();

                    foreach(var block in pool)
                    {
                        MapCoordinate coo = block.Tile.Coordinate;
                        block.Tile = factory.CreateItem("tiles", waterTile);
                        block.Tile.Coordinate = coo;
                    }
                }
            }

            #endregion

            for (int i = 0; i < details[biome].TreeCount; i++)
            {
                int treeID = 0;
                MapItem item = null;

                item = factory.CreateItem(Archetype.MUNDANEITEMS, details[biome].TreeTag, out treeID);

                //try 50 times to put it somewhere
                int tries = 0;

                while (tries < 50)
                {
                    MapBlock randomBlock = map[random.Next(map.GetLength(0)), random.Next(map.GetLength(1))];

                    if (randomBlock.MayContainItems)
                    {
                        randomBlock.ForcePutItemOnBlock(item);
                        break;
                    }

                    tries++;
                }
            }

            List<Actor> actorList = new List<Actor>();

            //There, now that's done, lets generate some animals
            if (herdAmount + banditAmount > 0)
            {
                var herds = ActorGeneration.CreateAnimalHerds(biome, false, null, herdAmount);
                var bandits = CampGenerator.CreateBandits(banditAmount);

                var actorGroups = herds.Union(bandits);

                //Each actor group will be placed in a random 3 radius circle

                foreach (var actorGroup in actorGroups)
                {
                    MapBlock randomBlock = map[random.Next(map.GetLength(0)), random.Next(map.GetLength(1))];

                    Rectangle wanderRect = new Rectangle(randomBlock.Tile.Coordinate.X - 2, randomBlock.Tile.Coordinate.Y - 2, 4, 4);

                    //Put the actor groups somewhere around that block
                    var blocks = map.Cast<MapBlock>().ToArray().Where(b => Math.Abs(b.Tile.Coordinate - randomBlock.Tile.Coordinate) < 4).ToArray();

                    //Pick a number of random blocks

                    foreach (var newActor in actorGroup)
                    {
                        int tries = 0;

                        while (tries < 50)
                        {
                            var actorBlock = blocks[random.Next(blocks.Length)];

                            if (actorBlock.MayContainItems)
                            {
                                //Put it there
                                newActor.MapCharacter.Coordinate = actorBlock.Tile.Coordinate;
                                actorBlock.ForcePutItemOnBlock(newActor.MapCharacter);

                                //Make them wander
                                newActor.MissionStack.Push(new WanderMission() { LoiterPercentage = 50, WanderPoint = actorBlock.Tile.Coordinate, WanderRectangle = wanderRect });

                                actorList.Add(newActor);

                                break;
                            }

                            tries++;
                        }
                    }
                }

            }

            //Drop the player in the thick of it
            MapBlock center = map[map.GetLength(0) / 2, map.GetLength(1) / 2];

            center.RemoveTopItem(); //Remove it if someone else wanted it

            startPoint = center.Tile.Coordinate;

            actors = actorList.ToArray();

            return map;
        }
    }
}
