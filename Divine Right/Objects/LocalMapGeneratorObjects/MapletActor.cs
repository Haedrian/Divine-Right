using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling;
using DRObjects.Enums;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// Represent an Actor within a Maplet
    /// </summary>
    public class MapletActor:MapletContents
    {
        public int EnemyID { get; set; }

        public string EnemyType { get; set; }
        public string EnemyTag { get; set; }
        public VendorType? VendorType { get; set; }
        public int? VendorLevel { get; set; }

        /// <summary>
        /// Whether to use the type of enemy that dominates the area
        /// Ie - for a human town, use humans.
        /// </summary>
        public bool UseLocalType { get; set; }

        public ActorMissionType EnemyMission { get; set; }
    }
}
