using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame.Managers.HelperObjects;
using DRObjects;
using DRObjects.Items.Tiles.Global;
using DRObjects.Enums;
using DivineRightGame.Managers.HelperFunctions;
using DRObjects.Items.Archetypes.Global;
using DivineRightGame.SettlementHandling;
using DivineRightGame.LocalMapGenerator;
using DivineRightGame.ActorHandling;
using DivineRightGame.Pathfinding;
using DivineRightGame.CivilisationHandling;
using DRObjects.CivilisationHandling;

namespace DivineRightGame.Managers
{
    public class WorldGenerationManager
    {
        /// <summary>
        /// How many tiles each side of the world
        /// </summary>
        public const int WORLDSIZE = 250;
        public const int EXPONENTWEIGHT = 2;
        public const int RIVERCOUNT = 50;
        public const int RAINCENTERCOUNT = 6;

        public const int RESOURCES_PER_TYPE = 10;

        public const int HUMAN_CAPITAL_COUNT = 4;
        public const int HUMAN_SETTLEMENTS_PER_CIVILIZATION = 7;
        public const int HUMAN_CAPITAL_SEARCH_RADIUS = 150;
        public const int HUMAN_COLONY_SEARCH_RADIUS = 20;
        public const int HUMAN_COLONY_BLOCKING_RADIUS = 5;

        public const int HUMAN_COLONY_CLAIMING_RADIUS = 10;
        public const int DUNGEON_CLAIMING_RADIUS = 10;
        public const int BANDIT_CLAIMING_RADIUS = 4;

        public const int DUNGEON_TOTAL = 20;

        public const int BANDIT_TOTAL = 40;

        public const int REGIONSIZE = WORLDSIZE * 2;

        protected static Region[] regions;

        public static string CurrentStep = "";

        protected static bool isGenerating = false;
        /// <summary>
        /// Determines whether the code is generating or not. This will be used to tell the interface to do something else.
        /// All other things (saving et cetera) should be done by the time this flag is triggered
        /// </summary>
        public static bool IsGenerating
        {
            get
            {
                lock (GlobalMap.lockMe)
                {
                    return isGenerating;
                }
            }
        }
        /// <summary>
        /// Generates the global map
        /// </summary>
        public static void GenerateWorld()
        {
            isGenerating = true;
            Random random = new Random();

            CurrentStep = "And in the Beginning, he Pondered on what the world would become...";
            Initialisation();

            CurrentStep = "And then he decreed the world would be surrounded by a border of waters";
            GenerateBorder();

            CurrentStep = "And he pondered on the shape of the world";
            AssignRegions();

            CurrentStep = "And with a thought, the world took shape";
            AssignRegionSizes();

            CurrentStep = "And the earthquakes came, and the earth shifted";
            RoughSmooth();

            CurrentStep = "And then the winds came, and the world was eroded";
            ErodeWorld();

            MarkHillSlopes();

            CurrentStep = "And the rains began, and the rivers formed";
            GenerateRivers();

            CurrentStep = "And the sun shone upon the world, and it saw heat";
            GenerateTemperatures();

            CurrentStep = "With a single word, the storms came, bringing water to the lands...";
            GenerateRains();

            CurrentStep = "And then came life";
            DetermineBiomes();

            CurrentStep = "And some resources came to light";
            GenerateResources();

            CurrentStep = "And then he looked upon the land and determined the desires of man";
            DetermineDesirability();

            CurrentStep = "And the humans came and they colonised the land";
            ColoniseWorld();

            CurrentStep = "And the humans awoke the beasts and monsters of the land";
            CreateOrcCitadels();

            CurrentStep = "And some humans took to thievery";
            CreateBandits();

            CurrentStep = "And they harvested the resources of the land";
            HarvestResources();

            CurrentStep = "And they built roads and paths between their properties";
            GenerateRoads();

            CurrentStep = "And thus the world was done. [Enter] to continue";
            isGenerating = false;
        }

        #region World Generation Functions

        /// <summary>
        /// Initialises the World Map in preperation. Creates the regions with empty ones, and sets the default tile settings
        /// </summary>
        private static void Initialisation()
        {
            //start the map
            GameState.GlobalMap = new GlobalMap(WORLDSIZE);

            regions = new Region[REGIONSIZE + 1];

            //fill the regions with new regions

            for (int i = 0; i < regions.Length; i++)
            {
                regions[i] = new Region();
            }

            //populate the world map with a number of tiles with an elevation of 40


            for (int x = 0; x < WORLDSIZE; x++)
            {
                lock (GlobalMap.lockMe)
                { //this is rather inefficient, but it will allow the interface to draw something
                    //CurrentStep = "Populating Tiles for line :" + x;

                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        MapBlock block = new MapBlock();
                        block.Tile = new GlobalTile();
                        block.Tile.Coordinate = new MapCoordinate(x, y, 0, MapType.GLOBAL);

                        GlobalTile gTile = (block.Tile as GlobalTile);
                        gTile.Elevation = 40;
                        gTile.ClimateTemperature = 0;
                        gTile.HasHillSlope = false;
                        gTile.HasRiver = false;
                        gTile.Rainfall = 0;

                        GameState.GlobalMap.AddToGlobalMap(block);
                    }
                }
            }



        }
        /// <summary>
        /// Generates the border, and sets the elevation to underwater
        /// </summary>
        private static void GenerateBorder()
        {

            lock (GlobalMap.lockMe)
            {
                //set region 0, this is the border of the map

                for (int x = 0; x < WORLDSIZE; x++)
                {
                    for (int y = 0; y < WORLDSIZE / 100; y++)
                    {
                        regions[0].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL)).Tile as GlobalTile).Region = 0;

                        regions[0].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(y, x, 0, MapType.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(y, x, 0, MapType.GLOBAL)).Tile as GlobalTile).Region = 0;

