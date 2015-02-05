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
    public class Effectivness
    {
        public string WeaponType { get; set; }
        public int EffectValue { get; set; }

        /// <summary>
        /// [1..3] - 1 being very effective, 2 being less so, 3 being even less
        /// </summary>
        public int EffectivnessLevel { get; set; }
    }
}
