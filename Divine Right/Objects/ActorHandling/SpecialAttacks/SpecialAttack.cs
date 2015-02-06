using DRObjects.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace DRObjects.ActorHandling.SpecialAttacks
{
    [Serializable]
    public class SpecialAttack
    {
        private static readonly string FILEPATH = "Resources/SpecialAttacks/SpecialAttacks.json";

        public string AttackName { get; set; }

        /// <summary>
        /// The required Level [1..10]
        /// </summary>
        public int Level { get; set; }

        /// <summary>
        /// The total point cost which created this
        /// </summary>
        public int PointCost { get; set; }

        /// <summary>
        /// How long it takes to refresh for the next use
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// How long is left before it's ready for next use
        /// </summary>
        public int TimeOutLeft { get; set; }

        /// <summary>
        /// What this special effect actually does
        /// </summary>
        public List<Effect> Effects { get; set; }
    }
}
