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

        /// <summary>
        /// Whether the child maplet set to edge will be placed in the first place it fits or not
        /// </summary>
        public bool FirstFit { get; set; }

        public MapletContentsMaplet()
        {
            //default false
            FirstFit = false;
        }
    }
}
