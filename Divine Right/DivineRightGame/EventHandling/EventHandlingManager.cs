using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.EventHandling;
using DRObjects.Graphics;
using DRObjects.Enums;

namespace DivineRightGame.EventHandling
{
    /// <summary>
    /// Class for handling and creation of events
    /// </summary>
    public static class EventHandlingManager
    {
        /// <summary>
        /// Creates an Event
        /// </summary>
        /// <param name="eventName"></param>
        /// <returns></returns>
        public static GameEvent CreateEvent(string eventName)
        {
            //For now this is hard coded for death. Eventually we want to use the database
            return new GameEvent()
            {
                Image = SpriteManager.GetSprite(InterfaceSpriteName.DEAD),
                Text = "You have fallen in battle\nin the service of your god.\n\nYour spirit enters the afterlife",
                Title = "You have died",
                EventChoices = new EventChoice[] 
                {
                 new EventChoice
                 {
                    InternalAction = InternalActionEnum.LOSE,
                    Text = "Receive your eternal reward",
                    Agrs = null
                 }   
                }

            };
        }
    }
}
