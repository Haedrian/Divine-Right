using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.LocalMapGeneratorObjects
{
    /// <summary>
    /// Represent a special typeof item
    /// </summary>
    public class MapletContentsItemSpecial
        : MapletContents
    {
        public string Type { get; set; }
    }
}
