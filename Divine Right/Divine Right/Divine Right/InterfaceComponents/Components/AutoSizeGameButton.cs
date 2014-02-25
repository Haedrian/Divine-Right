using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Divine_Right.InterfaceComponents.MainMenuComponents;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using DRObjects.Enums;
using DRObjects.Graphics;

namespace Divine_Right.InterfaceComponents.Components
{
    /// <summary>
    /// An autosize button for use in Playable Interface. With a scroll background.
    /// And smaller font
    /// </summary>
    public class AutoSizeGameButton:
        AutoSizeButton
    {

        public AutoSizeGameButton(string text, ContentManager content, InternalActionEnum action, Object[] args, int centerX, int centreY)
        {
            this.displayText = text;
            this.action = action;
            this.args = args;
            //determine where we're drawing
            SpriteFont font = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont");
            Vector2 fontVector = font.MeasureString(text);

            //since this is the centre, we need to build the rectangle around it

            this.drawRect = new Rectangle((int)(centerX - fontVector.X / 2), (int)(centreY - fontVector.Y / 2), (int)fontVector.X, (int)fontVector.Y);
        }

        public override void Draw(ContentManager content, SpriteBatch batch)
        {
            //draw a box
           batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE).path), drawRect, Color.White);

           SpriteFont font = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont");

            Vector2 stringSize = font.MeasureString(this.displayText);

            Vector2 stringDraw = new Vector2(drawRect.Center.X - (stringSize.X / 2), drawRect.Center.Y - (stringSize.Y / 2));

            batch.DrawString(content.Load<SpriteFont>(@"Fonts/TextFeedbackFont"), this.displayText, stringDraw, Color.Black);
        }

    }
}
