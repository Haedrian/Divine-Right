using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Enums;

namespace DRObjects.Sites
{
    [Serializable]
    /// <summary>
    /// Contains some fixed data about the type of the site
    /// </summary>
    public class SiteTypeData
    {
        /// <summary>
        /// The type of the site
        /// </summary>
        public SiteType SiteType { get; set; }

        public List<SiteActorCount> ActorCounts { get; set; }
    }
}
