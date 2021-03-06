﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.CivilisationHandling;
using DRObjects.Enums;
using DRObjects.LocalMapGeneratorObjects;

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
        /// The civilisation that owns this item
        /// </summary>
        public Civilisation Civilisation { get; set; }


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

        /// <summary>
        /// Set as a flag when the owner changes. We will need to update things.
        /// </summary>
        public bool OwnerChanged { get; set; }

        /// <summary>
        /// Set as a flag when the map requires generation 'updates' (aka, changing the visibility of items or adding/removing characters)
        /// </summary>
        public bool MapRegenerationRequired { get; set; }


        /// <summary>
        /// Wander areas, used for regeneration
        /// </summary>
        public List<MapletActorWanderArea> WanderAreas { get; set; }

        /// <summary>
        /// Patrol routes, used for regeneration
        /// </summary>
        public List<PatrolRoute> PatrolRoutes { get; set; }

        public SiteData()
        {
            OwnerChanged = false;
        }

        /// <summary>
        /// Incremements the actor counts for this site up to the maximum amount for this type
        /// </summary>
        public void IncrementActorCounts()
        {
            List<SiteActorCount> counts = new List<SiteActorCount>();

            foreach (var profession in Enum.GetValues(typeof(ActorProfession)))
            {
                var prof = (ActorProfession)profession;

                //Try to find exact match

                var exactMatch = this.SiteTypeData.ActorCounts.Where(ac => ac.Owner.Equals(this.Owners) && ac.Profession.Equals(prof)).FirstOrDefault();

                if (exactMatch == null)
                {
                    //try to find the default
                    var almostMatch = this.SiteTypeData.ActorCounts.Where(ac => ac.Owner.HasFlag(this.Owners) && ac.Profession.Equals(prof)).FirstOrDefault();

                    if (almostMatch == null)
                    {
                        //nothing
                    }
                    else
                    {
                        counts.Add(almostMatch);
                    }
                }
                else
                {
                    counts.Add(exactMatch);
                }

            }

            foreach (var count in counts)
            {
                if (this.ActorCounts[count.Profession] < count.MaxAmount)
                {
                    this.ActorCounts[count.Profession]++;
                }
            }

            //We need to remake it
            this.MapRegenerationRequired = true;
        }

        /// <summary>
        /// Loads the actor counts from the SiteTypeData.
        /// Will first try to get an exact match. If it does not find one, it will get the default.
        /// Will overwrite the actor counts
        /// </summary>
        public void LoadAppropriateActorCounts()
        {
            List<SiteActorCount> counts = new List<SiteActorCount>();

            foreach(var profession in Enum.GetValues(typeof(ActorProfession)))
            {
                var prof = (ActorProfession)profession;

                //Try to find exact match

                var exactMatch = this.SiteTypeData.ActorCounts.Where(ac => ac.Owner.Equals(this.Owners) && ac.Profession.Equals(prof)).FirstOrDefault();

                if (exactMatch == null)
                {
                    //try to find the default
                    var almostMatch = this.SiteTypeData.ActorCounts.Where(ac => ac.Owner.HasFlag(this.Owners) && ac.Profession.Equals(prof)).FirstOrDefault();

                    if (almostMatch == null)
                    {
                        //nothing
                    }
                    else
                    {
                        counts.Add(almostMatch);
                    }
                }
                else
                {
                    counts.Add(exactMatch);
                }

            }

            this.ActorCounts = new Dictionary<ActorProfession, int>();

            foreach(var count in counts)
            {
                this.ActorCounts.Add(count.Profession, count.BaseAmount.Value);
            }

        }
    }
}
