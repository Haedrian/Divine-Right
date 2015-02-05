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

        public string Description { get; set; }

        public SpecialAttackType SpecialAttackType { get; set; }

        public InterfaceSpriteName SpriteName { get; set; }

        /// <summary>
        /// The required Fighter Skill Level
        /// </summary>
        public int RequiredSkillLevel { get; set; }

        /// <summary>
        /// How long it takes to refresh for the next use
        /// </summary>
        public int TimeOut { get; set; }

        /// <summary>
        /// How long is left before it's ready for next use
        /// </summary>
        public int TimeOutLeft { get; set; }

        public List<Effectivness> Effectivness { get; set; }
    
        /// <summary>
        /// Get;s the standard list of special attacks
        /// </summary>
        /// <returns></returns>
        public static List<SpecialAttack> GetStandardSpecialAttacks()
        {
           //Get it from the JSON
            string fileContents = String.Empty;

            using (TextReader reader = new StreamReader(FILEPATH))
            {
                fileContents = reader.ReadToEnd();
            }

            var parsed = JsonConvert.DeserializeObject<SpecialAttack[]>(fileContents);

            return parsed.ToList();
        }

        public SpecialAttack() { }
    
        /// <summary>
        /// Produces a deep clone of the passed special attack
        /// </summary>
        /// <param name="attack"></param>
        public SpecialAttack Clone()
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, this);
                ms.Position = 0;

                return (SpecialAttack)formatter.Deserialize(ms);
            }
        }

    }
}
