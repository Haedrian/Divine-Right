using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.ActorHandling.CharacterSheet
{
    [Serializable]
    /// <summary>
    /// A piece of knowledge stating that a particular potion may be identified by the character
    /// </summary>
    public class PotionIdentification
    {
        public PotionType PotionType { get; set; }
    }
}
