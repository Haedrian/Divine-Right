using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Database;
using DRObjects.Enums;

namespace DivineRightGame.ActorHandling
{
    /// <summary>
    /// Class for generating actor attributes
    /// </summary>
    public static class ActorGeneration
    {
        private static Random random = new Random();
        private static List<RaceData> database = null;

        /// <summary>
        /// Gets enemy data for a particular enemy with a particular ID
        /// Will also calculate any random effects as required, so this will not be deterministic if you run it more than once.
        /// </summary>
        /// <param name="enemyID"></param>
        /// <returns></returns>
        public static EnemyData GetEnemyData(int enemyID)
        {
            List<string> properties = DatabaseHandling.GetItemProperties(Archetype.ENEMIES, enemyID);

            if (properties == null || properties.Count == 0)
            {
                throw new Exception("Enemy with ID " + enemyID + " not found");
            }

            //Hard coded sadly
            return new EnemyData()
            {
                EnemyID = enemyID,
                EnemyLineOfSight = Int32.Parse(properties[6]),
                EnemyName = properties[1],
                EnemyType = properties[4],
                Intelligent = Boolean.Parse(properties[7]),
                Profession = Boolean.Parse(properties[8]) ? ActorProfession.WARRIOR: ActorProfession.CIVILIAN //Keep it simple for now
            };

        }

        /// <summary>
        /// Picks a random enemy type which is either intelligent or unintelligent
        /// </summary>
        /// <param name="intelligent"></param>
        /// <returns></returns>
        public static string GetEnemyType(bool intelligent)
        {
            var dictionary = DatabaseHandling.GetDatabase(Archetype.ENEMIES);

            //Get all unique enemy types which are either intelligent or unintelligent
            var possibleMatches = dictionary.Values.Where(v => Boolean.Parse(v[7]).Equals(intelligent)).Select(v => v[4]).Distinct();

            //How many are there? pick one at random

            return possibleMatches.ToArray()[random.Next(possibleMatches.Count() - 1)];
        }

        /// <summary>
        /// Creates an Enemy of a paticular type (optional), having a particular tag (optional) and having a particular intelligence (optional).
        /// Will return the enemy ID so that you can get the actual item from the crea
        /// </summary>
        /// <param name="enemyType"></param>
        /// <param name="enemyTag"></param>
        /// <param name="intelligent"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static DRObjects.Actor CreateEnemy(string enemyType, string enemyTag, bool? intelligent,int level, out int enemyID)
        {
            //Get all the data from the database and we'll make our own filtering
            var dictionary = DatabaseHandling.GetDatabase(Archetype.ENEMIES);

            var possibleMatches = dictionary.Values.AsQueryable();

            if (!String.IsNullOrEmpty(enemyType))
            {
                possibleMatches = possibleMatches.Where(v => v[4].Equals(enemyType));
            }

            if (!String.IsNullOrEmpty(enemyTag))
            {
                possibleMatches = possibleMatches.Where(v => v[5].Equals(enemyTag));
            }

            if (intelligent.HasValue)
            {
                possibleMatches = possibleMatches.Where(v => Boolean.Parse(v[7]).Equals(intelligent.Value));
            }

            //Put the possible matches and pick one at random
            if (possibleMatches.Count() == 0)
            {
                enemyID = -1;
                return null; //No match
            }

            //Pick one at random
            var selected = possibleMatches.ToArray()[random.Next(possibleMatches.Count())];

            //Create the details
            enemyID = Int32.Parse(selected[0]);
            EnemyData data = GetEnemyData(enemyID);

            DRObjects.Actor actor = new DRObjects.Actor();
            actor.EnemyData = data;
            actor.IsPlayerCharacter = false;
            actor.LineOfSight = data.EnemyLineOfSight;
            actor.UniqueId = Guid.NewGuid();

            //Give him attributes 
            actor.Attributes = GenerateAttributes(data.EnemyType, data.Profession, level);

            //Set his anatomy too
            actor.Anatomy = GenerateAnatomy(data.EnemyType);

            //link one to another
            actor.Attributes.Health = actor.Anatomy;

            return actor;
        }

        public static ActorAttributes GenerateAttributes(string race, ActorProfession profession, int level)
        {
            //Roll 6 3d3s and put the results in an array
            int[] results = new int[6];

            //We'll throw away the smallest one

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = random.Next(3) + 1 + random.Next(3) + 1 + random.Next(3) + 1; 
            }

