using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.SpecialAttacks
{
    /// <summary>
    /// Holds data on the points and effects required to progress in a particular special attack component
    /// </summary>
    public class SpecialAttackComponentProgression
    {
        public SpecialAttackType Type { get; set; }
        public int[] Progressions { get; set; }
        public int PointCost { get; set; }
    }
}
