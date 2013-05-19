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
            int[] items = dictionary[archetype].Values.Where(v => v[v.Count - 1].ToLower().Contains("," + tag + ",")).Select(v => Int32.Parse(v[0])).ToArray();

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
            Random random = new Random();

            return items[random.Next(items.Length)];
        }

        /// <summary>
        /// Gets a number of item at random which share the correct tag.
        /// The first value of the list will be the archetype of the item
        /// If unable to find anything, will throw an exception
        /// </summary>
        /// <param name="tag">The tag to look for</param>
        /// <param name="archetype">The Archetype to search in</param>
        /// <param name="maxAmount">The maximum amount of items to return</param>
        /// <param name="probability">The probability out of 100% that one will be chosen</param>
        /// <returns></returns>
        public static List<string>[] GetItemsFromTag(Archetype archetype,string tag,double probability, int maxAmount=1)
        {
           //Get all the data belonging to the archtyle

            if (!dictionary.Keys.Contains(archetype))
            {
                ReadTableIntoMemory(archetype);
            }

            //now go through the values of the dictionary and pick out those who have that tag

            //The tags will be the last one
            List<string>[] items = dictionary[archetype].Values.Where(v => v[v.Count - 1].ToLower().Contains("," + tag + ",")).ToArray();

            //do we have at least one?
            if (items.Length == 0)
            {
                //this is an exception
                throw new Exception("No item with the selected tag was found");
            }

            List<List<string>> returnList = new List<List<string>>();

            Random random = new Random();

            for (int i = 0; i < maxAmount; i++)
            {
                if (random.NextDouble() * 100 <= probability)
                {
                    //Select one to add to the list
                    if (items.Length == 1)
                    {
                        returnList.Add(items[0]);
                    }
                    //otherwise we pick one at random
                    returnList.Add(items[random.Next(items.Length)]);
                }
            }

            return returnList.ToArray();
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
