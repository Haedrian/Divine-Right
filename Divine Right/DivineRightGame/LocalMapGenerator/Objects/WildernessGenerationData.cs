using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightGame.LocalMapGenerator.Objects
{
    /// <summary>
    /// Holds some basic data about the Wilderness to Generate
    /// The base tile
    /// The normal vegetation
    /// And how much of each
    /// </summary>
    public class WildernessGenerationData
    {
        public string BaseTileTag { get; set; }
        public int TreeCount { get; set; }
        public string TreeTag { get; set; }
    }
}
