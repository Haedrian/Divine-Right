using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.EventHandling
{
    /// <summary>
    /// Represents an EventChoice - a way of resolving a particular Event
    /// </summary>
    public class EventChoice
    {
        /// <summary>
        /// The text to display
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The internal action to invoke when clicked
        /// </summary>
        public InternalActionEnum? InternalAction { get; set; }
        /// <summary>
        /// The action to invoke when clicked
        /// </summary>
        public ActionTypeEnum? Action { get; set; }

        /// <summary>
        /// Arguments
        /// </summary>
        public object[] Agrs { get; set; }
    }
}
