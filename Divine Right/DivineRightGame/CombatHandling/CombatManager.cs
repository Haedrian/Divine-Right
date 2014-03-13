using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.ActorHandling;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Graphics;
using Microsoft.Xna.Framework;

namespace DivineRightGame.CombatHandling
{
    /// <summary>
    /// Contains functions pertaining to combat
    /// </summary>
    public static class CombatManager
    {
        private static readonly Dictionary<AttackLocation, int> penaltyDict;
        private static Random random;

        static CombatManager()
        {
            //Create the penalty dict
            penaltyDict = new Dictionary<AttackLocation, int>();
            penaltyDict.Add(AttackLocation.CHEST,0);
            penaltyDict.Add(AttackLocation.HEAD,-5);
            penaltyDict.Add(AttackLocation.LEFT_ARM,+1);
            penaltyDict.Add(AttackLocation.LEGS, -3);
            penaltyDict.Add(AttackLocation.RIGHT_ARM, -3);
        }

        /// <summary>
        /// Calculates the chance of hitting a particular body part.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static int CalculateHitPercentage(Actor attacker, Actor defender, AttackLocation location)
        {
            int atk = 0;
            int def = 0;

            GetStanceEffect(out atk,out def,attacker.CombatStance);

            //Chance to hit -
            // Attacker Skill + Brawn - 5 + location penalty + stance effect  VS Defender Skill + Agil + stance effect
            int hitChance = attacker.Attributes.HandToHand + attacker.TotalBrawn - 5 + penaltyDict[location] + atk;

            GetStanceEffect(out atk, out def, defender.CombatStance);

            int defendChance = defender.Attributes.Dodge + def;

            //See what the difference is
            int difference = 10 + (hitChance - defendChance);

            return difference > 10 ? 100 : difference < 0 ? 0 : difference*10;
        }

        /// <summary>
        /// Gets a random attack location to attack. Will prefer attack locations which have more chance of hitting, and will never return any locations which have a 0 chance to hit.
        /// If this is not possible, return null
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <returns></returns>
        public static AttackLocation? GetRandomAttackLocation(Actor attacker, Actor defender)
        {
            List<AttackLocation> attackLocations = new List<AttackLocation>();

            foreach(AttackLocation loc in  Enum.GetValues(typeof(AttackLocation)).Cast<AttackLocation>())
            {
                for (int i = 0; i < CalculateHitPercentage(attacker, defender, loc); i++ )
                {
                    attackLocations.Add(loc);
                }
            }

            //Now pick one at random
            return attackLocations.OrderBy(o => random.Next(100)).FirstOrDefault();
        }

