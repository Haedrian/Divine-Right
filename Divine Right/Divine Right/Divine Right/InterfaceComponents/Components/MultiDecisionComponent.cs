using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Divine_Right.InterfaceComponents.Objects;
using DRObjects.Graphics;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.EventHandling;
using Divine_Right.HelperFunctions;
using DRObjects.Enums;
using DRObjects;
using DRObjects.EventHandling.MultiEvents;

namespace Divine_Right.InterfaceComponents.Components
{
    public class MultiDecisionComponent
        : IGameInterfaceComponent
    {
        #region Members

        private int locationX;
        private int locationY;
      
        private Rectangle rect;

        private Rectangle titleRect;
        private Rectangle drawingRect;
        private Rectangle textRect;
        private SpriteData image;

        private SpriteFont font;

        private bool visible;

        /// <summary>
        /// The choices that have been made
        /// </summary>
        private List<string> choicesMade = new List<string>();

        private GameMultiEvent currentEvent;

        #endregion

        /// <summary>
        /// Creates a Decision Popup Component from a game event
        /// </summary>
        /// <param name="locationX"></param>
        /// <param name="locationY"></param>
        /// <param name="gameEvent"></param>
        public MultiDecisionComponent(int locationX, int locationY, GameMultiEvent gameEvent)
        {
            this.locationX = locationX;
            this.locationY = locationY;

            this.visible = true;

            this.currentEvent = gameEvent;

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
            batch.Draw(content, this.currentEvent.Image, drawingRect, Color.White);

            //The title
            batch.DrawString(font, this.currentEvent.Title, titleRect, Alignment.Center, Color.Black);

            //Put in the text
            batch.DrawString(font, this.currentEvent.Text, textRect, Alignment.Center, Color.Black);

            //Draw the options 
            foreach (var option in this.currentEvent.Choices)
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

            if (mouseAction != Objects.Enums.MouseActionEnum.LEFT_CLICK)
            {
                return true;
            }

            //Did the user click on one of the choices?
            foreach (var decision in this.currentEvent.Choices)
            {
                if (decision.Rect.Contains(point))
                {
                    //Decision has been made
                    choicesMade.Add(decision.ChoiceName);

                    //Do we have a next choice?
                    if (decision.NextChoice != null)
                    {
                        //Change the current choice
                        this.currentEvent = decision.NextChoice;
                        this.PerformDrag(0, 0); //force recreation
                    }
                    else
                    {
                        //terminate! Send back that its a multidecision, and the event name and the choices made
                        actionType = ActionTypeEnum.MULTIDECISION;
                        args = new object[] { this.currentEvent.EventName, this.choicesMade };
                        destroy = true;
                    }
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

            //Create a rectangle. It'll be 500 x 200 + 30for every option
            this.rect = new Rectangle(this.locationX, this.locationY, 500, 350 + (30 * this.currentEvent.Choices.Length));

            this.titleRect = new Rectangle(this.locationX + 10, this.locationY + 5, rect.Width - 20, 25);
            this.drawingRect = new Rectangle(this.locationX + 10, this.locationY + 30, rect.Width - 20, 200);
            this.textRect = new Rectangle(this.locationX + 10, this.locationY + 240, rect.Width -20, 100);

            //the rectangles for the choices
            for (int i = 0; i < this.currentEvent.Choices.Length; i++)
            {
                MultiEventChoice choice = this.currentEvent.Choices[i];

                choice.Rect = new Rectangle(this.locationX, this.locationY + 350 + (i * 30), rect.Width, 30);
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
