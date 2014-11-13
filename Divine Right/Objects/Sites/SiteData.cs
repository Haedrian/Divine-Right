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
    /// Holds some data about a particular Site
    /// </summary>
    public class SiteData
    {
        /// <summary>
        /// Who owns the site
        /// </summary>
        public OwningFactions Owners { get; set; }

        /// <summary>
        /// The biome this site stands upon
        /// </summary>
        public GlobalBiome Biome { get; set; }

        /// <summary>
        /// The count of Actors within that site
        /// </summary>
        public Dictionary<ActorProfession,int> ActorCounts { get; set; }

        /// <summary>
        /// The fixed data for this particular site
        /// </summary>
        public SiteTypeData SiteTypeData { get; set; }

        public bool OwnerChanged { get; set; }

        public SiteData()
        {
            OwnerChanged = false;
        }
    }
}
