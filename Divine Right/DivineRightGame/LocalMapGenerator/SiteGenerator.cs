using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame.ActorHandling;
using DRObjects;
using DRObjects.ActorHandling;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.ActorHandling.CharacterSheet.Enums;
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
            MapBlock[,] map = WildernessGenerator.GenerateMap(siteData.Biome, 0, 0, out actors, out startPoint);

            //Now, clear the tiles from between 4,4 till 24,24
            for (int x = 4; x < 25; x++)
            {
                for (int y = 4; y < 25; y++)
                {
                    MapBlock block = map[x, y];
                    block.RemoveAllItems();
                }
            }

            LocalMapGenerator lmg = new LocalMapGenerator();

            LocalMapXMLParser parser = new LocalMapXMLParser();

            Maplet maplet = parser.ParseMapletFromTag(siteData.SiteTypeData.SiteType.ToString().Replace("_", " ").ToLower());

            var tileID = DatabaseHandling.GetItemIdFromTag(Archetype.TILES, WildernessGenerator.details[siteData.Biome].BaseTileTag);

            MapletActorWanderArea[] wanderAreas = null;
            MapletPatrolPoint[] patrolPoints = null;

            //Now generate the actual map
            MapBlock[,] siteMap = lmg.GenerateMap(tileID, null, maplet, false, "", siteData.Owners, out actors, out wanderAreas, out patrolPoints);

            //Now lets fuse the maps
            map = lmg.JoinMaps(map, siteMap, 4, 4);

            foreach (var actor in actors)
            {
                if (actor.CurrentMission != null && actor.CurrentMission.GetType() == typeof(WanderMission))
                {
                    WanderMission wMiss = actor.CurrentMission as WanderMission;

                    wMiss.WanderPoint.X += 4;
                    wMiss.WanderPoint.Y += 4;

                    wMiss.WanderRectangle = new Rectangle(wMiss.WanderRectangle.X + 4, wMiss.WanderRectangle.Y + 4, wMiss.WanderRectangle.Width, wMiss.WanderRectangle.Height);
                }
            }

             //Fix the patrol points

            foreach(var point in patrolPoints)
            {
                point.Point.X += 4;
                point.Point.Y += 4;
            }

            //Let's fix the patrol points, we need to merge them into PatrolRoutes
            var patrolRoutes = PatrolRoute.GetPatrolRoutes(patrolPoints);

            foreach (var area in wanderAreas)
            {
                area.WanderRect = new Rectangle(area.WanderRect.X + 4, area.WanderRect.Y + 4, area.WanderRect.Width, area.WanderRect.Height);
            }

            List<Actor> actorList = new List<Actor>();

            //Let's generate a number of actors then
            foreach (ActorProfession profession in Enum.GetValues(typeof(ActorProfession)))
            {
                //So do we have any wander areas for them ?
                var possibleAreas = wanderAreas.Where(wa => wa.Factions.HasFlag(siteData.Owners) && wa.Profession.Equals(profession));
                var possibleRoutes = patrolRoutes.Where(pr => pr.Owners.HasFlag(siteData.Owners) && pr.Profession.Equals(profession));

                //Any actors?
                if (siteData.ActorCounts.ContainsKey(profession))
                {
                    //Yes, how many
                    int total = siteData.ActorCounts[profession];

                    var a = ActorGeneration.CreateActors(siteData.Owners, profession, total);

                    actorList.AddRange(a);

                    foreach (var actor in a)
                    {
                        //So, where we going to drop them off ? Randomly
                        int tries = 0;

                        for (; ; )
                        {
                            int randomX = GameState.Random.Next(map.GetLength(0));
                            int randomY = GameState.Random.Next(map.GetLength(1));

                            if (map[randomX, randomY].MayContainItems)
                            {
                                //Plop it on there
                                actor.MapCharacter.Coordinate = new MapCoordinate(randomX, randomY, 0, MapType.LOCAL);
                                map[randomX, randomY].ForcePutItemOnBlock(actor.MapCharacter);
                                tries = 0;
                                break;
                            }
                            else
                            {
                                tries++;
                            }

                            if (tries >= 150)
                            {
                                //give up
                                break;
                            }
                        }

                        //Go through each actor, and either tell them to wander in the whole map, or within any possible area which matches
                        //Any possible area avaialble?
                        List<object> possibleMissions = new List<object>(); //I know :( But Using an interface or trying to mangle together an inheritance was worse

                        possibleMissions.AddRange(possibleAreas.Where(pa => pa.MaxAmount > pa.CurrentAmount));
                        possibleMissions.AddRange(possibleRoutes);

                        var chosenArea = possibleMissions.OrderBy(pa => GameState.Random.Next(100)).FirstOrDefault();

                        if (chosenArea == null)
                        {
                            //Wander around the whole map
                            actor.CurrentMission = new WanderMission() { LoiterPercentage = 25, WanderPoint = new MapCoordinate(map.GetLength(0) / 2, map.GetLength(1) / 2, 0, MapType.LOCAL), WanderRectangle = new Rectangle(0, 0, map.GetLength(0), map.GetLength(1)) };
                        }
                        else
                        {
                            //Is this a patrol or a wander ?
                            if (chosenArea.GetType().Equals(typeof(PatrolRoute)))
                            {
                                var patrolDetails = chosenArea as PatrolRoute;

                                PatrolRouteMission pm = new PatrolRouteMission();
                                pm.PatrolRoute.AddRange(patrolDetails.Route);

                                actor.CurrentMission = pm;
                            }
                            else if (chosenArea.GetType().Equals(typeof(MapletActorWanderArea)))
                            {
                                var wanderDetails = chosenArea as MapletActorWanderArea;

                                //Wander around in that area
                                actor.CurrentMission = new WanderMission() { LoiterPercentage = 25, WanderPoint = new MapCoordinate(wanderDetails.WanderPoint), WanderRectangle = wanderDetails.WanderRect };
                                wanderDetails.CurrentAmount++;
                            }
                        }

                    }

                }

            }

            actors = actorList.ToArray();

            return map;
        }

        /// <summary>
        /// Regenerates a site's actors based on the sitedata. If the owner hasn't changed, will do nothing
        /// </summary>
        /// <param name="sitedata"></param>
        /// <param name="currentMap"></param>
        /// <param name="actors"></param>
        /// <returns></returns>
        public static MapBlock[,] RegenerateSite(SiteData sitedata, MapBlock[,] currentMap, Actor[] currentActors, out Actor[] actors)
        {
            throw new NotImplementedException();
        }
    }
}
