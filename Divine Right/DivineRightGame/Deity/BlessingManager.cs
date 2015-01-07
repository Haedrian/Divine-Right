using DivineRightGame.ActorHandling;
using DivineRightGame.CombatHandling;
using DivineRightGame.ItemFactory.ItemFactoryManagers;
using DRObjects;
using DRObjects.ActorHandling;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.ActorHandling.Enums;
using DRObjects.Deity.Enums;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightGame.Deity
{
    /// <summary>
    /// For handling blessings
    /// </summary>
    public class BlessingManager
    {
        /// <summary>
        /// Get a blessing for this particular actor, and apply it.
        /// </summary>
        /// <param name="actor"></param>
        public static void GetAndApplyBlessing(Actor actor, out LogFeedback logFeedback)
        {
            //Determine how much skill they have
            int effectiveSkill = actor.Attributes.GetSkill(SkillName.RITUALIST) + actor.Attributes.Char - 5;

            effectiveSkill = effectiveSkill > 1 ? effectiveSkill : 1; //at least 1

            //Get a random number from 0 to effectiveskill * 2
            int randomNumber = GameState.Random.Next(0, effectiveSkill * 2);

            //Grab this number and pick the Blessing with the right enum. if we're too big then go for the biggest value

            BlessingType[] blessings = (BlessingType[])Enum.GetValues(typeof(BlessingType));

            if (randomNumber >= blessings.Length)
            {
                randomNumber = blessings.Length;
            }

            //And your blessing is....

            BlessingType blessing = blessings[randomNumber];
            
            //Now let's see what type of blessing it is

            InventoryItemManager iim = new InventoryItemManager();

            Effect effect = new Effect();
            logFeedback = null;
            switch (blessing)
            {
                case BlessingType.AGIL_1:
                    //Apply +1 agility for 4 times the effective skill
                    effect.EffectAmount = 1;
                    effect.Name = EffectName.AGIL;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.AGIL_2:
                    effect.EffectAmount = 2;
                    effect.Name = EffectName.AGIL;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.ARMOUR:
                    {
                        //Spawn a piece of armour worth 50 * effective skill
                        var inventoryItem = iim.GetBestCanAfford("ARMOUR", 50 * effectiveSkill);
                        if (inventoryItem != null)
                        {
                            inventoryItem.InInventory = true;
                            actor.Inventory.Inventory.Add(inventoryItem.Category, inventoryItem);
                            logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "Your have been given a gift...");
                        }
                        else
                        {
                            logFeedback = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkRed, "Your prayers have not been answered");
                        }
                    }
                    break;
                case BlessingType.BRAWN_1:
                    effect.EffectAmount = 1;
                    effect.Name = EffectName.BRAWN;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.BRAWN_2:
                    effect.EffectAmount = 2;
                    effect.Name = EffectName.BRAWN;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.CHAR_1:
                    effect.EffectAmount = 1;
                    effect.Name = EffectName.CHAR;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.CHAR_2:
                    effect.EffectAmount = 2;
                    effect.Name = EffectName.CHAR;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.DEFENCE:
                    //TODO LATER
                    break;
                case BlessingType.EXPERIENCE:
                    //Increase the skill in Rituals by a 10 times (fixed)
                    for (int i = 0; i < 10; i++)
                    {
                        actor.Attributes.IncreaseSkill(SkillName.RITUALIST);
                    }
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "Nothing happens, but you feel smarter for having tried");
                    break;
                case BlessingType.EXPLORE:
                    //Explore the entire map
                    for (int x = 0; x < GameState.LocalMap.localGameMap.GetLength(0); x++)
                    {
                        for (int y = 0; y < GameState.LocalMap.localGameMap.GetLength(1); y++)
                        {
                            GameState.LocalMap.localGameMap[x, y, 0].WasVisited = true;
                        }
                    }
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You have been granted knowledge on the structure of this level");
                    break;
                case BlessingType.FEEDING:
                    //A slap-up meal
                    actor.FeedingLevel = FeedingLevel.STUFFED;
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel very full...");
                    break;
                case BlessingType.HEALING:
                    //Heal the user for as many rounds as effective skill
                    HealthCheckManager.HealCharacter(actor, effectiveSkill);
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel your wounds knit together");
                    break;
                case BlessingType.INTEL_1:
                    effect.EffectAmount = 1;
                    effect.Name = EffectName.INTEL;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.INTEL_2:
                    effect.EffectAmount = 2;
                    effect.Name = EffectName.INTEL;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.KILL:
                    //Go through the map and slaughter a total amount of enemies equal to effective skill
                    for (int i = 0; i < effectiveSkill; i++)
                    {
                        var deadActor = GameState.LocalMap.Actors.Where(a => a.IsAggressive && a.IsActive && a.IsAlive && !a.IsPlayerCharacter).FirstOrDefault();

                        if (deadActor != null)
                        {
                            CombatManager.KillCharacter(deadActor);
                        }
                    }
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "A sprit of death travels through the location and slaughters a number of your enemies");
                    break;
                case BlessingType.LOOT:
                    {
                        //Spawn a piece of loot worth 50 * effective skill
                        var inventoryItem = iim.GetBestCanAfford("LOOT", 50 * effectiveSkill);
                        if (inventoryItem != null)
                        {
                            inventoryItem.InInventory = true;
                            actor.Inventory.Inventory.Add(inventoryItem.Category, inventoryItem);
                            logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "Your have been given a gift...");
                        }
                        else
                        {
                            logFeedback = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkRed, "Your prayers have not been answered");
                        }
                    }
                    break;
                case BlessingType.PERC_1:
                    effect.EffectAmount = 1;
                    effect.Name = EffectName.PERC;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.PERC_2:
                    effect.EffectAmount = 2;
                    effect.Name = EffectName.PERC;
                    effect.MinutesLeft = 4 * effectiveSkill;
                    effect.EffectDisappeared = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "The effect of the blessing disappears");
                    logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "You feel different. Good different");
                    break;
                case BlessingType.WEAPON:
                    {
                        //Spawn a piece of loot worth 50 * effective skill
                        var inventoryItem = iim.GetBestCanAfford("WEAPON", 50 * effectiveSkill);
                        if (inventoryItem != null)
                        {
                            inventoryItem.InInventory = true;
                            actor.Inventory.Inventory.Add(inventoryItem.Category, inventoryItem);
                            logFeedback = new LogFeedback(InterfaceSpriteName.SUN, Color.ForestGreen, "Your have been given a gift...");
                        }
                        else
                        {
                            logFeedback = new LogFeedback(InterfaceSpriteName.MOON, Color.DarkRed, "Your prayers have not been answered");
                        }
                    }
                    break;
            }

            if (effect != null)
            {
                //Apply it
                EffectsManager.PerformEffect(actor, effect);
            }
        }

    }
}
