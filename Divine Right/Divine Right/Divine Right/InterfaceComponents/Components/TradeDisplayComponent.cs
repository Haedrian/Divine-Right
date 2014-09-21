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
using DRObjects.Items.Archetypes.Local;
using Microsoft.Xna.Framework.Content;

namespace Divine_Right.InterfaceComponents.Components
{
    public class TradeDisplayComponent :
        IGameInterfaceComponent
    {
        public int ChosenCategory { get; set; }
        System.Array enums = Enum.GetValues(typeof(InventoryCategory));

        private const int ROW_TOTAL = 12;

        private bool visible = true;

        protected int locationX;
        protected int locationY;

        public Actor VendorActor { get; set; }
        public Actor PlayerActor { get; set; }

        public bool Buy { get; set; }

        private Rectangle rect;
        private Rectangle inventoryBackgroundRect;

        private Rectangle borderRect;

        private Rectangle categoryBackground;

        private Rectangle titleBackgroundRect;
        private Rectangle titleRect;

        private Rectangle playerFundsIconRect;
        private Rectangle vendorFundsIconRect;
        private Rectangle playerFundsRect;
        private Rectangle vendorFundsRect;

        private List<Rectangle> categories;
        private List<Rectangle> categoryBackgrounds;

        private List<InventoryItemRectangle> row1Items;
        private List<InventoryItemRectangle> row2Items;
        private List<InventoryItemRectangle> row3Items;

        private string detailsToShow = String.Empty;
        private Rectangle detailsRect;
        private SpriteFont font;

        private Rectangle contextMenu;
        private List<ContextMenuItem> contextMenuChoices = new List<ContextMenuItem>();
        private ContentManager content;

        private Rectangle swapButton;
        private Rectangle closeButton;

        private Rectangle totalSectionRect;
        private Rectangle totalTextRect;
        private Rectangle confirmButton;

        private int totalSelected = 0;

        //Drawing stuff
        public TradeDisplayComponent(int locationX, int locationY, Actor currentActor, Actor vendorActor)
        {
            this.locationX = locationX;
            this.locationY = locationY;
            this.VendorActor = vendorActor;
            this.PlayerActor = currentActor;
            ChosenCategory = 0;

            PerformDrag(locationX, locationY);

            Buy = true;
        }


        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if (!Visible)
            {
                return;
            }

            //Calculate the total - later we'll multiply it by what the vendor wants
            totalSelected = this.GetSelected().Sum(t => (int)(t.Item.BaseValue * this.VendorActor.VendorDetails.GetPriceMultiplier(t.Item.Category, !Buy)));

            this.content = content;

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

                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.CLOSE), closeButton, Color.White);

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

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.CLOSE), closeButton, Color.White);

            InventoryItem[] inventoryitems = null;

            //Now draw the items
            if (Buy)
            {
                inventoryitems = this.VendorActor.VendorDetails.Stock.GetObjectsByGroup(enums.GetValue(ChosenCategory)).Where(i => !i.IsEquipped).OrderByDescending(i => i.BaseValue).ToArray();
            }
            else
            {
                inventoryitems = this.PlayerActor.Inventory.Inventory.GetObjectsByGroup(enums.GetValue(ChosenCategory)).Where(i => !i.IsEquipped).ToArray();
            }

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
                int row = (int)Math.Floor((double)i / (double)ROW_TOTAL);

                if (row == 0)
                {
                    if (row1Items[column].Selected)
                    {
                        //Highlight it
                        batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.WHITE), row1Items[column].Rect, Color.LightBlue);
                    }

                    //Also assign the item
                    row1Items[column].Item = inventoryitems[i];
                    batch.Draw(content, inventoryitems[i].Graphic, row1Items[column].Rect, Color.White);
                }
                else if (row == 1)
                {
                    if (row2Items[column].Selected)
                    {
                        //Highlight it
                        batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.WHITE), row2Items[column].Rect, Color.LightBlue);
                    }

                    row2Items[column].Item = inventoryitems[i];
                    batch.Draw(content, inventoryitems[i].Graphic, row2Items[column].Rect, Color.White);
                }
                else
                {
                    if (row3Items[column].Selected)
                    {
                        //Highlight it
                        batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.WHITE), row3Items[column].Rect, Color.LightBlue);
                    }

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

            batch.Draw(content, SpriteManager.GetSprite(LocalSpriteName.COINS), playerFundsIconRect, Color.White);
            batch.Draw(content, SpriteManager.GetSprite(LocalSpriteName.COINS), vendorFundsIconRect, Color.White);

            batch.DrawString(font, detailsToShow, detailsRect, Alignment.Left, Color.Black);

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), swapButton, Color.White);

            if (Buy)
            {
                batch.DrawString(font, "Buying from " + VendorActor.Name, titleRect, Alignment.Center, Color.Green);
                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.SELL), swapButton, Color.White);
            }
            else
            {
                batch.DrawString(font, "Selling  to " + VendorActor.Name, titleRect, Alignment.Center, Color.Blue);
                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.BUY), swapButton, Color.White);
            }

            batch.DrawString(font, "You: " + PlayerActor.Inventory.TotalMoney, playerFundsRect, Alignment.Left, Color.Green);
            batch.DrawString(font, "Vendor: " + VendorActor.VendorDetails.Money, vendorFundsRect, Alignment.Right, Color.Blue);

            if (Buy)
            {
                batch.DrawString(font, "Total:" + totalSelected, totalTextRect, Alignment.Left, this.PlayerActor.Inventory.TotalMoney >= totalSelected ? Color.Black : Color.Red);
            }
            else
            {
                batch.DrawString(font, "Total:" + totalSelected, totalTextRect, Alignment.Left, this.VendorActor.VendorDetails.Money >= totalSelected ? Color.Black : Color.Red);
            }

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE), confirmButton, Color.White);
            batch.DrawString(font, "CONFIRM", confirmButton, Alignment.Center, Color.Black);

        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out MapItem itm, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            internalActionType = null;
            args = null;
            coord = null;
            destroy = false;
            itm = null;

            //Clicked on the button?
            if (swapButton.Contains(x, y))
            {
                Buy = !Buy;

                //Remove all selections
                foreach (var item in row1Items.Union(row2Items).Union(row3Items))
                {
                    item.Selected = false;
                }

                return true;
            }

            if (closeButton.Contains(x, y))
            {
                //Close it
                destroy = true;
                return true;
            }

            if (confirmButton.Contains(x, y))
            {
                //Are they next to each other?
                if (this.VendorActor.MapCharacter.Coordinate - this.PlayerActor.MapCharacter.Coordinate > 2)
                {
                    return true; //invalid
                }

                //Valid ?
                if (this.Buy)
                {
                    if (this.totalSelected > PlayerActor.Inventory.TotalMoney)
                    {
                        //Nope
                        return true;
                    }

                    //Allright, lets do this

                    //Take the items
                    foreach (var item in GetSelected())
                    {
                        this.VendorActor.VendorDetails.Stock.Remove(item.Item.Category, item.Item);
                        this.PlayerActor.Inventory.Inventory.Add(item.Item.Category, item.Item);
                    }

                    //Give him the money
                    this.PlayerActor.Inventory.TotalMoney -= totalSelected;
                    this.VendorActor.VendorDetails.Money += totalSelected;

                    //Remove all selections
                    foreach (var item in row1Items.Union(row2Items).Union(row3Items))
                    {
                        item.Selected = false;
                    }

                }
                else
                {
                    if (this.totalSelected > this.VendorActor.VendorDetails.Money)
                    {
                        //Nope
                        return true;
                    }

                    //Allright, lets do this

                    //Take the items
                    foreach (var item in GetSelected())
                    {
                        this.PlayerActor.Inventory.Inventory.Remove(item.Item.Category, item.Item);
                        this.VendorActor.VendorDetails.Stock.Add(item.Item.Category, item.Item);
                    }

                    //Give him the money
                    this.PlayerActor.Inventory.TotalMoney += totalSelected;
                    this.VendorActor.VendorDetails.Money -= totalSelected;

                    //Remove all selections
                    foreach (var item in row1Items.Union(row2Items).Union(row3Items))
                    {
                        item.Selected = false;
                    }
                }

            }

            for (int i = 0; i < categories.Count; i++)
            {
                if (categories[i].Contains(x, y))
                {
                    //Change category!
                    this.ChosenCategory = i;

                    //Remove all selections
                    foreach (var item in row1Items.Union(row2Items).Union(row3Items))
                    {
                        item.Selected = false;
                    }

                    //Handled. Naught else
                    return true;
                }
            }

            //Have we clicked on an item ?
            List<InventoryItemRectangle> allItemBoxes = new List<InventoryItemRectangle>();
            allItemBoxes.AddRange(row1Items);
            allItemBoxes.AddRange(row2Items);
            allItemBoxes.AddRange(row3Items);

            foreach (InventoryItemRectangle rect in allItemBoxes)
            {
                if (rect.Rect.Contains(x, y))
                {
                    //Does it contain an item?
                    if (rect.Item != null)
                    {
                        //Toggle the selection
                        rect.Selected = !rect.Selected;
                    }
                    else
                    {
                        return true; //Empty box
                    }

                }
            }

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

            rect = new Rectangle(locationX, locationY, 360, 320);
            inventoryBackgroundRect = new Rectangle(locationX, locationY + 50, 360, 190);
            borderRect = new Rectangle(locationX - 2, locationY - 2, rect.Width + 4, rect.Height + 4);

            categoryBackground = new Rectangle(locationX, locationY + 100, rect.Width, 5);

            titleBackgroundRect = new Rectangle(locationX, locationY, rect.Width, 50);
            titleRect = new Rectangle(locationX, locationY, rect.Width, 25);

            playerFundsIconRect = new Rectangle(locationX, locationY + 25, 25, 25);
            vendorFundsIconRect = new Rectangle(locationX + rect.Width - 25, locationY + 25, 25, 25);
            playerFundsRect = new Rectangle(locationX + 25, locationY + 25, (rect.Width / 2) - 50, 25);
            vendorFundsRect = new Rectangle(locationX + rect.Width / 2, locationY + 25, (rect.Width / 2) - 25, 25);

            categoryBackgrounds = new List<Rectangle>();
            categories = new List<Rectangle>();

            for (int i = 0; i < enums.Length; i++)
            {
                categoryBackgrounds.Add(new Rectangle(locationX + (50 * i), locationY + 50, 50, 50));
                categories.Add(new Rectangle(locationX + (50 * i), locationY + 50, 50, 50));
            }

            //Create new ones. Otherwise update existing ones
            if (row1Items == null)
            {
                row1Items = new List<InventoryItemRectangle>();
                row2Items = new List<InventoryItemRectangle>();
                row3Items = new List<InventoryItemRectangle>();

                for (int i = 0; i < ROW_TOTAL; i++)
                {
                    row1Items.Add(new InventoryItemRectangle { Rect = new Rectangle((locationX + (30 * i)), locationY + 60 + 50, 30, 30) });
                    row2Items.Add(new InventoryItemRectangle { Rect = new Rectangle((locationX + (30 * i)), locationY + 60 + 80, 30, 30) });
                    row3Items.Add(new InventoryItemRectangle { Rect = new Rectangle((locationX + (30 * i)), locationY + 60 + 110, 30, 30) });
                }
            }
            else
            {
                for (int i = 0; i < ROW_TOTAL; i++)
                {
                    row1Items[i].Rect = new Rectangle((locationX + (30 * i)), locationY + 60 + 50, 30, 30);
                    row2Items[i].Rect = new Rectangle((locationX + (30 * i)), locationY + 60 + 80, 30, 30);
                    row3Items[i].Rect = new Rectangle((locationX + (30 * i)), locationY + 60 + 110, 30, 30);
                }
            }

            detailsRect = new Rectangle(locationX, locationY + 240, rect.Width, 40);

            swapButton = new Rectangle(locationX, locationY, 20, 20);
            closeButton = new Rectangle(locationX + rect.Width - 30, locationY, 30, 30);

            totalSectionRect = new Rectangle(locationX, locationY + 280, rect.Width, 40);
            totalTextRect = new Rectangle(locationX, locationY + 280, rect.Width, 20);
            confirmButton = new Rectangle(locationX + 100, locationY + 300, rect.Width - 200, 20);

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

            //Inventory item?
            foreach (InventoryItemRectangle rect in allItemBoxes)
            {
                if (rect.Rect.Contains(x, y))
                {
                    //Does it contain an item?
                    if (rect.Item != null)
                    {
                        //Yes

                        //We need to add the actual price. Are we buying or selling?
                        if (Buy)
                        {
                            detailsToShow = rect.Item.Description + "\nVendor Price : " + rect.Item.BaseValue * this.VendorActor.VendorDetails.GetPriceMultiplier(rect.Item.Category, !Buy);
                        }
                        else
                        {
                            detailsToShow = rect.Item.Description + "\nVendor Will Pay : " + rect.Item.BaseValue * this.VendorActor.VendorDetails.GetPriceMultiplier(rect.Item.Category, !Buy);
                        }

                    }
                    else
                    {
                        break;
                    }

                    return;
                }
            }

        }

        #region Helper Functions

        /// <summary>
        /// Gets a list of all selected inventoryitems
        /// </summary>
        /// <returns></returns>
        private List<InventoryItemRectangle> GetSelected()
        {
            List<InventoryItemRectangle> items = new List<InventoryItemRectangle>();

            foreach (var item in row1Items.Union(row2Items).Union(row3Items))
            {
                if (item.Selected)
                {
                    items.Add(item);
                }
            }

            return items;
        }

        public void AddContextMenuItem(ActionTypeEnum action, object[] args, ContentManager content)
        {
            ContextMenuItem item = new ContextMenuItem();
            item.Action = action;
            item.Args = args;

            Rectangle itemRect = new Rectangle();

            //where will this rectangle start?
            if (contextMenuChoices.Count == 0)
            {
                itemRect.X = contextMenu.X;
                itemRect.Y = contextMenu.Y;

            }
            else
            {
                ContextMenuItem prev = this.contextMenuChoices[contextMenuChoices.Count - 1];
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
            contextMenuChoices.Add(item);

            //update the draw rectangle

            if (contextMenu.Width < itemRect.Width)
            {
                contextMenu.Width = itemRect.Width;
            }

            //update the height
            contextMenu.Height += itemRect.Height;

        }
        #endregion
    }
}
