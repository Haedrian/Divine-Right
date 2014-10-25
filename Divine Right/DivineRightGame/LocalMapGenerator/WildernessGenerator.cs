﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame.ActorHandling;
using DRObjects;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.Enums;
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
        private static int MAP_EDGE = 25;

        private static int TREE_AMOUNT_FOREST = 100;
        private static int TREE_AMOUNT_WOODLAND = 50;

        /// <summary>
        /// Generates a light forest wilderness
        /// </summary>
        /// <param name="herdAmount">The total amount of herds to generate</param>
        /// <param name="BanditAmount">The total amount of bandits to generate.</param>
        /// <param name="actors"></param>
        /// <returns></returns>
        public static MapBlock[,] GenerateMap(GlobalBiome biome ,int herdAmount, int banditAmount, out Actor[] actors,out MapCoordinate startPoint)
        {
            MapBlock[,] map = new MapBlock[MAP_EDGE, MAP_EDGE];

            Random random = new Random();

            if (biome == GlobalBiome.DENSE_FOREST || biome == GlobalBiome.WOODLAND || biome == GlobalBiome.RAINFOREST)
            {
                ItemFactory.ItemFactory factory = new ItemFactory.ItemFactory();

                int tileID = 0;

                if (biome == GlobalBiome.DENSE_FOREST || biome == GlobalBiome.WOODLAND)
                {
                    factory.CreateItem(Archetype.TILES, "grass", out tileID);
                }
                else if (biome == GlobalBiome.RAINFOREST)
                {
                    factory.CreateItem(Archetype.TILES, "jungle", out tileID);
                }
                //Create a new map which is edge X edge in dimensions and made of grass
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

                //Since this is a forest, let's put in a bunch of trees
                for (int i = 0; i < (biome == GlobalBiome.DENSE_FOREST || biome == GlobalBiome.RAINFOREST ? TREE_AMOUNT_FOREST : TREE_AMOUNT_WOODLAND); i++)
                {
                    int treeID = 0;
                    MapItem item = null;

                    if (biome == GlobalBiome.DENSE_FOREST || biome == GlobalBiome.WOODLAND)
                    {
                        item = factory.CreateItem(Archetype.MUNDANEITEMS, "tree", out treeID);
                    }
                    else if (biome == GlobalBiome.RAINFOREST)
                    {
                        item = factory.CreateItem(Archetype.MUNDANEITEMS, "jungle tree", out treeID);
                    }

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
            }
            List<Actor> actorList = new List<Actor>();

            //There, now that's done, lets generate some animals
            if (herdAmount > 0)
            {
                var herds = ActorGeneration.CreateAnimalHerds(biome, false, herdAmount);

                //Each herd will be placed in a random 3 radius circle

                foreach (var herd in herds)
                {
                    MapBlock randomBlock = map[random.Next(map.GetLength(0)), random.Next(map.GetLength(1))];

                    Rectangle wanderRect = new Rectangle(randomBlock.Tile.Coordinate.X - 2, randomBlock.Tile.Coordinate.Y - 2, 4, 4);

                    //Put the herd animals somewhere around that block
                    var blocks = map.Cast<MapBlock>().ToArray().Where(b => Math.Abs(b.Tile.Coordinate - randomBlock.Tile.Coordinate) < 4).ToArray();
                    
                    //Pick a number of random blocks

                    foreach (var animal in herd)
                    {
                        int tries = 0;

                        while (tries < 50)
                        {
                            var animalBlock = blocks[random.Next(blocks.Length)];

                            if (animalBlock.MayContainItems)
                            {
                                //Put it there
                                animal.MapCharacter.Coordinate = animalBlock.Tile.Coordinate;
                                animalBlock.ForcePutItemOnBlock(animal.MapCharacter);

                                //Make them wander
                                animal.MissionStack.Push(new WanderMission() { LoiterPercentage = 50,WanderPoint = animalBlock.Tile.Coordinate,WanderRectangle = wanderRect });

                                actorList.Add(animal);

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