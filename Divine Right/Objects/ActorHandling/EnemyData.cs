﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet.Enums;

namespace DRObjects.ActorHandling
{
    [Serializable]
    /// <summary>
    /// Contains data about a particular enemy, such as line of sight
    /// </summary>
    public class EnemyData
    {
        public int EnemyID { get; set; }
        public string EnemyName { get; set; }
        public int EnemyLineOfSight { get; set; }
        public string EnemyType { get; set; }
        public bool Intelligent { get; set; }
        public ActorProfession Profession { get; set; }

        
    }
}
