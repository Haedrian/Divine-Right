using DRObjects;
using DRObjects.ActorHandling;
using DRObjects.ActorHandling.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightGame.ActorHandling
{
    /// <summary>
    /// For Handling of Effects
    /// </summary>
    public static class EffectsManager
    {
        /// <summary>
        /// Performs the effect on this actor. This includes assigning it to the effects list of the GameState
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="effect"></param>
        public static void PerformEffect(Actor actor,Effect effect)
        {
            switch(effect.Name)
            {
                case EffectName.AGIL: actor.Attributes.TempAgil = actor.Attributes.TempAgil ?? 0 + effect.EffectAmount; break;
                case EffectName.BRAWN: actor.Attributes.TempBrawn = actor.Attributes.TempBrawn ?? 0 + effect.EffectAmount; break;
                case EffectName.CHAR: actor.Attributes.TempChar = actor.Attributes.TempChar ?? 0 + effect.EffectAmount; break;
                case EffectName.INTEL: actor.Attributes.TempIntel = actor.Attributes.TempIntel ?? 0 + effect.EffectAmount; break;
                case EffectName.PERC: actor.Attributes.TempPerc = actor.Attributes.TempPerc ?? 0 + effect.EffectAmount; break;
            }

            effect.Actor = actor;

            GameState.LocalMap.ActiveEffects.Add(effect);
        }

        /// <summary>
        /// Removes the effect of the effect (heh) on the actor. This includes removing it from GameState
        /// </summary>
        /// <param name="actor"></param>
        /// <param name="effect"></param>
        public static void RemoveEffect(Effect effect)
        {
            switch (effect.Name)
            {
                case EffectName.AGIL: effect.Actor.Attributes.TempAgil -= effect.EffectAmount; break;
                case EffectName.BRAWN: effect.Actor.Attributes.TempBrawn -= effect.EffectAmount; break;
                case EffectName.CHAR: effect.Actor.Attributes.TempChar -= effect.EffectAmount; break;
                case EffectName.INTEL: effect.Actor.Attributes.TempIntel -= effect.EffectAmount; break;
                case EffectName.PERC: effect.Actor.Attributes.TempPerc -= effect.EffectAmount; break;
            }

            GameState.LocalMap.ActiveEffects.Remove(effect);
        }

    }
}
