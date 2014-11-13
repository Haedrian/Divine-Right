﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.Database;
using DRObjects.Enums;
using DRObjects.Items.Tiles;
using DRObjects.LocalMapGeneratorObjects;
using DRObjects.Sites;
using Microsoft.Xna.Framework;

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
        public static MapBlock[,] GenerateSite(SiteData siteData, out Actor[] actors)
        {
            MapCoordinate startPoint = null;

            //First we generate some empty wilderness of the right type
            MapBlock[,] map = WildernessGenerator.GenerateMap(siteData.Biome, 0, 0,out actors,out startPoint);

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

            Maplet maplet = parser.ParseMapletFromTag(siteData.SiteTypeData.SiteType.ToString().Replace("_"," ").ToLower());

            var tileID = DatabaseHandling.GetItemIdFromTag(Archetype.TILES,WildernessGenerator.details[siteData.Biome].BaseTileTag);

            MapletActorWanderArea[] wanderAreas = null;

            //Now generate the actual map
            MapBlock[,] siteMap = lmg.GenerateMap(tileID, null, maplet, false, "", siteData.Owners, out actors, out wanderAreas);

            foreach(var actor in actors)
            {
                if (actor.CurrentMission!= null && actor.CurrentMission.GetType() == typeof(WanderMission))
                {
                    WanderMission wMiss = actor.CurrentMission as WanderMission;

                    wMiss.WanderPoint.X += 4;
                    wMiss.WanderPoint.Y += 4;

                    wMiss.WanderRectangle = new Rectangle(wMiss.WanderRectangle.X + 4, wMiss.WanderRectangle.Y + 4, wMiss.WanderRectangle.Width, wMiss.WanderRectangle.Height);
                }
            }

            foreach(var area in wanderAreas)
            {
                area.WanderRect = new Rectangle(area.WanderRect.X + 4, area.WanderRect.Y + 4, area.WanderRect.Width, area.WanderRect.Height);
            }

            

            //Now lets fuse the maps
            map = lmg.JoinMaps(map, siteMap, 4, 4);

            map[wanderAreas[0].WanderRect.X, wanderAreas[0].WanderRect.Y].RemoveAllItems();
            map[wanderAreas[0].WanderRect.X, wanderAreas[0].WanderRect.Y].ForcePutItemOnBlock(new Air());

            return map;
        }

        /// <summary>
        /// Regenerates a site's actors based on the sitedata. If the owner hasn't changed, will do nothing
        /// </summary>
        /// <param name="sitedata"></param>
        /// <param name="currentMap"></param>
        /// <param name="actors"></param>
        /// <returns></returns>
        public static MapBlock[,] RegenerateSite(SiteData sitedata,MapBlock[,] currentMap,Actor[] currentActors,out Actor[] actors)
        {
            throw new NotImplementedException();
        }
    }
}
