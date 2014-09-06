using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using Microsoft.Xna.Framework;
using Divine_Right.HelperFunctions;
using DRObjects.Graphics;
using DivineRightGame.ItemFactory.Object;
using DRObjects.Enums;
using Microsoft.Xna.Framework.Graphics;
using Divine_Right.InterfaceComponents.Objects;

namespace Divine_Right.InterfaceComponents.Components
{
    public class InventoryDisplayComponent :
        IGameInterfaceComponent
    {
        public int ChosenCategory { get; set; }
        System.Array enums = Enum.GetValues(typeof(InventoryCategory));

        private const int ROW_TOTAL = 12;

        private bool visible = true;

        protected int locationX;
        protected int locationY;
        public Actor CurrentActor { get; set; }

        private Rectangle rect;
        private Rectangle borderRect;

        private Rectangle categoryBackground;

        private List<Rectangle> categories;
        private List<Rectangle> categoryBackgrounds;

        private List<InventoryItemRectangle> row1Items;
        private List<InventoryItemRectangle> row2Items;
        private List<InventoryItemRectangle> row3Items;

        private string detailsToShow = String.Empty;
        private Rectangle detailsRect;
        private SpriteFont font;

        //Drawing stuff
        public InventoryDisplayComponent(int locationX, int locationY, Actor currentActor)
        {
            this.locationX = locationX;
            this.locationY = locationY;
            this.CurrentActor = currentActor;
            ChosenCategory = 0;

            PerformDrag(locationX, locationY);
        }


        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if (!Visible)
            {
                return;
            }

            batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.WHITE), borderRect, Color.DarkGray);

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), rect, Color.White);

            for (int i = 0; i < enums.Length; i++)
            {
                SpriteData sprite = null;

                switch ((InventoryCategory)enums.GetValue(i))
                {
                    case InventoryCategory.ARMOUR:
                        sprite = SpriteManager.GetSprite(LocalSpriteName.LIGHT_BREASTPLATE);
                        break;
                    case InventoryCategory.LOOT:
                        sprite = SpriteManager.GetSprite(LocalSpriteName.GREY_GEM_RING);
                        break;
                    case InventoryCategory.WEAPON:
                        sprite = SpriteManager.GetSprite(LocalSpriteName.SWORD_3);
                        break;
                    default:
                        throw new NotImplementedException("No code for that particular inventory category");
                }


                if (this.ChosenCategory != i)
                {
                    //Normal background
                    batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE), categoryBackgrounds[i], Color.White);
                }
                else
                {
                    //Darker background
                    batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE), categoryBackgrounds[i], Color.RosyBrown);
                }
                batch.Draw(content, sprite, categories[i], Color.Black);

                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE), categoryBackground, Color.White);
            }

            //Now draw the items
            var inventoryitems = this.CurrentActor.Inventory.GetObjectsByGroup(enums.GetValue(ChosenCategory)).ToArray();

            //Clear all the items held in the boxes
            foreach (var item in row1Items)
            {
                item.Item = null;
            }
            foreach (var item in row2Items)
            {
                item.Item = null;
            }
            foreach (var item in row3Items)
            {
                item.Item = null;
            }

            for (int i = 0; i < inventoryitems.Count(); i++)
            {
                int column = i % ROW_TOTAL;
                int row = (int) Math.Floor((double) i / (double) ROW_TOTAL);

                if (row == 0)
                {
                    //Also assign the item
                    row1Items[column].Item = inventoryitems[i];
                    batch.Draw(content, inventoryitems[i].Graphic, row1Items[column].Rect, Color.White);
                }
                else if (row == 1)
                {
                    row2Items[column].Item = inventoryitems[i];
                    batch.Draw(content, inventoryitems[i].Graphic, row2Items[column].Rect, Color.White);
                }
                else
                {
                    row3Items[column].Item = inventoryitems[i];
                    batch.Draw(content, inventoryitems[i].Graphic, row3Items[column].Rect, Color.White);
                }
            }

            //Draw any details 
            if (font == null)
            {
                //Load the font
                font = content.Load<SpriteFont>(@"Fonts/LightText");
            }

            batch.DrawString(font, detailsToShow, detailsRect, Alignment.Left, Color.Black);

        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            for (int i=0; i < categories.Count; i++)
            {
                if (categories[i].Contains(x, y))
                {
                    //Change category!
                    this.ChosenCategory = i;

                    //Handled. Naught else
                    actionType = null;
                    internalActionType = null;
                    args = null;
                    coord = null;
                    destroy = false;

                    return true;
                }
            }

            actionType = null;
            internalActionType = null;
            args = null;
            coord = null;
            destroy = false;

            return true;
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
            //Update locationX and Y
            locationX += deltaX;
            locationY += deltaY;

            rect = new Rectangle(locationX, locationY, 360, 190);
            borderRect = new Rectangle(locationX - 2, locationY - 2, rect.Width + 4, rect.Height + 4);

            categoryBackground = new Rectangle(locationX, locationY + 50, rect.Width, 5);

            categoryBackgrounds = new List<Rectangle>();
            categories = new List<Rectangle>();

            for (int i = 0; i < enums.Length; i++)
            {
                categoryBackgrounds.Add(new Rectangle(locationX + (50 * i), locationY, 50, 50));
                categories.Add(new Rectangle(locationX + (50 * i), locationY, 50, 50));
            }

            row1Items = new List<InventoryItemRectangle>();
            row2Items = new List<InventoryItemRectangle>();
            row3Items = new List<InventoryItemRectangle>();

            for (int i = 0; i < ROW_TOTAL; i++)
            {
                row1Items.Add(new InventoryItemRectangle{ Rect = new Rectangle((locationX + (30 * i)), locationY +10 + 50, 30, 30) });
                row2Items.Add(new InventoryItemRectangle { Rect = new Rectangle((locationX + (30 * i)), locationY +10 + 80, 30, 30)});
                row3Items.Add(new InventoryItemRectangle { Rect = new Rectangle((locationX + (30 * i)), locationY + 10 + 110, 30, 30) });
            }

            detailsRect = new Rectangle(locationX, locationY + 150, rect.Width, 50);
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
                this.visible = value;
            }
        }


        public void HandleMouseOver(int x, int y)
        {
            //Clear it
            detailsToShow = String.Empty;

            List<InventoryItemRectangle> allItemBoxes = new List<InventoryItemRectangle>();
            allItemBoxes.AddRange(row1Items);
            allItemBoxes.AddRange(row2Items);
            allItemBoxes.AddRange(row3Items);

            foreach(InventoryItemRectangle rect in allItemBoxes)
            {
                if (rect.Rect.Contains(x, y))
                {
                  
                    //Does it contain an item?
                    if (rect.Item != null)
                    {
                        //Yes
                        detailsToShow = rect.Item.Description;
                    }
                    else
                    {
                        //User is mousing over an empty box
                        detailsToShow = String.Empty;
                    }

                    return;
                }
            }


        }
    }
}
