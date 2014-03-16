using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;

namespace DRObjects.EventHandling
{
    /// <summary>
    /// Represents an event which occured in which the player might or might not have to take a decision
    /// </summary>
    public class GameEvent
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public SpriteData Image { get; set; }
        public EventChoice[] EventChoices { get; set; }
    }
}
