using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling
{
    [Serializable]
    /// <summary>
    /// Represents an Actor's Health and anything that can go wrong with them
    /// </summary>
    public class HumanoidAnatomy
    {
        public const int BLOOD_STUN_AMOUNT = 15;

        public const int BLOODTOTAL = 100;
        public const int BODY_TIMER_FLIP = 5;

        public int Head { get; set; }
        public int HeadMax { get; set; }

        public int LeftArm { get; set; }
        public int LeftArmMax { get; set; }

        public int RightArm { get; set; }
        public int RightArmMax { get; set; }

        public int Chest { get; set; }
        public int ChestMax { get; set; }

        public int Legs { get; set; }
        public int LegsMax { get; set; }

        /// <summary>
        /// Total amount of blood in the body
        /// </summary>
        public int BloodTotal { get; set; }
        /// <summary>
        /// The amount of blood being lost
        /// </summary>
        public int BloodLoss { get; set; }
        /// <summary>
        /// The amount of stun which the character is suffering from
        /// </summary>
        public int StunAmount { get; set; }
        /// <summary>
        /// Where the character's body timer is. When this runs to BODY_TIMER_FLIP - both blood loss and stun amount go down by 1
        /// </summary>
        public int BodyTimer { get; set; }
    }
}
