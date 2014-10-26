using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Enums
{
    [Serializable]
    /// <summary>
    /// Describes the biome of the current tile
    /// </summary>
    public enum GlobalBiome
    {
        RAINFOREST,
        ARID_DESERT,
        POLAR_DESERT,
        POLAR_FOREST,
        DENSE_FOREST,
        WOODLAND,
        GRASSLAND,
        WETLAND,
        GARIGUE,
    }
}
