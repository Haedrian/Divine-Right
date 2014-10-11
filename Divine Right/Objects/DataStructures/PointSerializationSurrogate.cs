using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.Xna.Framework;

namespace DRObjects.DataStructures
{
    sealed class PointSerializationSurrogate:
        ISerializationSurrogate
    {

        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Point point = (Point)obj;

            info.AddValue("X", point.X);
            info.AddValue("Y", point.Y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            typeof(Point).GetField("X").SetValue(obj, info.GetString("X"));
            typeof(Point).GetField("Y").SetValue(obj, info.GetString("Y"));

            return null;
        }
    }
}
