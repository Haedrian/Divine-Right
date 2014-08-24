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
        //TODO: DUNGEONS

        public LocationChangeFeedback()
        {
            VisitSettlement = null;
            VisitMainMap = false;
        }

    }
}
