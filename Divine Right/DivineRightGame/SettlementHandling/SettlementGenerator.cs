using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Settlements;
using DRObjects.Items.Archetypes.Global;
using DRObjects;
using DRObjects.Settlements.Districts;
using DRObjects.Enums;
using DivineRightGame.SettlementHandling.Objects;
using DivineRightGame.LocalMapGenerator;
using DRObjects.LocalMapGeneratorObjects;
using DRObjects.LocalMapGeneratorObjects.Enums;
using DivineRightGame.ActorHandling;
using DRObjects.ActorHandling.ActorMissions;
using Microsoft.Xna.Framework;
using DRObjects.Graphics;
using System.Reflection;
using System.Threading;

namespace DivineRightGame.SettlementHandling
{
    /// <summary>
    /// Helper Class for generating settlements
    /// </summary>
    public static class SettlementGenerator
    {
        private static Random random = new Random();
        private const int MAXLOCATION = 9;
        private const int MAX_SMALL_LOCATION = 4;

        /// <summary>
        /// Generates a completly random settlement with completly random statistics at a particular location having a particular size
        /// </summary>
        /// <param name="globalCoordinates"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Settlement GenerateSettlement(MapCoordinate globalCoordinates, int size)
        {
            Settlement settlement = new Settlement();

            settlement.Coordinate = globalCoordinates.Clone();
            settlement.Name = SettlementNameGenerator.GenerateName();
            settlement.Description = "the settlement of " + settlement.Name;
            settlement.MayContainItems = false;
            settlement.SettlementSize = size;

            settlement.RichPercentage = random.Next(6) + random.Next(6) + random.Next(6);
            //settlement.RichPercentage = 50;

            settlement.MiddlePercentage = 0;

            for (int i = 0; i < 10; i++)
            {
                settlement.MiddlePercentage += random.Next(6);
            }

            settlement.PoorPercentage = 100 - settlement.RichPercentage - settlement.MiddlePercentage;

            //Generate the districts
            settlement.Districts = GenerateDistricts(size);


            return settlement;
        }

