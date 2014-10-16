using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects;
using DRObjects.GraphicsEngineObjects;
using DivineRightGame.EventHandling;
using DRObjects.ActorHandling;
using DRObjects.Graphics;
using Microsoft.Xna.Framework;
using DRObjects.Items.Archetypes.Local;
using DRObjects.ActorHandling.CharacterSheet.Enums;

namespace DivineRightGame.CombatHandling
{
    /// <summary>
    /// Class for handling Health Checks, bleeding, stunning and the like
    /// </summary>
    public static class HealthCheckManager
    {
        private static Random random = new Random();

        /// <summary>
        /// Checks for health and decides whether the character should be dead
        /// Also bleeds the character
        /// Also checks whether the character is stunned or not
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public static ActionFeedback[] CheckHealth(Actor actor)
        {
            if (!actor.IsAlive)
            {
                //Nothing we can do here
                return new ActionFeedback[] { };
            }

            //Check for body part damage
            if (actor.Anatomy.Head < 0)
            {
                //Character is dead
                actor.IsAlive = false;
                if (actor.IsPlayerCharacter)
                {
                    //Inform the character
                    return new ActionFeedback[] { new CreateEventFeedback("DEATH") };
                }
            }

            if (actor.Anatomy.Chest < 0)
            {
                //Character is dead
                actor.IsAlive = false;
                if (actor.IsPlayerCharacter)
                {
                    //Inform the character
                    return new ActionFeedback[] { new CreateEventFeedback("DEATH") };
                }
            }

            //Bleed a bit
            actor.Anatomy.BloodTotal -= actor.Anatomy.BloodLoss;

            if (actor.Anatomy.BloodLoss <= 0 && actor.Anatomy.BloodTotal < HumanoidAnatomy.BLOODTOTAL)
            {
                //Increase the blood amount
                actor.Anatomy.BloodTotal++;
            }
            else if (actor.Anatomy.BloodLoss > 0)
            {
                //Is it time to reduce the blood level?
                if (actor.Anatomy.BodyTimer++ >= HumanoidAnatomy.BODY_TIMER_FLIP)
                {
                    //Decrease bleeding amount
                    actor.Anatomy.BloodLoss--;
                    actor.Anatomy.BodyTimer = 0;
                }
                else
                {
                    //tick
                    actor.Anatomy.BodyTimer++;
                }
            }

            //Are we low on blood?

            if (actor.Anatomy.BloodTotal < HumanoidAnatomy.BLOOD_STUN_AMOUNT)
            {
                //Increase the stun level
                actor.Anatomy.StunAmount++;
            }

            if (actor.Anatomy.BloodTotal < 0)
            {
                //Death
                CombatManager.KillCharacter(actor); //Drop the stuff

                if (actor.IsPlayerCharacter)
                {
                    return new ActionFeedback[] { new CurrentLogFeedback(InterfaceSpriteName.BLEEDING, Color.Red, "You bleed to death"), new CreateEventFeedback("DEATH") };
                }
                else
                {
                    return new ActionFeedback[] { new CurrentLogFeedback(InterfaceSpriteName.BLEEDING, Color.Red, actor.Name + " has bled to death") };
                }
            }


            if (actor.IsStunned) //unstun
            {
                actor.IsStunned = false;
                if (actor.MapCharacter as LocalCharacter != null)
                {
                    (actor.MapCharacter as LocalCharacter).IsStunned = false;
                }
            }

            if (actor.Anatomy.StunAmount > 0)
            {
                if (actor.Anatomy.StunAmount > 10)
                {
                    //Reduce it to the max amount. 10
                    actor.Anatomy.StunAmount = 10;
                }

                //Check whether the actor is stunned or not

                if (actor.Anatomy.StunAmount > 0)
                {
                    //Stunned
                    actor.IsStunned = true;

                    if (actor.MapCharacter as LocalCharacter != null)
                    {
                        (actor.MapCharacter as LocalCharacter).IsStunned = true;
                    }

                    //Feel better
                    actor.Anatomy.StunAmount--;

                    if (actor.IsPlayerCharacter)
                    {
                        return new ActionFeedback[] { new CurrentLogFeedback(InterfaceSpriteName.SPIRAL, Color.Red, "You black out") };
                    }
                }
            }

            return new ActionFeedback[] { };

        }

