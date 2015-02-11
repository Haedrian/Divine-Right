using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.SpecialAttacks;
using Microsoft.Xna.Framework;

namespace Divine_Right.InterfaceComponents.Components
{
    public class CombatManualComponent
        : IGameInterfaceComponent
    {
        private Rectangle rect;
        private Rectangle borderRect;

        private Rectangle[] slotRectangles;
        private Rectangle newText;
        private Rectangle oldText;

        private Rectangle newName;
        private Rectangle oldName;

        private List<Tuple<SpecialAttackType,Rectangle>> newIcons;
        private List<Tuple<SpecialAttackType, Rectangle>> newDetails;

        private List<Tuple<SpecialAttackType, Rectangle>> oldIcons;
        private List<Tuple<SpecialAttackType, Rectangle>> oldDetails;

        private Rectangle learnRect;
        private Rectangle cancelRect;

        private int locationX;
        private int locationY;

        private int clickedNumber;
        private SpecialAttack newAttack;
        private SpecialAttack oldAttack;

        public CombatManualComponent(SpecialAttack newAttack)
        {
            this.newAttack = newAttack;

            this.locationX = 100;
            this.locationY = 100;

            this.PerformDrag(0, 0);
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            throw new NotImplementedException();
        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionType? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            throw new NotImplementedException();
        }

        public void HandleMouseOver(int x, int y)
        {
            throw new NotImplementedException();
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionType? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            throw new NotImplementedException();
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            throw new NotImplementedException();
        }

        public void PerformDrag(int deltaX, int deltaY)
        {
            this.locationX += deltaX;
            this.locationY += deltaY;

            this.rect = new Rectangle(this.locationX, this.locationY, 400, 300);

            this.borderRect = new Rectangle(this.rect.X - 2, this.rect.Y - 2, this.rect.Width + 4, this.rect.Width + 4);

            this.slotRectangles = new Rectangle[5];

            for (int i=0; i < slotRectangles.Length; i++)
            {
                slotRectangles[i] = new Rectangle(locationX + (80 * i), locationY, 80,30);
            }

            this.newText = new Rectangle(locationX, 30, 200, 30);
            this.newName = new Rectangle(locationX, 60, 200, 30);

            this.newIcons = new List<Tuple<SpecialAttackType, Rectangle>>();

            //Icons

            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.BLEED, new Rectangle(locationX + 10, locationY + 90, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ACCURACY, new Rectangle(locationX + 70, locationY + 90, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.STUN, new Rectangle(locationX + 130, locationY + 90, 30, 30)));

            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.DAMAGE, new Rectangle(locationX + 10, locationY + 120, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ATTACKS, new Rectangle(locationX + 70, locationY + 120, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PIERCING, new Rectangle(locationX + 130, locationY + 120, 30, 30)));

            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.SUNDER, new Rectangle(locationX + 10, locationY + 150, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PUSH, new Rectangle(locationX + 70, locationY + 150, 30, 30)));
            this.newIcons.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.TARGETS, new Rectangle(locationX + 130, locationY + 150, 30, 30)));

            //Details

            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.BLEED, new Rectangle(locationX + 40, locationY + 90, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ACCURACY, new Rectangle(locationX + 100, locationY + 90, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.STUN, new Rectangle(locationX + 160, locationY + 90, 30, 30)));
       
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.DAMAGE, new Rectangle(locationX + 40, locationY + 120, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.ATTACKS, new Rectangle(locationX + 100, locationY + 120, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PIERCING, new Rectangle(locationX + 160, locationY + 120, 30, 30)));

            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.SUNDER, new Rectangle(locationX + 40, locationY + 150, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.PUSH, new Rectangle(locationX + 100, locationY + 150, 30, 30)));
            this.newDetails.Add(new Tuple<SpecialAttackType, Rectangle>(SpecialAttackType.TARGETS, new Rectangle(locationX + 160, locationY + 150, 30, 30)));

            this.oldText = new Rectangle(locationX + 200, 30, 200, 30);

        }

        public bool IsModal()
        {
            throw new NotImplementedException();
        }

        public bool Visible
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
