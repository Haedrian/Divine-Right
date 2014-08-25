﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Global
{
    /// <summary>
    /// A Dungeon on the map. A maze full of mean creatures
    /// </summary>
    public class Dungeon
        :MapItem
    {
        public int TierCount { get; set; }
        public int TrapRooms { get; set; }
        public int GuardRooms { get; set; }
        public int TreasureRoom { get; set; }
        public string OwnerCreatureType { get; set; }
        public double PercentageOwned { get; set; }
        public int MaxWildPopulation { get; set; }
        public int MaxOwnedPopulation { get; set; }
    }
}