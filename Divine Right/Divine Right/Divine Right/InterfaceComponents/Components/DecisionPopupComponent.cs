using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Divine_Right.InterfaceComponents.Objects;
using DRObjects.Graphics;
using Divine_Right.HelperFunctions;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Enums;

namespace Divine_Right.InterfaceComponents.Components
{
    public class DecisionPopupComponent
        : IGameInterfaceComponent
    {
        #region Members

        private int locationX;
        private int locationY;
        private string text;
        private string title;
        private Rectangle rect;
        private DecisionPopupChoice[] decisions;

        private Rectangle titleRect;
        private Rectangle drawingRect;
        private Rectangle textRect;
        private SpriteData image;

        private SpriteFont font;

        private bool visible;

        #endregion

        public DecisionPopupComponent(int locationX, int locationY, string title, string text, SpriteData image, DecisionPopupChoice[] decisions)
        {
            this.locationX = locationX;
            this.locationY = locationY;
            this.decisions = decisions;
            this.title = title;
            this.text = text;
            this.image = image;
            this.visible = true;


            PerformDrag(0, 0); //Trigger the drag code
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if (font == null)
            {
                font = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont");
            }

            //Get the partchment
            var parchment = SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE);

            //Draw
            batch.Draw(content, parchment, rect, Color.White);

            //Draw the values
            batch.Draw(content, image, drawingRect, Color.White);

            //The title
            batch.DrawString(font, title, titleRect, Alignment.Center, Color.Black);

            //Put in the text
            batch.DrawString(font, text, textRect, Alignment.Center, Color.Black);

            //Draw the options 
            foreach (var option in decisions)
            {
                batch.Draw(content, parchment, option.rect, Color.White);
                batch.DrawString(font, option.Text, option.rect, Alignment.Center, Color.Black);
            }

        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            args = null;
            coord = null;
            destroy = false;
            internalActionType = null;

            return true;
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            args = null;
            coord = null;
            destroy = false;

            return true;
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return rect;
        }

        public void PerformDrag(int deltaX, int deltaY)
        {
            this.locationX += 0;
            this.locationY += 0;

            //Create a rectangle. It'll be 300 x 200 + 30for every option
            this.rect = new Rectangle(this.locationX, this.locationY, 300, 300 + (30 * this.decisions.Length));

            this.titleRect = new Rectangle(this.locationX + 10, this.locationY + 5, 280, 25);
            this.drawingRect = new Rectangle(this.locationX + 10, this.locationY + 30, 280, 200);
            this.textRect = new Rectangle(this.locationX + 10, this.locationY + 220, 280, 50);

            //the rectangles for the choices
            for (int i = 0; i < decisions.Length; i++)
            {
                DecisionPopupChoice choice = decisions[i];

                choice.rect = new Rectangle(this.locationX, this.locationY + 300 + (i * 30), 300, 30);
            }
        }

        public bool IsModal()
        {
            return true; //Modal
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                this.visible = value;
            }
        }
    }
}
