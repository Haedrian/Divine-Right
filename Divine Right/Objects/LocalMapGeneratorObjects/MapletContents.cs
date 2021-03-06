﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.LocalMapGeneratorObjects.Enums;

namespace DRObjects.LocalMapGeneratorObjects
{
    [Serializable]
    /// <summary>
    /// The Possible Contents of the Maplet.
    /// For each possible amount, there will be a check whether probability is reached
    /// If you reach the probability, one will be created at each loop.
    /// If there is less than min amount after this is done, then we'll forget all about it
    /// </summary>
    public class MapletContents
    {
        /// <summary>
        /// The probability of creating the Contents. Will be run for each potential amount.
        /// </summary>
        public double ProbabilityPercentage { get; set; }
        /// <summary>
        /// The Maximum Amount to be created
        /// </summary>
        public int MaxAmount { get; set; }

        /// <summary>
        /// If this is set to true, the item will allow other items to be placed on top of it
        /// </summary>
        public bool AllowItemsOnTop { get; set; }

        /// <summary>
        /// Determines whether this maplet prefers to be positioned.
        /// </summary>
        public PositionAffinity Position { get; set; }

        /// <summary>
        /// For items with an affinity of "sides" - will shift the sides by the padding (ie - with a padding of N, will skip N tiles).
        /// For other affinities, it is ignored
        /// </summary>
        public int? Padding { get; set; }

        /// <summary>
        /// If the position is fixed, the SINGLE item will be placed in this location. Otherwise will be ignored
        /// </summary>
        public int? x { get; set; }

        /// <summary>
        /// If the position is fixed, the SINGLE item will be placed in this location. Otherwise will be ignored
        /// </summary>
        public int? y { get; set; }

        /// <summary>
        /// A comma seperated representation of the factions which own this mcit
        /// </summary>
        public string OwnerFactions { get; set; }

        /// <summary>
        /// The factions which own this object
        /// </summary>
        public OwningFactions Factions
        {
            get
            {
                if (string.IsNullOrEmpty(OwnerFactions))
                {
                    //All of them
                    return OwningFactions.ABANDONED | OwningFactions.BANDITS | OwningFactions.HUMANS | OwningFactions.ORCS | OwningFactions.UNDEAD;
                }

                OwningFactions fact =  OwningFactions.ABANDONED;
                //Parse it
                foreach (var own in OwnerFactions.Split(','))
                {
                    OwningFactions ownParsed = (OwningFactions)Enum.Parse(typeof(OwningFactions), own.ToUpper());

                    if (fact == OwningFactions.ABANDONED)
                    {
                        fact = ownParsed;
                    }
                    else
                    {
                        fact = fact | ownParsed;
                    }
                }

                return fact;
            }
        }
    }
}