        /// <summary>
        /// Generates a Map from a Settlement
        /// </summary>
        /// <param name="settlement"></param>
        /// <returns></returns>
        public static MapBlock[,] GenerateMap(Settlement settlement, out List<Actor> actors)
        {
            actors = new List<Actor>();

            //Create the Main empty map - 60x80
            MapBlock[,] mainMap = new MapBlock[60, 80];

            ItemFactory.ItemFactory factory = new ItemFactory.ItemFactory();

            int grassTileID = 0;

            factory.CreateItem(Archetype.TILES, "grass",out grassTileID);

            //Go through each square and create a grass tile
            for (int x = 0; x < mainMap.GetLength(0); x++)
            {
                for (int y = 0; y < mainMap.GetLength(1); y++)
                {
                    MapItem tile = factory.CreateItem("tile", grassTileID);
                    tile.Coordinate = new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

                    MapBlock block = new MapBlock();
                    block.Tile = tile;

                    mainMap[x, y] = block;
                }
            }

            //Now we need to see where everything goes
            LocalMapGenerator.LocalMapGenerator gen = new LocalMapGenerator.LocalMapGenerator();
            LocalMapXMLParser parser = new LocalMapXMLParser();
         
            foreach (SettlementBuilding district in settlement.Districts)
            {
                //For reach settlement building, create the appropriate map
                SettlementBuildingMap map = new SettlementBuildingMap();

                int x = 0;
                int y = 0;

                GetMapletLocation(out x, out y, district.LocationNumber);

                map.X = x;
                map.Y = y;
                map.District = district.District;

                //maps.Add(map);

                //Generate it
                Maplet borderMaplet = new Maplet()
                {
                    SizeX = 17,
                    SizeY = 17,
                    Walled = false,
                    Tiled = false,
                    TileTag= "Pavement",
                    MapletContents = new List<MapletContents>() 
                    {
                        new MapletContentsMaplet()
                        {
                            FirstFit = true,
                            Position = PositionAffinity.FIXED,
                            x = 1,
                            y = 1,
                            ProbabilityPercentage = 100,
                            //Maplet = parser.ParseMaplet(MapletDatabaseHandler.GetMapletByTagAndLevel(district.District.GetDistrictMapletTag(),district.District.Level)),
                            Maplet = parser.ParseMaplet(MapletDatabaseHandler.GetMapletByTagAndLevel(district.District.GetDistrictMapletTag(),district.District.Level)),
                            MaxAmount = 1
                        }

                    }                   
                    
                };

                //Fix the x and y of the contents so they're in the center 
                int mapletSizeX = (borderMaplet.MapletContents[0] as MapletContentsMaplet).Maplet.SizeX;
                int mapletSizeY = (borderMaplet.MapletContents[0] as MapletContentsMaplet).Maplet.SizeY;

                MapletContentsMaplet subMapletWrapper = (borderMaplet.MapletContents[0] as MapletContentsMaplet);

                subMapletWrapper.x = (17 - mapletSizeX) / 2;
                subMapletWrapper.y = (17 - mapletSizeY) / 2;

                //Depending on the location, we'll need to position them to touch the plaza

                if (district.LocationNumber == 0 || district.LocationNumber == 1 || district.LocationNumber == 2)
                {
                    subMapletWrapper.y = 1;
                }
                if (district.LocationNumber == 0 || district.LocationNumber == 3 || district.LocationNumber == 5)
                {
                    subMapletWrapper.x = (17 - mapletSizeX) -1;
                }
                if (district.LocationNumber == 5 || district.LocationNumber == 6 || district.LocationNumber == 7)
                {
                    subMapletWrapper.y = (17 - mapletSizeY) -1 ;
                }
                if (district.LocationNumber == 2 || district.LocationNumber == 4 || district.LocationNumber == 7)
                {
                    subMapletWrapper.x = 1;
                }

                Actor[] localAct = null;

                var gennedMap = gen.GenerateMap(grassTileID, null, borderMaplet , true,"human",out localAct);

                actors.AddRange(localAct);

                foreach (var actor in localAct)
                {
                    if (actor.MissionStack.Count() > 0)
                    {
                        WanderMission miss = actor.MissionStack.Peek() as WanderMission;

                        miss.WanderPoint.X += x;
                        miss.WanderPoint.Y += y;

                        miss.WanderRectangle = new Rectangle(miss.WanderRectangle.X + x, miss.WanderRectangle.Y + y, miss.WanderRectangle.Width, miss.WanderRectangle.Height);
                    }
                }

                //And join them into one map
                gen.JoinMaps(mainMap, gennedMap, x, y);

            }

            //Let's put the plaza in

            int plazaTile = 0;

            factory.CreateItem(Archetype.TILES, "Pavement", out plazaTile);

            int yShift = 0;

            if (settlement.Districts.Any(d => d.LocationNumber >= MAX_SMALL_LOCATION))
            {
                //Big plaza
                yShift = 0;
            }
            else
            {
                //Small plaza
                yShift = 21;
            }

            for (int x = 17; x < 15 + 21; x++)
            {
                for (int y = 17 + yShift; y < 18 + 36; y++)
                {
                    MapItem tile = factory.CreateItem("tile", plazaTile);
                    tile.Coordinate = new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

                    MapBlock block = new MapBlock();
                    block.Tile = tile;

                    mainMap[x, y] = block;
                }
            }

              //TODO: DECOR


            //Now we put some people in
            var plazaActors = GenerateTownsfolk(settlement);

            actors.AddRange(plazaActors);

            //Go through them one at a time and position them on the plaza
            Rectangle plazaRect = new Rectangle(18, 18 + yShift, 18, 35 - yShift);

            foreach (Actor actor in plazaActors)
            {
                var actorMapItem = actor.MapCharacter;

                //Drop them in a random location in the plaza
                int tries = 0;

                while (tries < 50)
                {
                    //If we do this more than 50 times, stop
                    int randomX = random.Next(35 - 18) + 18;
                    int randomY = random.Next(36 - yShift) + 18 + yShift;

                    //Can we put the character there ?
                    if (mainMap[randomX, randomY].MayContainItems)
                    {
                        //break
                        actorMapItem.Coordinate = new MapCoordinate(randomX, randomY, 0, MapTypeEnum.LOCAL);

                        //Put the item in
                        mainMap[randomX, randomY].ForcePutItemOnBlock(actorMapItem);

                        //Give him a mission
                        actor.MissionStack.Push(new WanderMission() { WanderPoint = new MapCoordinate(randomX, randomY, 0, MapTypeEnum.LOCAL), WanderRectangle = plazaRect, LoiterPercentage = random.Next(40) + 50 });

                        break;
                    }

                    tries++;
                }

            }


            //Trimming
            if (yShift > 0)
            {
                //We can split the map into an even smaller size
                ShrinkArray<MapBlock>(ref mainMap, 60, 0,50,30);

                //Fix the coordinates
                foreach (var blocks in mainMap)
                {
                    foreach (var item in blocks.GetItems())
                    {
                        item.Coordinate.Y -= 30;
                    }

                    blocks.Tile.Coordinate.Y -= 30;
                }

                foreach (Actor actor in actors)
                {
                    //Update the rectangle
                    WanderMission miss = actor.MissionStack.Peek() as WanderMission;

                    miss.WanderPoint.Y -= 30;
                    miss.WanderRectangle = new Rectangle(miss.WanderRectangle.X, miss.WanderRectangle.Y - 30, miss.WanderRectangle.Width, miss.WanderRectangle.Height);
                }
            }
                      
            return mainMap;
        }

