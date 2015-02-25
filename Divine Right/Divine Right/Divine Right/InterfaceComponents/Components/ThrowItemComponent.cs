using Divine_Right.InterfaceComponents.Objects;
using DRObjects;
using DRObjects.Enums;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Divine_Right.HelperFunctions;
using DRObjects.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Divine_Right.InterfaceComponents.Components
{
    /// <summary>
    /// For letting the user choose what item he wishes to throw
    /// </summary>
    public class ThrowItemComponent
        : IGameInterfaceComponent
    {
        private MapCoordinate targetCoordinate;
        private Actor Actor;

        private Rectangle rect;
        private Rectangle borderRect;

        private Rectangle descriptionRect;
        private List<InventoryItemRectangle> ItemRects;

        private int locationX;
        private int locationY;

        private string description;
        private SpriteFont font;

        public ThrowItemComponent(int x, int y, MapCoordinate targetCoordinate, Actor actor)
        {
            this.locationX = x;
            this.locationY = y;
            this.targetCoordinate = targetCoordinate;
            this.Actor = actor;

            PerformDrag(0, 0);

        }


        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
              if (font == null)
            {
                //Load the font
                font = content.Load<SpriteFont>(@"Fonts/LightText");
            }

            //Draw the background and the borders
            batch.Draw(content, SpriteManager.GetSprite(ColourSpriteName.WHITE), borderRect, Color.DarkGray);
            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), rect, Color.White);

            //Now draw the description, if any
            if (description != null)
            {
                batch.DrawString(font, description, descriptionRect, Alignment.Center, Color.Black);
            }

            //Now draw the items
            foreach(var item in ItemRects)
            {
                batch.Draw(content, item.Item.Graphic, item.Rect, Color.White);
            }
    
        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionType? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            internalActionType = null;
            args = null;
            coord = null;
            destroy = false;
            item = null;

            //Since this is modal it'll get all clicks. If the user has clicked out of the element, then we can destroy it
            if (!rect.Contains(x,y))
            {
                destroy = true;
            }


            return true; //modal
        }

        public void HandleMouseOver(int x, int y)
        {
            this.description = null;

           foreach(var item in ItemRects)
           {
               if (item.Rect.Contains(x,y))
               {
                   item.Selected = true;
                   this.description = item.Item.Description;
                   break;
               }
           }
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionType? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            args = null;
            coord = null;
            destroy = false;

            if (keyboard.GetPressedKeys().Length > 0)
            {
                //Destroy it
                destroy = true;
            }

            return true;
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return rect;
        }

        public void PerformDrag(int deltaX, int deltaY)
        {
            this.locationX += deltaX;
            this.locationY += deltaY;

            this.rect = new Rectangle(locationX, locationY, 360, 120);
            this.borderRect = new Rectangle(locationX - 2, locationY - 2, rect.Width + 4, rect.Height + 4);
            this.descriptionRect = new Rectangle(locationX, locationY  + 90, rect.Width, 30);

            this.ItemRects = new List<InventoryItemRectangle>();

            int nextX = 0;
            int nextY = 0;

            //Go through all the POTIONS that the actor has in their inventory, and create a bunch of rectangles
            foreach (var item in Actor.Inventory.Inventory.GetObjectsByGroup(InventoryCategory.POTION))
            {
                //Where we put it?
                if (nextX + 30 >= rect.Width)
                {
                    nextX = 0;
                    nextY += 30;
                }

                if (nextY + 30 >= 90)
                {
                    break; //Can't show anymore
                }

                InventoryItemRectangle iir = new InventoryItemRectangle();

                iir.Item = item;
                iir.Selected = false;
                iir.Rect = new Rectangle(locationX +nextX, locationY + nextY, 30, 30);

                ItemRects.Add(iir);

                nextX += 30;
            }
        }

        public bool IsModal()
        {
            return true;
        }

        public bool Visible
        {
            get
            {
                return true;
            }
            set
            {
                return;
            }
        }
    }
}
