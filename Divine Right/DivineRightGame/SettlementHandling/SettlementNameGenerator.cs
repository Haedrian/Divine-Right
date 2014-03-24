using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DivineRightGame.SettlementHandling
{
    
    /// <summary>
    /// Helper Class for Generating the names of settlements
    /// </summary>
    public static class SettlementNameGenerator
    {
        private const string folderPath = "Resources/SettlementNames";
        private static List<string> prefixes;
        private static List<string> suffixes;
        private static Random random;

        static SettlementNameGenerator()
        {
            //Populate the prefixes and suffixes
            using (TextReader reader = new StreamReader(folderPath + Path.DirectorySeparatorChar + "Prefix.txt"))
            {
                //Read them all, split them into components and populate the string
                prefixes = new List<string>();

                prefixes.AddRange(reader.ReadToEnd().Replace("\r","").Split('\n'));
            }

            //And suffixes
            using (TextReader reader = new StreamReader(folderPath + Path.DirectorySeparatorChar + "Suffix.txt"))
            {
                //Read them all, split them into components and populate the string
                suffixes = new List<string>();

                suffixes.AddRange(reader.ReadToEnd().Replace("\r", "").Split('\n'));
            }

            random = new Random();
        }

        /// <summary>
        /// Generates the name of the town
        /// </summary>
        /// <returns></returns>
        public static string GenerateName()
        {
            return prefixes[random.Next(prefixes.Count)] + "" + suffixes[random.Next(suffixes.Count)];   
        }
    }
}
