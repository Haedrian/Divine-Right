using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;

namespace DivineRightGame.Managers.HelperObjects
{
    /// <summary>
    /// Represents a line in the map
    /// </summary>
    public class Line
    {
        public MapCoordinate Start { get; set; }
        public MapCoordinate End { get; set; }


#region Constructors
        public Line(MapCoordinate start, MapCoordinate end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Creates a line on a 2D plane with specified x1y1 x2y2 coordinates
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="endX"></param>
        /// <param name="endY"></param>
        public Line(int startX, int startY, int endX, int endY)
        {
            this.Start = new MapCoordinate(startX, startY, 0, DRObjects.Enums.MapType.LOCAL);
            this.End = new MapCoordinate(endX, endY, 0, DRObjects.Enums.MapType.LOCAL);
        }

        public Line()
        {

        }
#endregion

        /// <summary>
        /// Returns the Euclidean lenght of the line. If any of the values are null, will throw an exception
        /// </summary>
        /// <returns></returns>
        public double Length()
        {
            //Using pytagoras
            double deltaX = Math.Abs(Start.X - End.X);
            double deltaY = Math.Abs(Start.Y - End.Y);
            double deltaZ = Math.Abs(Start.Z - End.Z);

            //Yes I know about Math.Pow - but readability...
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
        }

        /// <summary>
        /// Gets the midpoint coordinate of this line. Rounded
        /// </summary>
        /// <returns></returns>
        public MapCoordinate GetRoundedMidpoint()
        {
            return new MapCoordinate((int)Math.Round((double)(Start.X + End.X) / 2), (int)Math.Round((double)(Start.Y + End.Y) / 2), (int)Math.Round((double)(Start.Z + End.Z) / 2), Start.MapType);
        }
    }
}
