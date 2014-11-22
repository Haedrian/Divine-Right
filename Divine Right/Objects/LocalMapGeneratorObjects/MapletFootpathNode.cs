using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.LocalMapGeneratorObjects
{
    [Serializable]
    public class MapletFootpathNode:
        MapletContents
    {
        /// <summary>
        /// Whether this is the primary node which everyone connects to
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Point set by Generator.
        /// </summary>
        public MapCoordinate Point { get; set; }

    }
}
