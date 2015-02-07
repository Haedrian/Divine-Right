using DRObjects.ActorHandling.SpecialAttacks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DRObjects.Extensions;

namespace DivineRightGame.CombatHandling
{
    /// <summary>
    /// Class for generating Special Attacks
    /// </summary>
    public static class SpecialAttacksGenerator
    {
        private static readonly string FILEPATH = "Resources/SpecialAttacks/SpecialAttacks.json";

        private static readonly string ANIMALNAME = "Resources/SpecialAttacks/AnimalName.txt";
        private static readonly string DESCRIPTION = "Resources/SpecialAttacks/Description.txt";
        private static readonly string ACTS = "Resources/SpecialAttacks/Acts.txt";

        private static List<string> AnimalNames;
        private static List<string> Descriptions;
        private static List<string> Acts;

        private static SpecialAttackSettings Settings { get; set; }


        /// <summary>
        /// Generates a special attack having a particular level
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public static SpecialAttack GenerateSpecialAttack(int level)
        {
            //Let's see how many points this counts as
            int pointTotal = Settings.PointProgression[level - 1];

            SpecialAttack attack = new SpecialAttack();
            attack.AttackName = GenerateName();
            attack.Level = level;
            attack.PointCost = pointTotal;
            attack.Effects = new List<Effect>();

            Random random = GameState.Random;

            int attempts = 0;

            while(pointTotal > 0 && attempts++ < 100)
            {
                //Spend!
                var randomEffect = Settings.EffectCosts.GetRandom();

                if (randomEffect.PointCost > pointTotal)
                {
                    continue;
                }

                //Do we have it already?
                var current = attack.Effects.FirstOrDefault(e => e.EffectType == randomEffect.Type);

                if (current != null)
                {
                    int index = 0;

                    //Can we progress?
                    for (index = 0; index < randomEffect.Progressions.Length; index++)
                    {
                        if (randomEffect.Progressions[index] == current.EffectValue)
                        {
                            break;
                        }
                    }

                    //Is that the last one?
                    if (index == randomEffect.Progressions.Length -1)
                    {
                        //Yep. Continue
                        continue;
                    }
                    else
                    {
                        //Nope, we can
                        current.EffectValue = randomEffect.Progressions[index + 1];
                        pointTotal -= randomEffect.PointCost;
                    }
                    


                }
                else 
                {
                    //Nope, create a new one
                    Effect effect = new Effect();
                    effect.EffectType = randomEffect.Type;
                    effect.EffectValue = randomEffect.Progressions[0];

                    attack.Effects.Add(effect);

                    pointTotal -= randomEffect.PointCost;
                }


            }

            attack.TimeOutLeft = 0;
            var timeOut = attack.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.TIMEOUT);

            attack.TimeOut = timeOut == null ? 15 : timeOut.EffectValue;

            return attack;

        }

        /// <summary>
        /// Generates a name at random
        /// </summary>
        /// <returns></returns>
        private static string GenerateName()
        {
            return AnimalNames.GetRandom() + " " + Acts.GetRandom(); 
        }

        static SpecialAttacksGenerator()
        {
            //Load the Settings using the power of JSON
            string fileContents = String.Empty;

            using (TextReader reader = new StreamReader(FILEPATH))
            {
                fileContents = reader.ReadToEnd();
            }

            var parsed = JsonConvert.DeserializeObject<SpecialAttackSettings>(fileContents);

            Settings = parsed;

            //Load the animal names and other such things

            using (TextReader reader = new StreamReader(ANIMALNAME))
            {
                string contents = reader.ReadToEnd();

                //clean it up!
                contents = contents.Replace("\r", "");

                AnimalNames = contents.Split('\n').Where(a => !(String.IsNullOrWhiteSpace(a))).ToList();
            }

            using (TextReader reader = new StreamReader(DESCRIPTION))
            {
                string contents = reader.ReadToEnd();

                //clean it up!
                contents = contents.Replace("\r", "");

                Descriptions = contents.Split('\n').Where(a => !(String.IsNullOrWhiteSpace(a))).ToList();
            }

            using (TextReader reader = new StreamReader(ACTS))
            {
                string contents = reader.ReadToEnd();

                //clean it up!
                contents = contents.Replace("\r", "");

                Acts = contents.Split('\n').Where(a => !(String.IsNullOrWhiteSpace(a))).ToList();
            }
        }
    }
}
