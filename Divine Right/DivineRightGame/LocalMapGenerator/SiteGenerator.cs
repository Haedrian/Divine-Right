using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame.ActorHandling;
using DivineRightGame.Pathfinding;
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

            //Now, clear the tiles from between 5,5 till 25,25
            for (int x = 5; x < 26; x++)
            {
                for (int y = 5; y < 26; y++)
                {
                    MapBlock block = map[x, y];
                    block.RemoveAllItems();
                }
            }

            ItemFactory.ItemFactory itemFactory = new ItemFactory.ItemFactory();

            int waterID = 0;

            MapItem waterTile = itemFactory.CreateItem(Archetype.TILES, "water", out waterID);

            //If it's a fishing village, put some water in
            if (siteData.SiteTypeData.SiteType == SiteType.FISHING_VILLAGE)
            {
                for (int x = 0; x < map.GetLength(0); x++)
                {
                    for (int y = map.GetLength(1) - 10; y < map.GetLength(1); y++)
                    {
                        MapBlock block = map[x, y];
                        block.RemoveAllItems();

                        //Set the tile to water
                        block.Tile = itemFactory.CreateItem("tiles", waterID);
                        block.Tile.Coordinate = new MapCoordinate(x, y, 0, MapType.LOCAL);
                    }
                }
            }

            LocalMapGenerator lmg = new LocalMapGenerator();

            LocalMapXMLParser parser = new LocalMapXMLParser();

            Maplet maplet = parser.ParseMapletFromTag(siteData.SiteTypeData.SiteType.ToString().Replace("_", " ").ToLower());

            var tileID = DatabaseHandling.GetItemIdFromTag(Archetype.TILES, WildernessGenerator.details[siteData.Biome].BaseTileTag);

            MapletActorWanderArea[] wanderAreas = null;
            MapletPatrolPoint[] patrolPoints = null;
            MapletFootpathNode[] footPath = null;

            //Now generate the actual map
            MapBlock[,] siteMap = lmg.GenerateMap(tileID, null, maplet, false, "", siteData.Owners, out actors, out wanderAreas, out patrolPoints, out footPath);

            //Now lets fuse the maps
            map = lmg.JoinMaps(map, siteMap, 5, 5);

            foreach (var actor in actors)
            {
                if (actor.CurrentMission != null && actor.CurrentMission.GetType() == typeof(WanderMission))
                {
                    WanderMission wMiss = actor.CurrentMission as WanderMission;

                    wMiss.WanderPoint.X += 5;
                    wMiss.WanderPoint.Y += 5;

                    wMiss.WanderRectangle = new Rectangle(wMiss.WanderRectangle.X + 5, wMiss.WanderRectangle.Y + 5, wMiss.WanderRectangle.Width, wMiss.WanderRectangle.Height);
                }
            }

            //Fix the patrol points

            foreach (var point in patrolPoints)
            {
                point.Point.X += 5;
                point.Point.Y += 5;
            }

            //Let's fix the patrol points, we need to merge them into PatrolRoutes
            var patrolRoutes = PatrolRoute.GetPatrolRoutes(patrolPoints);

            foreach (var area in wanderAreas)
            {
                area.WanderRect = new Rectangle(area.WanderRect.X + 5, area.WanderRect.Y + 5, area.WanderRect.Width, area.WanderRect.Height);
            }

            //And fix the path nodes
            foreach (var pn in footPath)
            {
                pn.Point.X += 5;
                pn.Point.Y += 5;
            }

            //If the map already has any actors in it, make the characters prone
            foreach (var actor in actors)
            {
                actor.IsProne = true;
            }

            //Now generate the pathfinding map
            PathfinderInterface.Nodes = GeneratePathfindingMap(map);

            int pathTileID = -1;

            var dummy = itemFactory.CreateItem(Archetype.TILES, "stone", out pathTileID);

            //Go through each footpath node. Attempt to connect the node with the other primary nodes
            foreach (var fp in footPath)
            {
                foreach (var primary in footPath.Where(p => p.IsPrimary))
                {
                    //Join them up
                    var path = PathfinderInterface.GetPath(fp.Point, primary.Point);
                    if (path != null)
                    {
                        foreach (var coord in path)
                        {
                            MapBlock block = map[coord.X, coord.Y];

                            //Only do this if the tile isn't wood, or stone
                            if (!block.Tile.InternalName.ToUpper().Contains("STONE") && !block.Tile.InternalName.ToUpper().Contains("WOOD"))
                            {
                                block.Tile = itemFactory.CreateItem("tiles", pathTileID);
                                block.Tile.Coordinate = new MapCoordinate(coord);
                            }

                        }
                    }
                }
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

        /// <summary>
        /// Generates a pathfinding map from the mapl pushed in
        /// </summary>
        /// <returns></returns>
        private static byte[,] GeneratePathfindingMap(MapBlock[,] map)
        {
            //Generate a byte map of x and y
            int squareSize = PathfinderInterface.CeilToPower2(Math.Max(map.GetLength(0), map.GetLength(1)));

            byte[,] pf = new byte[squareSize, squareSize];

            for (int i = 0; i < map.GetLength(0); i++)
            {
                for (int j = 0; j < map.GetLength(1); j++)
                {
                    if (i < map.GetLength(0) - 1 && j < map.GetLength(1) - 1)
                    {
                        //Copyable - if it may contain items, put a weight of 1, otherwise an essagerated one. Also consider who the owners are. If it's owned by someone in particular then ignore it
                        pf[i, j] = map[i, j] != null ? map[i, j].MayContainItems || map[i,j].GetTopItem().OwnedBy != (OwningFactions.ABANDONED | OwningFactions.BANDITS | OwningFactions.HUMANS | OwningFactions.ORCS | OwningFactions.UNDEAD )
                            ? (byte)1 : Byte.MaxValue : Byte.MaxValue;
                    }
                    else
                    {
                        //Put in the largest possible weight
                        pf[i, j] = Byte.MaxValue;
                    }
                }
            }

            return pf;
        }
    }
}
