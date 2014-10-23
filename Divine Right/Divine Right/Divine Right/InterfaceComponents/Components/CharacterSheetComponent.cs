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

            PerformDrag(0, 0);
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

            //Resize if new skills have popped up
            var skillTotal = this.actor.Attributes.Skills.Values.Count;

            rect = new Rectangle(locationX, locationY, 250, 200 + (skillTotal * 15));
            this.borderRect = new Rectangle(rect.X - 2, rect.Y - 2, rect.Width + 4, rect.Height + 4);


            var white = SpriteManager.GetSprite(ColourSpriteName.WHITE);

            batch.Draw(content, white, borderRect, Color.DarkGray);

            //Draw the background
            var scrollBackground = SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE);

            batch.Draw(content.Load<Texture2D>(scrollBackground.path), rect, scrollBackground.sourceRectangle, Color.White);

            var attributes = actor.Attributes;

            //Do the attributes
            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.BRAWN).path), new Rectangle(locationX + 10, locationY + 10, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.BRAWN).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Brawn.ToString(), new Vector2(locationX + 50, locationY + 15), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.CHA).path), new Rectangle(locationX + 10, locationY + 40, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.CHA).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Char.ToString(), new Vector2(locationX + 50, locationY + 45), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.AGIL).path), new Rectangle(locationX + 10, locationY + 70, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.AGIL).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Agil.ToString(), new Vector2(locationX + 50, locationY + 75), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.PERC).path), new Rectangle(locationX + 10, locationY + 100, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.PERC).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Perc.ToString(), new Vector2(locationX + 50, locationY + 105), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.INTEL).path), new Rectangle(locationX + 10, locationY + 130, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.INTEL).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Intel.ToString(), new Vector2(locationX + 50, locationY + 135), Color.Black);

            //Fighting stuff

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.SWORD).path), new Rectangle(locationX + 150, locationY + 10, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.SWORD).sourceRectangle, Color.White);
            batch.DrawString(font, (attributes.HandToHand).ToString(), new Vector2(locationX + 190, locationY + 15), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.DEFENSE).path), new Rectangle(locationX + 150, locationY + 40, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.DEFENSE).sourceRectangle, Color.White);
            batch.DrawString(font, attributes.Dodge.ToString(), new Vector2(locationX + 190, locationY + 45), Color.Black);

            batch.Draw(content.Load<Texture2D>(SpriteManager.GetSprite(InterfaceSpriteName.BLOOD).path), new Rectangle(locationX + 150, locationY + 70, 30, 30), SpriteManager.GetSprite(InterfaceSpriteName.BLOOD).sourceRectangle, Color.White);
            batch.DrawString(font,(attributes.Brawn - 5).ToString("+#;-#;0"), new Vector2(locationX + 190, locationY + 75), Color.Black);

            //Work on the skills
            //batch.DrawString(font, "SKILLS", new Rectangle(locationX, locationY + 150, rect.Width, 30), Alignment.Center, Color.DarkBlue);

            int multiplier = 0;

            foreach (var skill in this.actor.Attributes.Skills.Values.OrderBy(sv => sv.SkillName.ToString()))
            {
                multiplier++;

                batch.DrawString(font, skill.SkillLevelString, new Rectangle(locationX, locationY + 170 + (multiplier * 15), (rect.Width/2) - 5, 15), Alignment.Right, Color.Black);
                batch.DrawString(font, skill.SkillNameString, new Rectangle(locationX + rect.Width/2 + 5, locationY + 170 + (multiplier * 15), (rect.Width / 2) -5, 15), Alignment.Left, Color.Black);
            }
        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionType? actionType, out InternalActionEnum? internalActionType, out object[] args, out MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
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

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionType? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
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

            var skillTotal = this.actor.Attributes.Skills.Values.Count;

            rect = new Rectangle(locationX, locationY, 210, 200 + (skillTotal*15));
            this.borderRect = new Rectangle(rect.X - 2, rect.Y - 2, rect.Width + 4, rect.Height + 4);
        }


        public void HandleMouseOver(int x, int y)
        {
            //Do nothing
        }
    }
}
