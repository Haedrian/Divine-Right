using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.ActorHandling;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Graphics;
using Microsoft.Xna.Framework;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.Enums;

namespace DivineRightGame.CombatHandling
{
    /// <summary>
    /// Contains functions pertaining to combat
    /// </summary>
    public static class CombatManager
    {
        private static readonly Dictionary<AttackLocation, int> penaltyDict;
        private static Random random = new Random();

        static CombatManager()
        {
            //Create the penalty dict
            penaltyDict = new Dictionary<AttackLocation, int>();
            penaltyDict.Add(AttackLocation.CHEST, 0);
            penaltyDict.Add(AttackLocation.HEAD, -5);
            penaltyDict.Add(AttackLocation.LEFT_ARM, +1);
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
            //If the bodypart is destroyed, give a percentage of -1 so we can filter it out later
            switch (location)
            {
                case AttackLocation.CHEST:
                    if (defender.Anatomy.Chest <= -5)
                    {
                        return -1;
                    }
                    break;
                case AttackLocation.HEAD:
                    if (defender.Anatomy.Head <= -5)
                    {
                        return -1;
                    }
                    break;
                case AttackLocation.LEFT_ARM:
                    if (defender.Anatomy.LeftArm <= -5)
                    {
                        return - 1;
                    }
                    break;
                case AttackLocation.LEGS:
                    if (defender.Anatomy.Legs <= -5)
                    {
                        return -1;
                    }
                    break;
                case AttackLocation.RIGHT_ARM:
                    if (defender.Anatomy.RightArm <= -5)
                    {
                        return -1;
                    }
                    break;
            }

            int atk = 0;
            int def = 0;

            GetStanceEffect(out atk, out def, attacker.CombatStance);

            //Chance to hit -
            // Attacker Skill + Brawn - 5 + location penalty + stance effect  VS Defender Skill + Agil + stance effect
            int hitChance = attacker.Attributes.HandToHand + attacker.TotalBrawn - 5 + penaltyDict[location] + atk;

            GetStanceEffect(out atk, out def, defender.CombatStance);

            int defendChance = defender.Attributes.Dodge + def;

            //See what the difference is
            int difference = 10 + (hitChance - defendChance);

            return difference > 10 ? 100 : difference < 0 ? 0 : difference * 10;
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

            foreach (AttackLocation loc in Enum.GetValues(typeof(AttackLocation)).Cast<AttackLocation>())
            {
                //Avoid hitting parts which have been disabled already
                switch (loc)
                {
                    case AttackLocation.CHEST:
                        if (defender.Anatomy.Chest < 0)
                        {
                            continue; //No use
                        }
                        break;
                    case AttackLocation.HEAD:
                        if (defender.Anatomy.Head < 0)
                        {
                            continue;
                        }
                        break;
                    case AttackLocation.LEFT_ARM:
                        if (defender.Anatomy.LeftArm < 0)
                        {
                            continue;
                        }
                        break;
                    case AttackLocation.LEGS:
                        if (defender.Anatomy.Legs < 0)
                        {
                            continue;
                        }
                        break;
                    case AttackLocation.RIGHT_ARM:
                        if (defender.Anatomy.RightArm < 0)
                        {
                            continue;
                        }
                        break;
                }

                for (int i = 0; i < CalculateHitPercentage(attacker, defender, loc); i++)
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
        public static PlayerFeedback[] Attack(Actor attacker, Actor defender, AttackLocation location)
        {
            List<PlayerFeedback> feedback = new List<PlayerFeedback>();

            //Do we succeed in the attack?
            int atk = 0;
            int def = 0;

            int weaponDamage = 3;
            int weaponWoundPotential = 9;
            DamageType damageType = DamageType.SLASH;

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

            if (difference + diceRoll > 0)
            {
                //We have a hit

                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.HIT, diceRoll));

                //TODO: HP AND CONCIOUSNESS

                //Do we wound the character?
                diceRoll = random.Next(10) + 1;

                int woundRoll = diceRoll / 2 + defender.Attributes.WoundResist - weaponWoundPotential;

                //TODO: ACTUAL WEAPON DAMAGE
                int damage = weaponDamage;

                if (woundRoll > 0)
                {
                    //No damage!
                    feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.NO_WOUND, diceRoll));
                    return feedback.ToArray();
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

                if (damage <= 0)
                {
                    //Bounced off the armour
                    feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.BOUNCE, diceRoll));
                    return feedback.ToArray();
                }

