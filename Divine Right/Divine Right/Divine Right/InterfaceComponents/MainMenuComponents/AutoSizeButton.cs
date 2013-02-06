using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Enums;

namespace Divine_Right.InterfaceComponents.MainMenuComponents
{
    public class AutoSizeButton: 
        ISystemInterfaceComponent
    {
        #region members

        Rectangle drawRect;
        string displayText;
        InternalActionEnum action;
        Object[] args;

        #endregion

        #region Constructor

        /// <summary>
        /// Creats a Main Menu Button
        /// </summary>
        /// <param name="text">The text to show</param>
        /// <param name="content">The content manager</param>
        /// <param name="action">The action to perform when clicked</param>
        /// <param name="args">Arguments to pass when clicked</param>
        /// <param name="centerX">The CENTRE of the button</param>
        /// <param name="centreY">The CENTRE of the button</param>
        public AutoSizeButton(string text, ContentManager content,InternalActionEnum action,Object[] args, int centerX, int centreY)
        {
            this.displayText = text;
            this.action = action;
            this.args = args;
            //determine where we're drawing
            SpriteFont font = content.Load<SpriteFont>(@"Fonts/ButtonTextFont");
            Vector2 fontVector = font.MeasureString(text);

            //since this is the centre, we need to build the rectangle around it

            this.drawRect = new Rectangle((int)(centerX - fontVector.X/2),(int)(centreY - fontVector.Y/2),(int)fontVector.X,(int)fontVector.Y);
        }

        #endregion

        #region Functions

        public void Draw(ContentManager content, SpriteBatch batch)
        {
            //draw a box
           // batch.Draw(content.Load<Texture2D>("MarbleTexture"), drawRect, Color.White);

            SpriteFont font = content.Load<SpriteFont>(@"Fonts/ButtonTextFont");

            Vector2 stringSize = font.MeasureString(this.displayText);

            Vector2 stringDraw = new Vector2(drawRect.Center.X - (stringSize.X / 2), drawRect.Center.Y - (stringSize.Y / 2));

            batch.DrawString(content.Load<SpriteFont>(@"Fonts/ButtonTextFont"),this.displayText , stringDraw, Color.White);
        }

        public bool HandleClick(int x, int y, out DRObjects.Enums.InternalActionEnum? instruction, out object[] args)
        {
            //We always handle a click by sending the instruction

            instruction = this.action;
            args = this.args;

            return true;
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return drawRect;
        }

        #endregion
    }
}
