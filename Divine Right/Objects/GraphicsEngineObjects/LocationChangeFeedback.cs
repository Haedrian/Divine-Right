using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Items.Archetypes.Global;
using DRObjects.GraphicsEngineObjects.Abstract;

namespace DRObjects.GraphicsEngineObjects
{
    /// <summary>
    /// Asks for a location change
    /// </summary>
    public class LocationChangeFeedback:
        PlayerFeedback
    {
        public Settlement VisitSettlement { get; set; }
        public bool VisitMainMap { get; set; }
        public Dungeon VisitDungeon { get; set; }

        public LocationChangeFeedback()
        {
            VisitSettlement = null;
            VisitMainMap = false;
            VisitDungeon = null;
        }

    }
}
