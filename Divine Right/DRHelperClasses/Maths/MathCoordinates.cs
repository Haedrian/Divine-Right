using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;

namespace DRHelperClasses.Maths
{
    /// <summary>
    /// Static helper class for performing Mathematics on coordinates
    /// </summary>
    public static class MathCoordinates
    {
        #region Distance Calculation

        /// <summary>
        /// Calculates the Cartisian Displacement between two coordinates on the XY plane
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public double GetCartisianDisplacementOnXYPlane(MapCoordinate c1, MapCoordinate c2)
        {
            //using pythagoras
            //H = sqrt(Delta X ^2 + Delta Y ^ 2)

            int deltaX = Math.Abs(c1.X - c2.X);
            int deltaY = Math.Abs(c1.Y - c2.Y);

            return Math.Sqrt(Math.Pow(deltaX, 2) + Math.Pow(deltaY, 2));
        }
        /// <summary>
        /// Calculates the Manhatten distance on the XY plane between two coordinates
        /// </summary>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        /// <returns></returns>
        public int GetManhattenDistanceOnXYPlane(MapCoordinate c1, MapCoordinate c2)
        {
            int deltaX = Math.Abs(c1.X - c2.X);
            int deltaY = Math.Abs(c1.Y - c2.Y);

            return deltaX + deltaY; 

        }

        #endregion Distance Calculation

    }
}
