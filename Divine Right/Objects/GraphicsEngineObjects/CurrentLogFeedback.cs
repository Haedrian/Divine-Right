using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.GraphicsEngineObjects
{
    /// <summary>
    /// Represents Text which is shown on the player's current log
    /// </summary>
    class CurrentLogFeedback:
        Abstract.PlayerFeedback
    {
        /// <summary>
        /// The text to show
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Creates a new Current log Feedback and the text to be shown
        /// </summary>
        /// <param name="text"></param>
        public CurrentLogFeedback(string text)
        {
            this.Text = text;
        }

        public override string ToString()
        {
            return "CLF: " + Text;
        }
    }
}
