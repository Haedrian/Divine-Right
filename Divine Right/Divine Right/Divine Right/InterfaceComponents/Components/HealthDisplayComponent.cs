using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Graphics;
using DRObjects.Enums;
using Divine_Right.HelperFunctions;

namespace Divine_Right.InterfaceComponents.Components
{
    /// <summary>
    /// An interface component which shows the health of the actor in question
    /// </summary>
    public class HealthDisplayComponent :
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

        private Rectangle bleedingRect;
        private Rectangle stunnedRect;

        private bool visible;
        #endregion

        public bool Visible
        {
            get { return visible; }
            set { visible = value; }
        }

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
            this.visible = true;

            PerformDrag(0, 0);

        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            if (!visible)
            {
                return; //draw nothing
            }

            //Draw the background
            var scrollBackground = SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE);

            batch.Draw(content.Load<Texture2D>(scrollBackground.path), rect, scrollBackground.sourceRectangle, Color.White);

            //Let's determine how injured each part is.
            var health = actor.Anatomy;

            var head = SpriteManager.GetSprite(InterfaceSpriteName.HEAD);
            var leftArm = SpriteManager.GetSprite(InterfaceSpriteName.LEFT_ARM);
            var chest = SpriteManager.GetSprite(InterfaceSpriteName.CHEST);
            var rightArm = SpriteManager.GetSprite(InterfaceSpriteName.RIGHT_ARM);
            var legs = SpriteManager.GetSprite(InterfaceSpriteName.LEGS);
            var spiral = SpriteManager.GetSprite(InterfaceSpriteName.SPIRAL);
            var bleeding = SpriteManager.GetSprite(InterfaceSpriteName.BLEEDING);

            batch.Draw(content.Load<Texture2D>(head.path), headRect, head.sourceRectangle, this.GetColour(health.Head, health.HeadMax));
            batch.Draw(content.Load<Texture2D>(leftArm.path), leftArmRect, leftArm.sourceRectangle, this.GetColour(health.LeftArm, health.LeftArmMax));
            batch.Draw(content.Load<Texture2D>(rightArm.path), rightArmRect, rightArm.sourceRectangle, this.GetColour(health.RightArm, health.RightArmMax));
            batch.Draw(content.Load<Texture2D>(chest.path), chestRect, chest.sourceRectangle, this.GetColour(health.Chest, health.ChestMax));
            batch.Draw(content.Load<Texture2D>(legs.path), legRect, legs.sourceRectangle, this.GetColour(health.Legs, health.LegsMax));

            batch.Draw(content, spiral, stunnedRect, Color.DarkGray);
            batch.Draw(content, bleeding, bleedingRect, Color.Black);
        }

        /// <summary>
        /// Gets the right colour to draw depending on the health
        /// </summary>
        /// <param name="health"></param>
        /// <param name="maxHealth"></param>
        /// <returns></returns>
        private Color GetColour(int health, int maxHealth)
        {
            var currentColour = Color.Blue;
            double healthPercentage = (double)health / maxHealth;

            if (health <= -5) //missing
            {
                currentColour = Color.Transparent;
            }
            else
                if (health <= 0) //destroyed
                {
                    // currentColour = Color.Red;
                    currentColour = Color.Black;
                }
                else
                    if (healthPercentage > 0.66)
                    {
                        //fine
                        currentColour = Color.GhostWhite;
                    }
                    else
                        if (healthPercentage < 0.33)
                        {
                            //Really hurt
                            //currentColour = Color.Orange;
                            currentColour = Color.DarkGray;
                        }
                        else
                            if (healthPercentage < 0.66)
                            {
                                //Hurt
                                //currentColour = Color.Yellow;
                                currentColour = Color.LightGray;
                            }

            return currentColour;

        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            //This does nothing

            args = null;
            coord = null;
            destroy = false;
            actionType = null;
            internalActionType = null;

            return visible; //If it's visible - block it. Otherwise do nothing
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            //This does nothing
            args = null;
            coord = null;
            destroy = false;
            actionType = null;

            return false; //This never does anything
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return rect;
        }

        public bool IsModal()
        {
            return false;
        }


        public void PerformDrag(int x, int y)
        {
            this.locationX += x;
            this.locationY += y;

            //Move everything
            rect = new Rectangle(locationX, locationY, 125, 209); //100 x 200 ?

            //Divide everything by 2.5
            headRect = new Rectangle(locationX + 20, locationY, 43, 38);
            leftArmRect = new Rectangle(locationX, locationY + 35, 33, 85);
            chestRect = new Rectangle(locationX + 27, locationY + 37, 55, 72);
            rightArmRect = new Rectangle(locationX + 60, locationY + 36, 33, 85);
            legRect = new Rectangle(locationX + 23, locationY + 108, 50, 87);

            stunnedRect = new Rectangle(locationX + 90, locationY + 5, 30, 30);
            bleedingRect = new Rectangle(locationX + 90, locationY + 45, 30, 30);
        }
    }
}
