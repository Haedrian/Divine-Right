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
