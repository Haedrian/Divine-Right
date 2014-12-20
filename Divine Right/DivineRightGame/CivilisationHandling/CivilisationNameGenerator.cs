using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DRObjects.Extensions;

namespace DivineRightGame.CivilisationHandling
{
    /// <summary>
    /// Generates Civilisation names
    /// </summary>
    public static class CivilisationNameGenerator
    {
        private static List<string> Titles { get; set; }
        private static List<string> Prefixes { get; set; }
        private static List<string> Suffixes { get; set; }

        /// <summary>
        /// Loads the name components from the files
        /// </summary>
        private static void LoadNames()
        {
            //Start with titles
            using (TextReader reader = new StreamReader("Resources/CivilisationNames/Title.txt"))
            {
                string allFile = reader.ReadToEnd();

                Titles = allFile.Replace("\r","").Split('\n').Where(a => !String.IsNullOrEmpty(a)).ToList();
            }

            using (TextReader reader = new StreamReader("Resources/CivilisationNames/Prefix.txt"))
            {
                string allFile = reader.ReadToEnd();

                Prefixes = allFile.Replace("\r","").Split('\n').Where(a => !String.IsNullOrEmpty(a)).ToList();
            }

            using (TextReader reader = new StreamReader("Resources/CivilisationNames/Suffix.txt"))
            {
                string allFile = reader.ReadToEnd();

                Suffixes = allFile.Replace("\r", "").Split('\n').Where(a => !String.IsNullOrEmpty(a)).ToList();

                Suffixes.Add(String.Empty); //Add an empty string
            }
        }

        static CivilisationNameGenerator()
        {
            //Load them
            LoadNames();
        }

        /// <summary>
        /// Creates a name for the civilisation
        /// </summary>
        /// <returns></returns>
        public static string GetName()
        {
            Random random = GameState.Random;

            string name = Titles.GetRandom() + " " + Prefixes.GetRandom() + Suffixes.GetRandom();

            return name.Trim();
        }


    }
}
