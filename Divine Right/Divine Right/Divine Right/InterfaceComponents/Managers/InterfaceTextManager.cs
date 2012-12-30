using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Divine_Right.InterfaceComponents.Objects;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace Divine_Right.InterfaceComponents.Managers
{
    public static class InterfaceTextManager
    {
        public static void DrawTextFeedback(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice device, List<InterfaceTextFeedback> feedbackList)
        {
            for (int i = 0; i < feedbackList.Count; i++)
            {
                //check whether we show them or destroy them
                InterfaceTextFeedback feedback = feedbackList[i];

                if (feedback.TimeToDestroy > DateTime.Now)
                {
                    //display it
                    DrawTextFeedback(spriteBatch, content,device, feedback);
                }
                else
                {
                    //destroy it
                    feedbackList.RemoveAt(i);
                }

            }

        }

        public static void DrawTextFeedback(SpriteBatch spriteBatch, ContentManager content, GraphicsDevice device, InterfaceTextFeedback feedback)
        {
            Vector2 fontVector = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont").MeasureString(feedback.Text);
            Vector2 locationVector = new Microsoft.Xna.Framework.Vector2(feedback.InterfaceX, feedback.InterfaceY);

            Rectangle box = new Rectangle((int) locationVector.X-5, (int) locationVector.Y, (int) fontVector.X + 10, (int) fontVector.Y);

//            Texture2D defTex = new Texture2D(device, 1, 1);
 //           defTex.SetData(new[] { Color.White});

            spriteBatch.Draw(content.Load<Texture2D>("Scroll"), box, Color.White);
            spriteBatch.DrawString(content.Load<SpriteFont>(@"Fonts/TextFeedbackFont"),feedback.Text,locationVector,Color.Black);
        }

    }
}
