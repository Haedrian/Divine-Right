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
        private static Random _random = new Random();

        private static Dictionary<Archetype, Dictionary<int, List<string>>> dictionary = new Dictionary<Archetype, Dictionary<int, List<string>>>();

        public static List<string> GetItemProperties(Archetype archetype, int itemID)
        {
            if (!dictionary.Keys.Contains(archetype))
            {
                ReadTableIntoMemory(archetype);
            }
            
           return dictionary[archetype][itemID];
           
        }

        public static Dictionary<int,List<string>> GetDatabase(Archetype archetype)
        {
            //Get's a particular archetype's worth of data
            if (!dictionary.Keys.Contains(archetype))
            {
                ReadTableIntoMemory(archetype);
            }

            return dictionary[archetype];
        }

        /// <summary>
        /// Gets the id of an item randomly belonging to a particular archetype and having the right tag
        /// If unable to find anything, will throw an exception
        /// </summary>
        /// <param name="archetype">The archetype to search in</param>
        /// <param name="tag">The tag to look for</param>
        /// <returns></returns>
        public static int GetItemIdFromTag(Archetype archetype, string tag)
        {
            //Get all the data belonging to the archetype

            if (!dictionary.Keys.Contains(archetype))
            {
                ReadTableIntoMemory(archetype);
            }

            //Now go through the values of the dictionary and pick out those who have that tag
            //Tags will be the last one
            int[] items = null;

            if (archetype == Archetype.MUNDANEITEMS)
            {
                //Multiply by the amount of graphics they have. If they have more than one graphic, they must appear multiple times
                items = dictionary[archetype].Values.Where(v => v[v.Count - 1].ToLower().Split(',').Contains(tag.ToLower())).SelectMany(v => Enumerable.Repeat(Int32.Parse(v[0]),v[3].Split(',').Length > 0 ? v[3].Split(',').Length : 1)).ToArray();
            }
            else if (archetype == Archetype.ENEMIES)
            {
                items = dictionary[archetype].Values.Where(v => v[5].ToLower().Split(',').Contains(tag.ToLower())).Select(v => Int32.Parse(v[0])).ToArray();
            }
            else if (archetype == Archetype.INVENTORYITEMS)
            {
                items = dictionary[archetype].Values.Where(v => v[9].ToLower().Split(',').Contains(tag.ToLower())).Select(v => Int32.Parse(v[0])).ToArray();
            }
            else if (archetype == Archetype.ANIMALS)
            {
                items = dictionary[archetype].Values.Where(v => v[2].ToLower().Split(',').Contains(tag.ToLower())).Select(v => Int32.Parse(v[0])).ToArray();
            }
            else
            {
                items = dictionary[archetype].Values.Where(v => v[v.Count - 1].ToLower().Split(',').Contains(tag.ToLower())).Select(v => Int32.Parse(v[0])).ToArray();
            }

            //do we have at least one?
            if (items.Length == 0)
            {
                throw new Exception("No item with the selected tag was found");
            }

            //do we have one?
            if (items.Length == 1)
            {
                return items[0];
            }

            //otherwise pick one at random to return
            int id = _random.Next(items.Length);

            return items[id];
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
        /// </summary>s
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
