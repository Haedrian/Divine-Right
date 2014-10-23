using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DRObjects;
using Divine_Right.InterfaceComponents.Objects;
using Microsoft.Xna.Framework;
using DRObjects.Enums;
using Divine_Right.InterfaceComponents.Objects.Enums;
using DRObjects.Graphics;

namespace Divine_Right.InterfaceComponents.Components
{
    /// <summary>
    /// The component for a context menu which is caused when the user right clicks
    /// </summary>
    public class ContextMenuComponent
        : IGameInterfaceComponent
    {

        #region Members

        protected List<ContextMenuItem> contextMenuItems;
        protected Rectangle drawRectangle;
        protected MapCoordinate coordinate;
        private bool visible;

        #endregion

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }


        #region Constructor

        /// <summary>
        /// Creates a new Context Menu starting at x,y
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public ContextMenuComponent(int x, int y,MapCoordinate coordinate)
        {
            this.contextMenuItems = new List<ContextMenuItem>();
            this.drawRectangle = new Rectangle();
            this.drawRectangle.X = x;
            this.drawRectangle.Y = y;
            this.coordinate = coordinate;
            Visible = true;
        }


        #endregion

        #region Helper Functions
        /// <summary>
        /// Adds a new Context Menu Item to the list from an actiontype and arguments
        /// </summary>
        public void AddContextMenuItem(ActionType action, object[] args, ContentManager content)
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

        #endregion

        public void Draw(ContentManager content, SpriteBatch batch)
        {
            //first the background
            SpriteData sprite = SpriteManager.GetSprite(InterfaceSpriteName.SCROLL);

            Rectangle drawHere = new Rectangle(this.drawRectangle.X - 10, this.drawRectangle.Y - 25, this.drawRectangle.Width + 20, this.drawRectangle.Height + 40);

            batch.Draw(content.Load<Texture2D>(sprite.path), drawHere, sprite.sourceRectangle, Color.White);

            //now the items
            foreach (ContextMenuItem item in contextMenuItems)
            {
                batch.DrawString(content.Load<SpriteFont>(@"Fonts/TextFeedbackFont"), item.Text, new Vector2(item.Rect.X, item.Rect.Y), Color.Black);
            }
        }

        public bool HandleClick(int x, int y, MouseActionEnum mouseAction, out ActionType? actionType, out InternalActionEnum? internalActionType, out object[] args, out MapItem itm, out MapCoordinate coord, out bool destroy)
        {
            itm = null;
            internalActionType = null;

            //We only handle left clicks properly
            if (mouseAction.Equals(MouseActionEnum.LEFT_CLICK))
            {
                //check whether x and y was within a context menu item
                foreach (ContextMenuItem item in this.contextMenuItems)
                {
                    if (item.Rect.Contains(new Point(x, y)))
                    {
                        actionType = item.Action;
                        args = item.Args;
                        coord = this.coordinate;

                        //destroy the component
                        destroy = true;

                        return true;
                    }

                }
                args = null;
                actionType = null;
                coord = null;
                destroy = false;

                return false;
            }
            else
            {
                actionType = null;
                args = null;
                coord = null;
                //destroy it
                destroy = true;

                return false;
            }
        }
        public Rectangle ReturnLocation()
        {
            return drawRectangle;
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out ActionType? actionType, out object[] args, out MapCoordinate coord, out bool destroy)
        {
            //This component doesn't handle any keyboard
            actionType = null;
            args = null;
            coord = null;
            //But because we're moving, destroy it
            destroy = false;

            return false;
          }

        public bool IsModal()
        {
            return false;
        }


        public void PerformDrag(int x, int y)
        {
            return; //Do nothing
        }


        public void HandleMouseOver(int x, int y)
        {
            return; //Do nothing
        }
    }
}