        /// <summary>
        /// Performs an attack on a character. Will also give an amount of feedback. Also reduces the health of the defender if the attack hits
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="defender"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        public static CurrentLogFeedback[] Attack(Actor attacker, Actor defender, AttackLocation location)
        {
            List<CurrentLogFeedback> feedback = new List<CurrentLogFeedback>();

            //Do we succeed in the attack?
            int atk = 0;
            int def = 0;

            int weaponDamage = 3;
            int weaponWoundPotential = 7;

            GetStanceEffect(out atk, out def, attacker.CombatStance);

            //Chance to hit -
            // Attacker Skill + Brawn - 5 + location penalty + stance effect  VS Defender Skill + Agil + stance effect
            int hitChance = attacker.Attributes.HandToHand + attacker.TotalBrawn - 5 + penaltyDict[location] + atk;

            GetStanceEffect(out atk, out def, defender.CombatStance);

            int defendChance = defender.Attributes.Dodge + def;

            //See what the difference is
            int difference = hitChance - defendChance;

            //Now roll a d10 and see whether we hit
            int diceRoll = random.Next(10) + 1;

            //TODO: CRITICAL HANDLING

            if (difference + diceRoll > 10)
            {
                //We have a hit
                if (attacker.IsPlayerCharacter)
                {
                    feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.SWORD,Color.DarkGreen,"You swing (" + diceRoll + ") at " + defender.EnemyData.EnemyName + " and hit him in the " + location.ToString().ToLower().Replace("_", " ")));
                }
                else 
                {
                    feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.SWORD,Color.DarkRed,defender.EnemyData.EnemyName + " swings (" + diceRoll + ") at you and hits you in the " + location.ToString().ToLower().Replace("_", " ")));
                }

                //TODO: HP AND CONCIOUSNESS

                //Do we wound the character?
                diceRoll = random.Next(10) + 1;

                int woundRoll = diceRoll + defender.Attributes.WoundResist - weaponWoundPotential;

                //TODO: ACTUAL WEAPON DAMAGE
                int damage = weaponDamage;

                if (woundRoll > 0)
                {
                    //No damage!
                    if (attacker.IsPlayerCharacter)
                    {
                        feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.BLOOD,Color.DarkRed,"You however fail to wound your opponent"));
                    }
                    else
                    {
                        feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.BLOOD,Color.DarkGreen,"It fails to properly wound you, however"));
                    }
                }
                else if (woundRoll < 0)
                {
                    //Full damage
                    damage = weaponDamage;
                }
                else
                {
                    //Half damage
                    damage = weaponDamage / 2;
                }

                //Apply the damage

                //TODO: ARMOUR

                if (damage < 0)
                {
                    if (attacker.IsPlayerCharacter)
                    {
                        feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.DEFENSE,Color.DarkRed,"The attack bounces off your opponent's armour"));
                    }
                    else 
                    {
                        feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.DEFENSE,Color.DarkGreen,"The attack bounces off your armour"));
                    }
                }

                //Apply the damage
                switch(location)
                {
                    case AttackLocation.CHEST:
                        defender.Anatomy.Chest -= damage;
                        break;
                    case AttackLocation.HEAD:
                        defender.Anatomy.Head -= damage;
                        break;
                    case AttackLocation.LEFT_ARM:
                        defender.Anatomy.LeftArm -= damage;
                        break;
                    case AttackLocation.LEGS:
                        defender.Anatomy.Legs -= damage;
                        break;
                    case AttackLocation.RIGHT_ARM:
                        defender.Anatomy.RightArm -= damage;
                        break;
                    default:
                        throw new NotImplementedException(location + " has no code prepared for damage");
                }
                
                //TODO : Is he dead?

            }
            else
            {
                //We have a miss
                if (attacker.IsPlayerCharacter)
                {
                    feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.SWORD,Color.DarkRed,"You swing (" + diceRoll + ") at " + defender.EnemyData.EnemyName + "'s " +  location.ToString().ToLower().Replace("_"," ") +" but miss"));
                }
                else
                {
                    //Player is the defender
                    feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.SWORD,Color.DarkGreen,"The " + defender.EnemyData.EnemyName + " swings (" + diceRoll + ") at your "+ location.ToString().ToLower().Replace("_"," ") +", but misses"));
                }
            }

            //We're done

            return feedback.ToArray();
        }

        /// <summary>
        /// Gets the effect the stance has on attack and defence
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="defence"></param>
        /// <param name="stance"></param>
        private static void GetStanceEffect(out int attack, out int defence, ActorStance stance)
        {
            switch(stance)
            {
                case ActorStance.COMPLETE_DEFENSIVE:
                    attack = -6;
                    defence = 3;
                    break;
                case ActorStance.DEFENSIVE:
                    attack = -2;
                    defence = 1;
                    break;
                case ActorStance.NEUTRAL:
                    attack = 0;
                    defence = 0;
                    break;
                case ActorStance.AGGRESSIVE:
                    attack = 1;
                    defence = -2;
                    break;
                case ActorStance.COMPLETE_AGGRESSIVE:
                    attack = 3;
                    defence = -6;
                    break;
                default:
                    throw new NotImplementedException("No stance effects for " + stance.ToString());
            }

            return;
        }
    }
}
