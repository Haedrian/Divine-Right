using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    /// <summary>
    /// A location that the player can visit
    /// </summary>
    public class Location
    {
        private Guid _uniqueGUID;
        public Guid UniqueGUID { get { return _uniqueGUID; } }

        public MapCoordinate Coordinate { get; set; }

        public Location()
        {
            _uniqueGUID = Guid.NewGuid();
        }
    }
}
