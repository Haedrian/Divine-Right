using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling
{
    /// <summary>
    /// Represents an Actor's Health and anything that can go wrong with them
    /// </summary>
    public class HumanoidAnatomy
    {
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

    }
}
