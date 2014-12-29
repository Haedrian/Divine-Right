using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Global
{
    /// <summary>
    /// Contains some details about the Dungeon Level
    /// </summary>
    public class Dungeon
        : Location
    {
        public int DifficultyLevel { get; set; }
        public List<SummoningCircle> SummoningCircles { get; set; }


    }
}
