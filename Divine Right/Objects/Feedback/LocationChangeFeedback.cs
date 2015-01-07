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
        public bool VisitMainMap { get; set; }
        public Location Location { get; set; }

        public GlobalBiome? RandomEncounter { get; set; }

        public LocationChangeFeedback()
        {
            Location = null;
            VisitMainMap = false;
            RandomEncounter = null;
        }
    }
}
