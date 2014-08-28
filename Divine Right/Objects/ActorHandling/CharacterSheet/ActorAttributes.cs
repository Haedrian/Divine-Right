using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling
{
    [Serializable]
    /// <summary>
    /// The attributes and skills of the particular actor
    /// </summary>
    public class ActorAttributes
    {
        #region Members

        private int brawn;
        private int dex;
        private int agil;
        private int perc;
        private int intel;

        #region Links
        public HumanoidAnatomy Health { get; set; }
        #endregion

        #endregion


        #region Properties

        #region Character Attributes

        public int BaseBrawn{get{return this.brawn;}}
        public int BaseDex{get{return this.dex;}}
        public int BaseAgil { get { return this.agil ;} }
        public int BasePerc{get{return this.perc;}}
        public int BaseIntel{get{return this.intel;}}

        public int Brawn { get { int total = TempBrawn?? 0 + (int)((double)brawn * Health.RightArm / Health.RightArmMax); return total > 1 ? total : 1; } set { brawn = value; } }
        public int Dex { get { int total = TempDex??0 + (int)((double)dex * Health.RightArm / Health.RightArmMax); return total > 1 ? total : 1; } set { dex = value; } }
        public int Agil { get { int total = TempAgil ?? 0 + (int)((double)brawn * Health.Legs/ Health.LegsMax); return total > 1 ? total : 1; } set { agil = value; } }
        public int Perc { get { int total = perc + (TempPerc ?? 0); return total > 1 ? total : 1; } set { perc = value; } }
        public int Intel { get { int total = intel + (TempIntel ?? 0); return total > 1 ? total : 1; } set { intel = value; } }

        public int? TempBrawn { get; set; }
        public int? TempDex { get; set; }
        public int? TempAgil { get; set; }
        public int? TempPerc { get; set; }
        public int? TempIntel { get; set; }

        public int HandToHand { get; set; }
        public int Ranged { get; set; }
        public int Evasion { get; set; }
        #endregion

        public int Dodge
        {
            get
            {
                return Evasion + Agil+5;
            }
        }

        public int WoundResist
        {
            get
            {
                return Brawn - 5;
            }
        }

        #endregion
    }
}
