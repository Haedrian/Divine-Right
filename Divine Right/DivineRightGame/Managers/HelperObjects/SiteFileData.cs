using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightGame.Managers.HelperObjects
{
    /// <summary>
    /// The type of class the site data files will parse into
    /// </summary>
    public class SiteFileData
    {
        public List<Site> Sites{get;set;}
    }

    public class Site
    {
        public string Name { get; set; }
        public List<ActorCount> ActorCounts { get; set; }
    }

    public class ActorCount
    {
        public string Profession { get; set; }
        public string Owner { get; set; }
        public int BaseAmount { get; set; }
        public int MaxAmount { get; set; }
    }
}
