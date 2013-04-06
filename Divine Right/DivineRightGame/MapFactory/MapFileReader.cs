using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DivineRightGame.MapFactory
{
    /// <summary>
    /// Helper functions for reading a map file
    /// </summary>
    public class MapFileReader
    {
        public string[] ReadFileFromPath(string filePath)
        {
            using (TextReader reader = new StreamReader(filePath))
            {
                string entireFile = reader.ReadToEnd().Replace("\r","");

                //split it by newlines, then filter out what we need exactly and what we don't

                List<string> fileContents = new List<string>();

                foreach (string s in entireFile.Split('\n'))
                {
                    if (string.IsNullOrEmpty(s))
                    {
                        continue;
                    }
                    else if (s.StartsWith("--") )
                    {
                        //comment
                        continue;
                    }
                    else
                    {
                        fileContents.Add(s);
                    }

                }

                return fileContents.ToArray();
            }

        }

        /// <summary>
        /// Reads a particular file from the map folder
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string[] ReadFile(string fileName)
        {
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"/My Games/DivineRight"; 
            path += "/MapFiles/" + fileName + ".csv";

            return ReadFileFromPath(path);

        }

    }
}
