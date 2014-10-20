using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DRObjects.Graphics
{
    [Serializable]
    /// <summary>
    /// This is an overridden version of SpriteData which writes text on the screen instead.
    /// Text is written at the bottom corner in the default font
    /// </summary>
    public class TextSpriteData
        : SpriteData
    {
        /// <summary>
        /// The text to display
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The Colour to Display this text in
        /// </summary>
        public Color Colour { get; set; }
    }
}
