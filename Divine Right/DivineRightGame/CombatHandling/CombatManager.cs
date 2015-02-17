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
using DRObjects.Items.Archetypes.Local;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.ActorHandling.SpecialAttacks;

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
            penaltyDict.Add(AttackLocation.HEAD, -10);
            penaltyDict.Add(AttackLocation.LEFT_ARM, +1);
            penaltyDict.Add(AttackLocation.LEGS, -6);
            penaltyDict.Add(AttackLocation.RIGHT_ARM, -6);
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
                        return -1;
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

            //Does the defender have a shield?
            int shieldBonus = 0;

            if (defender.Inventory.EquippedItems.ContainsKey(EquipmentLocation.SHIELD) && defender.Anatomy.LeftArm != 0)
            {
                shieldBonus = defender.Inventory.EquippedItems[EquipmentLocation.SHIELD].ArmourRating;
            }

            //Don't forget the shield if it's there
            int defendChance = defender.Attributes.Dodge + def + shieldBonus;

            //See what the difference is
            int difference = 20 + (hitChance - defendChance);

            return difference > 20 ? 100 : difference < 0 ? 0 : difference / 2 * 10;
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
        /// <param name="special">Special attack modifications to make. THIS DOES NOT CONSIDER TARGETS, ATTACKS OR PUSH</param>
        /// <returns></returns>
        public static ActionFeedback[] Attack(Actor attacker, Actor defender, AttackLocation location, SpecialAttack special = null)
        {
            List<ActionFeedback> feedback = new List<ActionFeedback>();

            if (defender.MapCharacter == null)
            {
                return new ActionFeedback[] { }; //What on earth are you doing?
            }

            int distance = attacker.MapCharacter.Coordinate - defender.MapCharacter.Coordinate; //This will later be used for ranged attacks

            if (distance >= 2)
            {
                //If the defender is not the character, get thhe defender to walk to the attacked location
                if (!defender.IsPlayerCharacter)
                {
                    defender.MissionStack.Push(defender.CurrentMission);
                    //It's not retainable, so if it's preempted by another mission he won't walk back to the attacked location for nothing
                    defender.CurrentMission = new WalkToMission() { isRetainable = false, TargetCoordinate = attacker.MapCharacter.Coordinate };
                }
            }

            //Do we succeed in the attack?
            int atk = 0;
            int def = 0;

            //The type of dice we roll 3 of to determine weapon damage
            int weaponDiceRolls = attacker.UnarmedDamageDice; //Default damage
            int weaponWoundPotential = 0;
            int weaponStunAmount = 0;

            if (distance < 2)
            {
                if (attacker.Inventory.EquippedItems.ContainsKey(EquipmentLocation.WEAPON) && attacker.Anatomy.RightArm > 0)
                {
                    //Do we have a weapon and an arm to use it?
                    weaponDiceRolls = attacker.Inventory.EquippedItems[EquipmentLocation.WEAPON].DamageDice;
                    weaponWoundPotential = attacker.Inventory.EquippedItems[EquipmentLocation.WEAPON].WoundPotential;
                    weaponStunAmount = attacker.Inventory.EquippedItems[EquipmentLocation.WEAPON].StunAmount;
                }
            }
            else
            {
                //Grab the weapon from the BOW slot
                if (attacker.Inventory.EquippedItems.ContainsKey(EquipmentLocation.BOW) && attacker.Anatomy.RightArm > 0 && attacker.Anatomy.LeftArm > 0)
                {
                    weaponDiceRolls = attacker.Inventory.EquippedItems[EquipmentLocation.BOW].DamageDice;
                    weaponWoundPotential = attacker.Inventory.EquippedItems[EquipmentLocation.BOW].WoundPotential;
                    weaponStunAmount = attacker.Inventory.EquippedItems[EquipmentLocation.BOW].StunAmount;
                }
                else
                {
                    //Firing with a destroyed upper body? For shame
                    feedback.Add(new LogFeedback(null, Color.Red, "You are unable to use a ranged weapon under these conditions"));
                }
            }

            DamageType damageType = DamageType.SLASH;
            if (distance >= 2)
            {
                //Ranged
                damageType = DamageType.PIERCE;
            }

            GetStanceEffect(out atk, out def, attacker.CombatStance);

            //Does the defender have a shield?
            int shieldBonus = 0;

            if (defender.Inventory.EquippedItems.ContainsKey(EquipmentLocation.SHIELD) && defender.Anatomy.LeftArm != 0)
            {
                shieldBonus = defender.Inventory.EquippedItems[EquipmentLocation.SHIELD].ArmourRating;
            }

            int hitChance = 0;


            if (distance < 2)
            {
                //Chance to hit  for hand to hand-
                // Attacker Skill + Brawn - 5 + location penalty + stance effect  VS Defender Skill + Agil + stance effect + shield bonus
                hitChance = attacker.Attributes.HandToHand + attacker.TotalBrawn - 5 + penaltyDict[location] + atk;
            }
            else
            {
                //Chance to hit  for ranged-
                // Attacker Skill + perc - 5 + location penalty + stance effect - distance*2  VS Defender Skill + Agil + stance effect + shield bonus
                hitChance = attacker.Attributes.Ranged + attacker.TotalPerc - 5 - distance * 2 + penaltyDict[location] + atk;
            }

            if (special != null)
            {
                hitChance += special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.ACCURACY) == null ? 0 : special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.ACCURACY).EffectValue;
            }

            GetStanceEffect(out atk, out def, defender.CombatStance);

            int defendChance = defender.Attributes.Dodge + def + shieldBonus;

            //See what the difference is
            int difference = hitChance - defendChance;

            //Now roll a d20 and see whether we hit
            int diceRoll = random.Next(20) + 1;

            if (difference + diceRoll > 0)
            {
                //We have a hit

                //The defender will learn something
                defender.Attributes.IncreaseSkill(SkillName.DODGER);

                //Are they wearing armour ?
                if (defender.Inventory.EquippedItems.ContainsKey(EquipmentLocation.BODY))
                {
                    defender.Attributes.IncreaseSkill(SkillName.ARMOUR_USER);
                }

                //Are they using a shield?
                if (defender.Inventory.EquippedItems.ContainsKey(EquipmentLocation.SHIELD))
                {
                    defender.Attributes.IncreaseSkill(SkillName.BLOCKER);
                }

                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.HIT, diceRoll));

                //Calculate the amount of damage we're going to do. Roll 3 dice of a particular kind

                int weaponDamage = random.Next(weaponDiceRolls + 1) + random.Next(weaponDiceRolls + 1) + random.Next(weaponDiceRolls + 1);

                Console.WriteLine("Damage Roll : " + weaponDamage);

                int damage = weaponDamage;

                if (special != null)
                {
                    damage += special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.DAMAGE) == null ? 0 : special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.DAMAGE).EffectValue;
                }

                if (damage > 1)
                {
                    //Do we have any defences?
                    if (defender.IsPlayerCharacter && defender.CurrentDefences > 0)
                    {
                        //Break one instead
                        defender.CurrentDefences--;

                        feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.DEFENDED, diceRoll));

                        //And mark it
                        GameState.LocalMap.TemporaryGraphics.Add(new TemporaryGraphic()
                        {
                            Coord = new MapCoordinate(defender.MapCharacter.Coordinate),
                            Graphic = SpriteManager.GetSprite(distance < 2 ? InterfaceSpriteName.SWORD_BLOCKED : InterfaceSpriteName.BOW_BLOCKED),
                            LifeTime = attacker.IsPlayerCharacter ? 2 : 2
                        });

                        return feedback.ToArray();

                    }
                }

                //Apply the damage
                //Is the user wearing any armour?
                if (location == AttackLocation.CHEST || location == AttackLocation.LEFT_ARM || location == AttackLocation.RIGHT_ARM)
                {
                    //Determine how much damage we can get through due to pierce
                    int piercing = 0;

                    if (special != null)
                    {
                        piercing = special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.PIERCING) == null ? 0 : special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.PIERCING).EffectValue;
                    }

                    //Use the chest armour
                    if (defender.Inventory.EquippedItems.ContainsKey(EquipmentLocation.BODY))
                    {
                        int block = defender.Inventory.EquippedItems[EquipmentLocation.BODY].ArmourRating - piercing;

                        damage -= (block > 0 ? block : 0);

                        //Do we damage the armour?
                        int sunder = 0;

                        if (special != null)
                        {
                            sunder = special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.SUNDER) == null ? 0 : special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.SUNDER).EffectValue;
                        }

                        if (sunder > 0 && defender.Inventory.EquippedItems[EquipmentLocation.BODY].ArmourRating > 0)
                        {
                            defender.Inventory.EquippedItems[EquipmentLocation.BODY].ArmourRating -= sunder;

                            if (defender.Inventory.EquippedItems[EquipmentLocation.BODY].ArmourRating < 0) //no negative block
                            {
                                defender.Inventory.EquippedItems[EquipmentLocation.BODY].ArmourRating = 0;
                            }
                             
                        }

                    }
                }

                //Other location ?
                if (location == AttackLocation.HEAD)
                {
                    //Use the head armour
                    if (defender.Inventory.EquippedItems.ContainsKey(EquipmentLocation.HEAD))
                    {
                        damage -= defender.Inventory.EquippedItems[EquipmentLocation.HEAD].ArmourRating;
                    }
                }

                if (location == AttackLocation.LEGS)
                {
                    //Use the head armour
                    if (defender.Inventory.EquippedItems.ContainsKey(EquipmentLocation.LEGS))
                    {
                        damage -= defender.Inventory.EquippedItems[EquipmentLocation.LEGS].ArmourRating;
                    }
                }

                if (damage <= 0)
                {
                    //Bounced off the armour
                    feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.BOUNCE, diceRoll));

                    GameState.LocalMap.TemporaryGraphics.Add(new TemporaryGraphic()
                    {
                        Coord = new MapCoordinate(defender.MapCharacter.Coordinate),
                        Graphic = SpriteManager.GetSprite(distance < 2 ? InterfaceSpriteName.SWORD_ARMOUR : InterfaceSpriteName.BOW_ARMOUR),
                        LifeTime = attacker.IsPlayerCharacter ? 2 : 2
                    });

                    return feedback.ToArray();
                }

                //Do we wound the character?
                diceRoll = random.Next(10) + 1;

                int woundRoll = diceRoll + (defender.Attributes.WoundResist - weaponWoundPotential) - 5;

                int woundBonus = 0;

                if (special != null)
                {
                    woundBonus = special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.BLEED) == null ? 0 : special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.BLEED).EffectValue;

                }
                woundRoll += woundBonus;

                if (woundRoll <= 0)
                {
                    //Yes - open a wound
                    defender.Anatomy.BloodLoss++;
                    defender.Anatomy.BloodLoss += woundBonus; //Yeah, bleed makes you bleed more AND increases chance of bleed. Not entirely sure this won't be overpowered

                    Console.WriteLine("Wounded " + woundRoll);

                    //Log it
                    feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.BLEED, woundRoll));
                }

                //Roll again
                diceRoll = random.Next(10) + 1;
                int stunRoll = diceRoll + (defender.Attributes.WoundResist - weaponStunAmount) - 5;

                int stunBonus = 0;

                if (special != null)
                {
                    stunBonus = special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.STUN) == null ? 0 : special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.STUN).EffectValue;
                }

                stunRoll += stunBonus;


                if (stunRoll <= 0)
                {
                    //Stun him
                    defender.Anatomy.StunAmount++;
                    defender.Anatomy.StunAmount += stunRoll; //Yeah stun makes you more likely to get stunned and makes you stun longer. Not entirely sure this won't be as overpowered as ell

                    Console.WriteLine("Stunned " + stunRoll);
                }

                //Any damage bonus ?
                if (special != null)
                {
                    damage += special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.DAMAGE) == null ? 0 : special.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.DAMAGE).EffectValue;
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

                GameState.LocalMap.TemporaryGraphics.Add(new TemporaryGraphic()
                {
                    Coord = new MapCoordinate(defender.MapCharacter.Coordinate),
                    Graphic = SpriteManager.GetSprite(distance < 2 ? InterfaceSpriteName.SWORD_HIT : InterfaceSpriteName.BOW_HIT),
                    LifeTime = attacker.IsPlayerCharacter ? 2 : 2
                });


                //Damage assessment - Do this properly later
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
                            feedback.Add(new InterfaceToggleFeedback(InternalActionEnum.OPEN_ATTACK, false, defender));
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

                        }
                        break;

                    default:
                        throw new NotImplementedException("You've injured an unknown body part " + location);
                }

            }
            else
            {
                //We have a miss

                //The attacker learned something
                attacker.Attributes.IncreaseSkill(SkillName.FIGHTER);

                if (distance > 2)
                {
                    attacker.Attributes.IncreaseSkill(SkillName.ARCHER);
                }
                else
                    //Are they armed?
                    if (attacker.Inventory.EquippedItems.ContainsKey(EquipmentLocation.WEAPON))
                    {
                        switch (attacker.Inventory.EquippedItems[EquipmentLocation.WEAPON].WeaponType.ToUpper())
                        {
                            case "SWORD": attacker.Attributes.IncreaseSkill(SkillName.SWORDFIGHTER); break;
                            case "AXE": attacker.Attributes.IncreaseSkill(SkillName.AXEFIGHTER); break;
                            case "BOW": attacker.Attributes.IncreaseSkill(SkillName.ARCHER); break;
                            default: throw new NotImplementedException("No skill pertains to the weapon type " + attacker.Inventory.EquippedItems[EquipmentLocation.WEAPON].WeaponType);
                        }
                    }
                    else
                    {
                        //Unarmed then
                        attacker.Attributes.IncreaseSkill(SkillName.BRAWLER);
                    }

                feedback.Add(LogAction(attacker, defender, location, damageType, LogMessageStatus.MISS, diceRoll));

                GameState.LocalMap.TemporaryGraphics.Add(new TemporaryGraphic()
                {
                    Coord = new MapCoordinate(defender.MapCharacter.Coordinate),
                    Graphic = SpriteManager.GetSprite(distance < 2 ? InterfaceSpriteName.SWORD_BLOCKED : InterfaceSpriteName.BOW_BLOCKED),
                    LifeTime = (attacker.IsPlayerCharacter ? 2 : 2)
                });
            }

            //We're done

            return feedback.ToArray();
        }

        /// <summary>
        /// Attack a particular target using this special attack
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="target"></param>
        /// <param name="attack"></param>
        /// <returns></returns>
        public static ActionFeedback[] PerformSpecialAttack(Actor attacker, Actor target, SpecialAttack attack)
        {
            List<ActionFeedback> feedback = new List<ActionFeedback>();

            feedback.Add(new LogFeedback(InterfaceSpriteName.SA1, Color.Blue, "You strike using " + attack.AttackName));

            if (target.MapCharacter == null)
            {
                return new ActionFeedback[] { }; //Huh ?
            }

            //So, first we need to determine how many targets we're going to attack, and how many times
            List<Actor> targets = new List<Actor>();

            targets.Add(target);

            for (int i = 1; i < attack.Effects.Where(e => e.EffectType == SpecialAttackType.TARGETS).Select(e => e.EffectValue).FirstOrDefault(); i++)
            {
                //Go around the attacker and add another target - later we'll need to check if they're hostile
                foreach (var block in GameState.LocalMap.GetBlocksAroundPoint(attacker.MapCharacter.Coordinate, 1))
                {
                    foreach (var character in block.GetItems().Where(gi => gi.IsActive && gi.GetType() == typeof(LocalCharacter)))
                    {
                        Actor newTarget = (character as LocalCharacter).Actor;

                        if (!targets.Contains(newTarget))
                        {
                            //Add it!
                            targets.Add(newTarget);
                            continue;
                        }
                    }
                }

                break; //No more actors
            }

            int attacksToMake = attack.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.ATTACKS) == null ? 1 : attack.Effects.FirstOrDefault(e => e.EffectType == SpecialAttackType.ATTACKS).EffectValue;

            for (int i = 0; i < attacksToMake; i++)
            {
                foreach (var defender in targets)
                {
                    //Are we alive?
                    if (!defender.IsAlive)
                    {
                        continue; //yeah he's dead jim
                    }
                    else
                    {
                        //Attack!
                        feedback.AddRange(Attack(attacker, defender, AttackLocation.CHEST, attack));
                    }
                }
            }

            //So, let's handle the pushback now
            //TODO: Pushbac

            //Done
            return feedback.ToArray();

        }

        /// <summary>
        /// Kills the current character
        /// </summary>
        /// <param name="actor"></param>
        public static void KillCharacter(Actor actor)
        {
            actor.IsAlive = false;

            //Drop any stuff they have
            foreach (InventoryItem item in actor.Inventory.Inventory.GetAllObjects())
            {
                //Always drop loot & supplies,  10% chance of dropping something else
                if (item.Category == InventoryCategory.LOOT || item.Category == InventoryCategory.SUPPLY || GameState.Random.Next(10) == 5)
                {
                    //Drop them
                    item.InInventory = false;
                    item.IsEquipped = false;
                    GameState.LocalMap.GetBlockAtCoordinate(actor.MapCharacter.Coordinate).PutItemUnderneathOnBlock(item);
                }
            }


        }

        /// <summary>
        /// Returns a particular logged action.
        /// </summary>
        /// <returns></returns>
        private static ActionFeedback LogAction(Actor attacker, Actor defender, AttackLocation loc, DamageType type, LogMessageStatus status, int diceroll)
        {
            //Later we'll expand this and put in some randomisation and stuff

            ActionFeedback log = null;

            if (status == LogMessageStatus.BOUNCE)
            {
                //Bounces off the armour
                if (!attacker.IsPlayerCharacter)
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "The hit bounces off your armour");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "The hit scratches the armour, but doesn't cut through");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "The hit fails to punch through your armour");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "The hit fails to punch through your armour");
                    }
                    else if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "The missile fails to punch through your armour");
                    }
                }
                else
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "The hit bounces off your target's armour");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "The hit scratches the armour, but doesn't cut through");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "The hit fails to punch through your target's armour");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "The hit fails to punch through your target's armour");
                    }
                    else if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "The missile fails to punch through your target's armour");
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
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " smashes into pulp under the force of the blow");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " flies off in an arc");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " dangles off by its skin");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " dangles off by its skin");
                    }
                    else if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your perforated " + loc.ToString().ToLower().Replace("_", " ") + " hangs limply off your side");
                    }
                }
                else
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " smashes into pulp under the force of the blow");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " flies off in an arc");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " dangles off by its skin");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " dangles off by its skin");
                    }
                    else if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "Your opponent's perforated " + loc.ToString().ToLower().Replace("_", " ") + " hangs limply off their side");
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
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " cracks loudly as the bones smash under the blow");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " is cut open with the attack");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the attack goes through");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the attack goes through");
                    }
                    else if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkRed, "Your " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the missile pierces through");
                    }
                }
                else
                {
                    if (type == DamageType.CRUSH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " cracks loudly as the bones smash under the blow");
                    }
                    else if (type == DamageType.SLASH)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " is cut open with the attack");
                    }
                    else if (type == DamageType.STAB)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the attack goes through");
                    }
                    else if (type == DamageType.THRUST)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the attack goes through");
                    }
                    else if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkGreen, "Your opponent's " + loc.ToString().ToLower().Replace("_", " ") + " bleeds heavily as the missile pierces through");
                    }
                }

                return log;
            }

            if (status == LogMessageStatus.HIT)
            {
                if (attacker.IsPlayerCharacter)
                {
                    if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.BOW, Color.DarkGreen, "You loose (" + diceroll + ") at " + defender.EnemyData.EnemyName + " and hit him in the " + loc.ToString().ToLower().Replace("_", " "));
                    }
                    else
                    {
                        log = new LogFeedback(InterfaceSpriteName.SWORD, Color.DarkGreen, "You swing (" + diceroll + ") at " + defender.EnemyData.EnemyName + " and hit him in the " + loc.ToString().ToLower().Replace("_", " "));
                    }
                }
                else
                {
                    if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.SWORD, Color.DarkRed, attacker.EnemyData.EnemyName + " looses (" + diceroll + ") a projectile at you and hits you in the " + loc.ToString().ToLower().Replace("_", " "));
                    }
                    else
                    {
                        log = new LogFeedback(InterfaceSpriteName.SWORD, Color.DarkRed, attacker.EnemyData.EnemyName + " swings (" + diceroll + ") at you and hits you in the " + loc.ToString().ToLower().Replace("_", " "));
                    }
                }

                return log;
            }

            if (status == LogMessageStatus.KILL)
            {
                if (attacker.IsPlayerCharacter)
                {
                    log = new LogFeedback(InterfaceSpriteName.HEAD, Color.DarkGreen, "You strike your opponent down");
                }
                else
                {
                    //log = new CurrentLogFeedback(InterfaceSpriteName.HEAD, Color.DarkRed, "You die");
                    log = new CreateEventFeedback("Death");
                }

                return log;
            }

            if (status == LogMessageStatus.MISS)
            {
                //We have a miss
                if (attacker.IsPlayerCharacter)
                {
                    if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.SWORD, Color.DarkRed, "You loose  (" + diceroll + ") your projectile at " + defender.EnemyData.EnemyName + "'s " + loc.ToString().ToLower().Replace("_", " ") + " but miss");
                    }
                    else
                    {
                        log = new LogFeedback(InterfaceSpriteName.SWORD, Color.DarkRed, "You swing (" + diceroll + ") at " + defender.EnemyData.EnemyName + "'s " + loc.ToString().ToLower().Replace("_", " ") + " but miss");
                    }
                }
                else
                {
                    //Player is the defender

                    if (type == DamageType.PIERCE)
                    {
                        log = new LogFeedback(InterfaceSpriteName.SWORD, Color.DarkGreen, "The " + attacker.EnemyData.EnemyName + " looses (" + diceroll + ") their projectile at your " + loc.ToString().ToLower().Replace("_", " ") + ", but misses");
                    }
                    else
                    {
                        log = new LogFeedback(InterfaceSpriteName.SWORD, Color.DarkGreen, "The " + attacker.EnemyData.EnemyName + " swings (" + diceroll + ") at your " + loc.ToString().ToLower().Replace("_", " ") + ", but misses");
                    }
                }

                return log;
            }

            if (status == LogMessageStatus.NO_WOUND)
            {
                //No damage!
                if (attacker.IsPlayerCharacter)
                {
                    log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkRed, "You however fail to wound your opponent");
                }
                else
                {
                    log = new LogFeedback(InterfaceSpriteName.BLOOD, Color.DarkGreen, "It fails to properly wound you, however");
                }

                return log;
            }

            if (status == LogMessageStatus.BLEED)
            {
                if (defender.IsPlayerCharacter)
                {
                    if (defender.Anatomy.BloodLoss == 1)
                    {
                        //Wound opened
                        log = new LogFeedback(InterfaceSpriteName.BLEEDING, Color.DarkRed, "The attack opens a wound");
                    }
                    else
                    {
                        //Wound worsended
                        log = new LogFeedback(InterfaceSpriteName.BLEEDING, Color.DarkRed, "The attack worsens your wound");
                    }
                }

                return log;
            }

            if (status == LogMessageStatus.DEFENDED)
            {
                log = new LogFeedback(InterfaceSpriteName.DEFENSE, Color.DarkBlue, "You dodge the blow at the last moment");

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
                    attack = -8;
                    defence = 6;
                    break;
                case ActorStance.DEFENSIVE:
                    attack = -4;
                    defence = 2;
                    break;
                case ActorStance.NEUTRAL:
                    attack = 0;
                    defence = 0;
                    break;
                case ActorStance.AGGRESSIVE:
                    attack = 2;
                    defence = -4;
                    break;
                case ActorStance.COMPLETE_AGGRESSIVE:
                    attack = 6;
                    defence = -8;
                    break;
                default:
                    throw new NotImplementedException("No stance effects for " + stance.ToString());
            }

            return;
        }
    }
}
