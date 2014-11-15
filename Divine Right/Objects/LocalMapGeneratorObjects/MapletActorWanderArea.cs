using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using Microsoft.Xna.Framework;

namespace DRObjects.LocalMapGeneratorObjects
{
    [Serializable]
    /// <summary>
    /// Describes an area where actors of a particular profession may wander
    /// </summary>
    public class MapletActorWanderArea:
        MapletContents
    {
        /// <summary>
        /// The total current amount of actors in that area
        /// </summary>
        public int CurrentAmount { get; set; }

        public ActorProfession Profession { get; set; }

        /// <summary>
        /// The rectangle to wander in. Set by the generator
        /// </summary>
        public Rectangle WanderRect { get; set; }

        /// <summary>
        /// The point to wander around
        /// </summary>
        public MapCoordinate WanderPoint { get; set; }

        public MapletActorWanderArea()
        {
            CurrentAmount = 0;
        }
    }
}
