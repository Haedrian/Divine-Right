using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling
{
    /// <summary>
    /// The attributes and skills of the particular actor
    /// </summary>
    public class ActorAttributes
    {
        public int Brawn { get; set; }
        public int Dex { get; set; }
        public int Agil { get; set; }
        public int Perc { get; set; }
        public int Intel { get; set; }

        //public int Sword { get; set; }
        //public int Axe { get; set; }
        //public int Club { get; set; }
        //public int Spear { get; set; }

        public int HandToHand { get; set; }
        public int Ranged { get; set; }
        public int Evasion { get; set; }

        public int Dodge
        {
            get
            {
                return Evasion + Agil - 5;
            }
        }

        public int WoundResist
        {
            get
            {
                return Brawn;
            }
        }
    }
}
