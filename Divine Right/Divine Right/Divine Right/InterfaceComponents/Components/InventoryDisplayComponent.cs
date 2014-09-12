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
        private Rectangle inventoryBackgroundRect;
        private Rectangle equippedBackgroundRect;

        private List<EquippedItemRectangle> equipmentRectangles;

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

        private InventoryItem selectedItem;
        private Rectangle contextMenu;
        private List<ContextMenuItem> contextMenuChoices = new List<ContextMenuItem>();
        private ContentManager content;

        private Rectangle moneyRect;
        private Rectangle moneyTextRect;

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

            this.content = content;

            batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.WHITE), borderRect, Color.DarkGray);

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), inventoryBackgroundRect, Color.White);

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
            var inventoryitems = this.CurrentActor.Inventory.Inventory.GetObjectsByGroup(enums.GetValue(ChosenCategory)).Where(i => !i.IsEquipped).ToArray();

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

            //Draw the equipped items
            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE), equippedBackgroundRect, Color.White);

            //Go through the inventory
            foreach (var equippedRect in equipmentRectangles)
            {
                if (this.CurrentActor.Inventory.EquippedItems.ContainsKey(equippedRect.Location))
                {
                    equippedRect.InventoryItem = this.CurrentActor.Inventory.EquippedItems[equippedRect.Location];
                }
                else
                {
                    equippedRect.InventoryItem = null;
                }
            }

            //Loop through them
            foreach (var equippedItem in equipmentRectangles)
            {
                //Put a coloured box around them
                batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), equippedItem.Rect, Color.White);

                SpriteData backgroundItem = null;

                //Draw the background item
                switch(equippedItem.Location)
                {
                    case EquipmentLocation.BODY: backgroundItem = SpriteManager.GetSprite(LocalSpriteName.CHAIN_ARMOUR); break;
                    case EquipmentLocation.HEAD: backgroundItem = SpriteManager.GetSprite(LocalSpriteName.HELM_1); break;
                    case EquipmentLocation.MONEY: backgroundItem = SpriteManager.GetSprite(LocalSpriteName.COINS); break;
                    case EquipmentLocation.LEGS: backgroundItem = SpriteManager.GetSprite(LocalSpriteName.PLATE_LEGGINGS); break;
                    case EquipmentLocation.RING1: backgroundItem = SpriteManager.GetSprite(LocalSpriteName.GREY_GEM_RING); break;
                    case EquipmentLocation.RING2: backgroundItem = SpriteManager.GetSprite(LocalSpriteName.GREY_GEM_RING); break;
                    case EquipmentLocation.SHIELD: backgroundItem = SpriteManager.GetSprite(LocalSpriteName.SHIELD_8); break;
                    case EquipmentLocation.WEAPON: backgroundItem = SpriteManager.GetSprite(LocalSpriteName.SWORD_1); break;
                    default: throw new NotImplementedException("No background for " + equippedItem.Location);
                }

                //If there's an item, draw it
                if (equippedItem.InventoryItem != null)
                {
                    batch.Draw(content, equippedItem.InventoryItem.Graphic, equippedItem.Rect, Color.White);
                }
                else
                { //otherwise draw the background
                    batch.Draw(content, backgroundItem, equippedItem.Rect, equippedItem.Location == EquipmentLocation.MONEY ? Color.White: Color.Black);
                }
            }
            //Create the money thing
            batch.Draw(content, SpriteManager.GetSprite(LocalSpriteName.COINS), moneyRect, Color.White);
            //And the total money
            batch.DrawString(font, this.CurrentActor.Inventory.TotalMoney.ToString(), moneyTextRect, Alignment.Left & Alignment.Top, Color.Black);           


            if (this.contextMenu.Width > 0)
            {
                //Draw the context menu if present
                SpriteData scroll = SpriteManager.GetSprite(InterfaceSpriteName.SCROLL);

                Rectangle drawHere = new Rectangle(this.contextMenu.X - 10, this.contextMenu.Y - 25, this.contextMenu.Width + 20, this.contextMenu.Height + 40);

                batch.Draw(content.Load<Texture2D>(scroll.path), drawHere, scroll.sourceRectangle, Color.White);

                //now the items
                foreach (ContextMenuItem item in contextMenuChoices)
                {
                    batch.DrawString(content.Load<SpriteFont>(@"Fonts/TextFeedbackFont"), item.Text, new Vector2(item.Rect.X, item.Rect.Y), Color.Black);
                }
            }
        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out MapItem itm, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            internalActionType = null;
            args = null;
            coord = null;
            destroy = false;
            itm = null;

            //Clicked on a context menu item?
            foreach (var contextMenu in contextMenuChoices)
            {
                if (contextMenu.Rect.Contains(x, y))
                {
                    //Yes. Perform the action
                    //this.selectedItem.PerformAction(contextMenu.Action, this.CurrentActor, contextMenu.Args);
                    actionType = contextMenu.Action;
                    args = contextMenu.Args;
                    itm = selectedItem;
                }
            }

            //remove the contextual menu
            this.contextMenu = new Rectangle(0, 0, 0, 0);
            contextMenuChoices = new List<ContextMenuItem>();
            selectedItem = null;

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

                        foreach (var action in rect.Item.GetPossibleActions(this.CurrentActor))
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

            //Have we clicked on an equipped item?
            foreach (EquippedItemRectangle rect in equipmentRectangles)
            {
                if (rect.Rect.Contains(x, y))
                {
                    //remove the contextual menu
                    contextMenu = new Rectangle(0, 0, 0, 0);
                    contextMenuChoices = new List<ContextMenuItem>();
                    selectedItem = null;

                    //Does it contain an item?
                    if (rect.InventoryItem != null)
                    {
                        //Yes - open contextual menu
                        contextMenu = new Rectangle(x + 15, y + 15, 0, 0);
                        selectedItem = rect.InventoryItem;

                        foreach (var action in rect.InventoryItem.GetPossibleActions(this.CurrentActor))
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

            rect = new Rectangle(locationX, locationY, 495, 190);
            inventoryBackgroundRect = new Rectangle(locationX, locationY, 360, 190);
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

            equippedBackgroundRect = new Rectangle(locationX + 360, locationY + 0, rect.Width - inventoryBackgroundRect.Width, rect.Height);

            equipmentRectangles = new List<EquippedItemRectangle>();

            int left = locationX + 365;
            int middle = locationX + 410;
            int right = locationX + 455;

            equipmentRectangles.Add(new EquippedItemRectangle { Location = EquipmentLocation.HEAD, Rect = new Rectangle(middle, locationY + 0, 40, 40) });
            equipmentRectangles.Add(new EquippedItemRectangle { Location = EquipmentLocation.BODY, Rect = new Rectangle(middle, locationY + 45, 40, 40) });
            equipmentRectangles.Add(new EquippedItemRectangle { Location = EquipmentLocation.WEAPON, Rect = new Rectangle(left, locationY + 45, 40, 40) });
            equipmentRectangles.Add(new EquippedItemRectangle { Location = EquipmentLocation.SHIELD, Rect = new Rectangle(right, locationY + 45, 40, 40) });
            equipmentRectangles.Add(new EquippedItemRectangle { Location = EquipmentLocation.LEGS, Rect = new Rectangle(middle, locationY + 90, 40, 40) });
            equipmentRectangles.Add(new EquippedItemRectangle { Location = EquipmentLocation.RING1, Rect = new Rectangle(left, locationY + 90, 40, 40) });
            equipmentRectangles.Add(new EquippedItemRectangle { Location = EquipmentLocation.RING2, Rect = new Rectangle(right, locationY + 90, 40, 40) });

            moneyRect = new Rectangle(locationX + 360, locationY + 190 - 30, 30, 30);
            moneyTextRect = new Rectangle(locationX + 390, locationY + 190 - 30, 100, 30);
            
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

            //Equipped item?
            foreach (EquippedItemRectangle rect in equipmentRectangles)
            {
                if (rect.Rect.Contains(x, y))
                {
                    //Does it contain an item?
                    if (rect.InventoryItem != null)
                    {
                        //Yes
                        detailsToShow = rect.InventoryItem.Description;
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
