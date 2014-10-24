using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame.ActorHandling;
using DRObjects;
using DRObjects.Enums;

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
        private static int MAP_EDGE = 25;

        private static int TREE_AMOUNT_FOREST = 100;

        /// <summary>
        /// Generates a light forest wilderness
        /// </summary>
        /// <param name="herdAmount">The total amount of herds to generate</param>
        /// <param name="BanditAmount">The total amount of bandits to generate.</param>
        /// <param name="actors"></param>
        /// <returns></returns>
        public static MapBlock[,] GenerateWoodland(int herdAmount, int banditAmount, out Actor[] actors)
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

            //Since this is a forest, let's put in a bunch of trees
            for (int i=0; i < TREE_AMOUNT_FOREST; i++)
            {
                int treeID = 0;
                MapItem item = factory.CreateItem(Archetype.MUNDANEITEMS, "tree",out treeID);

                //try 50 times to put it somewhere
                int tries = 0;

                while(tries < 50)
                {
                    MapBlock randomBlock = map[random.Next(map.GetLength(0)),random.Next(map.GetLength(1))];

                    if (randomBlock.MayContainItems)
                    {
                        randomBlock.ForcePutItemOnBlock(item);
                    }
                }
            }

            //There, now that's done, lets generate some animals
            if (herdAmount > 0)
            {
                var herds = ActorGeneration.CreateAnimalHerds(GlobalBiome.WOODLAND, false, herdAmount);

                //Each herd will be placed in a random 5x5 rectangle

                foreach (var herd in herds)
                { 
                    
                }

            }

        }
    }
}
