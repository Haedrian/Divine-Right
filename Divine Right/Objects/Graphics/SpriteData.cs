using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DRObjects.Graphics
{
    /// <summary>
    /// Describes how to obtain a particular sprite
    /// </summary>
    public class SpriteData
    {
        public string path;
        public Rectangle? sourceRectangle;

        /// <summary>
        /// A new SpriteData which points to a particular path within a rectangle
        /// </summary>
        /// <param name="path"></param>
        /// <param name="rect"></param>
        public SpriteData(string path, Rectangle? rect = null)
        {
            this.path = path;
            this.sourceRectangle = rect;
        }
    
    }


}
