using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet.Enums;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// Represent the Composition of Maplet Actors - that is to say how many of each type should be generated under which conditions
    /// </summary>
    public class MapletActorComposition:
        MapletContents
    {
        /// <summary>
        /// The profession we're considering
        /// </summary>
        public ActorProfession Profession { get; set; }
        /// <summary>
        /// The minimum possible amount to properly use this maplet
        /// </summary>
        public int MinAmount { get; set; }
    }
}
