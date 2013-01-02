using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using DivineRightGame.ItemFactory.Object;
using System.IO;

namespace DivineRightGame.ItemFactory
{
    /// <summary>
    /// For reading item factory files
    /// </summary>
    public class FileReader
    {
        /// <summary>
        /// Reads the csv file and returns a multidictionary
        /// The filename should have no extension
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public MultiDictionary Read(string filename)
        {
            //get the file's path
            string path = System.Configuration.ConfigurationSettings.AppSettings["GamePath"].ToString();
            path += "/ItemFiles/" + filename + ".csv";
            string file = "";

            using (TextReader reader = new StreamReader(path))
            {
                file = reader.ReadToEnd();
            }

            string[] fileLines = file.Split('\n');

            MultiDictionary dict = new MultiDictionary();

            foreach (string line in fileLines)
            {
                //chheck if its a comment

                if (line.StartsWith("--"))
                {
                    continue; //its a comment
                }

                //split them in commas

                List<string> data = new List<string>();

                string[] cells = line.Split(',');

                foreach (string cell in cells)
                {
                    data.Add(cell);
                }

                dict.Add(data[0], data); //add the data with the key

            }

            return dict;

        }

    }
}
