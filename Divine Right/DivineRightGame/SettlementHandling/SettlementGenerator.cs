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

namespace DivineRightGame.SettlementHandling
{
    /// <summary>
    /// Helper Class for generating settlements
    /// </summary>
    public static class SettlementGenerator
    {
        private static Random random = new Random();
        private const int MAXLOCATION = 11;

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
        public static MapBlock[,] GenerateMap(Settlement settlement)
        {
            //Create the Main empty map - 150x 150
            MapBlock[,] mainMap = new MapBlock[150, 150];

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
            ////List<SettlementBuildingMap> maps = new List<SettlementBuildingMap>();

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
                var gennedMap = gen.GenerateMap(grassTileID, null, parser.ParseMapletFromTag(("House")), true);
            
                //And join them into one map
                gen.JoinMaps(mainMap, gennedMap, x, y);

            }

            //Let's put the plaza in

            int plazaTile = 0;

            factory.CreateItem(Archetype.TILES, "Pavement", out plazaTile);

            for (int x = 45; x <= 70+45; x++)
            {
                for (int y = 45; y < 70+45; y++)
                {
                    MapItem tile = factory.CreateItem("tile", plazaTile);
                    tile.Coordinate = new MapCoordinate(x, y, 0, DRObjects.Enums.MapTypeEnum.LOCAL);

                    MapBlock block = new MapBlock();
                    block.Tile = tile;

                    mainMap[x, y] = block;
                }
            }


            //TODO: PEOPLE

            

            //TODO: DECOR
            

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

            var districtTypes = (DistrictType[]) Enum.GetValues(typeof(DistrictType));

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

            List<SettlementBuilding> buildings = new List<SettlementBuilding>();

            foreach (District district in districts)
            {
                int position = random.Next(MAXLOCATION);

                //Is the slot empty?
                while (buildings.Any(b => b.LocationNumber.Equals(position)))
                {
                    position = random.Next(MAXLOCATION);
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

            switch(locationID)
            {
                case 0:
                    x = 45;
                    y = 135;
                    break;
                case 1:
                    x = 70;
                    y = 135;
                    break;
                case 2:
                    x = 95;
                    y = 135;
                    break;
                case 3:
                    x = 115;
                    y = 115;
                    break;
                case 4:
                    x = 115;
                    y = 95;
                    break;
                case 5:
                    x = 115;
                    y = 70;
                    break;
                case 6:
                    x = 95;
                    y = 45;
                    break;
                case 7:
                    x = 70;
                    y = 45;
                    break;
                case 8:
                    x = 45;
                    y = 45;
                    break;
                case 9:
                    x = 25;
                    y = 70;
                    break;
                case 10:
                    x = 25;
                    y = 95;
                    break;
                case 11:
                    x = 25;
                    y = 115;
                    break;
                default:
                    throw new NotImplementedException("No idea where " + locationID + " fits");
            }
        }
    }
}
