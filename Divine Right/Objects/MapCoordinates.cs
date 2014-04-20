using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects
{
    /// <summary>
    /// Represents a coordinate on an xyz plane upon a map.
    /// </summary>
    public class MapCoordinate : IEquatable<MapCoordinate>
    {
        /// <summary>
        /// The location of this coordinate on the X plane
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// The location of this coordinate on the Y plane
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// The location of this coordinate on the Z plane
        /// </summary>
        public int Z { get; set; }

        /// <summary>
        /// Whether the map is global, local or something else
        /// </summary>
        public MapTypeEnum MapType { get; set; }

        /// <summary>
        /// Two coordinates are equal if they share the same X Y and Z coordinate
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //Is the type correct?
            if (obj.GetType().Equals(typeof(MapCoordinate)))
            {
                MapCoordinate comp = (MapCoordinate)obj;

                if (this.X.Equals(comp.X) && this.Y.Equals(comp.Y) && this.Z.Equals(comp.Z))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Compares two MapCoordinates with each other
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MapCoordinate other)
        {
            //this is used for the .contains
            if (this.X.Equals(other.X))
            {
                if (this.Y.Equals(other.Y))
                {
                    if (this.Z.Equals(other.Z))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Performs minus on two map coordinates, and gives the cartesian distance between them. The coordinates must be on the same MapType to work
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static int operator -(MapCoordinate c1, MapCoordinate c2)
        {
            //First check whether the types are the same
            if (c1.MapType.Equals(c2.MapType))
            {
                int dx = c1.X - c2.X;
                int dy = c1.Y - c2.Y;
                int dz = c1.Z - c2.Z;

                int distance = (int)Math.Round(Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2) + Math.Pow(dx, 2)));

                return distance;
            }
            else
            {
                throw new Exception("The maptypes for the two coordinates don't match");
            }
        }

        /// <summary>
        /// Performs plus on two map coordinates, and gives the map coordinate with the total of the two
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public static MapCoordinate operator +(MapCoordinate c1, MapCoordinate c2)
        {
            return new MapCoordinate(c1.X + c2.X, c1.Y + c2.Y, c1.Z + c2.Z, c1.MapType);
        }
        
        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
        }

        #region Constructor
        public MapCoordinate()
        {

        }

        public MapCoordinate(MapCoordinate coordinate)
           : this (coordinate.X,coordinate.Y,coordinate.Z,coordinate.MapType)
        {

        }

        public MapCoordinate(int x, int y, int z, MapTypeEnum mapType)
        {
            this.MapType = mapType;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public MapCoordinate Clone()
        {
            return this.MemberwiseClone() as MapCoordinate;
        }

        #endregion

    }
}
