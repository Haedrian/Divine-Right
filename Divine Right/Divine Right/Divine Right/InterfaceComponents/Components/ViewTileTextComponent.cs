using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Divine_Right.InterfaceComponents.Objects.Enums;

namespace Divine_Right.InterfaceComponents.Components
{
    /// <summary>
    /// A component which shows some text describing a tile
    /// </summary>
    class ViewTileTextComponent: 
        IGameInterfaceComponent
    {
        #region Constants
        /// <summary>
        /// How long this component will last in seconds before it disappears
        /// </summary>
        protected const int LIFETIME = 2;
        #endregion

        #region Members


        #endregion

        #region Properties

        protected int locationX;
        protected int locationY;
        DateTime destroyTime;
        protected Rectangle componentRectangle;

        protected string text;


        #endregion

        #region Constructor

        /// <summary>
        /// Creates a tile text component displaying particular text
        /// </summary>
        /// <param name="displayText"></param>
        public ViewTileTextComponent(int x, int y,string displayText)
        {
            this.locationX = x;
            this.locationY = y;
            this.text = displayText;

            destroyTime = DateTime.Now.AddSeconds(LIFETIME);
        }

        #endregion

        void IGameInterfaceComponent.Draw(ContentManager content, SpriteBatch batch)
        {
            if (destroyTime < DateTime.Now)
            {
                //destroy
                this.componentRectangle = new Rectangle(0, 0, 0, 0); //collapse

                //don't draw it
            }
            else
            {
                Vector2 fontVector = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont").MeasureString(text);
                Vector2 locationVector = new Microsoft.Xna.Framework.Vector2(locationX, locationY);

                Rectangle box = new Rectangle((int)locationVector.X, (int)locationVector.Y, (int)fontVector.X + 20, (int)fontVector.Y);

                Vector2 fontDrawVector = new Vector2(locationX + 10, locationY);

                batch.Draw(content.Load<Texture2D>("Scroll"), box, Color.White);
                batch.DrawString(content.Load<SpriteFont>(@"Fonts/TextFeedbackFont"), text, fontDrawVector, Color.Black);

                this.componentRectangle = box;
            }
        }

        bool IGameInterfaceComponent.HandleClick(int x, int y, MouseActionEnum mouse, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            //This component will 'absorb' the click, but do nothing whatsoever.
            actionType = null;
            args = null;
            coord = null;
            destroy = false;

            return true;

        }

        Rectangle IGameInterfaceComponent.ReturnLocation()
        {
            return componentRectangle;
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            //we don't handle the keyboard
            actionType = null;
            args = null;
            coord = null;
            destroy = false;
            
            return false;
        }

        public bool IsModal()
        {
            return false;
        }
    }
}
