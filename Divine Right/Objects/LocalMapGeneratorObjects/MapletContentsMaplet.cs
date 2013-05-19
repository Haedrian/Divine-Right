using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// A MapletContent which contains another Maplet.
    /// </summary>
    public class MapletContentsMaplet:
        MapletContents
    {
        /// <summary>
        /// A maplet
        /// </summary>
        public Maplet Maplet { get; set; }
    }
}
