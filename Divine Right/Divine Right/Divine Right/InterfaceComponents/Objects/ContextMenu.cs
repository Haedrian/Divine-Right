using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects.Enums;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DRObjects;

namespace Divine_Right.InterfaceComponents.Objects
{
    public class ContextMenu
    {
        #region Members
        private List<ContextMenuItem> contextMenuItems;
        private Rectangle drawRectangle;
        private MapCoordinate coordinate;
        #endregion


        #region Constructors

        /// <summary>
        /// Creates a new Context Menu starting at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ContextMenu(int x, int y,MapCoordinate coordinate)
        {
            this.contextMenuItems = new List<ContextMenuItem>();
            this.drawRectangle = new Rectangle();
            this.drawRectangle.X = x;
            this.drawRectangle.Y = y;
            this.coordinate = coordinate;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Adds a new Context Menu Item to the list from an actiontype and arguments
        /// </summary>
        public void AddContextMenuItem(ActionTypeEnum action, object[] args, ContentManager content)
        {
            ContextMenuItem item = new ContextMenuItem();
            item.Action = action;
            item.Args = args;

            Rectangle itemRect = new Rectangle();

            //where will this rectangle start?
            if (contextMenuItems.Count == 0)
            {
                itemRect.X = drawRectangle.X;
                itemRect.Y = drawRectangle.Y;

            }
            else
            {
                ContextMenuItem prev = this.contextMenuItems[contextMenuItems.Count - 1];
                //below the previous one
                itemRect.X = prev.Rect.X;
                itemRect.Y = prev.Rect.Y + prev.Rect.Height;
            }

            //determine the size of the text
            Vector2 fontVector = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont").MeasureString(item.Text);

            itemRect.Width = (int)fontVector.X;
            itemRect.Height = (int)fontVector.Y;

            //assign the rectangle
            item.Rect = itemRect;

            //add to the list
            contextMenuItems.Add(item);

            //update the draw rectangle

            if (drawRectangle.Width < itemRect.Width)
            {
                drawRectangle.Width = itemRect.Width;
            }

            //update the height
            drawRectangle.Height += itemRect.Height;

        }

        /// <summary>
        /// Draws the menu on the screen
        /// </summary>
        /// <param name="content"></param>
        /// <param name="batch"></param>
        public void DrawMenu(ContentManager content, SpriteBatch batch)
        {
            //first the background
            batch.Draw(content.Load<Texture2D>("Scroll"), this.drawRectangle, Color.White);

            //now the items
            foreach (ContextMenuItem item in contextMenuItems)
            {
                batch.DrawString(content.Load<SpriteFont>(@"Fonts/TextFeedbackFont"), item.Text, new Vector2(item.Rect.X, item.Rect.Y), Color.Blue);
            }

        }
        /// <summary>
        /// Handles the click - returns true or false depending on whether the click was within the context menu
        /// </summary>
        /// <param name="x">Mouse x</param>
        /// <param name="y">Mouse y</param>
        /// <param name="actionType">The type of action</param>
        /// <param name="args"></param>
        /// <param name="coord"></param>
        /// <returns></returns>
        public bool HandleClick(int x, int y,out ActionTypeEnum actionType, out object[] args, out MapCoordinate coord)
        {
            //check whether x and y was within a context menu item
            foreach (ContextMenuItem item in this.contextMenuItems)
            {
                if (item.Rect.Contains(new Point(x, y)))
                {
                    actionType = item.Action;
                    args = item.Args;
                    coord = this.coordinate;

                    return true;
                }

            }
            args = null;
            actionType = ActionTypeEnum.IDLE;
            coord = null;

            return false;
        }

        #endregion

     

    }
}
