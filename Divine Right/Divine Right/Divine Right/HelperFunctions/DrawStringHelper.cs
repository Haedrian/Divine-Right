using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Divine_Right.HelperFunctions
{
   public static class DrawStringHelper
    {
       public static void DrawString(this SpriteBatch batch, SpriteFont font, string text, Rectangle bounds, Alignment align, Color color)
       {
           Vector2 size = font.MeasureString(text);
           Point pos = bounds.Center;
           Vector2 origin = size * 0.5f;

           if (align.HasFlag(Alignment.Left))
               origin.X += bounds.Width / 2 - size.X / 2;

           if (align.HasFlag(Alignment.Right))
               origin.X -= bounds.Width / 2 - size.X / 2;

           if (align.HasFlag(Alignment.Top))
               origin.Y += bounds.Height / 2 - size.Y / 2;

           if (align.HasFlag(Alignment.Bottom))
               origin.Y -= bounds.Height / 2 - size.Y / 2;

           batch.DrawString(font, new StringBuilder(text), new Vector2(pos.X,pos.Y), color, 0f, origin, 1, SpriteEffects.None, 0);
       }

    }
}
