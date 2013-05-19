using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// A maplet contents which contains an item
    /// </summary>
    public class MapletContentsItem:MapletContents
    {
        /// <summary>
        /// The category of the item
        /// </summary>
        public string ItemCategory { get; set; }
        /// <summary>
        /// The ID of the item
        /// </summary>
        public int ItemID { get; set; }
    }
}
