using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;

namespace DRObjects.EventHandling.MultiEvents
{
    /// <summary>
    /// Represents a large sequence of events which the player must go through.
    /// The event is handled upon termination
    /// </summary>
    public class GameMultiEvent
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public SpriteData Image { get; set; }
        /// <summary>
        /// The choices the player may make 
        /// </summary>
        public MultiEventChoice[] Choices { get; set; }

        /// <summary>
        /// The name of the Event itself. Use this to determine what code should run
        /// </summary>
        public string EventName { get; set; }
    }
}
