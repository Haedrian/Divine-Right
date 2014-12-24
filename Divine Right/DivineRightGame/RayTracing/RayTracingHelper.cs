using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using Microsoft.Xna.Framework;

namespace DivineRightGame.RayTracing
{
    /// <summary>
    /// Class for helping with Ray Tracing
    /// </summary>
    public static class RayTracingHelper
    {
        private static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        // Returns the list of points from p0 to p1 
        public static List<Point> BresenhamLine(Point p0, Point p1)
        {
            return BresenhamLine(p0.X, p0.Y, p1.X, p1.Y);
        }

        /// <summary>
        /// Returns the list of points from (x0, y0) to (x1, y1)
        /// </summary>
        /// <param name="x0"></param>
        /// <param name="y0"></param>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <author>http://www.codeproject.com/Articles/15604/Ray-casting-in-a-D-tile-based-environment</author>
        /// <returns></returns>
        public static List<Point> BresenhamLine(int x0, int y0, int x1, int y1)
        {
            // Optimization: it would be preferable to calculate in
            // advance the size of "result" and to use a fixed-size array
            // instead of a list.
            List<Point> result = new List<Point>();

            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                Swap(ref x0, ref y0);
                Swap(ref x1, ref y1);
            }
            if (x0 > x1)
            {
                Swap(ref x0, ref x1);
                Swap(ref y0, ref y1);
            }

            int deltax = x1 - x0;
            int deltay = Math.Abs(y1 - y0);
            int error = 0;
            int ystep;
            int y = y0;
            if (y0 < y1) ystep = 1; else ystep = -1;
            for (int x = x0; x <= x1; x++)
            {
                if (steep) result.Add(new Point(y, x));
                else result.Add(new Point(x, y));
                error += deltay;
                if (2 * error >= deltax)
                {
                    y += ystep;
                    error -= deltax;
                }
            }

            return result;
        }

        /// <summary>
        /// Perform a ray trace on the map passed, following the usual rules.
        /// This will modify the map collection passed.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="player"></param>
        public static void RayTrace(GraphicalBlock[] blocks, MapCoordinate player)
        {
            foreach (GraphicalBlock block in blocks)
            {
                var points = BresenhamLine(block.MapCoordinate.X, block.MapCoordinate.Y, player.X, player.Y).ToList();

                //Remove first and last point from the trace
                if (points.Count > 0)
                {
                    points.RemoveAt(points.Count - 1);
                }
                if (points.Count > 0)
                {
                    points.RemoveAt(0);
                }

                if (points.Any(p => blocks.Any(b => b.MapCoordinate.Equals(new MapCoordinate(p.X, p.Y, 0, MapType.LOCAL)) && !b.IsSeeThrough)))
                {
                    //Can't be seen
                    if (!block.WasVisited)
                    {
                        block.TileGraphics = new SpriteData[] { };
                        block.ItemGraphics = new SpriteData[] { };
                    }
                    else
                    {
                        block.IsOld = true;
                    }
                }

            }
        }

        public static MapBlock[] RayTraceForExploration(MapBlock[] blocks, MapCoordinate player)
        {
            List<MapBlock> visibles = new List<MapBlock>();
            foreach (MapBlock block in blocks)
            {
                var points = BresenhamLine(block.Tile.Coordinate.X, block.Tile.Coordinate.Y, player.X, player.Y).ToList();

                //Remove first and last point from the trace
                if (points.Count > 0)
                {
                    points.RemoveAt(points.Count - 1);
                }
                if (points.Count > 0)
                {
                    points.RemoveAt(0);
                }

                if (points.Any(p => blocks.Any(b => b.Tile.Coordinate.Equals(new MapCoordinate(p.X, p.Y, 0, MapType.LOCAL)) && !b.IsSeeThrough)))
                {
                    //Can't be seen
                }
                else
                {
                    visibles.Add(block);
                }
            }

            return visibles.ToArray();
        }
    }
}
