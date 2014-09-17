using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.Enums;
using System.IO;
using System.Globalization;

namespace DRObjects.ActorHandling
{
    /// <summary>
    /// Generates the name of an Actor
    /// </summary>
    public class ActorNameGenerator
    {
        private const string folderPath = "Resources";

        private static readonly List<string> maleHuman = new List<string>();
        private static readonly List<string> femaleHuman = new List<string>();
        private static readonly List<string> humanSurname = new List<string>();

        private static readonly List<string> orcName = new List<string>();
        private static readonly List<string> orcSurnameFore = new List<string>();
        private static readonly List<string> orcSurnameAft = new List<string>();

        private static Random random;

        static ActorNameGenerator()
        {
            TextInfo myTI = new CultureInfo("en-gb", false).TextInfo;

            //Load everything
            using (TextReader reader = new StreamReader(folderPath + Path.DirectorySeparatorChar + "HumanNames" + Path.DirectorySeparatorChar + "FemaleNames.txt"))
            {
                femaleHuman.AddRange(reader.ReadToEnd().ToLower().Replace("\r", "").Split('\n').Where(r => !String.IsNullOrEmpty(r)));
            }

            using (TextReader reader = new StreamReader(folderPath + Path.DirectorySeparatorChar + "HumanNames" + Path.DirectorySeparatorChar + "MaleNames.txt"))
            {
                maleHuman.AddRange(reader.ReadToEnd().ToLower().Replace("\r", "").Split('\n').Where(r => !String.IsNullOrEmpty(r)));
            }

            using (TextReader reader = new StreamReader(folderPath + Path.DirectorySeparatorChar + "HumanNames" + Path.DirectorySeparatorChar + "Surnames.txt"))
            {
                humanSurname.AddRange(reader.ReadToEnd().ToLower().Replace("\r", "").Split('\n').Where(r => !String.IsNullOrEmpty(r)));
            }

            using (TextReader reader = new StreamReader(folderPath + Path.DirectorySeparatorChar + "OrcNames" + Path.DirectorySeparatorChar + "FirstName.txt"))
            {
                orcName.AddRange(reader.ReadToEnd().Replace("\r", "").Split('\n').Where(r => !String.IsNullOrEmpty(r)));
            }

            using (TextReader reader = new StreamReader(folderPath + Path.DirectorySeparatorChar + "OrcNames" + Path.DirectorySeparatorChar + "SurnameAft.txt"))
            {
                orcSurnameAft.AddRange(reader.ReadToEnd().Replace("\r", "").Split('\n').Where(r => !String.IsNullOrEmpty(r)));
            }

            using (TextReader reader = new StreamReader(folderPath + Path.DirectorySeparatorChar + "OrcNames" + Path.DirectorySeparatorChar + "SurnameFore.txt"))
            {
                orcSurnameFore.AddRange(reader.ReadToEnd().Replace("\r", "").Split('\n').Where(r => !String.IsNullOrEmpty(r)));
            }

            //Title case all of them
            for (int i = 0; i < femaleHuman.Count; i++)
            {
                femaleHuman[i] = myTI.ToTitleCase(femaleHuman[i]);
            }

            for (int i = 0; i < maleHuman.Count; i++)
            {
                maleHuman[i] = myTI.ToTitleCase(maleHuman[i]);
            }

            for (int i = 0; i < humanSurname.Count; i++)
            {
                humanSurname[i] = myTI.ToTitleCase(humanSurname[i]);
            }

            random = new Random();
        }

        private static string GenerateHumanName(Gender gender)
        {
            string name = String.Empty;

            if (gender == Gender.M)
            {
                name += maleHuman[random.Next(maleHuman.Count)];
            }
            else
            {
                name += femaleHuman[random.Next(femaleHuman.Count)];
            }

            name += " " + humanSurname[random.Next(humanSurname.Count)];

            return name;

        }

        private static string GenerateOrcName(Gender gender)
        {
            return orcName[random.Next(orcName.Count)] + " " + orcSurnameFore[random.Next(orcSurnameFore.Count)] + orcSurnameAft[random.Next(orcSurnameAft.Count)];
        }

        public static string GenerateName(string race, Gender gender)
        {
            switch (race.ToLower())
            {
                case "human":
                    return GenerateHumanName(gender);
                case "orc":
                    return GenerateOrcName(gender);
                default:
                    return String.Empty;
            }
        }

       
    }
}
