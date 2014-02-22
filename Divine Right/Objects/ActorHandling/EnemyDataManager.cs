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
                EnemyName = properties[1]
            };

        }


    }
}
