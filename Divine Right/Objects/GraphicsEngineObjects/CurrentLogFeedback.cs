using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;
using Microsoft.Xna.Framework;

namespace DRObjects.GraphicsEngineObjects
{
    /// <summary>
    /// Represents Text which is shown on the player's current log. Has an icon to differentiate the types
    /// </summary>
    public class CurrentLogFeedback:
        Abstract.PlayerFeedback
    {
        /// <summary>
        /// The text to show
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The Icon to display - if any
        /// </summary>
        public InterfaceSpriteName? Icon { get; set; }

        /// <summary>
        /// The colour to draw the text and icon in
        /// </summary>
        public Color DrawColour { get; set; }

        /// <summary>
        /// Creates a new Current log Feedback and the text to be shown
        /// </summary>
        /// <param name="text"></param>
        public CurrentLogFeedback(InterfaceSpriteName? icon,Color colour,string text)
        {
            this.Icon = icon;
            this.DrawColour = colour;
            this.Text = text;
        }

        public override string ToString()
        {
            return "CLF: " + Text;
        }
    }
}
