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
    public class MapCoordinate
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
        
        public override string ToString()
        {
            return "(" + X + "," + Y + "," + Z + ")";
        }

        #region Constructor
        public MapCoordinate()
        {

        }

        public MapCoordinate(int x, int y, int z, MapTypeEnum mapType)
        {
            this.MapType = mapType;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        #endregion
    }
}
