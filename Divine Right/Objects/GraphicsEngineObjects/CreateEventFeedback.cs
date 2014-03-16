using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects.Abstract;

namespace DRObjects.GraphicsEngineObjects
{
    /// <summary>
    /// Feedback to create an event
    /// </summary>
    public class CreateEventFeedback:
        PlayerFeedback
    {
        public string EventName { get; set; }

        public CreateEventFeedback(string eventName)
        {
            this.EventName = eventName;
        }
    }
}
