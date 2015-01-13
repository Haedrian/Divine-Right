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
using DivineRightGame;

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

        private string descriptionShown = String.Empty;

        private SpriteFont font;

        public LootComponent(int locationX, int locationY, TreasureChest treasureChest)
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

            for (int i = 0; i < itemRectangles.Count; i++)
            {
                Rectangle r = itemRectangles[i];

                MapItem mi = null;

                if (treasureChest.Contents.Count > i)
                {
                    mi = treasureChest.Contents[i];
                    batch.Draw(content, mi.Graphics[0], r, Color.White);
                }

            }


            batch.DrawString(font, descriptionShown, descriptionRect, Alignment.Left, Color.Black);
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


            if (this.crossRect.Contains(x, y))
            {
                //close it
                destroy = true;
                return true;
            }

            if (this.takeAllRect.Contains(x, y))
            {
                //Take it, take it all!
                foreach (var i in this.treasureChest.Contents)
                {
                    //Do we have an item with the same name in the inventory?
                    var oldItem = GameState.PlayerCharacter.Inventory.Inventory.GetObjectsByGroup(i.Category).Where(g => g.Name.Equals(i.Name)).FirstOrDefault();

                    if (oldItem != null)
                    {
                        //Instead we increment the total in that item in the inventory
                        oldItem.TotalAmount++;
                    }
                    else
                    {
                        GameState.PlayerCharacter.Inventory.Inventory.Add(i.Category, i);
                    }

                }
                //Remove them
                this.treasureChest.Contents = new List<InventoryItem>();

                destroy = true; //and close it
                return true;
            }

            for (int i = 0; i < this.itemRectangles.Count; i++)
            {
                if (this.itemRectangles[i].Contains(x, y))
                {
                    //Overlap! Put in the description

                    if (this.treasureChest.Contents.Count > i)
                    {
                        InventoryItem inv = this.treasureChest.Contents[i] as InventoryItem;

                        //take it!
                        GameState.PlayerCharacter.Inventory.Inventory.Add(inv.Category, inv);

                        //Remove it
                        this.treasureChest.Contents.RemoveAt(i);

                        if (this.treasureChest.Contents.Count == 0)
                        {
                            //Cleared it out. Close it
                            destroy = true;
                        }
                    }
                    return true;
                }
            }


            return true;

        }

        public void HandleMouseOver(int x, int y)
        {
            descriptionShown = String.Empty;

            for (int i = 0; i < this.itemRectangles.Count; i++)
            {
                if (this.itemRectangles[i].Contains(x, y))
                {
                    //Overlap! Put in the description

                    if (this.treasureChest.Contents.Count > i)
                    {
                        descriptionShown = (this.treasureChest.Contents[i] as InventoryItem).Description;
                    }
                    return;
                }
            }
        }

        public bool HandleKeyboard(KeyboardState keyboard, out DRObjects.Enums.ActionType? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            args = null;
            coord = null;
            destroy = false; //If the user moves, destroy it

            if (keyboard.GetPressedKeys().Contains(Keys.Left) || keyboard.GetPressedKeys().Contains(Keys.Right) || keyboard.GetPressedKeys().Contains(Keys.Down) || keyboard.GetPressedKeys().Contains(Keys.Up))
            {
                destroy = true;
            }

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

            rect = new Rectangle(locationX, locationY, 250, 140);
            borderRect = new Rectangle(locationX - 2, locationY - 2, rect.Width + 4, rect.Height + 4);

            objectNameRect = new Rectangle(locationX, locationY, rect.Width, 20);
            crossRect = new Rectangle(locationX + rect.Width - 20, locationY, 20, 20);

            itemRectangles = new List<Rectangle>();

            for (int i = 0; i < 14; i++)
            {
                Rectangle ir = new Rectangle(
                    locationX + 10 + (i % 7) * (30),
                    locationY + 30 + (((int)i / 7) * 30),
                    30,
                    30);

                itemRectangles.Add(ir);
            }

            descriptionRect = new Rectangle(locationX, locationY + 90, rect.Width, 20);
            takeAllRect = new Rectangle(locationX, locationY + 120, rect.Width, 20);
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
