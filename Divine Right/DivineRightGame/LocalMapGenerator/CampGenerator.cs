using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Enums;
using Microsoft.Xna.Framework;
using DRObjects.LocalMapGeneratorObjects;
using DRObjects.Items.Archetypes.Local;

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

        /// <summary>
        /// Generates a camp
        /// </summary>
        /// <returns></returns>
        public static MapBlock[,] GenerateCamp(out MapCoordinate startPoint, out DRObjects.Actor[] enemyArray)
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
            int endCoord = MAP_EDGE - ((MAP_EDGE -  FORTIFICATION_EDGE)/2);

            for (int x = startCoord+1; x < endCoord; x++)
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

            for (int y = startCoord+1; y < endCoord; y++)
            {
                MapBlock block = map[startCoord, y];
                MapItem item = factory.CreateItem("mundaneitems", pallisadeID);

                block.ForcePutItemOnBlock(item);

                block = map[endCoord,y];
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
                if (randomBlock.Tile.Coordinate.X >= startCoord && randomBlock.Tile.Coordinate.X <= endCoord && randomBlock.Tile.Coordinate.Y >= startCoord && randomBlock.Tile.Coordinate.Y <= endCoord )
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


            startPoint = new MapCoordinate(map.GetLength(0) / 2, 0, 0, MapType.LOCAL);
            enemyArray = null;
            return map;


        }
    }
}
