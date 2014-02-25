using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Graphics;

namespace Divine_Right.InterfaceComponents.Components
{
    /// <summary>
    /// An interface component which shows the health of the actor in question
    /// </summary>
    public class HealthDisplayComponent:
        IGameInterfaceComponent
    {
        #region Properties
        protected int locationX;
        protected int locationY;
        private Actor actor;

        private Rectangle rect;
        private Rectangle headRect;
        private Rectangle leftArmRect;
        private Rectangle chestRect;
        private Rectangle rightArmRect;
        private Rectangle legRect;

        private Texture2D blackTexture;
        #endregion

        /// <summary>
        /// Creates a new HealthDisplayComponent for a particular actor at a particular position
        /// </summary>
        /// <param name="locationX"></param>
        /// <param name="locationY"></param>
        /// <param name="actor"></param>
        public HealthDisplayComponent(int locationX, int locationY, Actor actor)
        {
            this.locationX = locationX;
            this.locationY = locationY;
            this.actor = actor;

            rect = new Rectangle(locationX, locationY, 100, 209); //100 x 200 ?

            //Divide everything by 2.5
            headRect = new Rectangle(locationX, locationY, 100, 37);
            leftArmRect = new Rectangle(locationX,locationY + 37, 28, 84);
            chestRect = new Rectangle(locationX + 28, locationY + 37, 43, 84);
            rightArmRect = new Rectangle(locationX + 71, locationY + 37, 28, 84);
            legRect = new Rectangle(locationX, locationY + 121, 100, 87);
            
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            //Time to draw :)
            if (blackTexture == null)
            {
                //Create it, then cache it
                blackTexture = new Texture2D(batch.GraphicsDevice, 1, 1);
                blackTexture.SetData(new Color[] { Color.Black });
            }

            //Draw the background
            var scrollBackground = SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE);

            batch.Draw(content.Load<Texture2D>(scrollBackground.path), rect, scrollBackground.sourceRectangle, Color.White);

            
            //Start with head
            var head = SpriteManager.GetSprite(InterfaceSpriteName.HEAD_GOOD);
            var leftArm = SpriteManager.GetSprite(InterfaceSpriteName.LEFT_ARM_GOOD);
            var chest = SpriteManager.GetSprite(InterfaceSpriteName.CHEST_GOOD);
            var rightArm = SpriteManager.GetSprite(InterfaceSpriteName.RIGHT_ARM_GOOD);
            var legs = SpriteManager.GetSprite(InterfaceSpriteName.LEGS_GOOD);

           // batch.Draw(content.Load<Texture2D>(head.path,))
            batch.Draw(content.Load<Texture2D>(head.path), headRect, head.sourceRectangle, Color.White);
            batch.Draw(content.Load<Texture2D>(leftArm.path), leftArmRect, leftArm.sourceRectangle, Color.White);
            batch.Draw(content.Load<Texture2D>(chest.path), chestRect, chest.sourceRectangle, Color.White);
            batch.Draw(content.Load<Texture2D>(rightArm.path), rightArmRect, rightArm.sourceRectangle, Color.White);
            batch.Draw(content.Load<Texture2D>(legs.path), legRect, legs.sourceRectangle, Color.White);

        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            //This does nothing

            args = null;
            coord = null;
            destroy = false;
            actionType = null;

            return true; //but it won't allow click through
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            //This does nothing
            args = null;
            coord = null;
            destroy = false;
            actionType = null;

            return false;
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return rect;
        }

        public bool IsModal()
        {
            return false;
        }
    }
}
