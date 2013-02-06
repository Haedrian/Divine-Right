using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects.Enums;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Divine_Right.InterfaceComponents.MainMenuComponents
{
    class SystemButton:
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
        /// <param name="drawArea">The rectangle to draw in</param>
        public SystemButton(string text, ContentManager content,InternalActionEnum action,Object[] args, Rectangle drawArea)
        {
            this.displayText = text;
            this.action = action;
            this.args = args;
            //determine where we're drawing
            SpriteFont font = content.Load<SpriteFont>(@"Fonts/ButtonTextFont");

            this.drawRect = drawArea;
        }

        #endregion

        #region Functions

        public void Draw(ContentManager content, SpriteBatch batch)
        {
            //draw a box
            batch.Draw(content.Load<Texture2D>("MarbleTexture"), drawRect, Color.White);

            SpriteFont font = content.Load<SpriteFont>(@"Fonts/ButtonTextFont");

            Vector2 stringSize = font.MeasureString(this.displayText);

            Vector2 stringDraw = new Vector2(drawRect.Center.X - (stringSize.X / 2), drawRect.Center.Y - (stringSize.Y / 2));

            batch.DrawString(content.Load<SpriteFont>(@"Fonts/ButtonTextFont"),this.displayText , stringDraw, Color.Black);
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
