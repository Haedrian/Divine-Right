using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Enums;
using Microsoft.Xna.Framework;
using DRObjects.LocalMapGeneratorObjects;

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

            startPoint = new MapCoordinate(0, 0, 0, MapType.LOCAL);
            enemyArray = null;
            return map;


        }
    }
}
