using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet.Enums;

namespace DRObjects.LocalMapGeneratorObjects
{
    public class MapletPatrolPoint
        : MapletContents
    {
        /// <summary>
        /// To be set by XML parsser
        /// </summary>
        public MapCoordinate Point { get; set; }

        /// <summary>
        /// The name of that particular patrol route
        /// </summary>
        public string PatrolName { get; set; }

        /// <summary>
        /// The profession for whom this patrol point is valid
        /// </summary>
        public ActorProfession Profession { get; set; }

        /// <summary>
        /// The acceptable radius to reach to be considered at the point
        /// </summary>
        public int PointRadius { get; set; }
    }
}
