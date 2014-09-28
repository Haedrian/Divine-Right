using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using Microsoft.Xna.Framework;

namespace DRObjects.EventHandling.MultiEvents
{
    public class MultiEventChoice
    {
        /// <summary>
        /// The text to display
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The name of the choice to report to the user
        /// </summary>
        public string ChoiceName { get; set; }

        /// <summary>
        /// The Next Multievent to load after this choice is made.
        /// If this is null. It will terminate
        /// </summary>
        public GameMultiEvent NextChoice { get; set; }

        /// <summary>
        /// The location of the actual interface component
        /// </summary>
        public Rectangle Rect { get; set; }
    }
}
