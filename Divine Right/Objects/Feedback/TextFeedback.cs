using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.GraphicsEngineObjects
{
    /// <summary>
    /// Represents text shown temporarily upon the player's screen
    /// </summary>
    public class TextFeedback:
        Abstract.ActionFeedback
    {
        /// <summary>
        /// The text to show
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Creates a new text feedback and the text to be shown
        /// </summary>
        /// <param name="text"></param>
        public TextFeedback(string text)
        {
            this.Text = text;

        }

        public TextFeedback()
        {

        }

        public override string ToString()
        {
            return "TF: " + Text;
        }

    }
}
