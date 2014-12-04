using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Sites;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    public class MapSite
    {
        private Guid _uniqueGUID;

        public Guid UniqueGUID { get { return _uniqueGUID; } }

        public SiteData SiteData { get; set; }

        public MapSite()
        {
            this._uniqueGUID = Guid.NewGuid();
        }

        public MapCoordinate Coordinate { get; set; }

  
    }
}
