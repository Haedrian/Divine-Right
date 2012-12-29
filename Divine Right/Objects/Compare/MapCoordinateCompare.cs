using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Compare
{
    public class MapCoordinateCompare: IEqualityComparer<MapCoordinate>
    {
        public bool Equals(MapCoordinate a, MapCoordinate b)
        {
            if (a.X.Equals(b.X))
            {
                if (a.Y.Equals(b.Y))
                {
                    if (a.Z.Equals(b.Z))
                    {
                        return true;
                    }
                }

            }

            return false;
        }

        public int GetHashCode(MapCoordinate obj)
        {
            //TODO - MIGHT WANT TO MAKE THIS MORE EFFICIENT LATER
            return base.GetHashCode();
        }
    }
}