        /// <summary>
        /// Heal this character if they require it
        /// You heal one point each time, +1 for every 5 levels in Healing
        /// Healing happens top down
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="rounds">The amount of rounds we're going to do this for.</param>
        public static void HealCharacter(Actor actor, int rounds)
        {
            //Do we need healing?
            if (actor.Anatomy.Head > -5 && actor.Anatomy.Head == actor.Anatomy.HeadMax
              && actor.Anatomy.Chest > -5 && actor.Anatomy.Chest == actor.Anatomy.ChestMax
              && actor.Anatomy.Legs > -5 && actor.Anatomy.Legs == actor.Anatomy.LegsMax
              && actor.Anatomy.LeftArm > -5 && actor.Anatomy.LeftArm == actor.Anatomy.LeftArmMax
              && actor.Anatomy.RightArm > -5 && actor.Anatomy.RightArm == actor.Anatomy.RightArmMax)
            {
                //Nope, we're fine
                return;
            }

            //Let's determine how much healing we're able to do
            int healing = 1*rounds;

            int skill = 0;

            if (actor.Attributes.Skills.ContainsKey(SkillName.HEALER))
            {
                skill = (int) actor.Attributes.Skills[SkillName.HEALER].SkillLevel;
            }

            healing = (1 + (skill / 5)) * rounds;

            //Go through each body part and incremement as needed

            int headHealing = actor.Anatomy.Head > -5 ? actor.Anatomy.HeadMax - actor.Anatomy.Head : 0;
            int chestHealing = actor.Anatomy.Chest > -5 ? actor.Anatomy.ChestMax - actor.Anatomy.Chest : 0;
            int leftHealing = actor.Anatomy.LeftArm > -5 ? actor.Anatomy.LeftArmMax - actor.Anatomy.LeftArm : 0;
            int rightHealing = actor.Anatomy.RightArm > -5 ? actor.Anatomy.RightArmMax - actor.Anatomy.RightArm : 0;
            int legHealing = actor.Anatomy.Legs > -5 ? actor.Anatomy.LegsMax - actor.Anatomy.Legs : 0;

            if (headHealing > 0)
            {
                //Heal the head
                if (headHealing <= healing)
                {
                    actor.Anatomy.Head += headHealing;
                    healing -= headHealing;
                }
                else
                {
                    actor.Anatomy.Head += healing;
                    healing = 0;
                }
            }

            if (chestHealing > 0 && healing > 0)
            {
                if (chestHealing <= healing)
                {
                    actor.Anatomy.Chest += chestHealing;
                    healing -= chestHealing;
                }
                else
                {
                    actor.Anatomy.Chest += healing;
                    healing = 0;
                }
            }

            if (leftHealing > 0 && healing > 0)
            {
                if (leftHealing<= healing)
                {
                    actor.Anatomy.LeftArm+= leftHealing;
                    healing -= leftHealing;
                }
                else
                {
                    actor.Anatomy.LeftArm += healing;
                    healing = 0;
                }
            }

            if (rightHealing > 0 && healing > 0)
            {
                if (rightHealing <= healing)
                {
                    actor.Anatomy.RightArm+= rightHealing;
                    healing -= rightHealing;
                }
                else
                {
                    actor.Anatomy.RightArm += healing;
                    healing = 0;
                }
            }

            if (legHealing > 0 && healing > 0)
            {
                if (legHealing <= healing)
                {
                    actor.Anatomy.Legs+= legHealing;
                    healing -= legHealing;
                }
                else
                {
                    actor.Anatomy.Legs += healing;
                    healing = 0;
                }
            }

            //And increase some skill!
            for (int i = 0; i < rounds; i++)
            {
                actor.Attributes.IncreaseSkill(SkillName.HEALER);
            }

        }
    }
}
