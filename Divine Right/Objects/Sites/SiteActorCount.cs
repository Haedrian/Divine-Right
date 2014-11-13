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
    /// Holds a particular count of a particular actor type for a particular owner
    /// </summary>
    public class SiteActorCount
    {
        public OwningFactions Owner { get; set; }
        public ActorProfession Profession { get; set; }
        public int? BaseAmount { get; set; }
        public int? MaxAmount { get; set; }
    }
}
