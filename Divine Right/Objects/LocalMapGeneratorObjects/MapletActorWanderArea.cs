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
        public ActorProfession Profession { get; set; }

        /// <summary>
        /// The rectangle to wander in. Set by the generator
        /// </summary>
        public Rectangle WanderRect { get; set; }
    }
}
