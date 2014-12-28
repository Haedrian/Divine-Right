using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Graphics;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Enums;
using DRObjects;

namespace Divine_Right.InterfaceComponents.Components
{
    /// <summary>
    /// A Component which logs a number of text messages
    /// </summary>
    public class TextLogComponent
        : IGameInterfaceComponent
    {
        #region Properties
        protected int locationX;
        protected int locationY;

        private Rectangle rect;
        private SpriteFont font;

        private const int BIGSIZE = 200;
        private int bigSizeSwitch = 0;

        /// <summary>
        /// Cleaned feedback to display.
        /// We will basically take a look at the global log each time. If there's anything new, we clean it up and put it in the top.
        /// Then we try to draw all of them. If we run out of bottom draw space, then we trim the feedback list to save some memory.
        /// </summary>
        protected List<LogFeedback> feedback;

        /// <summary>
        /// A link to the log which we will be sampling from
        /// </summary>
        protected List<LogFeedback> globalLog;

        private bool visible;
        private bool bigMode;
        #endregion

        public TextLogComponent(int x, int y, List<LogFeedback> globalLog)
        {
            //Tall and wide (that's what she said)
            this.locationX = x;
            this.locationY = y;

            this.rect = new Rectangle(x, y, 750, 20);

            this.feedback = new List<LogFeedback>();
            this.globalLog = globalLog;

            bigMode = false;
        }

        /// <summary>
        /// Moves the TextLogComponent to a new location. This is intended to be used for resizing purposes
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(int x, int y)
        {
            locationX = x;
            locationY = y;

            if (bigMode)
            {
                this.rect.X = x;
                this.rect.Y = y - 200;
            }
            else
            {
                this.rect.X = x;
                this.rect.Y = y;
            }
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if (!visible)
            {
                return; //draw nothing
            }

            if (font == null)
            {
                //Load the font
                font = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont");
            }

            while (bigMode && bigSizeSwitch < BIGSIZE)
            {
                //Increment by 20
                bigSizeSwitch += 20;

                this.rect.Y -= 20;
                this.rect.Height += 20;
            }

            while (!bigMode && bigSizeSwitch > 0)
            {
                //Decrement by 20
                bigSizeSwitch -= 20;

                this.rect.Y += 20;
                this.rect.Height -= 20;
            }

            //Draw the background
            var scrollBackground = SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE);

            batch.Draw(content.Load<Texture2D>(scrollBackground.path), rect, scrollBackground.sourceRectangle, Color.White);

            //Now we start the drawing process

            Vector2 currentPosition = new Vector2(rect.X + 10, rect.Bottom - 20);

            for (int i = feedback.Count() - 1; i >= 0; i--)
            {
                var item = feedback[i];

                //Put the text in, and see how much space its going to take

                Vector2 v2 = font.MeasureString(item.Text);

                //Will it fit?

                if (currentPosition.Y < rect.Y)
                {
                    //Nope. Break
                    break;
                }

                //Put the text in
                batch.DrawString(font, item.Text, new Vector2(currentPosition.X + 30, currentPosition.Y), item.DrawColour);

                //Then we draw the icon - very small - call it 15x15 :)
                if (item.Icon.HasValue)
                {
                    //Draw it
                    batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(item.Icon.Value).path), new Rectangle((int)currentPosition.X, (int)currentPosition.Y, 15, 15), SpriteManager.GetSprite(item.Icon.Value).sourceRectangle, Color.Black);
                }

                //Update the current position
                currentPosition.Y -= (v2.Y + 5);
            }

        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionType? actionType,out InternalActionEnum? internalActionType, out object[] args,out MapItem itm, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            if (!visible)
            {
                itm = null;
                actionType = null;
                destroy = false;
                coord = null;
                args = null;
                internalActionType = null;
                return false;
            }

            //Check if we're in the middle of an animation cycle
            if (this.bigSizeSwitch == 0 || this.bigSizeSwitch == BIGSIZE) 
            {
                //toggle big mode
                this.bigMode = !this.bigMode;

                if (this.bigMode)
                {
                    //Expand rectangle by 200 pixels - gradually              

                    this.bigSizeSwitch = 0;

                    //  this.rect.Y -= 200;
                    // this.rect.Height += 200;
                }
                else
                {
                    this.bigSizeSwitch = 200;

                    // this.rect.Y += 200;
                    // this.rect.Height -= 200;
                }
            }

            actionType = null;
            destroy = false;
            coord = null;
            args = null;
            internalActionType = null;
            itm = null;
            return true;

        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionType? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            args = null;
            coord = null;
            destroy = false;
            return false;
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return rect;
        }

        public void PerformDrag(int deltaX, int deltaY)
        {
            return; //Do nothing
        }

        public bool IsModal()
        {
            return false;
        }

        public bool Visible
        {
            get
            {
                return visible;
            }
            set
            {
                visible = value;
            }
        }

        /// <summary>
        /// Updates the log with new items
        /// </summary>
        public void UpdateLog()
        {
            if (this.globalLog.Count() != 0)
            {
                this.feedback.AddRange(this.globalLog);

                this.globalLog.Clear(); //Clear it now that we're done

                //Do we have more than say 50 messages?

                if (this.feedback.Count > 50)
                {
                    //Remove 20 of them
                    this.feedback.RemoveRange(0, 20);
                }
            }
        }


        public void HandleMouseOver(int x, int y)
        {
            return;
        }
    }
}