        /// <summary>
        /// Generates the districts. For now just do this randomly. Later we'll want to know the resources and set a theme for each settlement
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public static List<SettlementBuilding> GenerateDistricts(int size)
        {
            List<District> districts = new List<District>(size);

            //Create the general store and the inn
            districts.Add(new District(DistrictType.INN, 1));
            districts.Add(new District(DistrictType.GENERAL_STORE, 1));

            var districtTypes = (DistrictType[])Enum.GetValues(typeof(DistrictType));

            //Generate the rest of it
            for (int i = 0; i < size - 2; i++)
            {
                //Pick one at random
                var type = districtTypes[random.Next(districtTypes.Length)];

                //Does the type exist already?
                var match = districts.Where(dt => dt.Type.Equals(type)).FirstOrDefault();

                if (match == null)
                {
                    //No match, create a new one
                    districts.Add(new District(type, 1));
                }
                else if (match.Level < 3)
                {
                    //Matched, increment the level
                    match.Level++;
                }
                else
                {
                    //Can't go any higher, try again
                    i--;
                }
            }

            //Now go through the districts and create SettlementBuildings for all of them

            int maxPosition = MAXLOCATION;

            //Do we have less than, or equal to 4 buildings?
            if (districts.Count <= 4)
            {
                maxPosition = MAX_SMALL_LOCATION; //its a small one
            }

            List<SettlementBuilding> buildings = new List<SettlementBuilding>();

            foreach (District district in districts)
            {
                int position = random.Next(maxPosition);

                //Is the slot empty?
                while (buildings.Any(b => b.LocationNumber.Equals(position)))
                {
                    position = random.Next(maxPosition);
                }

                //Found a clear one. Plop it there
                buildings.Add(new SettlementBuilding() { District = district, LocationNumber = position });
            }

            return buildings;
        }

        /// <summary>
        /// Gets the X and Y coordinate for a particular Maplet to be located at a particular location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="locationID"></param>
        private static void GetMapletLocation(out int x, out int y, int locationID)
        {
            //These are hard coded, nothing to do :(

            switch (locationID)
            {
                case 7:
                    x = 0;
                    y = 0;
                    break;
                case 8:
                    x = 18;
                    y = 0;
                    break;
                case 9:
                    x = 36;
                    y = 0;
                    break;
                case 5:
                    x = 0;
                    y = 18;
                    break;
                case 6:
                    x = 36;
                    y = 18;
                    break;
                case 3:
                    x = 0;
                    y = 36;
                    break;
                case 4:
                    x = 36;
                    y = 36;
                    break;
                case 0:
                    x = 0;
                    y = 54;
                    break;
                case 1:
                    x = 18;
                    y = 54;
                    break;
                case 2:
                    x = 36;
                    y = 54;
                    break;
                default:
                    throw new NotImplementedException("No idea where " + locationID + " fits");
            }
        }

        #region Helpers
        private static void ShrinkArray<T>(ref T[,] original, int newCoNum, int coOffset, int newRoNum, int roOffset)
        {
            var newArray = new T[newCoNum, newRoNum];
            int columnCount = original.GetLength(1);
            int columnCount2 = newRoNum;
            int columns = original.GetUpperBound(0);
            for (int co = 0; co < newCoNum; co++)
            {
                for (int row = 0; row < newRoNum; row++)
                {
                    newArray[co, row] = original[co + coOffset, row + roOffset];
                }
            }

            original = newArray;
        }
        #endregion

        /// <summary>
        /// Generates an amount of actors to populate the settlement. Does not put them on the map
        /// </summary>
        /// <param name="settlement"></param>
        /// <returns></returns>
        private static List<Actor> GenerateTownsfolk(Settlement settlement)
        {
            ItemFactory.ItemFactory fact = new ItemFactory.ItemFactory();

            //We'll create a population depending on the ratio of population, whether a barracks is built or not et cetera.

            int populationAmount = 0;

            if (settlement.SettlementSize > 5)
            {
                populationAmount = 10;
            }
            else
            {
                populationAmount = 5;
            }

            //Lets create population as per the ratio - then we add 1,2,3,4 population of guards

            List<Actor> actors = new List<Actor>();

            for (int i = 0; i < populationAmount; i++)
            {
                string socialClass = String.Empty;

                //Lets see what social class w'ere going to put them in 
                if (random.Next(100) < settlement.RichPercentage)
                {
                    //Rich
                    socialClass = "rich";
                }
                else if (random.Next(100) < settlement.MiddlePercentage)
                {
                    //Merchant caste
                    socialClass = "merchant";
                }
                else
                {
                    //Poor
                    socialClass = "poor";
                }

                //Create a new actor
                int actorID = 0;
                Actor act = ActorGeneration.CreateEnemy("human", socialClass, true, 5, out actorID);

                act.MapCharacter = fact.CreateItem("enemies", actorID);

                actors.Add(act);
            }

            //Now lets add some guards. Just how many?

            var barracks = settlement.Districts.Where(d => d.District.Type == DistrictType.BARRACKS).FirstOrDefault();

            int guardCount = 0;

            if (barracks == null)
            {
                //Just one
                guardCount = 1;
            }
            else
            {
                switch (barracks.District.Level)
                {
                    case 1:
                        guardCount = 2;
                        break;
                    case 2:
                        guardCount = 3;
                        break;
                    case 3:
                        guardCount = 4;
                        break;
                }
            }

            //Create them
            for (int i = 0; i <= guardCount; i++)
            {
                int actorID = 0;

                Actor actor = ActorGeneration.CreateEnemy("human", "guard", true, 10, out actorID);

                actor.MapCharacter = fact.CreateItem("enemies", actorID);

                actors.Add(actor);
            }

            return actors;
        }
    }
}
