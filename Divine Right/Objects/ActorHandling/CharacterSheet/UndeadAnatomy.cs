using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.CharacterSheet
{
    [Serializable]
    /// <summary>
    /// Anatomy for Undead
    /// Equivalent to humanoid anatomy, except that they don't stun and don't bleed
    /// </summary>
    public class UndeadAnatomy :
        IAnatomy
    {
        /// <summary>
        /// Impossible to bleed out
        /// </summary>
        public int BloodLoss { get { return 0; } set{} }
        public int BloodTotal { get { return 100; } set { } }
        public int BodyTimer { get; set; }
        public int Chest { get; set; }
        public int ChestMax { get; set; }
        public int Head { get; set; }
        public int HeadMax { get; set; }
        public int LeftArm { get; set; }
        public int LeftArmMax { get; set; }
        public int Legs { get; set; }
        public int LegsMax { get; set; }
        public int RightArm { get; set; }
        public int RightArmMax { get; set; }
        /// <summary>
        /// Impossible to stun
        /// </summary>
        public int StunAmount { get { return 0; } set { } }

    }
}
