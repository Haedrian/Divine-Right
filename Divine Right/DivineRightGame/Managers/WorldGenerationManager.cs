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

                CurrentStep = "Populating Tiles";

                for (int x = 0; x < WORLDSIZE; x++)
                {
                    lock (GlobalMap.lockMe)
                    { //this is rather inefficient, but it will allow the interface to draw something
                        CurrentStep = "Populating Tiles for line :" + x;

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

                CurrentStep = "Setting Border";

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
                CurrentStep = "Reducing Border Height";

                for (int i = 0; i < regions[0].Blocks.Count; i++)
                {
                    //set them all to -100
                    (regions[0].Blocks[i].Tile as GlobalTile).Elevation = -100;
                }
            }

            Random random = new Random();

            lock (GlobalMap.lockMe)
            {
                CurrentStep = "Choosing Regions";
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


                CurrentStep = "Populating the Regions";
                for (int x = 0; x < WORLDSIZE; x++)
                {
                    int assignedRegion = -1;

                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        //populate the regions
                        lock (GlobalMap.lockMe)
                        {
                            CurrentStep = "Assigning Point " + x + "," + y + " to a region";

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

                CurrentStep = "Setting elevations";
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
                        CurrentStep = "Setting elevation for block  " + block.Tile.Coordinate.X + " " + block.Tile.Coordinate.Y;
                        lock (GlobalMap.lockMe)
                        {
                            (block.Tile as GlobalTile).Elevation = (int) Math.Round((double) elev);
                        }


                    }

                }

            //now we smooth the map
                CurrentStep = "Smoothing the World";
            
                for (int x = 0; x < WORLDSIZE; x++)
                {
                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        CurrentStep = "Smoothing the tile " + x + "," + y;

                        MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x,y,0,MapTypeEnum.GLOBAL));
                        
                        lock (GlobalMap.lockMe)
                        {
                            (block.Tile as GlobalTile).Elevation = Interpolation.InverseDistanceWeighting(block,EXPONENTWEIGHT,WORLDSIZE);
                        }
                    }
                }                
            
            //and now we erode the map
                CurrentStep = "Eroding the World";

                for (int x = 0; x < WORLDSIZE; x++)
                {
                    for (int y = 0; y < WORLDSIZE; y++)
                    {
                        //We need to smooth the nearest neighbour

                        CurrentStep = "Eroding the tile " + x + "," + y;
                        lock (GlobalMap.lockMe)
                        {
                            MapBlock block = GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL));

                            (block.Tile as GlobalTile).Elevation = Interpolation.NearestNeighbour(WORLDSIZE, block);
                        }

                    }
                }

            /*
            //get the coordinates
                MapCoordinate[] coordinates = regions.Select(r => r.Center).ToArray();
            //get the elevation
                int[] elevation = regions.Select(r => (r.Blocks[0].Tile as GlobalTile).Elevation).ToArray();
            
            //smooth out the edges of region 0

                foreach (MapBlock block in regions[0].Blocks)
                {



                }

             */
            CurrentStep = "Done :) ";



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


#endregion

    }
}
