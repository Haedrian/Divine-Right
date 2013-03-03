using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Enums;
using DRObjects.Items.Tiles.Global;

namespace DivineRightGame.Managers.HelperFunctions
{
    /// <summary>
    /// Various methods of interpolation.
    /// Different methods can be more appropriate for certain applications than others
    /// </summary>
    public static class Interpolation
    {
        public static int InverseDistanceWeighting(MapBlock block, double p, int worldSize)
        {
            Random random = new Random();
            int sample = 50;
            int radius = 2;
            int x = 0;
            int y = 0;
            MapCoordinate[] coorddata = new MapCoordinate[sample];
            Double[] resultdata = new Double[sample];
            
            for (int i = 0; i < sample; i++)
            {
                x = block.Tile.Coordinate.X + random.Next(radius * 2) - radius;
                y = block.Tile.Coordinate.Y + random.Next(radius * 2) - radius;
                
                if (x < 0) 
                { 
                    x = 2; 
                }
                if (x >= worldSize) 
                { 
                    x = worldSize - 2; 
                }
                
                if (y < 0)
                { 
                    y = 2; 
                }
                
                if (y >= worldSize) 
                { 
                    y = worldSize - 2; 
                }

                coorddata[i] = new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL);

                resultdata[i] = (double)(GameState.GlobalMap.GetBlockAtCoordinate(new MapCoordinate(x, y, 0, MapTypeEnum.GLOBAL)).Tile as GlobalTile).Elevation;
            }
            
            double sum1 = 0;
            double sum2 = 0;

            for (int j = 0; j < coorddata.Length; j++)
            {
                if (coorddata[j] - (block.Tile.Coordinate) != 0)
                {
                    sum2 = sum2 + Math.Pow(1 / (coorddata[j] - (block.Tile.Coordinate)), p);
                }
            }
            for (int i = 0; i < coorddata.Length; i++)
            {
                if (coorddata[i] - (block.Tile.Coordinate) != 0) 
                { 
                    sum1 += Math.Pow(1 / (coorddata[i] - block.Tile.Coordinate), p) * resultdata[i] / sum2; 
                }
                else 
                { 
                    //sum1 = resultdata[i]; 
                }
            }


            return (int)sum1;
        }


    }
}
