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
        public static PlayerFeedback[] CheckHealth(Actor actor)
        {
            if (!actor.IsAlive)
            {
                //Nothing we can do here
                return new PlayerFeedback[]{};
            }

            //Check for body part damage
            if (actor.Anatomy.Head < 0)
            {
                //Character is dead
                actor.IsAlive = false;
                if (actor.IsPlayerCharacter)
                {
                    //Inform the character
                    return new PlayerFeedback[] { new CreateEventFeedback("DEATH") };
                }
            }

            if (actor.Anatomy.Chest < 0)
            {
                //Character is dead
                actor.IsAlive = false;
                if (actor.IsPlayerCharacter)
                {
                    //Inform the character
                    return new PlayerFeedback[] { new CreateEventFeedback("DEATH") };
                }
            }

            //Bleed a bit
            actor.Anatomy.BloodTotal -= actor.Anatomy.BloodLoss;

            if (actor.Anatomy.BloodLoss <= 0 && actor.Anatomy.BloodTotal < HumanoidAnatomy.BLOODTOTAL)
            {
                //Increase the blood amount
                actor.Anatomy.BloodTotal++;
            }
            else if (actor.Anatomy.BloodLoss <= 0)
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
                actor.IsAlive = false;
                if (actor.IsPlayerCharacter)
                {
                    return new PlayerFeedback[] { new CurrentLogFeedback(InterfaceSpriteName.BLEEDING, Color.Red, "You bleed to death"), new CreateEventFeedback("DEATH") };
                }
            }


           

            if (actor.IsStunned) //unstun
            {
                actor.IsStunned = false;
            }

            if (actor.Anatomy.StunAmount > 0)
            {
                if (actor.Anatomy.StunAmount < 10)
                {
                    //Reduce it to the max amount. 10
                    actor.Anatomy.StunAmount = 10;
                }

                //Check whether the actor is stunned or not

                int diceRoll = random.Next(10) + 1;

                if (diceRoll < actor.Anatomy.StunAmount)
                {
                    //Stunned
                    actor.IsStunned = true;
                    return new PlayerFeedback[] { new CurrentLogFeedback(InterfaceSpriteName.SPIRAL, Color.Red, "You black out")};
                }
                else
                {
                    //Feel better
                    actor.Anatomy.StunAmount--;
                }
            }

            return new PlayerFeedback[] { };

        }
    }
}
