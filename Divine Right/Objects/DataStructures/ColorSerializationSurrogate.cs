using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;

namespace DRObjects.DataStructures
{
    public sealed class ColorSerializationSurrogate:
        ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Color colour = (Color)obj;

            info.AddValue("A", colour.A);
            info.AddValue("B", colour.B);
            info.AddValue("G", colour.G);
            //info.AddValue("PackedValue", colour.PackedValue);
            info.AddValue("R", colour.R);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            typeof(Color).GetProperty("A").SetValue(obj, info.GetByte("A"),null);
            typeof(Color).GetProperty("B").SetValue(obj, info.GetByte("B"), null);
            typeof(Color).GetProperty("G").SetValue(obj, info.GetByte("G"), null);
            typeof(Color).GetProperty("R").SetValue(obj, info.GetByte("R"), null);

            return null;
        }
    }
}
