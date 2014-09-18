﻿using System;
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

        private InventoryItem selectedItem;
        private Rectangle contextMenu;
        private List<ContextMenuItem> contextMenuChoices = new List<ContextMenuItem>();
        private ContentManager content;

        private Rectangle swapButton;
        private Rectangle closeButton;

        private Rectangle moneyTextRect;
         
        //Drawing stuff
        public TradeDisplayComponent(int locationX, int locationY, Actor currentActor)
        {
            this.locationX = locationX;
            this.locationY = locationY;
            this.VendorActor = currentActor;
            ChosenCategory = 0;

            PerformDrag(locationX, locationY);

            Buy = true; //TODO: REMOVE AFTER TESTING
        }


        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if (!Visible)
            {
                return;
            }

            this.content = content;

            batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.WHITE), borderRect, Color.DarkGray);

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), inventoryBackgroundRect, Color.White);

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE), detailsRect, Color.White);

            batch.Draw(content,SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE),titleBackgroundRect,Color.White);

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
            var inventoryitems = this.VendorActor.Inventory.Inventory.GetObjectsByGroup(enums.GetValue(ChosenCategory)).Where(i => !i.IsEquipped).ToArray();

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

            batch.Draw(content, SpriteManager.GetSprite(LocalSpriteName.COINS), playerFundsIconRect, Color.White);
            batch.Draw(content, SpriteManager.GetSprite(LocalSpriteName.COINS), vendorFundsIconRect, Color.White);

            batch.DrawString(font, detailsToShow, detailsRect, Alignment.Left, Color.Black);

            //Create the money thing
            //batch.Draw(content, SpriteManager.GetSprite(LocalSpriteName.COINS), moneyRect, Color.White);
            //And the total money
            batch.DrawString(font, this.VendorActor.Inventory.TotalMoney.ToString(), moneyTextRect, Alignment.Left & Alignment.Top, Color.Black);

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), swapButton, Color.White);

            if (Buy)
            {
                batch.DrawString(font, "Buying from Joseph Borg", titleRect, Alignment.Center, Color.Green);
                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.SELL), swapButton, Color.White);
            }
            else 
            {
                batch.DrawString(font, "Selling  to Joseph Borg", titleRect, Alignment.Center, Color.DarkGoldenrod);
                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.BUY), swapButton, Color.White);
            }

            batch.DrawString(font, "You: 20523", playerFundsRect, Alignment.Left, Color.Green);
            batch.DrawString(font, "Vendor: 029123", vendorFundsRect, Alignment.Right, Color.DarkGoldenrod);

            
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
            }

            for (int i=0; i < categories.Count; i++)
            {
                if (categories[i].Contains(x, y))
                {
                    //Change category!
                    this.ChosenCategory = i;

                    //remove the contextual menu
                    contextMenu = new Rectangle(0, 0, 0, 0);
                    contextMenuChoices = new List<ContextMenuItem>();
                    selectedItem = null;

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
                    //remove the contextual menu
                    contextMenu = new Rectangle(0, 0, 0, 0);
                    contextMenuChoices = new List<ContextMenuItem>();
                    selectedItem = null;

                    //Does it contain an item?
                    if (rect.Item != null)
                    {
                        //Yes - open contextual menu
                        contextMenu = new Rectangle(x+15, y+15, 0, 0);
                        selectedItem = rect.Item;

                        foreach (var action in rect.Item.GetPossibleActions(this.VendorActor))
                        {
                            AddContextMenuItem(action, new object[0] { }, content);
                        }

                        //done
                        return true;
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

            rect = new Rectangle(locationX, locationY, 360, 280);
            inventoryBackgroundRect = new Rectangle(locationX, locationY+50, 360, 190);
            borderRect = new Rectangle(locationX - 2, locationY - 2, rect.Width + 4, rect.Height + 4);

            categoryBackground = new Rectangle(locationX, locationY + 100, rect.Width, 5);

            titleBackgroundRect = new Rectangle(locationX, locationY, rect.Width, 50);
            titleRect = new Rectangle(locationX, locationY, rect.Width, 25);

            playerFundsIconRect = new Rectangle(locationX, locationY + 25, 25, 25);
            vendorFundsIconRect = new Rectangle(locationX + rect.Width - 25, locationY + 25, 25, 25);
            playerFundsRect = new Rectangle(locationX + 25, locationY + 25, (rect.Width / 2) - 50, 25);
            vendorFundsRect = new Rectangle(locationX + rect.Width/2, locationY + 25, (rect.Width / 2) - 25, 25);

            categoryBackgrounds = new List<Rectangle>();
            categories = new List<Rectangle>();

            for (int i = 0; i < enums.Length; i++)
            {
                categoryBackgrounds.Add(new Rectangle(locationX + (50 * i), locationY+50, 50, 50));
                categories.Add(new Rectangle(locationX + (50 * i), locationY +50, 50, 50));
            }

            row1Items = new List<InventoryItemRectangle>();
            row2Items = new List<InventoryItemRectangle>();
            row3Items = new List<InventoryItemRectangle>();

            for (int i = 0; i < ROW_TOTAL; i++)
            {
                row1Items.Add(new InventoryItemRectangle{ Rect = new Rectangle((locationX + (30 * i)), locationY +60 + 50, 30, 30) });
                row2Items.Add(new InventoryItemRectangle { Rect = new Rectangle((locationX + (30 * i)), locationY +60 + 80, 30, 30)});
                row3Items.Add(new InventoryItemRectangle { Rect = new Rectangle((locationX + (30 * i)), locationY + 60 + 110, 30, 30) });
            }

            detailsRect = new Rectangle(locationX, locationY + 240, rect.Width, 40);

           moneyTextRect = new Rectangle(locationX + 390, locationY + 240 - 30, 100, 30);

           swapButton = new Rectangle(locationX, locationY, 20, 20);
           closeButton = new Rectangle(locationX - rect.Width + 30, locationY, 30, 30);
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
                        break;
                    }

                    return;
                }
            }

        }

        #region Helper Functions
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
