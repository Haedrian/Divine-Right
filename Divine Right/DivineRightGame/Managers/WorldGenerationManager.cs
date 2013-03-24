using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DivineRightGame.Managers.HelperObjects;
using DRObjects;
using DRObjects.Items.Tiles.Global;
using DRObjects.Enums;
using DivineRightGame.Managers.HelperFunctions;

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
        public const int RAINCENTERCOUNT = WORLDSIZE/50;

        public const int REGIONSIZE = WORLDSIZE*2;

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

            //start the map
            GameState.GlobalMap = new GlobalMap(WORLDSIZE);

            regions = new Region[REGIONSIZE + 1];

            //fill the regions with new regions

            for (int i = 0; i < regions.Length; i++)
            {
                regions[i] = new Region();
            }

                //populate the world map with a number of tiles with an elevation of 40

                CurrentStep = "And in the Beginning, he Pondered on what the world would become...";

                for (int x = 0; x < WORLDSIZE; x++)
                {
                    lock (GlobalMap.lockMe)
                    { //this is rather inefficient, but it will allow the interface to draw something
                        //CurrentStep = "Populating Tiles for line :" + x;

                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        MapBlock block = new MapBlock();
                        block.Tile = new GlobalTile();
                        block.Tile.Coordinate = new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL);
                        (block.Tile as GlobalTile).Elevation = 40;

                        GameState.GlobalMap.AddToGlobalMap(block);
                    }
                    }
                }
            

            lock (GlobalMap.lockMe)
            {
                //set region 0, this is the border of the map

                CurrentStep = "And then he decreed the world would be surrounded by a border of waters";

                for (int x = 0; x < WORLDSIZE; x++)
                {
                    for (int y = 0; y < WORLDSIZE/100; y++)
                    {
                        regions[0].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x,y,0,MapTypeEnum.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x,y,0,MapTypeEnum.GLOBAL)).Tile as GlobalTile).Region = 0;

                        regions[0].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(y,x, 0, MapTypeEnum.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(y,x,0,MapTypeEnum.GLOBAL)).Tile as GlobalTile).Region = 0;
                        
                        regions[0].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(WORLDSIZE - 1 - y, x, 0, MapTypeEnum.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(WORLDSIZE - 1 - y,x,0,MapTypeEnum.GLOBAL)).Tile as GlobalTile).Region = 0;
                        
                        regions[0].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x,WORLDSIZE - 1 - y, 0, MapTypeEnum.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x,WORLDSIZE - 1 - y,0,MapTypeEnum.GLOBAL)).Tile as GlobalTile).Region = 0;

                        
                        regions[0].Center = new MapCoordinate(-1,-1,-1,MapTypeEnum.GLOBAL);
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

            Random random = new Random();

            lock (GlobalMap.lockMe)
            {
                CurrentStep = "Then he pondered on the regions the world would have";
                //setting the region centres - 0 doesn't count
                for (int i = 1; i < regions.Length; i++)
                {
                    int x = random.Next(WORLDSIZE - 1);
                    int y = random.Next(WORLDSIZE - 1);

                    if ((GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL)).Tile as GlobalTile).Region == -1)
                    {
                        //not assigned

                        //set this location as the centre
                        regions[i].Center = new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL);
                        regions[i].Blocks.Add(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL)));
                        (GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL)).Tile as GlobalTile).Region = i;
                    }
                    else
                    {
                        i--; //try again
                    }

                 }
            }


                CurrentStep = "And he pondered on the shape of the world";
                for (int x = 0; x < WORLDSIZE; x++)
                {
                    int assignedRegion = -1;

                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        //populate the regions
                        lock (GlobalMap.lockMe)
                        {
                           // CurrentStep = "And thus the Point " + x + "," + y + " was grouped with its brethren";

                            //Is this tile already in a region?

                            MapCoordinate currentCoordinate = new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL);

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

            //now we set random elevations for different regions

                CurrentStep = "And with a thought, the world took shape";
                for (int i = 1; i <= REGIONSIZE; i++)
                {
                    MapCoordinate regionCentre = regions[i].Center;
                    int elev;

                    double cartDistance = Math.Sqrt(Math.Pow(WORLDSIZE/2 - regionCentre.X,2) + Math.Pow(WORLDSIZE/2 - regionCentre.Y,2));

                    if (cartDistance > WORLDSIZE * 0.4 - (GetRandomGaussian() * WORLDSIZE * 0.1 + WORLDSIZE * 0.1))
                    {

                        elev = random.Next(90) - 100;
                    }
                    else
                    {
                        elev = random.Next(180);
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
                            (block.Tile as GlobalTile).Elevation = (int) Math.Round((double) elev);
                        }


                    }

                }

            //now we smooth the map
                CurrentStep = "And the earthquakes came, and the earth shifted";
            
                for (int x = 0; x < WORLDSIZE; x++)
                {
                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        //CurrentStep = "And the tile " + x + "," + y + " was shifted by the earth";

                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x,y,0,MapTypeEnum.GLOBAL));
                        
                        lock (GlobalMap.lockMe)
                        {
                            (block.Tile as GlobalTile).Elevation = Interpolation.InverseDistanceWeighting(block,EXPONENTWEIGHT,WORLDSIZE);
                        }
                    }
                }                
            
            //and now we erode the map
                CurrentStep = "And then the winds came, and the world was eroded";

                for (int x = 0; x < WORLDSIZE; x++)
                {
                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        //We need to smooth the nearest neighbour

                        //CurrentStep = "Eroding the tile " + x + "," + y;
                        lock (GlobalMap.lockMe)
                        {
                            MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL));

                            (block.Tile as GlobalTile).Elevation = Interpolation.NearestNeighbour(WORLDSIZE, block);
                        }

                    }
                }

                for (int x = WORLDSIZE-1; x >= 0; x--)
                {
                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        //We need to smooth the nearest neighbour

                        //CurrentStep = "Eroding the tile " + x + "," + y;
                        lock (GlobalMap.lockMe)
                        {
                            MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL));

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
                            MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL));

                            (block.Tile as GlobalTile).Elevation = Interpolation.NearestNeighbour(WORLDSIZE, block);
                        }

                    }
                }

                for (int y = 0; y < WORLDSIZE; y++)
                {
                    for (int x = WORLDSIZE-1; x >= 0; x--)
                    {
                        //We need to smooth the nearest neighbour

                        //CurrentStep = "Eroding the tile " + x + "," + y;
                        lock (GlobalMap.lockMe)
                        {
                            MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL));

                            (block.Tile as GlobalTile).Elevation = Interpolation.NearestNeighbour(WORLDSIZE, block);
                        }

                    }
                }


           //How about some rivers?
                CurrentStep = "And the rains began, and the rivers formed";

            //We need the get the River-Count highest points on the map
            
                List<MapBlock> blocks = new List<MapBlock>();

                //Lets take the highest point of each region
                foreach (Region region in regions)
                {
                    blocks.AddRange(region.Blocks.OrderByDescending(b => (b.Tile as GlobalTile).Elevation).Take(1));
                }
                
                //now take the top River-Count

                List<MapBlock> blocksToProcess = blocks.OrderByDescending(b => (b.Tile as GlobalTile).Elevation).Take(RIVERCOUNT).ToList();

                for (int i=0; i < blocksToProcess.Count; i++)
                {
                    MapBlock block = blocksToProcess[i];

                    CurrentStep = "And the rivers travelled to the waves or the flatter land";

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
                                surroundingTiles.Add((GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(currentTile.Coordinate.X + x, currentTile.Coordinate.Y + y, 0, MapTypeEnum.GLOBAL)).Tile as GlobalTile));
                            }
                        }

                        //now pick the smallest tile which has no river

                        if (surroundingTiles.Count == 0)
                        {
                            run = false;
                            break;
                        }

                        nextTile = surroundingTiles.Where(st => !st.HasRiver).OrderBy(st => st.Elevation).FirstOrDefault();

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
            /*
                //CurrentStep = "Drawing Contours";

            //we need to go through them one at a time

                for (int x = 0; x < WORLDSIZE; x++)
                {
                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x,y,0,MapTypeEnum.GLOBAL));

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
                                    surroundingTiles.Add((GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(block.Tile.Coordinate.X + x2, block.Tile.Coordinate.Y + y2, 0, MapTypeEnum.GLOBAL)).Tile as GlobalTile));
                                }
                            }

                            //now we check the top one
                            if (surroundingTiles.OrderByDescending(st => st.Elevation).FirstOrDefault().Elevation > (block.Tile as GlobalTile).Elevation +25)
                            {
                                //it is - mark this tile as a contour
                                (block.Tile as GlobalTile).HasContour = true;
                            }

                        }
                    }
                }
             * */

                CurrentStep = "And the sun shone upon the world, and it saw heat";

            //To set temperatures we will do the following. We will design a gaussian graph, with a max of 40 at the equator, descending with distance from it
            //We will also set a similar graph, with a max of 0 and a min of -40 with respect to elevation.
            //The temperature of each point will be the sum of both

                for (int x = 0; x < WORLDSIZE; x++)
                {
                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL));

                        //determine the temperature we'll assign it
                        GlobalTile tile = (block.Tile as GlobalTile);

                        tile.ClimateTemperature = (decimal)WorldGenerationManager.GetPointOnGaussianCurve(31, WORLDSIZE / 2, WORLDSIZE / 10, tile.Coordinate.Y);
                            
                            if (tile.Elevation > 0)
                            {
                                tile.ClimateTemperature+= (decimal) WorldGenerationManager.GetPointOnGaussianCurve(20,0,300,tile.Elevation) - 20;
                            }
                            else if (tile.HasRiver)
                            {
                                //rivers tend to be cooler
                                tile.ClimateTemperature -= 5;
                            }
                            else 
                            {
                                //water tiles tend to be warmer in their temperature
                                tile.ClimateTemperature+=5;
                            }
                    }

                }

                CurrentStep = "With a single word, the storms came, bringing water to the lands...";

            //To determine rain we will do the following. First we set rain at each location at 0 (dry)
            //Then we will drop a number of 'centres of rain' randomly which will have 10 'rain'
            //We will then put decreasing amounts of rain from these centres.

            //first set all tiles to 0
                for (int i = 0; i < WORLDSIZE; i++)
                {
                    for (int j = 0; j < WORLDSIZE; j++)
                    {
                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(i, j, 0, MapTypeEnum.GLOBAL));

                        //determine the temperature we'll assign it
                        GlobalTile tile = (block.Tile as GlobalTile);

                        tile.Rainfall = 0;
                    }
                }

            //now we create a random x and y coordinate and we create some rain

                for (int i = 0; i < RAINCENTERCOUNT; i++)
                {
                    int xRain = random.Next(WORLDSIZE);
                    int yRain = random.Next(WORLDSIZE);

                    decimal rainAmount = 10;
                    int radius = 0;

                    while (rainAmount > 0)
                    {
                        MapBlock[] rainBlocks =GetCircleAroundPoint(new MapCoordinate(xRain, yRain, 0, MapTypeEnum.GLOBAL),radius);

                        foreach (MapBlock block in rainBlocks)
                        {
                            (block.Tile as GlobalTile).Rainfall += rainAmount;
                        }

                        rainAmount -= .10M;

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

                CurrentStep = "And then came life";

            //And now we determine their biome
                for (int x = 0; x < WORLDSIZE; x++)
                {
                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL));

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
                                tile.Biome = GlobalBiome.WETLAND;
                            }
                            else if (tile.Rainfall < 3)
                            {
                                tile.Biome = GlobalBiome.POLAR_DESERT;
                            }

                            //otherwise, there's a small chance you get a forest

                            if (randomInt > 7)
                            {
                                tile.Biome = GlobalBiome.WOODLAND;
                            }

                        }
                        else if (tile.ClimateTemperature  < 4)
                        {
                            //freezing cold
                            tile.Biome = GlobalBiome.POLAR_DESERT;
                        }

                    }
                }


            CurrentStep = "And he looked upon all he had created, and it was done";



        }

#region Helper Functions

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
                         0 + 1* randStdNormal; //random normal(mean,stdDev^2)

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
                        MapCoordinate coord = new MapCoordinate(xLoop, yLoop, 0,MapTypeEnum.GLOBAL);

                        if (xLoop >= 0 && xLoop < WORLDSIZE && yLoop >= 0 && yLoop < WORLDSIZE)
                        { //make sure they're in the map
                            returnList.Add(GameState.GlobalMap.GetBlockAtCoordinate(coord));
                        }
                    }
                }

                return returnList.Where(r => r.Tile.Coordinate.X.Equals(minX) || r.Tile.Coordinate.X.Equals(maxX) || r.Tile.Coordinate.Y.Equals(minY) || r.Tile.Coordinate.Y.Equals(maxY)).ToArray();
        }

        /// <summary>
        /// Plots a gaussian curve, and returns the value of f(x) for the given x
        /// </summary>
        /// <param name="peak"></param>
        /// <param name="centre"></param>
        /// <param name="width"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static double GetPointOnGaussianCurve(double peak, double centre, double width,double x)
        {
            return peak * Math.Pow(Math.E, -(Math.Pow((x - centre), 2) / Math.Pow(2 * width, 2)));


        }

#endregion

    }
}
