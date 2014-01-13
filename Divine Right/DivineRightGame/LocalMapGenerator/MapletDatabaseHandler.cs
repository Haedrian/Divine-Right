using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Data.SqlServerCe;
using DRObjects.LocalMapGeneratorObjects.DatabaseHandling;

namespace DivineRightGame.LocalMapGenerator
{
    /// <summary>
    /// Handles data from the Maplet Database
    /// </summary>
    public static class MapletDatabaseHandler
    {
        /// <summary>
        /// Holds a  path file-data pair
        /// </summary>
        private static Dictionary<string, XElement> maplets = new Dictionary<string, XElement>();
        /// <summary>
        /// Holds an id details pair
        /// </summary>
        private static Dictionary<int, MapletDatabaseDatum> databaseData;

        private static Random _random = new Random(DateTime.Now.Millisecond);

        public static XElement GetMapletByID(int id)
        {
            if (databaseData == null)
            {
                ReadData();
            }

            //Load the maplet data from the dictionary
            MapletDatabaseDatum datum = databaseData[id];

            //Now load the document and return it
            return LoadFile(datum.MapletPath);
        }

        public static XElement GetMapletByTag(string tag)
        {
            if (databaseData == null)
            {
                ReadData();
            }

            //Search for it
            var data = databaseData.Values.Where(v => v.Tags.Split(',').Contains(tag));

            return LoadFile(data.Select(d => d.MapletPath).ToArray()[_random.Next(data.Count())]);
        }

        /// <summary>
        /// Reads the data into the dictionary
        /// </summary>
        private static void ReadData()
        {
            databaseData = new Dictionary<int, MapletDatabaseDatum >();

            //Lazily load the entire table
            using (SqlCeConnection conn = new SqlCeConnection("Data Source=maplets.sdf;Max Database Size=256;Persist Security Info=False;"))
            {
                conn.Open();

                SqlCeCommand com = new SqlCeCommand("SELECT * from Maplet",conn);

                SqlCeDataReader reader = com.ExecuteReader();

                while (reader.Read())
                {
                    MapletDatabaseDatum datum = new MapletDatabaseDatum();
                    datum.MapletID = (int) reader[0];
                    datum.MapletName = reader[1].ToString();
                    datum.MapletPath = reader[2].ToString();
                    datum.Tags = reader[3].ToString();

                    databaseData.Add(datum.MapletID,datum);
                }
            }
        }

        /// <summary>
        /// Loads a file in the particular path from cache if available, or from the disk if it is not
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static XElement LoadFile(string path)
        {
            if (maplets.Keys.Contains(path))
            {
                //serve it
                return maplets[path];
            }
            else
            {
                //Load it
                XElement element = (XElement) XDocument.Load(path).FirstNode;

                //add it to the cache
                maplets.Add(path,element);

                //and return it
                return element;
            }
        }
    }
}