            //sort the array by size so top one goes first
            results = results.OrderByDescending(r => r).ToArray();

            //Create a new ActorAttribute
            ActorAttributes att = new ActorAttributes();

            if (profession == ActorProfession.WARRIOR)
            {
                //Prefer Brawn, Agil, Perc, Intel then Dex
                att.Brawn = results[0];
                att.Agil = results[1];
                att.Perc = results[2];
                att.Intel = results[3];
                att.Dex = results[4];
                
                //Combat skills - give him evasion and attack in an amount equal to level
                att.Evasion = level;
                att.HandToHand = level;

            }
            else if (profession == ActorProfession.CIVILIAN)
            {
                //Prefer things randomly
                results = results.OrderByDescending(r => random.Next(10)).ToArray();

                att.Brawn = results[0];
                att.Agil = results[1];
                att.Perc = results[2];
                att.Intel = results[3];
                att.Dex = results[4];

                //No combat skills
            }
            else if (profession == ActorProfession.CRAFTER)
            {
                //Prefer Dex, Intel, Perc, Brawn, Agil
                att.Dex = results[0];
                att.Intel = results[1];
                att.Perc = results[2];
                att.Brawn = results[3];
                att.Agil = results[4];

                //No combat skills
            }
            else if (profession == ActorProfession.HARVESTER)
            {
                //Prefer Brawn, Perc, Dex, Intel, Agil
                att.Brawn = results[0];
                att.Perc = results[1];
                att.Dex = results[2];
                att.Intel = results[3];
                att.Agil = results[4];

                //Give some combat skills, a third of the level
                att.HandToHand = level / 3;
                att.Evasion = level / 3;
            }
            else
            {
                throw new NotImplementedException("No code for profession " + profession);
            }

            //Add the racial bonuses
            RaceData data = ReadRaceData(race);

            att.Agil = att.BaseAgil + data.AgilModifier;
            att.Brawn = att.BaseBrawn + data.BrawnModifier;
            att.Dex = att.BaseDex + data.DexModifier;
            att.Intel = att.BaseIntel + data.IntelModifier;
            att.Perc = att.BasePerc + data.PercModifier;

            //Make sure all attributes are larger than 0
            att.Agil = att.BaseAgil > 0 ? att.BaseAgil : 0;
            att.Brawn = att.BaseBrawn > 0 ? att.BaseBrawn : 0;
            att.Dex = att.BaseDex > 0 ? att.BaseDex : 0;
            att.Intel = att.BaseIntel > 0 ? att.BaseIntel : 0;
            att.Perc = att.BasePerc > 0 ? att.BasePerc : 0;

            //TODO: actual skills


            return att;
        }

        public static HumanoidAnatomy GenerateAnatomy(string race)
        {
            //This is a dummmy for now. Just generate the standard humanoid anatomy
            HumanoidAnatomy anatomy = new HumanoidAnatomy()
            {
                    Head = 4,
                    HeadMax = 4,
                    LeftArm = 6,
                    LeftArmMax = 6,
                    RightArm = 6,
                    RightArmMax = 6,
                    Chest = 12,
                    ChestMax = 12,
                    Legs = 8,
                    LegsMax = 8,

                    BloodLoss = 0, 
                    BloodTotal = HumanoidAnatomy.BLOODTOTAL,
                    BodyTimer = 0, 
                    StunAmount = 0
            };

            return anatomy;

        }

        /// <summary>
        /// Reads the race data for a particular race
        /// </summary>
        /// <param name="race"></param>
        /// <returns></returns>
        private static RaceData ReadRaceData(string race)
        {
            if (database == null)
            {
                database = new List<RaceData>();

                //Get the entirety of the race database
                var raceDB = DatabaseHandling.GetDatabase(Archetype.RACE);

                //Parse it and shove it in the array
                foreach (List<string> dbRace in raceDB.Values)
                {
                    RaceData datum = new RaceData()
                    {
                        RaceName = dbRace[1],
                        IsIntelligent = bool.Parse(dbRace[2]),
                        BrawnModifier = Int32.Parse(dbRace[3]),
                        AgilModifier = Int32.Parse(dbRace[4]),
                        DexModifier = Int32.Parse(dbRace[5]),
                        PercModifier = Int32.Parse(dbRace[6]),
                        IntelModifier = Int32.Parse(dbRace[7])
                    };

                    database.Add(datum);
                }
            }

            return database.Where(d => d.RaceName.Equals(race, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
        }



    }
}
