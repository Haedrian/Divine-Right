using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling
{
    /// <summary>
    /// Data regarding the particular race
    /// </summary>
    public class RaceData
    {
        public String RaceName { get; set; }
        public int BrawnModifier { get; set; }
        public int AgilModifier { get; set; }
        public int DexModifier { get; set; }
        public int PercModifier { get; set; }
        public int IntelModifier { get; set; }
        public bool IsIntelligent { get; set; }
    }
}
