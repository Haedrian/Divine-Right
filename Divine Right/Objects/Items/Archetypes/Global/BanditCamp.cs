using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    /// <summary>
    /// A Bandit Camp. A small map containing bandits.
    /// </summary>
    public class BanditCamp
    {
        private Guid _uniqueGUID;

        public Guid UniqueGUID { get { return _uniqueGUID; } }

        public int BanditTotal { get; set; }

        public BanditCamp()
        {
            this._uniqueGUID = Guid.NewGuid();
        }

        public MapCoordinate Coordinate { get; set; }
    }
}
