using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Database;
using DRObjects.Enums;
using DRObjects.LocalMapGeneratorObjects;

namespace DivineRightGame.LocalMapGenerator
{
    /// <summary>
    /// Code for generating a site on the map
    /// </summary>
    public static class SiteGenerator
    {
        /// <summary>
        /// Generates a site
        /// </summary>
        /// <param name="siteType"></param>
        /// <param name="biomeType"></param>
        /// <param name="owner"></param>
        /// <param name="startPoint"></param>
        /// <param name="actors"></param>
        /// <returns></returns>
        public static MapBlock[,] GenerateSite(SiteType siteType,GlobalBiome biome,OwningFactions owner, out Actor[] actors)
        {
            MapCoordinate startPoint = null;

            //First we generate some empty wilderness of the right type
            MapBlock[,] map = WildernessGenerator.GenerateMap(biome, 0, 0,out actors,out startPoint);

            //Now, clear the tiles from between 4,4 till 24,24
            for(int x = 4; x < 25; x++)
            {
                for (int y = 4; y < 25; y++)
                {
                    MapBlock block = map[x, y];
                    block.RemoveAllItems();
                }
            }

            LocalMapGenerator lmg = new LocalMapGenerator();

            LocalMapXMLParser parser = new LocalMapXMLParser();

            Maplet maplet = parser.ParseMapletFromTag(siteType.ToString().Replace("_"," ").ToLower());

            var tileID = DatabaseHandling.GetItemIdFromTag(Archetype.TILES,WildernessGenerator.details[biome].BaseTileTag);

            //Now generate the actual map
            MapBlock[,] siteMap = lmg.GenerateMap(tileID, null, maplet, false, "", owner, out actors);

            //Now lets fuse the maps
            map = lmg.JoinMaps(map, siteMap, 4, 4);

            return map;
        }
    }
}
