using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DRObjects.Graphics;
using Divine_Right.HelperFunctions;
using DivineRightGame;
using DRObjects.DataStructures.Enum;

namespace Divine_Right.InterfaceComponents.Components
{
    /// <summary>
    /// Displays the date. And time.
    /// </summary>
    public class TimeDisplayComponent:
     IGameInterfaceComponent
    {
        protected int locationX;
        protected int locationY;

        private SpriteFont font;
        private Rectangle rect;
        private Rectangle dateRect;
        private Rectangle timeRect;

        private Rectangle[] timePositionRects;

        public TimeDisplayComponent(int x, int y)
        {
            locationX = x;
            locationY = y;

            PerformDrag(0, 0);
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            var woodBackground = SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE);

            batch.Draw(content, woodBackground, rect, Color.White);

            if (font == null)
            {
                font = content.Load<SpriteFont>(@"Fonts/LightText");
            }

            //Draw the text
            batch.DrawString(font, GameState.UniverseTime.GetDateString(), dateRect, Alignment.Center, Color.Black);

            //Day or night? And put in right position

            if (GameState.UniverseTime.GetTimeComponent(DRTimeComponent.HOUR) >= 5)
            {
                //Night
                batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.MARBLEBLUE), timeRect, Color.DarkBlue);
                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.MOON), timePositionRects[GameState.UniverseTime.GetTimeComponent(DRTimeComponent.HOUR) - 5], Color.White);
            }
            else
            {
                //Day
                batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.MARBLEBLUE), timeRect, Color.LightSkyBlue);
                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.SUN), timePositionRects[GameState.UniverseTime.GetTimeComponent(DRTimeComponent.HOUR)], Color.White);
            }

            //Put the sun in the right position
            //foreach(var r in timePositionRects)
            //{
            //    batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.MOON), r, Color.White);
            //}
        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            internalActionType = null;
            args = null;
            item = null;
            coord = null;
            destroy = false;
            return true; 
        }

        public void HandleMouseOver(int x, int y)
        {
            //Do nothing
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
            this.locationX += deltaX;
            this.locationY += deltaY;

            this.rect = new Rectangle(locationX, locationY, 200, 100);
            this.dateRect = new Rectangle(locationX, locationY+70, 200, 30);
            this.timeRect = new Rectangle(locationX, locationY, 200, 70);

            this.timePositionRects = new Rectangle[5] 
            {
                new Rectangle(locationX +0,locationY + 40,30,30),
                new Rectangle(locationX +43,locationY + 20,30,30),
                new Rectangle(locationX + 86, locationY,30,30),
                new Rectangle(locationX + 129,locationY + 20, 30,30),
                new Rectangle(locationX + 170,locationY + 40,30,30)
            };
        }

        public bool IsModal()
        {
            return false;
        }

        public bool Visible
        {
            get
            {
                return true;
            }
            set
            {
                //dummy
            }
        }
    }
}
