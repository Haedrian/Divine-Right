using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling
{
    /// <summary>
    /// Contains data about a particular enemy, such as line of sight, health and stuff like that
    /// </summary>
    public class EnemyData
    {
        public int EnemyID { get; set; }
        public string EnemyName { get; set; }
        public int EnemyLineOfSight { get; set; }
        public string EnemyType { get; set; }
        public bool Intelligent { get; set; }
    }
}
