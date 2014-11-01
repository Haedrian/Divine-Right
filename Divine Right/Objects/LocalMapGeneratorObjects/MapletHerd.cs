using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// Represents a herd of animals in a maplet
    /// </summary>
    public class MapletHerd : MapletContents
    {
        /// <summary>
        /// The string representation of Biome Name
        /// </summary>
        public string BiomeName { get; set; }
        /// <summary>
        /// The Global Biome to generate the herd from
        /// </summary>
        public GlobalBiome? Biome
        {
            get
            {
                if (string.IsNullOrEmpty(this.BiomeName))
                {
                    return null;
                }
                else
                {
                    return (GlobalBiome)Enum.Parse(typeof(GlobalBiome), this.BiomeName);
                }
            }
        }
        /// <summary>
        /// Whether the herd is domesticated, undomesticated or either.
        /// </summary>
        public bool? Domesticated { get; set; }
    }
}
