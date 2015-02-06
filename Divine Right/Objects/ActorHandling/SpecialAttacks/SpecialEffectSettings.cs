using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.SpecialAttacks
{
    /// <summary>
    /// Define the settings for special effects
    /// </summary>
    public class SpecialEffectSettings
    {
        /// <summary>
        /// Defines the Point Cost Progression
        /// </summary>
        public int[] PointProgression { get; set; }

        public List<SpecialAttackComponentProgression> EffectCosts { get; set; }
    }
}
