using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.Enums;

namespace DRObjects.ActorHandling
{
    /// <summary>
    /// Represents a temporary effect on a character
    /// </summary>
    public class Effect
    {
        public EffectName Name { get; set; }
        public int EffectAmount { get; set; }
        public int MinutesLeft { get; set; }
    }
}
