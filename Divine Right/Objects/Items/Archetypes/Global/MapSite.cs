﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    public class MapSite
    {
        private Guid _uniqueGUID;

        public Guid UniqueGUID { get { return _uniqueGUID; } }

        //public int GuardTotal { get; set; }

        public MapSite()
        {
            this._uniqueGUID = Guid.NewGuid();
        }

        public MapCoordinate Coordinate { get; set; }

        public SiteType SiteType { get; set; }
        public OwningFactions Owners { get; set; }
    }
}