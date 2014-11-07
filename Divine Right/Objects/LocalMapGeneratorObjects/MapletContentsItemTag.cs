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

        /// <summary>
        /// A comma seperated representation of the factions which own this mcit
        /// </summary>
        public string OwnerFactions { get; set; }

        /// <summary>
        /// The factions which own this object
        /// </summary>
        public OwningFactions? Factions
        {
            get
            {
                if (string.IsNullOrEmpty(OwnerFactions))
                {
                    return null;
                }

                OwningFactions? fact = null;
                //Parse it
                foreach(var own in OwnerFactions.Split(','))
                {
                    OwningFactions ownParsed = (OwningFactions) Enum.Parse(typeof(OwningFactions), own.ToUpper());

                    if (fact == null)
                    {
                        fact = ownParsed;
                    }
                    else
                    {
                        fact = fact | ownParsed;
                    }
                }

                return fact;
            }
        }
       
    }
}
