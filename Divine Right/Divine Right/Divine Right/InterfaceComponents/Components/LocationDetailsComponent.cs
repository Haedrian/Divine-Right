using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects.Items.Archetypes.Global;
using Divine_Right.HelperFunctions;
using DRObjects.Graphics;
using DRObjects.Enums;
using Microsoft.Xna.Framework.Graphics;
using DivineRightGame.SettlementHandling;
using DRObjects.Settlements.Districts;

namespace Divine_Right.InterfaceComponents.Components
{
    public class LocationDetailsComponent : IGameInterfaceComponent
    {
        private Rectangle rect;
        private Rectangle bannerRect;
        private Rectangle nameRect;
        private Rectangle titleRect;
        private Rectangle globalCoordinates;
        List<Rectangle> districtRectangles;

        private int x;
        private int y;

        private Settlement settlement;
        private bool visible;

        public LocationDetailsComponent(Settlement settlement, int x, int y)
        {
            this.settlement = settlement;
            this.x = x;
            this.y = y;

            this.PerformDrag(0, 0);
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            var banner = SpriteManager.GetSprite(InterfaceSpriteName.BANNER_GREEN);
            var wood = SpriteManager.GetSprite(InterfaceSpriteName.WOOD_TEXTURE);
            var font = content.Load<SpriteFont>("fonts/h1");
            var textFont = content.Load<SpriteFont>("fonts/textfeedbackfont");

            batch.Draw(content, wood, rect, Color.White);
            batch.Draw(content, banner, bannerRect , Color.White);
            batch.DrawString(font, char.ToUpper(settlement.SettlementType.ToString().ToLower()[0]) + settlement.SettlementType.ToString().ToLower().Substring(1) + " of ", titleRect, Alignment.Center, Color.White);
            batch.DrawString(font, settlement.Name , nameRect, Alignment.Center, Color.GhostWhite);
            batch.DrawString(textFont, "( " + settlement.Coordinate.X + "," + settlement.Coordinate.Y + " )", globalCoordinates, Alignment.Center, Color.White);

            var box = SpriteManager.GetSprite(InterfaceSpriteName.DISTRICT_BOX);
            var star = SpriteManager.GetSprite(InterfaceSpriteName.DISTRICT_STAR);

            //Let's draw the districts
            for (int i = 0; i < settlement.Districts.Count; i++)
            {
                //Put a box in the rectangle
                batch.Draw(content, box, districtRectangles[i], Color.Black);

                //Draw the icon inside it
                batch.Draw(content, settlement.Districts[i].GetInterfaceSprite(), new Rectangle(districtRectangles[i].X + 5, districtRectangles[i].Y, 40, 40), Color.Black);

                //10x10 stars!
                //10x10 with a 7 gap

                //What level is it?
                int level = settlement.Districts[i].Level;

                if (level >= 1)
                {
                    batch.Draw(content, star, new Rectangle(districtRectangles[i].X+3, districtRectangles[i].Y + 37, 10, 10),Color.Black);
                }

                if (level >= 2)
                {
                    batch.Draw(content, star, new Rectangle(districtRectangles[i].X + 20, districtRectangles[i].Y + 37, 10, 10), Color.Black);
                }

                if (level >= 3)
                {
                    batch.Draw(content, star, new Rectangle(districtRectangles[i].X + 37, districtRectangles[i].Y + 37, 10, 10), Color.Black);
                }

            }
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

        public void PerformDrag(int deltaX, int deltaY)
        {
            x+=deltaX;
            y+=deltaY;

            this.rect = new Rectangle(x, y, 150, 450);

            this.bannerRect = new Rectangle(x, y, 150, 100);
            this.titleRect = new Rectangle(x, y+15, 150, 25);
            this.nameRect = new Rectangle(x, y+40, 150, 25);
            this.globalCoordinates = new Rectangle(x, y + 65, 150, 10);

            this.districtRectangles = new List<Rectangle>();

            int districtX= 0;
            int districtY = 100; 

            foreach (District district in settlement.Districts)
            {
                //Create the boxes
                districtRectangles.Add(new Rectangle(districtX+x, districtY+y, 50, 50));

                districtX += 50;

                if (districtX >= 150) //edge of screen
                {
                    districtX = 0;
                    districtY += 60;
                }
            }
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
    }
}
