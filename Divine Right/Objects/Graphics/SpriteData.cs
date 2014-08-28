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
    /// <summary>
    /// This overrides how the rectangle is serialised
    /// </summary>
    public class MyRectangleConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var rectangle = (Rectangle)value;

            var x = rectangle.X;
            var y = rectangle.Y;
            var width = rectangle.Width;
            var height = rectangle.Height;

            var o = JObject.FromObject(new { x, y, width, height });

            o.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var o = JObject.Load(reader);

            var x = GetTokenValue(o, "x") ?? 0;
            var y = GetTokenValue(o, "y") ?? 0;
            var width = GetTokenValue(o, "width") ?? 0;
            var height = GetTokenValue(o, "height") ?? 0;

            return new Rectangle(x, y, width, height);
        }

        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        private static int? GetTokenValue(JObject o, string tokenName)
        {
            JToken t;
            return o.TryGetValue(tokenName, StringComparison.InvariantCultureIgnoreCase, out t) ? (int)t : (int?)null;
        }
    }


}
