using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using System.Data.SqlServerCe;
using System.Data;

namespace DRObjects.Database
{
    public static class DatabaseHandling
    {

        private static Dictionary<Archetype, Dictionary<int, List<string>>> dictionary = new Dictionary<Archetype, Dictionary<int, List<string>>>();

        public static List<string> GetItemProperties(Archetype archetype, int itemID)
        {
            if (!dictionary.Keys.Contains(archetype))
            {
                ReadTableIntoMemory(archetype);
            }
            
           return dictionary[archetype][itemID];
           
        }

        /// <summary>
        /// Dumps the stored tables
        /// </summary>
        public static void EmptyMemory()
        {
            dictionary = new Dictionary<Archetype, Dictionary<int, List<string>>>();
        }

        /// <summary>
        /// Read an entire table into memory
        /// </summary>
        /// <param name="archetype"></param>
        /// <param name="itemID"></param>
        public static void ReadTableIntoMemory(Archetype archetype)
        {
            //Lazily load the entire table
            using (SqlCeConnection conn = new SqlCeConnection("Data Source=items.sdf;Max Database Size=256;Persist Security Info=False;"))
            {
                conn.Open();
                string tableName = archetype.ToString().ToLower();
                string firstChar = tableName.Substring(0, 1);
                tableName = firstChar.ToUpper() + tableName.Substring(1);

                SqlCeCommand com = new SqlCeCommand("SELECT * from " + tableName, conn);

                SqlCeDataReader reader = com.ExecuteReader();
                Dictionary<int, List<string>> values = new Dictionary<int, List<string>>();

                while (reader.Read())
                {
                    List<string> data = new List<string>();
                    for (int i = 0; i < reader.VisibleFieldCount; i++)
                    {
                        data.Add(reader.GetValue(i).ToString());
                    }

                    //the first one will be an int, with the id
                    values.Add(reader.GetInt32(0), data);
                }

                dictionary.Add(archetype, values);
            }

        }

    }
}
