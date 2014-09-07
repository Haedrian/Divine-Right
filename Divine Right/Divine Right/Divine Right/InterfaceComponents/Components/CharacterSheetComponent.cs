using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using Microsoft.Xna.Framework;
using DRObjects.Graphics;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Enums;
using Divine_Right.HelperFunctions;

namespace Divine_Right.InterfaceComponents.Components
{
    public class CharacterSheetComponent
        : IGameInterfaceComponent
    {
        #region Properties
        private bool visible;

        protected int locationX;
        protected int locationY;
        private Actor actor;

        private Rectangle rect;
        private Rectangle borderRect;

        private SpriteFont font;

        #endregion

        public CharacterSheetComponent(int x, int y, Actor actor)
        {
            this.locationX = x;
            this.locationY = y;
            this.actor = actor;

            rect = new Rectangle(x, y, 210, 170);
        }


        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            //Draw the background
            if (!visible)
            {
                return; //draw nothing
            }

            if (font == null)
            {
                //Load the font
                font = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont");
            }

            var white = SpriteManager.GetSprite(ColourSpriteName.WHITE);

            batch.Draw(content, white, borderRect, Color.DarkGray);

            //Draw the background
            var scrollBackground = SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE);

            batch.Draw(content.Load<Texture2D>(scrollBackground.path), rect, scrollBackground.sourceRectangle, Color.White);

            var attributes = actor.Attributes;

            //Do the attributes
            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.BRAWN).path), new Rectangle(locationX + 10, locationY + 10, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.BRAWN).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Brawn.ToString(), new Vector2(locationX + 50, locationY + 15), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.DEX).path), new Rectangle(locationX + 10, locationY + 40, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.DEX).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Dex.ToString(), new Vector2(locationX + 50, locationY + 45), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.AGIL).path), new Rectangle(locationX + 10, locationY + 70, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.AGIL).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Agil.ToString(), new Vector2(locationX + 50, locationY + 75), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.PERC).path), new Rectangle(locationX + 10, locationY + 100, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.PERC).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Perc.ToString(), new Vector2(locationX + 50, locationY + 105), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.INTEL).path), new Rectangle(locationX + 10, locationY + 130, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.INTEL).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Intel.ToString(), new Vector2(locationX + 50, locationY + 135), Color.Black);

            //Weapon proficiencies

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.SWORD).path), new Rectangle(locationX + 80, locationY + 10, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.SWORD).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.HandToHand.ToString(), new Vector2(locationX + 120, locationY + 15), Color.Gray);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.DEFENSE).path), new Rectangle(locationX + 80, locationY + 40, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.DEFENSE).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Evasion.ToString(), new Vector2(locationX + 120, locationY + 45), Color.Gray);

            /*
            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.AXE).path), new Rectangle(locationX + 80, locationY + 40, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.AXE).sourceRectangle, Color.White);
            batch.DrawString(font, "2", new Vector2(locationX + 120, locationY + 45), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.SPEAR).path), new Rectangle(locationX + 80, locationY + 70, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.SPEAR).sourceRectangle, Color.White);
            batch.DrawString(font, "2", new Vector2(locationX + 120, locationY + 75), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.MACE).path), new Rectangle(locationX + 80, locationY + 100, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.MACE).sourceRectangle, Color.White);
            batch.DrawString(font, "0", new Vector2(locationX + 120, locationY + 105), Color.Black);
            */
            //Totals

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.SWORD).path), new Rectangle(locationX + 150, locationY + 10, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.SWORD).sourceRectangle, Color.White);
            batch.DrawString(font, (attributes.HandToHand + attributes.Brawn - 5).ToString(), new Vector2(locationX + 190, locationY + 15), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.DEFENSE).path), new Rectangle(locationX + 150, locationY + 40, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.DEFENSE).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Dodge.ToString(), new Vector2(locationX + 190, locationY + 45), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.BLOOD).path), new Rectangle(locationX + 150, locationY + 70, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.BLOOD).sourceRectangle, Color.White);
            batch.DrawString(font,(attributes.Brawn - 5).ToString("+#;-#;0"), new Vector2(locationX + 190, locationY + 75), Color.Black);


        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out InternalActionEnum? internalActionType, out object[] args, out MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            //This does nothing

            item = null;
            args = null;
            coord = null;
            destroy = false;
            actionType = null;
            internalActionType = null;

            return visible; //If it's visible - block it. Otherwise do nothing
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            args = null;
            coord = null;
            destroy = false;
            return false; //never do anything
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return rect;
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
                visible = value;
            }
        }


        public void PerformDrag(int x, int y)
        {
            this.locationX += x;
            this.locationY += y;

            rect = new Rectangle(locationX, locationY, 210, 170);
            this.borderRect = new Rectangle(rect.X - 2, rect.Y - 2, rect.Width + 4, rect.Height + 4);
        }


        public void HandleMouseOver(int x, int y)
        {
            //Do nothing
        }
    }
}
