using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.ActorMissions
{
    [Serializable]
    /// <summary>
    /// A point to patrol towards
    /// </summary>
    public class PatrolPoint
    {
        /// <summary>
        /// The coordinate to patrol towards
        /// </summary>
        public MapCoordinate Coordinate { get; set; }

        /// <summary>
        /// The Acceptable Radius to walk towards
        /// </summary>
        public int AcceptableRadius { get; set; }
    }
}
