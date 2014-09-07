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
using DRObjects.EventHandling;
using DRObjects;

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

        /// <summary>
        /// Creates a Decision Popup Component from a game event
        /// </summary>
        /// <param name="locationX"></param>
        /// <param name="locationY"></param>
        /// <param name="gameEvent"></param>
        public DecisionPopupComponent(int locationX, int locationY, GameEvent gameEvent)
        {
            this.locationX = locationX;
            this.locationY = locationY;

            this.decisions = gameEvent.EventChoices.Select(ec => new DecisionPopupChoice(ec.Text,ec.InternalAction,ec.Action,ec.Agrs)).ToArray();

            this.text = gameEvent.Text;
            this.title = gameEvent.Title;
            this.image = gameEvent.Image;

            this.visible = true;

            PerformDrag(0, 0);
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            var grey = SpriteManager.GetSprite(ColourSpriteName.WHITE);

            if (font == null)
            {
                font = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont");
            }

            //Background
            batch.Draw(content.Load<Texture2D>(grey.path), new Rectangle(0, 0, 5000, 5000), Color.Black*0.85f);

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
                batch.Draw(content, parchment, option.Rect, Color.White);
                batch.DrawString(font, option.Text, option.Rect, Alignment.Center, Color.Black);
            }

        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out InternalActionEnum? internalActionType, out object[] args, out MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            Point point = new Point(x, y);

            item = null;
            actionType = null;
            args = null;
            coord = null;
            destroy = false;
            internalActionType = null;

            if (!Visible)
            {
                return false;
            }

            //Did the user click on one of the choices?
            foreach (var decision in decisions)
            {
                if (decision.Rect.Contains(point))
                {
                    //Send the details pertaing to this decision
                    actionType = decision.ActionType;
                    internalActionType = decision.InternalAction;
                    args = decision.Args;
                    destroy = true; //User has made a choice, so close it

                    return true;
                }
            }

            //Not handled. But if it's modal, we expect it to catch all handling
            if (IsModal())
            {
                return true;
            }
            else
            {
                return false;
            }
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
            this.rect = new Rectangle(this.locationX, this.locationY, 300, 350 + (30 * this.decisions.Length));

            this.titleRect = new Rectangle(this.locationX + 10, this.locationY + 5, 280, 25);
            this.drawingRect = new Rectangle(this.locationX + 10, this.locationY + 30, 280, 200);
            this.textRect = new Rectangle(this.locationX + 10, this.locationY + 240, 280, 100);

            //the rectangles for the choices
            for (int i = 0; i < decisions.Length; i++)
            {
                DecisionPopupChoice choice = decisions[i];

                choice.Rect = new Rectangle(this.locationX, this.locationY + 350 + (i * 30), 300, 30);
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


        public void HandleMouseOver(int x, int y)
        {
          //TODO: ADD CONTEXTUAL INFORMATION
        }
    }
}
