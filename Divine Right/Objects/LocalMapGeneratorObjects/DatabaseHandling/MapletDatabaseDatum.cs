using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.LocalMapGeneratorObjects.DatabaseHandling
{
    /// <summary>
    /// Holds the data as stored in the database
    /// </summary>
    public class MapletDatabaseDatum
    {
        public int MapletID { get; set; }
        public string MapletName { get; set; }
        public string MapletPath { get; set; }
        public string Tags { get; set; }

        public override string ToString()
        {
            return MapletName + "(" + MapletID + ")";
        }
    }
}