                //Apply the damage
                switch (location)
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

                //Damage assessment - Do this properly later
                //TODO: ATTRIBUTE PENALTIES AND SUCH THINGS
                switch (location)
                {
                    case AttackLocation.HEAD:
                        if (defender.Anatomy.Head < 0)
                        {
                            if (defender.Anatomy.Head <= -5)
                            {
                                //Destroyed
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DESTROY, diceRoll));
                            }
                            else
                            {
                                //Disabled
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DISABLE, diceRoll));
                            }

                            //Dead
                            feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.KILL, diceRoll));
                            //Close the interface
                            feedback.Add(new InterfaceToggleFeedback(InternalActionEnum.OPEN_ATTACK,false,defender));
                            KillCharacter(defender);
                        }
                        break;

                    case AttackLocation.CHEST:
                        if (defender.Anatomy.Chest < 0)
                        {
                            if (defender.Anatomy.Chest <= -5)
                            {
                                //Destroyed
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DESTROY, diceRoll));
                            }
                            else
                            {
                                //Disabled
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DISABLE, diceRoll));
                            }

                            //Dead
                            feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.KILL, diceRoll));
                            feedback.Add(new InterfaceToggleFeedback(InternalActionEnum.OPEN_ATTACK, false, defender));
                            KillCharacter(defender);
                        }
                        break;

                    case AttackLocation.LEFT_ARM:
                        if (defender.Anatomy.LeftArm < 0)
                        {
                            if (defender.Anatomy.LeftArm <= -5)
                            {
                                //Destroyed
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DESTROY, diceRoll));
                            }
                            else
                            {
                                //Disabled
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DISABLE, diceRoll));
                            }
                        }
                        break;

                    case AttackLocation.LEGS:
                        if (defender.Anatomy.Legs < 0)
                        {
                            if (defender.Anatomy.Legs <= -5)
                            {
                                //Destroyed
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DESTROY, diceRoll));
                            }
                            else
                            {
                                //Disabled
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DISABLE, diceRoll));
                            }
                        }
                        break;

                    case AttackLocation.RIGHT_ARM:
                        if (defender.Anatomy.RightArm < 0)
                        {
                            if (defender.Anatomy.RightArm <= -5)
                            {
                                //Destroyed
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DESTROY, diceRoll));
                            }
                            else
                            {
                                //Disabled
                                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DISABLE, diceRoll));
                            }

                            //Dead
                            KillCharacter(defender);
                        }
                        break;

                    default:
                        throw new NotImplementedException("You've injured an unknown body part " + location);
                }

            }
            else
            {
                //We have a miss
                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.MISS, diceRoll));
            }

            //We're done

            return feedback.ToArray();
        }

        /// <summary>
        /// Kills the current character
        /// </summary>
        /// <param name="actor"></param>
        private static void KillCharacter(Actor actor)
        {
            actor.IsAlive = false;
        }

        /// <summary>
        /// Returns a particular logged action.
        /// </summary>
        /// <returns></returns>
        private static CurrentLogFeedback LogAction(Actor attacker, Actor defender, AttackLocation loc, DamageType type, LogMessageStatus status, int diceroll)
        {
            //Later we'll expand this and put in some randomisation and stuff

            CurrentLogFeedback log = null;

            if (status == LogMessageStatus.BOUNCE)
            {
                //Bounces off the armour
                if (!attacker.IsPlayerCharacter)
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "The hit bounces off your armour");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "The hit scratches the armour, but doesn't cut through");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "The hit fails to punch through your armour");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "The hit fails to punch through your armour");
                    }
                }
                else
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "The hit bounces off your target's armour");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "The hit scratches the armour, but doesn't cut through");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "The hit fails to punch through your target's armour");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "The hit fails to punch through your target's armour");
                    }
                }

                return log;
            }

            if (status == LogMessageStatus.DESTROY)
            {
                if (!attacker.IsPlayerCharacter)
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " smashes into pulp under the force of the blow");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " flies off in an arc");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " dangles off by its skin");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " dangles off by its skin");
                    }
                }
                else
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " smashes into pulp under the force of the blow");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " flies off in an arc");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " dangles off by its skin");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " dangles off by its skin");
                    }
                }

                return log;
            }

            if (status == LogMessageStatus.DISABLE)
            {
                if (!attacker.IsPlayerCharacter)
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " cracks loudly as the bones smash under the blow");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " is cut open with the attack");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the attack goes through");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the attack goes through");
                    }
                }
                else
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " cracks loudly as the bones smash under the blow");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " is cut open with the attack");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the attack goes through");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = new CurrentLogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the attack goes through");
                    }
                }

                return log;
            }

            if (status == LogMessageStatus.HIT)
            {
                if (attacker.IsPlayerCharacter)
                {
                    log = new CurrentLogFeedback(InterfaceSpriteName.SWORD, Color.DarkGreen, "You swing (" + diceroll + ") at " + defender.EnemyData.EnemyName + " and hit him in the " + loc.ToString().ToLower().Replace("_", " "));
                }
                else
                {
                    log = new CurrentLogFeedback(InterfaceSpriteName.SWORD, Color.DarkRed, defender.EnemyData.EnemyName + " swings (" + diceroll + ") at you and hits you in the " + loc.ToString().ToLower().Replace("_", " "));
                }

                return log;
            }

            if (status == LogMessageStatus.KILL)
            {
                if (attacker.IsPlayerCharacter)
                {
                    log = new CurrentLogFeedback(InterfaceSpriteName.HEAD, Color.DarkGreen, "You strike your opponent down");
                }
                else
                {
                    log = new CurrentLogFeedback(InterfaceSpriteName.HEAD, Color.DarkRed, "You die");
                }

                return log;
            }

            if (status == LogMessageStatus.MISS)
            {
                //We have a miss
                if (attacker.IsPlayerCharacter)
                {
                    log = new CurrentLogFeedback(InterfaceSpriteName.SWORD, Color.DarkRed, "You swing (" + diceroll + ") at " + defender.EnemyData.EnemyName + "'s " + loc.ToString().ToLower().Replace("_", " ") + " but miss");
                }
                else
                {
                    //Player is the defender
                    log = new CurrentLogFeedback(InterfaceSpriteName.SWORD, Color.DarkGreen, "The " + defender.EnemyData.EnemyName + " swings (" + diceroll + ") at your " + loc.ToString().ToLower().Replace("_", " ") + ", but misses");
                }

                return log;
            }

            if (status == LogMessageStatus.NO_WOUND)
            {
                //No damage!
                if (attacker.IsPlayerCharacter)
                {
                    log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "You however fail to wound your opponent");
                }
                else
                {
                    log = new CurrentLogFeedback(InterfaceSpriteName.BLOOD, Color.DarkGreen, "It fails to properly wound you, however");
                }

                return log;
            }

            throw new NotImplementedException("No idea what to do here");
        }

        /// <summary>
        /// Gets the effect the stance has on attack and defence
        /// </summary>
        /// <param name="attack"></param>
        /// <param name="defence"></param>
        /// <param name="stance"></param>
        private static void GetStanceEffect(out int attack, out int defence, ActorStance stance)
        {
            switch (stance)
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
