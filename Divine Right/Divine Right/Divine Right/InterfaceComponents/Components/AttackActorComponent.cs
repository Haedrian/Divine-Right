using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Graphics;
using Divine_Right.HelperFunctions;
using DivineRightGame.CombatHandling;

namespace Divine_Right.InterfaceComponents.Components
{
    public class AttackActorComponent:
        IGameInterfaceComponent
    {
        #region Properties
        private bool visible;

        protected int locationX;
        protected int locationY;
        private Actor attacker;
        public Actor TargetActor{get;set;}

        private Rectangle rect;

        //Drawing stuff
        private Rectangle headRect;
        private Rectangle leftArmRect;
        private Rectangle chestRect;
        private Rectangle rightArmRect;
        private Rectangle legRect;

        private Rectangle headPercentageRect;
        private Rectangle leftArmPercentageRect;
        private Rectangle chestPercentageRect;
        private Rectangle rightArmPercentageRect;
        private Rectangle legsPercentageRect;

        private Rectangle enemyNameRect;
        private Rectangle enemyWeaponIconRect;
        private Rectangle enemyArmourIconRect;

        private Rectangle enemyWeaponRect;
        private Rectangle enemyArmourRect;

        private Rectangle statusTitleRect;
        private Rectangle defensiveStatusRect;
        private Rectangle aggressiveStatusRect;
        private Rectangle normalStatusRect;
        private Rectangle tinyStatusRect;
        private Rectangle statusRect;

        private Rectangle seperatorRect;

        private Rectangle targetHealthStatusRect;
        private Rectangle attackButtonRectangle;

        private SpriteFont font;

        #endregion

        public AttackActorComponent(int locationX, int locationY, Actor attacker, Actor target)
        {
            this.locationX = locationX;
            this.locationY = locationY;
            this.attacker = attacker;
            this.TargetActor = target;

           //Force a move
            this.PerformDrag(locationX, locationY);
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

            //Draw the background
            var scrollBackground = SpriteManager.GetSprite(InterfaceSpriteName.PAPER_TEXTURE);

            batch.Draw(content.Load<Texture2D>(scrollBackground.path), rect, scrollBackground.sourceRectangle, Color.White);

            //Draw the body parts. 
            //Let's determine how injured each part is.
            var health = TargetActor.Anatomy;

            var head = SpriteManager.GetSprite(InterfaceSpriteName.HEAD);
            var leftArm = SpriteManager.GetSprite(InterfaceSpriteName.LEFT_ARM);
            var chest = SpriteManager.GetSprite(InterfaceSpriteName.CHEST);
            var rightArm = SpriteManager.GetSprite(InterfaceSpriteName.RIGHT_ARM);
            var legs = SpriteManager.GetSprite(InterfaceSpriteName.LEGS);

            batch.Draw(content.Load<Texture2D>(head.path), headRect, head.sourceRectangle, this.GetColour(health.Head, health.HeadMax));
            batch.Draw(content.Load<Texture2D>(leftArm.path), leftArmRect, leftArm.sourceRectangle, this.GetColour(health.LeftArm, health.LeftArmMax));
            batch.Draw(content.Load<Texture2D>(rightArm.path), rightArmRect, rightArm.sourceRectangle, this.GetColour(health.RightArm, health.RightArmMax));
            batch.Draw(content.Load<Texture2D>(chest.path), chestRect, chest.sourceRectangle, this.GetColour(health.Chest, health.ChestMax));
            batch.Draw(content.Load<Texture2D>(legs.path), legRect, legs.sourceRectangle, this.GetColour(health.Legs, health.LegsMax));

            //Draw the title and the statuses
           // batch.DrawString(font, "Attack", statusTitleRect, Alignment.Center, Color.GhostWhite);

            var shield = SpriteManager.GetSprite(InterfaceSpriteName.DEFENSE);
            var balanced = SpriteManager.GetSprite(InterfaceSpriteName.DEX);
            var sword = SpriteManager.GetSprite(InterfaceSpriteName.SWORD);
            var agressive = SpriteManager.GetSprite(InterfaceSpriteName.MACE);

            //If we have a particular chosen stance, draw a white box underneath to show its selected
            var seperator = SpriteManager.GetSprite(ColourSpriteName.WHITE);

            //pick the stance
            string stance = "Unknown Stance";


            switch (attacker.CombatStance)
            {
                case DRObjects.ActorHandling.ActorStance.AGGRESSIVE:
                    stance = "Aggressive";
                    batch.Draw(content.Load<Texture2D>(seperator.path), aggressiveStatusRect, seperator.sourceRectangle, Color.GhostWhite);
                    break;
                case DRObjects.ActorHandling.ActorStance.COMPLETE_AGGRESSIVE:
                    stance = "Berzerk"; break;
                case DRObjects.ActorHandling.ActorStance.COMPLETE_DEFENSIVE:
                    stance = "Hunkered"; break;
                case DRObjects.ActorHandling.ActorStance.DEFENSIVE:
                    stance = "Defensive";
                    batch.Draw(content.Load<Texture2D>(seperator.path), defensiveStatusRect, seperator.sourceRectangle, Color.GhostWhite);
                    break;
                case DRObjects.ActorHandling.ActorStance.NEUTRAL:
                    stance = "Neutral";
                    batch.Draw(content.Load<Texture2D>(seperator.path), normalStatusRect, seperator.sourceRectangle, Color.GhostWhite);
                    break;
                default:
                    throw new NotImplementedException("No text was prepared for stance " + stance);
            }


            batch.Draw(content.Load<Texture2D>(shield.path), defensiveStatusRect, shield.sourceRectangle, Color.White);
            batch.Draw(content.Load<Texture2D>(shield.path), tinyStatusRect, shield.sourceRectangle, Color.White);
            batch.Draw(content.Load<Texture2D>(sword.path), normalStatusRect, sword.sourceRectangle, Color.White);
            batch.Draw(content.Load<Texture2D>(agressive.path), aggressiveStatusRect, agressive.sourceRectangle, Color.White);

            batch.DrawString(font,stance,statusRect,Alignment.Center,Color.Black);

            batch.Draw(content.Load <Texture2D>(seperator.path), seperatorRect, seperator.sourceRectangle, Color.DarkGray);

            batch.DrawString(font, TargetActor.EnemyData == null ? "Unknown" : TargetActor.EnemyData.EnemyName, enemyNameRect, Alignment.Center, Color.White);
            batch.Draw(content.Load<Texture2D>(shield.path), enemyArmourIconRect, shield.sourceRectangle, Color.White);
            batch.Draw(content.Load<Texture2D>(sword.path), enemyWeaponIconRect, sword.sourceRectangle, Color.White);

            batch.DrawString(font, "TODO", enemyWeaponRect, Alignment.Center, Color.Black);
            batch.DrawString(font, "Later", enemyArmourRect, Alignment.Center, Color.Black);

            batch.Draw(content.Load<Texture2D>(scrollBackground.path), attackButtonRectangle, scrollBackground.sourceRectangle, Color.DarkGray);
            batch.DrawString(font, "ATTACK", attackButtonRectangle, Alignment.Center, Color.White);

            batch.DrawString(font,CombatManager.CalculateHitPercentage(attacker,TargetActor,AttackLocation.HEAD) + "%", headPercentageRect, Alignment.Center, Color.Red);
            batch.DrawString(font, CombatManager.CalculateHitPercentage(attacker, TargetActor, AttackLocation.LEFT_ARM) + "%", leftArmPercentageRect, Alignment.Center, Color.Red);
            batch.DrawString(font, CombatManager.CalculateHitPercentage(attacker, TargetActor, AttackLocation.CHEST) + "%", chestPercentageRect, Alignment.Center, Color.Red);
            batch.DrawString(font, CombatManager.CalculateHitPercentage(attacker, TargetActor, AttackLocation.RIGHT_ARM) + "%", rightArmPercentageRect, Alignment.Center, Color.Red);
            batch.DrawString(font, CombatManager.CalculateHitPercentage(attacker, TargetActor, AttackLocation.LEGS) + "%", legsPercentageRect, Alignment.Center, Color.Red);

        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            Point point = new Point(x,y);

            args = null;
            coord = null;
            destroy = false;
            actionType = null;

            if (!visible)
            {
                return false; //don't do anything
            }

            //If we pressed a stance button, just change it
            if (this.defensiveStatusRect.Contains(point))
            {
                //Swap stance to defensive
                attacker.CombatStance = DRObjects.ActorHandling.ActorStance.DEFENSIVE;
                return true;
            }
            else if (this.aggressiveStatusRect.Contains(point))
            {
                //Aggressive
                attacker.CombatStance = DRObjects.ActorHandling.ActorStance.AGGRESSIVE;
                return true;
            }
            else if (this.normalStatusRect.Contains(point))
            {
                //Normal
                attacker.CombatStance = DRObjects.ActorHandling.ActorStance.NEUTRAL;
                return true;
            }


            return visible; //If it's visible - block it. Otherwise do nothing
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionTypeEnum? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
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
            //Update locationX and Y

            locationX += deltaX;
            locationY += deltaY;

            //Create the rectangle
            this.rect = new Rectangle(locationX, locationY, 300, 200);

            //Divide everything by 2.5
            headRect = new Rectangle(locationX + 220, locationY, 43, 38);
            leftArmRect = new Rectangle(locationX + 200, locationY + 35, 33, 85);
            chestRect = new Rectangle(locationX + 227, locationY + 37, 55, 72);
            rightArmRect = new Rectangle(locationX + 260, locationY + 36, 33, 85);
            legRect = new Rectangle(locationX + 223, locationY + 108, 50, 87);

            //Let's put everything in place
            statusTitleRect = new Rectangle(locationX +10,locationY+ 10, 110, 10);

            defensiveStatusRect = new Rectangle(locationX + 10, locationY + 30, 30, 30);
            normalStatusRect = new Rectangle(locationX + 50, locationY + 30, 30, 30);
            tinyStatusRect = new Rectangle(locationX + 55, locationY + 35, 20, 20);
            aggressiveStatusRect = new Rectangle(locationX + 90, locationY + 30, 30, 30);

            statusRect = new Rectangle(locationX + 10, locationY + 70, 110, 10);

            seperatorRect = new Rectangle(locationX + 130, locationY + 10, 2, 180);

            enemyNameRect = new Rectangle(locationX + 140, locationY + 10, 50, 10);

            enemyArmourIconRect = new Rectangle(locationX + 140, locationY + 30, 15, 15);
            enemyWeaponIconRect = new Rectangle(locationX + 140, locationY + 55, 15, 15);

            enemyArmourRect = new Rectangle(locationX + 155, locationY + 30, 50, 15);
            enemyWeaponRect = new Rectangle(locationX + 155, locationY + 55, 50, 15);

            attackButtonRectangle = new Rectangle(locationX + 80, locationY + 160, 100, 30);

            headPercentageRect = new Rectangle(locationX + 225, locationY, 43, 38);
            leftArmPercentageRect = new Rectangle(locationX + 200, locationY + 35, 33, 85);
            chestPercentageRect = new Rectangle(locationX + 220, locationY + 37, 55, 50);
            rightArmPercentageRect = new Rectangle(locationX + 260, locationY + 35, 33, 85);
            legsPercentageRect = new Rectangle(locationX + 223, locationY + 108, 50, 87);
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
    }
}