                        regions[0].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(WORLDSIZE - 1 - y, x, 0, MapType.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(WORLDSIZE - 1 - y, x, 0, MapType.GLOBAL)).Tile as GlobalTile).Region = 0;

                        regions[0].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, WORLDSIZE - 1 - y, 0, MapType.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, WORLDSIZE - 1 - y, 0, MapType.GLOBAL)).Tile as GlobalTile).Region = 0;


                        regions[0].Center = new MapCoordinate(-1, -1, -1, MapType.GLOBAL);
                    }

                }
            }

            //setting region 0's properties - this will make deep oceans around the map

            lock (GlobalMap.lockMe)
            {
                CurrentStep = "And thus the border fell under the waves forever";

                for (int i = 0; i < regions[0].Blocks.Count; i++)
                {
                    //set them all to -100
                    (regions[0].Blocks[i].Tile as GlobalTile).Elevation = -100;
                }
            }
        }
        /// <summary>
        /// Assigns each location to a different region
        /// </summary>
        private static void AssignRegions()
        {
            Random random = new Random();

            lock (GlobalMap.lockMe)
            {

                //setting the region centres - 0 doesn't count
                for (int i = 1; i < regions.Length; i++)
                {
                    int x = random.Next(WORLDSIZE - 1);
                    int y = random.Next(WORLDSIZE - 1);

                    if ((GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL)).Tile as GlobalTile).Region == -1)
                    {
                        //not assigned

                        //set this location as the centre
                        regions[i].Center = new MapCoordinate(x, y, 0, MapType.GLOBAL);
                        regions[i].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL)).Tile as GlobalTile).Region = i;
                    }
                    else
                    {
                        i--; //try again
                    }

                }
            }

            for (int x = 0; x < WORLDSIZE; x++)
            {
                int assignedRegion = -1;

                for (int y = 0; y < WORLDSIZE; y++)
                {
                    //populate the regions
                    lock (GlobalMap.lockMe)
                    {
                        //Is this tile already in a region?

                        MapCoordinate currentCoordinate = new MapCoordinate(x, y, 0, MapType.GLOBAL);

                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(currentCoordinate);
                        //Find the closest region to the current unassigned block

                        if ((block.Tile as GlobalTile).Region.Equals(-1))
                        {
                            double minDistance = WORLDSIZE * WORLDSIZE;

                            for (int i = 1; i <= REGIONSIZE; i++)
                            {
                                double distance = Math.Abs(currentCoordinate - regions[i].Center);

                                if (distance <= minDistance)
                                {
                                    minDistance = distance;
                                    assignedRegion = i;
                                    //this is the smallest region
                                }

                            }
                            regions[assignedRegion].Blocks.Add(block);
                            (block.Tile as GlobalTile).Region = assignedRegion;
                        }
                    }
                }
            }

        }
        /// <summary>
        /// Assigns each region a different height
        /// </summary>
        private static void AssignRegionSizes()
        {
            Random random = new Random();

            for (int i = 1; i <= REGIONSIZE; i++)
            {
                MapCoordinate regionCentre = regions[i].Center;
                int elev;

                double cartDistance = Math.Sqrt(Math.Pow(WORLDSIZE / 2 - regionCentre.X, 2) + Math.Pow(WORLDSIZE / 2 - regionCentre.Y, 2));

                if (cartDistance > WORLDSIZE * 0.4 - (GetRandomGaussian() * WORLDSIZE * 0.1 + WORLDSIZE * 0.1))
                {

                    elev = random.Next(90) - 100;
                }
                else
                {
                    elev = random.Next(210);
                }

                if (Math.Abs(elev) < 10)
                {
                    elev = elev * 10;
                }


                foreach (MapBlock block in regions[i].Blocks)
                {
                    // CurrentStep = "Setting elevation for block  " + block.Tile.Coordinate.X + " " + block.Tile.Coordinate.Y;
                    lock (GlobalMap.lockMe)
                    {
                        (block.Tile as GlobalTile).Elevation = (int)Math.Round((double)elev);
                    }


                }

            }

        }
        /// <summary>
        /// Runs a rough Interpolation on the world
        /// </summary>
        private static void RoughSmooth()
        {
            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                    lock (GlobalMap.lockMe)
                    {
                        (block.Tile as GlobalTile).Elevation = Interpolation.InverseDistanceWeighting(block, EXPONENTWEIGHT, WORLDSIZE);
                    }
                }
            }
        }
        /// <summary>
        /// Erodes the world's elevation
        /// </summary>
        private static void ErodeWorld()
        {
            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    //We need to smooth the nearest neighbour

                    //CurrentStep = "Eroding the tile " + x + "," + y;
                    lock (GlobalMap.lockMe)
                    {
                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                        (block.Tile as GlobalTile).Elevation = Interpolation.NearestNeighbour(WORLDSIZE, block);
                    }

                }
            }

            for (int x = WORLDSIZE - 1; x >= 0; x--)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    //We need to smooth the nearest neighbour

                    //CurrentStep = "Eroding the tile " + x + "," + y;
                    lock (GlobalMap.lockMe)
                    {
                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                        (block.Tile as GlobalTile).Elevation = Interpolation.NearestNeighbour(WORLDSIZE, block);
                    }

                }
            }

            for (int y = WORLDSIZE - 1; y >= 0; y--)
            {
                for (int x = 0; x < WORLDSIZE; x++)
                {
                    //We need to smooth the nearest neighbour

                    //CurrentStep = "Eroding the tile " + x + "," + y;
                    lock (GlobalMap.lockMe)
                    {
                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                        (block.Tile as GlobalTile).Elevation = Interpolation.NearestNeighbour(WORLDSIZE, block);
                    }

                }
            }

            for (int y = 0; y < WORLDSIZE; y++)
            {
                for (int x = WORLDSIZE - 1; x >= 0; x--)
                {
                    //We need to smooth the nearest neighbour

                    //CurrentStep = "Eroding the tile " + x + "," + y;
                    lock (GlobalMap.lockMe)
                    {
                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                        (block.Tile as GlobalTile).Elevation = Interpolation.NearestNeighbour(WORLDSIZE, block);
                    }

                }
            }
        }
        /// <summary>
        /// Generates a number of rivers according to the maximum river count assigned
        /// </summary>
        private static void GenerateRivers()
        {
            //We need the get the River-Count highest points on the map

            List<MapBlock> blocks = new List<MapBlock>();

            //Lets take the highest point of each region
            foreach (Region region in regions)
            {
                blocks.AddRange(region.Blocks.OrderByDescending(b => (b.Tile as GlobalTile).Elevation).Take(1));
            }

            //now take the top River-Count

            List<MapBlock> blocksToProcess = blocks.OrderByDescending(b => (b.Tile as GlobalTile).Elevation).Take(RIVERCOUNT).ToList();

            for (int i = 0; i < blocksToProcess.Count; i++)
            {
                MapBlock block = blocksToProcess[i];

                //Start at the highest point, go around it and find the smallest point

                GlobalTile currentTile = (block.Tile as GlobalTile);
                GlobalTile nextTile = null;
                bool run = true;

                while (run)
                {
                    //find the next tile - the smallest point around it.
                    nextTile = null;

                    List<GlobalTile> surroundingTiles = new List<GlobalTile>();

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            surroundingTiles.Add((GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(currentTile.Coordinate.X + x, currentTile.Coordinate.Y + y, 0, MapType.GLOBAL)).Tile as GlobalTile));
                        }
                    }

                    //now pick the smallest tile which has no river

                    if (surroundingTiles.Count == 0)
                    {
                        run = false;
                        break;
                    }

                    nextTile = surroundingTiles.Where(st => st != null && !st.HasRiver).OrderBy(st => st.Elevation).ThenBy(st => Math.Abs(st.Coordinate.X - currentTile.Coordinate.X) + Math.Abs(st.Coordinate.Y - currentTile.Coordinate.Y)).FirstOrDefault();

                    //check whether this tile has a river, or is water

                    if (nextTile == null || nextTile.Elevation <= 0)
                    {
                        //stop
                        run = false;
                    }
                    else if (nextTile.Elevation > currentTile.Elevation)
                    {
                        //Can't go up a slope - stop
                        run = false;
                    }
                    else
                    {
                        //Valid. Give the next tile a river
                        nextTile.HasRiver = true;

                        //if the difference in elevations is larger than 75 - the river forms a delta
                        if (currentTile.Elevation - nextTile.Elevation >= 75)
                        {
                            //process seperatly
                            blocksToProcess.Add(block);
                        }


                        //swap them
                        currentTile = nextTile;
                    }
                }
            }
        }

        /// <summary>
        /// Marks certain places as having large slopes
        /// </summary>
        public static void MarkHillSlopes()
        {
            //we need to go through them one at a time

            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                    List<GlobalTile> surroundingTiles = new List<GlobalTile>();
                    //is the block underwater?

                    if ((block.Tile as GlobalTile).Elevation > 0)
                    {
                        //no, we can contour it
                        //look around this block, are there any other blocks which have an elevation +ve difference of 50 or more?
                        for (int x2 = -1; x2 <= 1; x2++)
                        {
                            for (int y2 = -1; y2 <= 1; y2++)
                            {
                                surroundingTiles.Add((GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(block.Tile.Coordinate.X + x2, block.Tile.Coordinate.Y + y2, 0, MapType.GLOBAL)).Tile as GlobalTile));
                            }
                        }

                        //Some surrounding tiles appear to be null. Let's remove them before they cause problems
                        for (int i = 0; i < surroundingTiles.Count; i++)
                        {
                            if (surroundingTiles[i] == null)
                            {
                                surroundingTiles.RemoveAt(i);
                                i--;
                            }
                        }


                        if (surroundingTiles.Count == 0)
                        {
                            continue;
                        }

                        //now we check the top one
                        if (surroundingTiles.OrderByDescending(st => st.Elevation).FirstOrDefault().Elevation > (block.Tile as GlobalTile).Elevation + 50)
                        {
                            //it is - mark this tile as a contour
                            (block.Tile as GlobalTile).HasHillSlope = true;
                        }

                    }
                }
            }
        }

        /// <summary>
        /// Generates the temperatures of the world
        /// </summary>
        public static void GenerateTemperatures()
        {
            //To set temperatures we will do the following. We will design a gaussian graph, with a max of 40 at the equator, descending with distance from it
            //We will also set a similar graph, with a max of 0 and a min of -40 with respect to elevation.
            //The temperature of each point will be the sum of both

            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                    //determine the temperature we'll assign it
                    GlobalTile tile = (block.Tile as GlobalTile);

                    tile.ClimateTemperature = (decimal)WorldGenerationManager.GetPointOnGaussianCurve(31, WORLDSIZE / 2, WORLDSIZE / 5, tile.Coordinate.Y);

                    if (tile.Elevation > 0)
                    {
                        tile.ClimateTemperature += (decimal)WorldGenerationManager.GetPointOnGaussianCurve(20, 0, 200, tile.Elevation) - 20;
                    }
                    else if (tile.HasRiver)
                    {
                        //rivers tend to be cooler
                        tile.ClimateTemperature -= 5;
                    }
                    else
                    {
                        //water tiles tend to be warmer in their temperature
                        tile.ClimateTemperature += 5;
                    }
                }

            }

        }
        /// <summary>
        /// Generates the amount of rain in the world
        /// </summary>
        public static void GenerateRains()
        {
            //To determine rain we will do the following. First we set rain at each location at 0 (dry)
            //Then we will drop a number of 'centres of rain' randomly which will have 10 'rain'
            //We will then put decreasing amounts of rain from these centres.

            //now we create a random x and y coordinate and we create some rain

            Random random = new Random();

            int failureCount = 0;
            for (int i = 0; i < RAINCENTERCOUNT; i++)
            {
                int xRain = random.Next(WORLDSIZE);
                int yRain = random.Next(WORLDSIZE);

                MapBlock targetBlock = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(xRain, yRain, 0, MapType.GLOBAL));

                if (failureCount > 100)
                {
                    //the world is saturated, lets break out before it crashes
                    break;
                }

                if ((targetBlock.Tile as GlobalTile).Elevation < 0 || (targetBlock.Tile as GlobalTile).Rainfall > 4)
                {
                    //don't put the block on the sea , and don't put it in places full of rain already
                    i--;
                    failureCount++;
                    continue;
                }

                decimal rainAmount = 9.0M;
                int radius = 0;

                while (rainAmount > 0)
                {
                    MapBlock[] rainBlocks = GetCircleAroundPoint(new MapCoordinate(xRain, yRain, 0, MapType.GLOBAL), radius);

                    foreach (MapBlock block in rainBlocks)
                    {
                        (block.Tile as GlobalTile).Rainfall += rainAmount;
                    }

                    rainAmount -= .10M;

                    if (radius.Equals(5))
                    {
                        //reduce quite a bit
                        rainAmount -= 2;
                    }

                    if (radius > 5)
                    {
                        rainAmount -= .10M;

                    }
                    if (radius > 20)
                    {
                        rainAmount += .10M;
                    }

                    radius++;

                }


            }

        }

        /// <summary>
        /// Determines the biomes for each tile from their traits
        /// </summary>
        public static void DetermineBiomes()
        {
            Random random = new Random();

            //And now we determine their biome
            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                    GlobalTile tile = (block.Tile as GlobalTile);

                    //start with temperature
                    if (tile.ClimateTemperature > 30)
                    {
                        //Very hot
                        if (tile.Rainfall > 6)
                        {
                            //rainforest
                            tile.Biome = GlobalBiome.RAINFOREST;
                        }
                        else
                        {
                            //desert
                            tile.Biome = GlobalBiome.ARID_DESERT;
                        }
                    }
                    else if (tile.ClimateTemperature > 20)
                    {
                        if (tile.Rainfall > 9)
                        {
                            //soaked
                            tile.Biome = GlobalBiome.WETLAND;
                        }
                        else if (tile.Rainfall > 1)
                        {
                            //we should use a random value to determine what's the chance of getting a forest

                            //from 2 to 10
                            int randomInt = random.Next(3) + (int)tile.Rainfall;

                            if (randomInt > 8)
                            {
                                tile.Biome = GlobalBiome.DENSE_FOREST;
                            }
                            else if (randomInt > 5)
                            {
                                tile.Biome = GlobalBiome.WOODLAND;
                            }
                            else
                            {
                                tile.Biome = GlobalBiome.GRASSLAND;
                            }

                        }
                        else
                        {
                            tile.Biome = GlobalBiome.GARIGUE;
                        }

                    }
                    else if (tile.ClimateTemperature > 5)
                    {
                        //pretty cold
                        int randomInt = random.Next(10);

                        if (tile.Rainfall > 9)
                        {
                            tile.Biome = GlobalBiome.POLAR_DESERT; //it'll be a very snowy desert
                        }
                        else if (tile.Rainfall < 3)
                        {
                            tile.Biome = GlobalBiome.POLAR_DESERT;
                        }

                        //otherwise, there's a small chance you get a forest

                        if (randomInt > 7)
                        {
                            tile.Biome = GlobalBiome.POLAR_FOREST;
                        }
                        else
                        {
                            //polar desert
                            tile.Biome = GlobalBiome.POLAR_DESERT;
                        }

                    }
                    else if (tile.ClimateTemperature < 4)
                    {
                        //freezing cold
                        tile.Biome = GlobalBiome.POLAR_DESERT;
                    }

                }
            }
        }

        /// <summary>
        /// Generates a number of resources on the map
        /// </summary>
        public static void GenerateResources()
        {
            //First lets put all the blocks into one collection

            List<MapBlock> allBlocks = new List<MapBlock>();

            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    allBlocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL)));
                }
            }

            //Prepare stuff we might need
            var grassLandBlocks = allBlocks.Where(ab => (ab.Tile as GlobalTile).Biome == GlobalBiome.GRASSLAND && (ab.Tile as GlobalTile).Elevation > 0 && !(ab.Tile as GlobalTile).HasResource && !(ab.Tile as GlobalTile).HasRiver).OrderBy(ab => GameState.Random.Next(1000));
            var waterBlocks = allBlocks.Where(ab => (ab.Tile as GlobalTile).Elevation <= 0 && !(ab.Tile as GlobalTile).HasResource && !(ab.Tile as GlobalTile).HasRiver).OrderBy(ab => GameState.Random.Next(1000));
            var denseForestBlocks = allBlocks.Where(ab => (ab.Tile as GlobalTile).Biome == GlobalBiome.DENSE_FOREST && (ab.Tile as GlobalTile).Elevation > 0 && !(ab.Tile as GlobalTile).HasResource && !(ab.Tile as GlobalTile).HasRiver).OrderBy(ab => GameState.Random.Next(1000));
            var forestBlocks = allBlocks.Where(ab => (ab.Tile as GlobalTile).Biome == GlobalBiome.WOODLAND && (ab.Tile as GlobalTile).Elevation > 0 && !(ab.Tile as GlobalTile).HasResource && !(ab.Tile as GlobalTile).HasRiver).OrderBy(ab => GameState.Random.Next(1000));
            var hillyLocations = allBlocks.Where(ab => (ab.Tile as GlobalTile).Elevation > 80 && !(ab.Tile as GlobalTile).HasResource && !(ab.Tile as GlobalTile).HasRiver).OrderBy(ab => GameState.Random.Next(1000));
            var nonWater = allBlocks.Where(ab => (ab.Tile as GlobalTile).Elevation > 0 && !(ab.Tile as GlobalTile).HasResource && !(ab.Tile as GlobalTile).HasRiver).OrderBy(ab => GameState.Random.Next(1000));

            foreach (GlobalResourceType resource in Enum.GetValues(typeof(GlobalResourceType)))
            {
                List<MapBlock> candidateBlocks = new List<MapBlock>();

                switch (resource)
                {
                    case GlobalResourceType.FARMLAND:
                        //Farmland only is possible on grassland
                        candidateBlocks.AddRange(grassLandBlocks);
                        break;
                    case GlobalResourceType.FISH:
                        //Fish is only possible in water
                        candidateBlocks.AddRange(waterBlocks);
                        break;
                    case GlobalResourceType.GAME:
                        //Game is found in dense forests, woodland and rarely grasslands
                        candidateBlocks.AddRange(denseForestBlocks.Take(150));
                        candidateBlocks.AddRange(forestBlocks.Take(100));
                        candidateBlocks.AddRange(grassLandBlocks.Take(50));
                        break;
                    case GlobalResourceType.GOLD:
                        //Gold is found in hilly locations
                        candidateBlocks.AddRange(hillyLocations);
                        break;
                    case GlobalResourceType.HORSE:
                        //Horses are found in grassland and forests
                        candidateBlocks.AddRange(grassLandBlocks.Take(150));
                        candidateBlocks.AddRange(forestBlocks.Take(50));
                        break;
                    case GlobalResourceType.IRON:
                        //Iron is found in hills
                        candidateBlocks.AddRange(hillyLocations);
                        break;
                    case GlobalResourceType.WOOD:
                        //Wood is found in dense forests and woodland
                        candidateBlocks.AddRange(denseForestBlocks.Take(150));
                        candidateBlocks.AddRange(forestBlocks.Take(100));
                        break;
                }

                //Shuffle the candidates, and take the first few blocks
                candidateBlocks = candidateBlocks.Where(cb => !(cb.Tile as GlobalTile).HasResource).OrderBy(ab => GameState.Random.Next(1000)).ToList();

                foreach (var chosenBlock in candidateBlocks.Take(RESOURCES_PER_TYPE))
                {
                    //Mark the tile
                    (chosenBlock.Tile as GlobalTile).HasResource = true;

                    //Create the resource item
                    MapResource resourceItem = new MapResource(resource);
                    resourceItem.Coordinate = chosenBlock.Tile.Coordinate;

                    chosenBlock.ForcePutItemOnBlock(resourceItem);
                }
            }
        }

        /// <summary>
        /// Replaces certain resources with sites instead
        /// </summary>
        public static void HarvestResources()
        {
            GameState.GlobalMap.MapSiteItems = new List<MapSiteItem>();

            //Go through each resource tile and see which of those are claimed by someone

            List<MapBlock> blocks = new List<MapBlock>();

            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                    var tile = block.Tile as GlobalTile;

                    if (tile.Owner.HasValue && tile.HasResource && block.GetTopItem().GetType().Equals(typeof(MapResource)))
                    {
                        blocks.Add(block);
                    }
                }
            }

            //Now we have to go through the blocks. If they are non-fish, just replace them with the right type
            //We'll handle fish differently
            foreach (var block in blocks)
            {
                var mapResource = block.GetItems().First(gi => gi.GetType().Equals(typeof(MapResource))) as MapResource;

                block.RemoveTopItem(); //remove it

                MapSite ms = new MapSite();
                ms.Coordinate = new MapCoordinate(block.Tile.Coordinate);
                ms.SiteData = new DRObjects.Sites.SiteData()
                {
                    Biome = (block.Tile as GlobalTile).Biome ?? GlobalBiome.GARIGUE,
                    OwnerChanged = false,
                    Owners = (block.Tile as GlobalTile).Owner.Value == 100 ? OwningFactions.ORCS : (block.Tile as GlobalTile).Owner.Value == 50 ? OwningFactions.BANDITS : OwningFactions.HUMANS, //TODO: EXPAND LATER
                    Civilisation = GameState.GlobalMap.Civilisations.FirstOrDefault(c => c.ID.Equals( (block.Tile as GlobalTile).Owner.Value))
                };

                MapSiteItem msi = new MapSiteItem();
                msi.Coordinate = new MapCoordinate(ms.Coordinate);
                msi.IsActive = true;
                msi.MayContainItems = true;
                msi.Site = ms;

                GameState.GlobalMap.MapSiteItems.Add(msi);

                switch (mapResource.ResourceType)
                {
                    case GlobalResourceType.FARMLAND:
                        //And lets put somethign else
                        ms.SiteData.SiteTypeData = SiteDataManager.GetData(SiteType.FARM);
                        break;
                    case GlobalResourceType.FISH:

                        //So, we can't put it on the actual fish, instead go around it and grab the first piece of land which we have
                        var fBlock = GetBlocksAroundPoint(ms.Coordinate, 2).Where(b => (b.Tile as GlobalTile).Elevation > 0 &&
                            !(b.Tile as GlobalTile).HasResource && !(b.Tile as GlobalTile).IsBlockedForColonisation).OrderBy(r => GameState.Random.Next(2)).FirstOrDefault();

                        if (fBlock != null)
                        {
                            //Use this as a location
                            msi.Coordinate = new MapCoordinate(fBlock.Tile.Coordinate);
                            ms.Coordinate = new MapCoordinate(fBlock.Tile.Coordinate);

                            ms.SiteData.SiteTypeData = SiteDataManager.GetData(SiteType.FISHING_VILLAGE);
                        }
                        else
                        {
                            continue; //Skip it. Can't be harvested
                        }


                        break;
                    case GlobalResourceType.GAME:
                        ms.SiteData.SiteTypeData = SiteDataManager.GetData(SiteType.HUNTER);
                        break;
                    case GlobalResourceType.GOLD:
                        ms.SiteData.SiteTypeData = SiteDataManager.GetData(SiteType.GOLD_MINE);
                        break;
                    case GlobalResourceType.HORSE:
                        ms.SiteData.SiteTypeData = SiteDataManager.GetData(SiteType.STABLES);
                        break;
                    case GlobalResourceType.IRON:
                        ms.SiteData.SiteTypeData = SiteDataManager.GetData(SiteType.IRON_MINE);
                        break;
                    case GlobalResourceType.WOOD:
                        ms.SiteData.SiteTypeData = SiteDataManager.GetData(SiteType.WOODCUTTER);
                        break;
                }

                msi.Site.SiteData.LoadAppropriateActorCounts();


                block.ForcePutItemOnBlock(msi);
            }

        }

        /// <summary>
        /// This determines the base desirability for all tile by examining the tile itself
        /// </summary>
        public static void DetermineDesirability()
        {
            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                    GlobalTile tile = (block.Tile as GlobalTile);

                    //calculate the desirability for the tile
                    tile.BaseDesirability = DetermineDesirability(block);
                }
            }
        }
        /// <summary>
        /// Drops a number of settlements within the world
        /// </summary>
        public static void ColoniseWorld()
        {
            //The way this will be done will be as follows
            //We will choose a random land region to colonise of a determined size
            //We will then calculate the desirability of each tile, sum it with the deseribility of each tile bordering it
            //The most desirable tile in the region will be colonised

            GameState.GlobalMap.WorldSettlements = new List<SettlementItem>();

            Random random = new Random();

            GameState.GlobalMap.Civilisations = new List<Civilisation>();

            for (int i = 0; i < HUMAN_CAPITAL_COUNT; i++)
            {
                //What do these people call themselves?
                Civilisation civilisation = new Civilisation();
                civilisation.Faction = OwningFactions.HUMANS;
                civilisation.ID = i;
                civilisation.Name = CivilisationNameGenerator.GetName();

                GameState.GlobalMap.Civilisations.Add(civilisation);

                List<Settlement> ownedSettlements = new List<Settlement>();

                //choose a random x and y

                MapBlock centreBlock = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(random.Next(WORLDSIZE), random.Next(WORLDSIZE), 0, MapType.GLOBAL));

                //Get all tiles in the 'region'
                MapBlock[] regionalBlocks = GetBlocksAroundPoint(centreBlock.Tile.Coordinate, HUMAN_CAPITAL_SEARCH_RADIUS);

                //order them by the sum of the desirabilty

                //We only need to consider tiles which have no river, and actual land which isn't a mountain or a resource
                //Also consider those which aren't blocked and which aren't claimed
                var candidateBlocks = regionalBlocks.Where(rb => !(rb.Tile as GlobalTile).HasRiver && !(rb.Tile as GlobalTile).HasResource && (rb.Tile as GlobalTile).Elevation > 0 && (rb.Tile as GlobalTile).Elevation < 250
                    && !(rb.Tile as GlobalTile).IsBlockedForColonisation
                    && ((rb.Tile as GlobalTile).Owner == null || (rb.Tile as GlobalTile).Owner == i)
                    && !(GetBlocksAroundPoint(rb.Tile.Coordinate,1).Any(s => (s.Tile as GlobalTile).HasResource)))
                    .OrderByDescending(rb => (rb.Tile as GlobalTile).BaseDesirability + (GetBlocksAroundPoint(rb.Tile.Coordinate, 3).Sum(rba => (rba.Tile as GlobalTile).BaseDesirability)));

                if (candidateBlocks.ToArray().Length == 0)
                {
                    //no possible tiles - start over
                    i--;
                    continue;
                }

                //colonise

                MapBlock block = candidateBlocks.First();

                //Look around in the 3x3 block and see if we have any resources which we are consuming
                var surroundingBlocks = GetBlocksAroundPoint(block.Tile.Coordinate, 1);

                List<GlobalResourceType> resources = new List<GlobalResourceType>();

                foreach (var sblock in surroundingBlocks)
                {
                    if (sblock.GetTopMapItem() != null && sblock.GetTopMapItem().GetType() == typeof(MapResource))
                    {
                        resources.Add((sblock.GetTopMapItem() as MapResource).ResourceType);
                    }
                }

                Settlement settlement = SettlementGenerator.GenerateSettlement(block.Tile.Coordinate, random.Next(5) + 10, resources,civilisation, true);
                settlement.IsCapital = true;
                settlement.Civilisation = civilisation;

                GameState.GlobalMap.WorldSettlements.Add(CreateSettlement(true, settlement, block, i));

                ownedSettlements.Add(settlement);

                //That's the capital done, now let's put in some more colonies for each civilisation

                for (int j = 0; j < HUMAN_SETTLEMENTS_PER_CIVILIZATION; j++)
                {
                    //Collect all the possible locations around all the settlements owned by that civilisation
                    List<MapBlock> possibleLocations = new List<MapBlock>();

                    foreach (Settlement settle in ownedSettlements)
                    {
                        possibleLocations.AddRange(GetBlocksAroundPoint(settle.Coordinate, HUMAN_COLONY_SEARCH_RADIUS));
                    }

                    candidateBlocks = possibleLocations.Where(rb => !(rb.Tile as GlobalTile).HasRiver && (rb.Tile as GlobalTile).Elevation > 0 && (rb.Tile as GlobalTile).Elevation < 250
                    && !(rb.Tile as GlobalTile).IsBlockedForColonisation
                    && ((rb.Tile as GlobalTile).Owner == null || (rb.Tile as GlobalTile).Owner.Value == i))
                    .OrderByDescending(rb => (rb.Tile as GlobalTile).BaseDesirability + (GetBlocksAroundPoint(rb.Tile.Coordinate, 1).Sum(rba => (rba.Tile as GlobalTile).BaseDesirability)));

                    if (candidateBlocks.Count() == 0)
                    {
                        //No more - stop colonising
                        break;
                    }

                    block = candidateBlocks.First();

                    //Look around in the 3x3 block and see if we have any resources which we are consuming
                    surroundingBlocks = GetBlocksAroundPoint(block.Tile.Coordinate, 1);

                    resources = new List<GlobalResourceType>();

                    foreach (var sblock in surroundingBlocks)
                    {
                        if (sblock.GetTopMapItem() != null && sblock.GetTopMapItem().GetType() == typeof(MapResource))
                        {
                            resources.Add((sblock.GetTopMapItem() as MapResource).ResourceType);
                        }
                    }

                    settlement = SettlementGenerator.GenerateSettlement(block.Tile.Coordinate, random.Next(7) + 1, resources, civilisation);
                    settlement.Civilisation = civilisation;

                    GameState.GlobalMap.WorldSettlements.Add(CreateSettlement(false, settlement, block, i));

                    ownedSettlements.Add(settlement);

                }

            }

        }

        /// <summary>
        /// Creates Citadels on the map
        /// Will colonise areas not claimed by humans.
        /// Later on we'll create ruins which will appear in areas claimed by humans
        /// </summary>
        public static void CreateOrcCitadels()
        {
            //Create the civilisation
            GameState.GlobalMap.Civilisations.Add(new Civilisation() { Faction = OwningFactions.ORCS, ID = 100, Name = "Orc Hoardes" });

            List<MapBlock> blocks = new List<MapBlock>();

            //Collect all the points which aren't claimed
            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                    var tile = block.Tile as GlobalTile;

                    if (!tile.HasRiver && !tile.IsBlockedForColonisation && !tile.Owner.HasValue && !tile.HasResource && tile.Elevation > 0 && tile.Elevation < 250)
                    {
                        blocks.Add(block);
                    }
                }
            }

            for (int i = 0; i < DUNGEON_TOTAL; i++)
            {
                //Create the actual item - Completly Randomly for now
                var enemyRace = ActorGeneration.GetEnemyType(true);

                MapBlock block = blocks[GameState.Random.Next(blocks.Count)];

                //Still valid?
                if ((block.Tile as GlobalTile).IsBlockedForColonisation || (block.Tile as GlobalTile).Owner.HasValue)
                {
                    //Nope
                    i--;
                    continue;
                }

                Citadel dungeon = new Citadel();
                dungeon.Coordinate = new MapCoordinate(block.Tile.Coordinate);
                dungeon.GuardRooms = GameState.Random.Next(2) + 1;
                dungeon.MaxOwnedPopulation = GameState.Random.Next(10) + 2;
                dungeon.MaxWildPopulation = GameState.Random.Next(4);
                dungeon.OwnerCreatureType = enemyRace;
                dungeon.PercentageOwned = GameState.Random.Next(50) + 50;
                dungeon.TierCount = GameState.Random.Next(4) + 3;
                dungeon.TrapRooms = GameState.Random.Next(5) + 2;
                dungeon.TreasureRoom = GameState.Random.Next(5);

                //Put an entire group of Dungeon Items on it
                MapCoordinate[] dungeonCoordinates = new MapCoordinate[10];

                dungeonCoordinates[7] = new MapCoordinate(block.Tile.Coordinate.X - 1, block.Tile.Coordinate.Y - 1, 0, MapType.GLOBAL);
                dungeonCoordinates[8] = new MapCoordinate(block.Tile.Coordinate.X, block.Tile.Coordinate.Y - 1, 0, MapType.GLOBAL);
                dungeonCoordinates[9] = new MapCoordinate(block.Tile.Coordinate.X + 1, block.Tile.Coordinate.Y - 1, 0, MapType.GLOBAL);
                dungeonCoordinates[4] = new MapCoordinate(block.Tile.Coordinate.X - 1, block.Tile.Coordinate.Y, 0, MapType.GLOBAL);
                dungeonCoordinates[5] = new MapCoordinate(block.Tile.Coordinate.X, block.Tile.Coordinate.Y, 0, MapType.GLOBAL);
                dungeonCoordinates[6] = new MapCoordinate(block.Tile.Coordinate.X + 1, block.Tile.Coordinate.Y, 0, MapType.GLOBAL);
                dungeonCoordinates[1] = new MapCoordinate(block.Tile.Coordinate.X - 1, block.Tile.Coordinate.Y + 1, 0, MapType.GLOBAL);
                dungeonCoordinates[2] = new MapCoordinate(block.Tile.Coordinate.X, block.Tile.Coordinate.Y + 1, 0, MapType.GLOBAL);
                dungeonCoordinates[3] = new MapCoordinate(block.Tile.Coordinate.X + 1, block.Tile.Coordinate.Y + 1, 0, MapType.GLOBAL);

                //Block surrounding tiles
                MapBlock[] regionalBlocks = GetBlocksAroundPoint(block.Tile.Coordinate, 1);

                foreach (MapBlock rblock in regionalBlocks)
                {
                    (rblock.Tile as GlobalTile).IsBlockedForColonisation = true;
                }

                //Also claim the surrounding areas for orcs. Let's give them an owner of 100
                MapBlock[] claimedBlocks = GetBlocksAroundPoint(block.Tile.Coordinate, DUNGEON_CLAIMING_RADIUS);

                foreach (MapBlock rblock in claimedBlocks)
                {
                    (rblock.Tile as GlobalTile).Owner = 100;
                }

                for (int corner = 1; corner < 10; corner++)
                {
                    var cornerBlock = GameState.GlobalMap.GetBlockAtCoordinate(dungeonCoordinates[corner]);

                    //Cut any forests down
                    switch ((cornerBlock.Tile as GlobalTile).Biome)
                    {
                        case GlobalBiome.DENSE_FOREST:
                            (cornerBlock.Tile as GlobalTile).Biome = GlobalBiome.GRASSLAND;
                            break;
                        case GlobalBiome.WOODLAND:
                            (cornerBlock.Tile as GlobalTile).Biome = GlobalBiome.GRASSLAND;
                            break;
                        case GlobalBiome.POLAR_FOREST:
                            (cornerBlock.Tile as GlobalTile).Biome = GlobalBiome.POLAR_DESERT;
                            break;
                    }


                    //Create a new dungeon
                    CitadelItem item = new CitadelItem(corner);
                    item.Coordinate = new MapCoordinate(cornerBlock.Tile.Coordinate);
                    item.Name = "Orc Citadel";
                    item.Description = "a dark dungeon owned by Orcs";
                    item.Dungeon = dungeon;

                    cornerBlock.ForcePutItemOnBlock(item);

                    blocks.Remove(block);
                }
            }
        }

        /// <summary>
        /// Create bandit camps
        /// </summary>
        public static void CreateBandits()
        {
            //Create the bandit civilisation
            GameState.GlobalMap.Civilisations.Add(new Civilisation() { ID = 50, Faction = OwningFactions.BANDITS, Name = "Bandits" });

            List<MapBlock> blocks = new List<MapBlock>();
            GameState.GlobalMap.CampItems = new List<BanditCampItem>();

            //Collect all the points
            for (int x = 0; x < WORLDSIZE; x++)
            {
                for (int y = 0; y < WORLDSIZE; y++)
                {
                    MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapType.GLOBAL));

                    var tile = block.Tile as GlobalTile;

                    if (!tile.HasRiver && !tile.IsBlockedForColonisation && !tile.HasResource && tile.Elevation > 0 && tile.Elevation < 250)
                    {
                        blocks.Add(block);
                    }
                }
            }

            //Just toss them anywhere
            for (int i = 0; i < BANDIT_TOTAL; i++)
            {
                int randomNumber = GameState.Random.Next(blocks.Count);

                MapBlock block = blocks[randomNumber];

                //And remove it
                blocks.RemoveAt(randomNumber);

                BanditCamp camp = new BanditCamp();
                camp.BanditTotal = GameState.Random.Next(5, 10);
                camp.Coordinate = new MapCoordinate(block.Tile.Coordinate);

                BanditCampItem item = new BanditCampItem();
                item.Camp = camp;
                item.Coordinate = new MapCoordinate(block.Tile.Coordinate);
                item.Name = "Bandit Camp";
                item.Description = "A camp full of bandits";

                block.ForcePutItemOnBlock(item);

                GameState.GlobalMap.CampItems.Add(item);

                //Also claim the surrounding areas for bandits. Let's give them an owner of 50 and use the bandit claiming radius
                MapBlock[] claimedBlocks = GetBlocksAroundPoint(block.Tile.Coordinate, BANDIT_CLAIMING_RADIUS);

                foreach (MapBlock rblock in claimedBlocks)
                {
                    (rblock.Tile as GlobalTile).Owner = 50;
                }

            }
        }

        /// <summary>
        /// Generates the roads
        /// </summary>
        public static void GenerateRoads()
        {
            //Build the pathfinding map
            PathfinderInterface.Nodes = GeneratePathfindingMap(GameState.GlobalMap.globalGameMap);

            //First lets build roads between all colonies and the capital
            for(int i=0; i < HUMAN_CAPITAL_COUNT; i++)
            {
                //Find the capital
                var capital = GameState.GlobalMap.WorldSettlements.Where(ws => ws.Settlement.IsCapital && ws.OwnerID == i).First();
                
                foreach(var village in GameState.GlobalMap.WorldSettlements.Where(ws => !ws.Settlement.IsCapital && ws.OwnerID == i))
                {

                    //Rebuild
                    PathfinderInterface.Nodes = GeneratePathfindingMap(GameState.GlobalMap.globalGameMap);

                    var path = PathfinderInterface.GetPath(capital.Coordinate,village.Coordinate);

                    if (path != null)
                    {
                        foreach(var coord in path)
                        {
                            (GameState.GlobalMap.GetBlockAtCoordinate(coord).Tile as GlobalTile).HasRoad = true; //road it
                        }
                    }
                }

                //Now lets build roads between all the sites owned by this person and the closest settlement
                
                foreach(var mapSite in GameState.GlobalMap.MapSiteItems.Where(ms => ms.Site.SiteData.Civilisation.ID == i))
                {
                    //Rebuild
                    PathfinderInterface.Nodes = GeneratePathfindingMap(GameState.GlobalMap.globalGameMap);

                    //Find the closest one

                    var closestSettlement = GameState.GlobalMap.WorldSettlements.Where(ws => ws.OwnerID == i).OrderBy(ws => Math.Abs(ws.Coordinate - mapSite.Coordinate ) ).First();

                    //And path it
                    var path = PathfinderInterface.GetPath(closestSettlement.Coordinate, mapSite.Coordinate);

                    if (path != null)
                    {
                        foreach (var coord in path)
                        {
                            (GameState.GlobalMap.GetBlockAtCoordinate(coord).Tile as GlobalTile).HasRoad = true; //road it
                        }
                    }

                }

            }

            


        }

        #endregion World Generation Functions

        #region Helper Functions

        /// <summary>
        /// Puts settlement items on a particular location pertianing to a particular settlement.
        /// </summary>
        /// <param name="capital">Whether it's the capital or not</param>
        /// <param name="settlement">The settlement which it represents</param>
        /// <param name="block">The block making up the center</param>
        private static SettlementItem CreateSettlement(bool capital, Settlement settlement, MapBlock block, int owner)
        {
            //Put an entire group of SettlementItems on it
            MapCoordinate[] settlementCoordinates = new MapCoordinate[10];

            SettlementItem retItem = null;

            settlementCoordinates[7] = new MapCoordinate(block.Tile.Coordinate.X - 1, block.Tile.Coordinate.Y - 1, 0, MapType.GLOBAL);
            settlementCoordinates[8] = new MapCoordinate(block.Tile.Coordinate.X, block.Tile.Coordinate.Y - 1, 0, MapType.GLOBAL);
            settlementCoordinates[9] = new MapCoordinate(block.Tile.Coordinate.X + 1, block.Tile.Coordinate.Y - 1, 0, MapType.GLOBAL);
            settlementCoordinates[4] = new MapCoordinate(block.Tile.Coordinate.X - 1, block.Tile.Coordinate.Y, 0, MapType.GLOBAL);
            settlementCoordinates[5] = new MapCoordinate(block.Tile.Coordinate.X, block.Tile.Coordinate.Y, 0, MapType.GLOBAL);
            settlementCoordinates[6] = new MapCoordinate(block.Tile.Coordinate.X + 1, block.Tile.Coordinate.Y, 0, MapType.GLOBAL);
            settlementCoordinates[1] = new MapCoordinate(block.Tile.Coordinate.X - 1, block.Tile.Coordinate.Y + 1, 0, MapType.GLOBAL);
            settlementCoordinates[2] = new MapCoordinate(block.Tile.Coordinate.X, block.Tile.Coordinate.Y + 1, 0, MapType.GLOBAL);
            settlementCoordinates[3] = new MapCoordinate(block.Tile.Coordinate.X + 1, block.Tile.Coordinate.Y + 1, 0, MapType.GLOBAL);

            //Block the radius around it from colonising
            MapBlock[] regionalBlocks = GetBlocksAroundPoint(block.Tile.Coordinate, HUMAN_COLONY_BLOCKING_RADIUS);

            foreach (MapBlock rblock in regionalBlocks)
            {
                (rblock.Tile as GlobalTile).IsBlockedForColonisation = true;
            }

            //Claim the land around it
            MapBlock[] claimedBlocks = GetBlocksAroundPoint(block.Tile.Coordinate, HUMAN_COLONY_CLAIMING_RADIUS);

            foreach (MapBlock rblock in claimedBlocks)
            {
                //This is a disputed region. For now let's not allow creep.
                //Later this might be cause for war
                if (!(rblock.Tile as GlobalTile).Owner.HasValue)
                {
                    (rblock.Tile as GlobalTile).Owner = owner;
                }
            }

            for (int corner = 1; corner < 10; corner++)
            {
                var cornerBlock = GameState.GlobalMap.GetBlockAtCoordinate(settlementCoordinates[corner]);

                //Cut any forests down
                switch ((cornerBlock.Tile as GlobalTile).Biome)
                {
                    case GlobalBiome.DENSE_FOREST:
                        (cornerBlock.Tile as GlobalTile).Biome = GlobalBiome.GRASSLAND;
                        break;
                    case GlobalBiome.WOODLAND:
                        (cornerBlock.Tile as GlobalTile).Biome = GlobalBiome.GRASSLAND;
                        break;
                    case GlobalBiome.POLAR_FOREST:
                        (cornerBlock.Tile as GlobalTile).Biome = GlobalBiome.POLAR_DESERT;
                        break;
                }



                var settlementItem = new SettlementItem()
                {
                    Coordinate = settlementCoordinates[corner],
                    IsCapital = capital,
                    MayContainItems = true,
                    SettlementCorner = corner,
                    SettlementSize = settlement.SettlementSize,
                    Description = (capital ? "the capital of " + settlement.Name : "the settlement of " + settlement.Name) + " owned by " + settlement.Civilisation.Name,
                    Name = settlement.Name,
                    Settlement = settlement,
                    OwnerID = owner
                };

                GameState.GlobalMap.GetBlockAtCoordinate(settlementCoordinates[corner])
                .ForcePutItemOnBlock(settlementItem);

                if (settlementItem.SettlementCorner == 5) //center
                {
                    retItem = settlementItem;
                }
            }

           return retItem;
        }

        /// <summary>
        /// Determines the Desirability of a particular global tile
        /// </summary>
        /// <param name="tile"></param>
        /// <returns></returns>
        public static int DetermineDesirability(MapBlock block)
        {
            GlobalTile tile = block.Tile as GlobalTile; //Get the tile
            int desirability = 0;

            #region Temperature
            //check the temperature
            if (tile.ClimateTemperature > 30)
            {
                //very hot
                desirability -= 2;
            }

            if (tile.ClimateTemperature < 30 && tile.ClimateTemperature > 15)
            {
                //nice temperature
                desirability += 2;
            }
            if (tile.ClimateTemperature > 5)
            {
                //ok temperature
            }
            if (tile.ClimateTemperature < 5)
            {
                //too cold
                desirability -= 3;
            }
            #endregion

            #region Rainfall
            if (tile.Rainfall > 8)
            {
                //too rainy
                desirability -= 2;
            }
            if (tile.Rainfall < 3)
            {
                //too dry
                desirability -= 5;
            }
            #endregion

            #region River?

            if (tile.HasRiver) //rivers are very popular
            {
                desirability += 4;
            }

            #endregion

            #region Elevation

            if (tile.Elevation < 0)
            {
                desirability += 3; //while we can't build on the sea, having the sea next to you is desirable

            }

            if (tile.Elevation > 200)
            {
                desirability += 4; //Having a mountain next to you is useful too
            }
            else if (tile.Elevation > 80)
            {
                desirability += 3; //High land is preferred
            }
            #endregion

            #region Got wood?
            if (tile.Biome.HasValue)
            {
                switch (tile.Biome.Value)
                {
                    case GlobalBiome.DENSE_FOREST:
                        desirability += 5; //Lots of wood
                        break;
                    case GlobalBiome.POLAR_FOREST:
                        desirability += 2; //some wood
                        break;
                    case GlobalBiome.WOODLAND:
                        desirability += 3; //wood
                        break;
                }

            }

            #endregion

            //Any resources?
            if (tile.HasResource)
            {
                //Find it
                MapResource resource = block.GetItems().Where(gi => gi.GetType().Equals(typeof(MapResource))).FirstOrDefault() as MapResource;

                desirability += resource.Desirability;

            }

            return desirability;
        }

        /// <summary>
        /// Gets a random number on a gaussian distribution with a mean 0.0 and standard deviation 1.0
        /// </summary>
        /// <returns></returns>
        public static double GetRandomGaussian()
        {
            Random rand = new Random(); //reuse this if you are generating many
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         0 + 1 * randStdNormal; //random normal(mean,stdDev^2)

            return randNormal;

        }
        /// <summary>
        /// Gets a 'circle' of points 'radius' distance away from it
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static MapBlock[] GetCircleAroundPoint(MapCoordinate centre, int radius)
        {
            int minY = centre.Y - Math.Abs(radius);
            int maxY = centre.Y + Math.Abs(radius);

            int minX = centre.X - Math.Abs(radius);
            int maxX = centre.X + Math.Abs(radius);

            List<MapBlock> returnList = new List<MapBlock>();

            //go through all of them

            for (int yLoop = maxY; yLoop >= minY; yLoop--)
            {
                for (int xLoop = minX; xLoop <= maxX; xLoop++)
                {
                    MapCoordinate coord = new MapCoordinate(xLoop, yLoop, 0, MapType.GLOBAL);

                    if (xLoop >= 0 && xLoop < WORLDSIZE && yLoop >= 0 && yLoop < WORLDSIZE)
                    { //make sure they're in the map
                        returnList.Add(GameState.GlobalMap.GetBlockAtCoordinate(coord));
                    }
                }
            }

            return returnList.Where(r => r.Tile.Coordinate.X.Equals(minX) || r.Tile.Coordinate.X.Equals(maxX) || r.Tile.Coordinate.Y.Equals(minY) || r.Tile.Coordinate.Y.Equals(maxY)).ToArray();
        }

        /// <summary>
        /// Returns a filled square around a particular point
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static MapBlock[] GetBlocksAroundPoint(MapCoordinate centre, int radius)
        {
            int minY = centre.Y - Math.Abs(radius);
            int maxY = centre.Y + Math.Abs(radius);

            int minX = centre.X - Math.Abs(radius);
            int maxX = centre.X + Math.Abs(radius);

            List<MapBlock> returnList = new List<MapBlock>();

            //go through all of them

            for (int yLoop = maxY; yLoop >= minY; yLoop--)
            {
                for (int xLoop = minX; xLoop <= maxX; xLoop++)
                {
                    MapCoordinate coord = new MapCoordinate(xLoop, yLoop, 0, MapType.GLOBAL);

                    if (xLoop >= 0 && xLoop < WORLDSIZE && yLoop >= 0 && yLoop < WORLDSIZE)
                    { //make sure they're in the map
                        returnList.Add(GameState.GlobalMap.GetBlockAtCoordinate(coord));
                    }
                }
            }

            return returnList.ToArray();
        }

        /// <summary>
        /// Plots a gaussian curve, and returns the value of f(x) for the given x
        /// </summary>
        /// <param name="peak"></param>
        /// <param name="centre"></param>
        /// <param name="width"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double GetPointOnGaussianCurve(double peak, double centre, double width, double x)
        {
            return peak * Math.Pow(Math.E, -(Math.Pow((x - centre), 2) / Math.Pow(2 * width, 2)));


        }

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
                        //Copyable - If it may contain items, put a weight of 5, otherwise a weight of MaxValue. If there's already a road there, put a weight of 1. This'll encourage road reuse.
                        pf[i, j] = map[i, j] != null ? map[i, j].MayContainItems ? (map[i,j].Tile as GlobalTile).HasRoad ? (byte)1 : (byte)5 : Byte.MaxValue : Byte.MaxValue;
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

        #endregion

    }
}
