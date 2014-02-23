using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Database;
using DRObjects.Enums;

namespace DRObjects.ActorHandling
{
    public static class EnemyDataManager
    {
        private static Random random = new Random();

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
                Intelligent = Boolean.Parse(properties[7])
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

            return possibleMatches.ToArray()[random.Next(possibleMatches.Count()-1)];
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
        public static Actor CreateEnemy(string enemyType, string enemyTag, bool? intelligent,out int enemyID)
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
            var selected = possibleMatches.ToArray()[random.Next(possibleMatches.Count() - 1)];

            //Create the details
            enemyID = Int32.Parse(selected[0]);
            EnemyData data = GetEnemyData(enemyID);

            Actor actor = new Actor();
            actor.EnemyData = data;
            actor.IsPlayerCharacter = false;
            actor.LineOfSight = data.EnemyLineOfSight;
            actor.UniqueId = Guid.NewGuid();
            
            return actor;
        }

    }
}
