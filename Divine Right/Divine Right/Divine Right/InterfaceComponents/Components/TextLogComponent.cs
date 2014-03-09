using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Graphics;
using Microsoft.Xna.Framework.Graphics;

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

        //Sorry :( Need to do this for a really ugly hack which involves resizing it to fit the bottom of the screen
        public Rectangle rect;
        private SpriteFont font;

        /// <summary>
        /// Cleaned feedback to display.
        /// We will basically take a look at the global log each time. If there's anything new, we clean it up and put it in the top.
        /// Then we try to draw all of them. If we run out of bottom draw space, then we trim the feedback list to save some memory.
        /// </summary>
        protected List<CurrentLogFeedback> feedback;

        /// <summary>
        /// A link to the log which we will be sampling from
        /// </summary>
        protected List<CurrentLogFeedback> globalLog;

        private bool visible;
        #endregion

        public TextLogComponent(int x, int y,List<CurrentLogFeedback> globalLog)
        {
            //Tall and wide (that's what she said)
            this.locationX = x;
            this.locationY = y;

            this.rect = new Rectangle(x, y, 750, 120);

            this.feedback = new List<CurrentLogFeedback>();
            this.globalLog = globalLog;

            //test data
            feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.SWORD,Color.Black, "Orc attacks your chest with his mace (7) and hits!"));
            feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.BLOOD,Color.DarkRed, "You start bleeding. Oh dear."));
            feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.HEAD,Color.ForestGreen, "Your skill in Being Awesome has increased."));
            feedback.Add(new CurrentLogFeedback(InterfaceSpriteName.DEFENSE,Color.Black, "The Orc swings at your chest (4), but you dodge away"));
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

            //Draw the background
            var scrollBackground = SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE);

            batch.Draw(content.Load<Texture2D>(scrollBackground.path), rect, scrollBackground.sourceRectangle, Color.White);

            //Now we start the drawing process

            Vector2 currentPosition = new Vector2(rect.X + 10, rect.Y + 10);

            for(int i=feedback.Count()-1; i >= 0; i--)
            {
                var item = feedback[i];

                //Put the text in, and see how much space its going to take

                Vector2 v2 = font.MeasureString(item.Text);

                //Will it fit?

                if (v2.Y + currentPosition.Y > rect.Bottom)
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
                    batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(item.Icon.Value).path), new Rectangle((int)currentPosition.X,(int)currentPosition.Y, 15, 15), SpriteManager.GetSprite(item.Icon.Value).sourceRectangle, Color.Black);
                }

                //Update the current position
                currentPosition.Y += v2.Y + 5;
            }

        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            if (!visible)
            {
                actionType = null;
                destroy = false;
                coord = null;
                args = null;
                return false;
            }

            throw new NotImplementedException();
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
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
            return; //do later
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
        /// Updates the log with new items, and cleans them up
        /// </summary>
        private void UpdateLog()
        {

        }
    }
}
