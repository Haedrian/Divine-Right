using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Enums;

namespace DRObjects.LocalMapGeneratorObjects
{
    [Serializable]
    public class PatrolRoute
    {
        public string PatrolName { get; set; }
        public OwningFactions Owners { get; set; }
        public ActorProfession Profession { get; set; }

        /// <summary>
        /// The collection of points that make up the route, as well as the acceptable radius of each
        /// </summary>
        public List<PatrolPoint> Route { get; set; }
    }
}
