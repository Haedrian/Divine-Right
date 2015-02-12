using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.SpecialAttacks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Divine_Right.HelperFunctions;
using DRObjects.Graphics;
using DivineRightGame;

namespace Divine_Right.InterfaceComponents.Components
{
    public class CombatManualComponent
        : IGameInterfaceComponent
    {
        private Rectangle rect;
        private Rectangle borderRect;

        private Rectangle slotBackground;
        private Rectangle[] slotRectangles;

        private Rectangle newBackground;
        private Rectangle oldBackground;

        private Rectangle newText;
        private Rectangle oldText;

        private Rectangle newName;
        private Rectangle oldName;

        private List<Tuple<SpecialAttackType, Rectangle>> newIcons;
        private List<Tuple<SpecialAttackType, Rectangle>> newDetails;

        private List<Tuple<SpecialAttackType, Rectangle>> oldIcons;
        private List<Tuple<SpecialAttackType, Rectangle>> oldDetails;

        private Rectangle learnRect;
        private Rectangle cancelRect;

        private int locationX;
        private int locationY;

        private int? clickedNumber = null;
        private SpecialAttack newAttack;
        private SpecialAttack oldAttack;

        private SpriteFont font = null;

        public CombatManualComponent(SpecialAttack newAttack)
        {
            this.newAttack = newAttack;

            this.locationX = 100;
            this.locationY = 100;

            this.PerformDrag(0, 0);

            this.clickedNumber = 2; //TODO: REMOVE AFTER TESTING
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if (font == null)
            {
                font = content.Load<SpriteFont>(@"Fonts/TextFeedbackFont");
            }

            var white = SpriteManager.GetSprite(ColourSpriteName.WHITE);

            batch.Draw(content, white, borderRect, Color.DarkGray);

            //Draw the background
            var scrollBackground = SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE);

            batch.Draw(content.Load<Texture2D>(scrollBackground.path), rect, scrollBackground.sourceRectangle, Color.White);

            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), oldBackground, Color.White);
            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), newBackground, Color.White);
            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE), slotBackground, Color.White);

            //The text
            batch.DrawString(font, newAttack.AttackName, newName, Alignment.Center, Color.Black);
            batch.DrawString(font, "New Attack", newText, Alignment.Center, Color.White);

            foreach (var icons in newIcons)
            {
                switch (icons.Item1)
                {
                    case SpecialAttackType.ACCURACY:
                        batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.ACCURATE_STRIKE), icons.Item2, Color.Black);
                        break;
                    case SpecialAttackType.ATTACKS:
                        batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.RAPID_STRIKES), icons.Item2, Color.Black);
                        break;
                    case SpecialAttackType.BLEED:
                        batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.BLEEDING_STRIKE), icons.Item2, Color.Black);
                        break;
                    case SpecialAttackType.DAMAGE:
                        batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.POWER_STRIKE), icons.Item2, Color.Black);
                        break;
                    case SpecialAttackType.PIERCING:
                        batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.ARMOUR_PIERCING_STRIKE), icons.Item2, Color.Black);
                        break;
                    case SpecialAttackType.PUSH:
                        batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PUSHBACK), icons.Item2, Color.Black);
                        break;
                    case SpecialAttackType.STUN:
                        batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.STUNNING_STRIKE), icons.Item2, Color.Black);
                        break;
                    case SpecialAttackType.SUNDER:
                        batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.SUNDER), icons.Item2, Color.Black);
                        break;
                    case SpecialAttackType.TARGETS:
                        batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WHIRLWIND), icons.Item2, Color.Black);
                        break;
                }
            }

            foreach (var details in newDetails)
            {
                DRObjects.ActorHandling.SpecialAttacks.Effect effect = newAttack.Effects.FirstOrDefault(e => e.EffectType == details.Item1);

                string display = "--";

                if (effect != null)
                {
                    display = effect.EffectValue.ToString();
                }

                batch.DrawString(font, display, details.Item2, Alignment.Center, Color.Black);
            }

            for (int i = 0; i < slotRectangles.Length; i++)
            {
                Rectangle slot = slotRectangles[i];

                SpriteData sprite = SpriteManager.GetSprite( (InterfaceSpriteName) Enum.Parse(typeof(InterfaceSpriteName),"SA" + (i+1) ));

                batch.Draw(content,sprite,slotRectangles[i],GameState.PlayerCharacter.SpecialAttacks[i] == null ? Color.Black : Color.White);
            }

            if (this.clickedNumber != null)
            {
                oldAttack = GameState.PlayerCharacter.SpecialAttacks[this.clickedNumber.Value];
            }
            else
            {
                oldAttack = null;
            }

            if (oldAttack != null)
            {
                //Draw the old attack stuff
                batch.DrawString(font, oldAttack.AttackName, oldName, Alignment.Center, Color.Black);
                batch.DrawString(font, "Old Attack", oldText, Alignment.Center, Color.White);

                foreach (var icons in oldIcons)
                {
                    switch (icons.Item1)
                    {
                        case SpecialAttackType.ACCURACY:
                            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.ACCURATE_STRIKE), icons.Item2, Color.Black);
                            break;
                        case SpecialAttackType.ATTACKS:
                            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.RAPID_STRIKES), icons.Item2, Color.Black);
                            break;
                        case SpecialAttackType.BLEED:
                            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.BLEEDING_STRIKE), icons.Item2, Color.Black);
                            break;
                        case SpecialAttackType.DAMAGE:
                            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.POWER_STRIKE), icons.Item2, Color.Black);
                            break;
                        case SpecialAttackType.PIERCING:
                            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.ARMOUR_PIERCING_STRIKE), icons.Item2, Color.Black);
                            break;
                        case SpecialAttackType.PUSH:
                            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.PUSHBACK), icons.Item2, Color.Black);
                            break;
                        case SpecialAttackType.STUN:
                            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.STUNNING_STRIKE), icons.Item2, Color.Black);
                            break;
                        case SpecialAttackType.SUNDER:
                            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.SUNDER), icons.Item2, Color.Black);
                            break;
                        case SpecialAttackType.TARGETS:
                            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.WHIRLWIND), icons.Item2, Color.Black);
                            break;
                    }
                }

                foreach (var details in oldDetails)
                {
                    DRObjects.ActorHandling.SpecialAttacks.Effect effect = oldAttack.Effects.FirstOrDefault(e => e.EffectType == details.Item1);

                    string display = "--";

                    if (effect != null)
                    {
                        display = effect.EffectValue.ToString();
                    }

                    batch.DrawString(font, display, details.Item2, Alignment.Center, Color.Black);
                }

            }
            else if (this.clickedNumber != null)
            {
                //This means a non-selected one is selected
                batch.DrawString(font, "Empty Slot", oldName, Alignment.Center, Color.Black);
                batch.DrawString(font, "Old Attack", oldText, Alignment.Center, Color.White);
            }
        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionType? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            throw new NotImplementedException();
        }

        public void HandleMouseOver(int x, int y)
        {
            return;
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionType? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            actionType = null;
            args = null;
            coord = null;
            destroy = false;
            return false; //never handle this
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return rect;
        }

        public void PerformDrag(int deltaX, int deltaY)
        {
            this.locationX += deltaX;
            this.locationY += deltaY;

            this.rect = new Rectangle(this.locationX, this.locationY, 400, 300);

            this.borderRect = new Rectangle(this.rect.X - 2, this.rect.Y - 2, this.rect.Width + 4, this.rect.Height + 4);

            this.slotBackground = new Rectangle(this.locationX, this.locationY, 400, 80);

            this.slotRectangles = new Rectangle[5];

            for (int i = 0; i < slotRectangles.Length; i++)
            {
                slotRectangles[i] = new Rectangle(locationX + (80 * i), locationY, 80, 80);
            }

            this.newText = new Rectangle(locationX, locationY + 30 + 50, 200, 30);
            this.newName = new Rectangle(locationX, locationY + 60 + 50, 200, 30);

            this.newIcons = new List<Tuple<SpecialAttackType, Rectangle>>();
            this.newDetails = new List<Tuple<SpecialAttackType, Rectangle>>();

            this.newBackground = new Rectangle(locationX +10, locationY + 85, 180, 230 - 70);

            //Icons

            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.BLEED, new Rectangle(locationX + 10, locationY + 90 +50, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ACCURACY, new Rectangle(locationX + 70, locationY + 90 + 50, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.STUN, new Rectangle(locationX + 130, locationY + 90 + 50, 30, 30)));

            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.DAMAGE, new Rectangle(locationX + 10, locationY + 120 + 50, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ATTACKS, new Rectangle(locationX + 70, locationY + 120 + 50, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PIERCING, new Rectangle(locationX + 130, locationY + 120 + 50, 30, 30)));

            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.SUNDER, new Rectangle(locationX + 10, locationY + 150 + 50, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PUSH, new Rectangle(locationX + 70, locationY + 150 + 50, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.TARGETS, new Rectangle(locationX + 130, locationY + 150 + 50, 30, 30)));

            //Details

            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.BLEED, new Rectangle(locationX + 40, locationY + 90 + 50, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ACCURACY, new Rectangle(locationX + 100, locationY + 90 + 50, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.STUN, new Rectangle(locationX + 160, locationY + 90 + 50, 30, 30)));

            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.DAMAGE, new Rectangle(locationX + 40, locationY + 120 + 50, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ATTACKS, new Rectangle(locationX + 100, locationY + 120 + 50, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PIERCING, new Rectangle(locationX + 160, locationY + 120 + 50, 30, 30)));

            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.SUNDER, new Rectangle(locationX + 40, locationY + 150 + 50, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PUSH, new Rectangle(locationX + 100, locationY + 150 + 50, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.TARGETS, new Rectangle(locationX + 160, locationY + 150 + 50, 30, 30)));

            this.oldText = new Rectangle(locationX + 200, locationY + 30 + 50, 200, 30);
            this.oldName = new Rectangle(locationX + 200, locationY + 50 + 60, 200, 30);

            this.oldIcons = new List<Tuple<SpecialAttackType, Rectangle>>();
            this.oldDetails = new List<Tuple<SpecialAttackType, Rectangle>>();

            this.oldBackground = new Rectangle(locationX + 210, locationY + 85, 180, 230 - 70);

            //Icons

            this.oldIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.BLEED, new Rectangle(locationX + 10 + 200, locationY + 90 + 50, 30, 30)));
            this.oldIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ACCURACY, new Rectangle(locationX + 70 + 200, locationY + 90 + 50, 30, 30)));
            this.oldIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.STUN, new Rectangle(locationX + 130 + 200, locationY + 90 + 50, 30, 30)));

            this.oldIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.DAMAGE, new Rectangle(locationX + 10 + 200, locationY + 120 + 50, 30, 30)));
            this.oldIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ATTACKS, new Rectangle(locationX + 70 + 200, locationY + 120 + 50, 30, 30)));
            this.oldIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PIERCING, new Rectangle(locationX + 130 + 200, locationY + 120 + 50, 30, 30)));

            this.oldIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.SUNDER, new Rectangle(locationX + 10 + 200, locationY + 150 + 50, 30, 30)));
            this.oldIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PUSH, new Rectangle(locationX + 70 + 200, locationY + 150 + 50, 30, 30)));
            this.oldIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.TARGETS, new Rectangle(locationX + 130 + 200, locationY + 150 + 50, 30, 30)));

            //Details

            this.oldDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.BLEED, new Rectangle(locationX + 40 + 200, locationY + 90 + 50, 30, 30)));
            this.oldDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ACCURACY, new Rectangle(locationX + 100 + 200, locationY + 90 + 50, 30, 30)));
            this.oldDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.STUN, new Rectangle(locationX + 160 + 200, locationY + 90 + 50, 30, 30)));
         
            this.oldDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.DAMAGE, new Rectangle(locationX + 40 + 200, locationY + 120 + 50, 30, 30)));
            this.oldDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ATTACKS, new Rectangle(locationX + 100 + 200, locationY + 120 + 50, 30, 30)));
            this.oldDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PIERCING, new Rectangle(locationX + 160 + 200, locationY + 120 + 50, 30, 30)));
         
            this.oldDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.SUNDER, new Rectangle(locationX + 40 + 200, locationY + 150 + 50, 30, 30)));
            this.oldDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PUSH, new Rectangle(locationX + 100 + 200, locationY + 150 + 50, 30, 30)));
            this.oldDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.TARGETS, new Rectangle(locationX + 160 + 200, locationY + 150 + 50, 30, 30)));


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
                throw new NotImplementedException();
            }
        }
    }
}
