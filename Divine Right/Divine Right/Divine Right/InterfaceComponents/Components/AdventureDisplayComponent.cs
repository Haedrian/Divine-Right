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
using DRObjects.Enums;

namespace Divine_Right.InterfaceComponents.Components
{
    /// <summary>
    /// Displays the date. And time.
    /// And how their tummy is feeling
    /// </summary>
    public class AdventureDisplayComponent:
     IGameInterfaceComponent
    {
        protected int locationX;
        protected int locationY;

        private Rectangle borderRect;

        private SpriteFont font;
        private Rectangle drawRect;
        private Rectangle dateRect;
        private Rectangle timeRect;

        private Rectangle[] timePositionRects;
        private Rectangle hungerRect;

        public AdventureDisplayComponent(int x, int y)
        {
            locationX = x;
            locationY = y;

            PerformDrag(0, 0);
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if (!GameState.LocalMap.IsGlobalMap)
            {
                return;
            }

            var white = SpriteManager.GetSprite(ColourSpriteName.WHITE);

            batch.Draw(content, white, borderRect, Color.DarkGray);

            var woodBackground = SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE);

            batch.Draw(content, woodBackground, drawRect, Color.White);

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

            if ((int)GameState.PlayerCharacter.FeedingLevel == 1)
            {
                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.HUNGRY), hungerRect, Color.DarkRed);
            }
            else if ((int) GameState.PlayerCharacter.FeedingLevel == 2 )
            {
                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.HUNGRY), hungerRect, Color.Red);
            }
        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionType? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
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
            return drawRect;
        }

        public void PerformDrag(int deltaX, int deltaY)
        {
            this.locationX += deltaX;
            this.locationY += deltaY;

            this.drawRect = new Rectangle(locationX, locationY, 150, 100);

            this.borderRect = new Rectangle(this.drawRect.X- 2, this.drawRect.Y - 2,  this.drawRect.Width + 4, this.drawRect.Height + 4);
            this.dateRect = new Rectangle(locationX, locationY+40, 150, 30);
            this.timeRect = new Rectangle(locationX, locationY, 150, 40);

            this.timePositionRects = new Rectangle[5] 
            {
                new Rectangle(locationX +0,locationY + 20,20,20),
                new Rectangle(locationX +30,locationY + 10,20,20),
                new Rectangle(locationX + 60, locationY,20,20),
                new Rectangle(locationX + 90,locationY + 10, 20,20),
                new Rectangle(locationX + 120,locationY + 20,20,20)
            };
            this.hungerRect = new Rectangle(locationX, locationY + 70, 30, 30);
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
