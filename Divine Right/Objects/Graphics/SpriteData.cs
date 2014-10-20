using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DRObjects.Graphics
{
    [Serializable]
    /// <summary>
    /// Describes how to obtain a particular sprite
    /// </summary>
    public class SpriteData
    {
        public string path;
        public Rectangle? sourceRectangle;
        /// <summary>
        /// The colour filter to apply, if any
        /// </summary>
        public Color? ColorFilter { get; set; }

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

        /// <summary>
        /// For use with serialising
        /// </summary>
        public SpriteData()
        {

        }
    
        /// <summary>
        /// Clones the Sprite Data
        /// </summary>
        /// <param name="clone"></param>
        public SpriteData(SpriteData clone)
        {
            this.path = clone.path;
            this.sourceRectangle = clone.sourceRectangle;
            this.ColorFilter = clone.ColorFilter;
        }

    }
 
}
