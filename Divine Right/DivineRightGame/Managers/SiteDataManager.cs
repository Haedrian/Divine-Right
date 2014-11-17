using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DivineRightGame.Managers.HelperObjects;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Enums;
using DRObjects.Sites;
using Newtonsoft.Json;

namespace DivineRightGame.Managers
{
    /// <summary>
    /// Helper class for obtaining site data
    /// </summary>
    public static class SiteDataManager
    {
        private static List<SiteTypeData> siteData = null;
        private static readonly string FILEPATH = "Resources/SiteData/SiteData.json";

        /// <summary>
        /// Reads data from the file, parses it and populates the sitedata structure
        /// </summary>
        private static void ReadData()
        {
            string fileContents = String.Empty;

            using ( TextReader reader = new StreamReader(FILEPATH))
            {
                fileContents = reader.ReadToEnd();
            }

            List<SiteTypeData> sites = new List<SiteTypeData>();

            var parsed = JsonConvert.DeserializeObject<SiteFileData>(fileContents);

            foreach (var site in parsed.Sites)
            {
                SiteTypeData data = new SiteTypeData();
                sites.Add(data);

                data.SiteType = (SiteType) Enum.Parse(typeof(SiteType),site.Name,true);
                data.ActorCounts = new List<SiteActorCount>();

                foreach(var actorCount in site.ActorCounts)
                {
                    SiteActorCount count = new SiteActorCount();
                    count.BaseAmount = actorCount.BaseAmount;
                    count.MaxAmount = actorCount.MaxAmount;

                    OwningFactions temp = OwningFactions.ABANDONED;

                    if ( Enum.TryParse<OwningFactions>(actorCount.Owner,true, out temp))
                    {
                        count.Owner = temp;
                    }

                    count.Profession = (ActorProfession)Enum.Parse(typeof(ActorProfession), actorCount.Profession);

                    data.ActorCounts.Add(count);
                }

            }

            siteData = sites;
        }

        /// <summary>
        /// Gets the data for a particular site type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SiteTypeData GetData(SiteType type)
        {
            if (siteData == null)
            {
                ReadData();
            }

            return siteData.Where(sd => sd.SiteType.Equals(type)).First();
        }

    }
}
