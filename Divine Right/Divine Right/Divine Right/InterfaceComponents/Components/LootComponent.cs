using DRObjects.Items.Archetypes.Local;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Divine_Right.HelperFunctions;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Graphics;
using DRObjects;

namespace Divine_Right.InterfaceComponents.Components
{
    public class LootComponent
        : IGameInterfaceComponent
    {
        private int locationX;
        private int locationY;
        private TreasureChest treasureChest;

        private Rectangle rect;
        private Rectangle borderRect;

        private Rectangle objectNameRect;
        private Rectangle crossRect;
        private List<Rectangle> itemRectangles = new List<Rectangle>();
        private Rectangle descriptionRect;
        private Rectangle takeAllRect;

        private SpriteFont font;

        public LootComponent(int locationX, int locationY,TreasureChest treasureChest)
        {
            this.treasureChest = treasureChest;
            this.locationX = locationX;
            this.locationY = locationY;

            PerformDrag(locationX, locationY);
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if (font == null)
            {
                //Load the font
                font = content.Load<SpriteFont>(@"Fonts/LightText");
            }

            batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.WHITE), borderRect, Color.DarkGray);
            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), rect, Color.White);

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE), objectNameRect, Color.WhiteSmoke);
            batch.DrawString(font, treasureChest.Name, objectNameRect, Alignment.Center, Color.Black);

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.CLOSE), crossRect, Color.White);

            for (int i = 0; i < itemRectangles.Count; i++ )
            {
                Rectangle r = itemRectangles[i];

                MapItem mi = null;

                if (treasureChest.Contents.Count > i)
                {
                    mi = treasureChest.Contents[i];
                    batch.Draw(content, mi.Graphics[0], r, Color.White);
                }

            }


            batch.DrawString(font, "Description Bla", descriptionRect, Alignment.Center, Color.Black);
            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE), takeAllRect, Color.White);
            batch.DrawString(font, "TAKE ALL", takeAllRect, Alignment.Center, Color.Black);
        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionType? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            args = null;
            coord = null;
            destroy = false;
            internalActionType = null;
            item = null;
            //TODO
  
            return true;
       
        }

        public void HandleMouseOver(int x, int y)
        {
            //TODO
        }

        public bool HandleKeyboard(KeyboardState keyboard, out DRObjects.Enums.ActionType? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            args = null;
            coord = null;
            destroy = false; //If the user moves, destroy it

            return false;
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return rect;
        }

        public void PerformDrag(int deltaX, int deltaY)
        {
            locationX += deltaX;
            locationY += deltaY;

            rect = new Rectangle(locationX, locationY, 170, 170);
            borderRect = new Rectangle(locationX - 2, locationY - 2, rect.Width + 4, rect.Height + 4);

            objectNameRect = new Rectangle(locationX, locationY, 170, 20);
            crossRect = new Rectangle(locationX + 150, locationY, 20, 20);

            itemRectangles = new List<Rectangle>();

            for(int i=0; i < 15; i++)
            {
                int dummy;
                Rectangle ir = new Rectangle(
                    locationX + 10 + (i % 5)*(30),
                    locationY + 30 + (((int)i/5)*30),
                    30,
                    30);

                itemRectangles.Add(ir);
            }

            descriptionRect = new Rectangle(locationX, locationY + 130, rect.Width, 20);
            takeAllRect = new Rectangle(locationX, locationY + 150, rect.Width, 20);
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
                throw new NotImplementedException();
            }
        }
    }
}
