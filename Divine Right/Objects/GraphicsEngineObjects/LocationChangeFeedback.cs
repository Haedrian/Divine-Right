using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Items.Archetypes.Global;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.Enums;

namespace DRObjects.GraphicsEngineObjects
{
    /// <summary>
    /// Asks for a location change
    /// </summary>
    public class LocationChangeFeedback:
        ActionFeedback
    {
        public Settlement VisitSettlement { get; set; }
        public bool VisitMainMap { get; set; }
        public Dungeon VisitDungeon { get; set; }
        public BanditCamp VisitCamp { get; set; }
        public MapSite VisitSite { get; set; }

        public GlobalBiome? RandomEncounter { get; set; }

        public LocationChangeFeedback()
        {
            VisitSettlement = null;
            VisitMainMap = false;
            VisitDungeon = null;
            VisitCamp = null;
            VisitSite = null;
            RandomEncounter = null;
        }
    }
}
