using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.SpecialAttacks
{
    [Serializable]
    /// <summary>
    /// Defines how effective a special attack is for a particular type of weapon, and the colour to mark it
    /// </summary>
    public class Effect
    {
        public SpecialAttackType EffectType { get; set; }
        public int EffectValue { get; set; }
    }
}
