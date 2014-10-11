using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;

namespace DRObjects.DataStructures
{
    public sealed class RectSerializationSurrogate:
        ISerializationSurrogate 
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Rectangle rect = (Rectangle)obj;

            info.AddValue("Height", rect.Height);
            info.AddValue("Width", rect.Width);
            info.AddValue("X", rect.X);
            info.AddValue("Y", rect.Y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            typeof(Rectangle).GetField("Height").SetValue(obj, info.GetInt32("Height"));
            typeof(Rectangle).GetField("Width").SetValue(obj, info.GetInt32("Width"));
            typeof(Rectangle).GetField("X").SetValue(obj, info.GetInt32("X"));
            typeof(Rectangle).GetField("Y").SetValue(obj, info.GetInt32("Y"));

            return null;
            
        }
    }
}
