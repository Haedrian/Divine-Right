using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// A maplet contents which contains a tag of items
    /// </summary>
    public class MapletContentsItemTag:MapletContents
    {
        /// <summary>
        /// The category within which to search for the tag for
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// A tag of the item to be created
        /// </summary>
        public string Tag { get; set; }       
    }
}
