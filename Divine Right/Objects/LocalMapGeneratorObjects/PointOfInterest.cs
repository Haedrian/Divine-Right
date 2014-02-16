using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// Describes a point of interest on the local map
    /// </summary>
    public class PointOfInterest
    {
        public MapCoordinate Coordinate { get; set; }
        public PointOfInterestType Type { get; set; }
    }
}
