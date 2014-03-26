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
        private Rectangle borderRect;

        private Rectangle poorRect;
        private Rectangle middleRect;
        private Rectangle richRect;

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
            var white = SpriteManager.GetSprite(ColourSpriteName.WHITE);
            var person = SpriteManager.GetSprite(InterfaceSpriteName.MAN);
            var halfPerson = SpriteManager.GetSprite(InterfaceSpriteName.HALFMAN);

            batch.Draw(content, white, borderRect , Color.DarkGray);
            batch.Draw(content, wood, rect, Color.White);
            batch.Draw(content, banner, bannerRect , Color.White);
            batch.DrawString(font, char.ToUpper(settlement.SettlementType.ToString().ToLower()[0]) + settlement.SettlementType.ToString().ToLower().Substring(1) + " of ", titleRect, Alignment.Center, Color.White);
            batch.DrawString(font, settlement.Name , nameRect, Alignment.Center, Color.GhostWhite);
            batch.DrawString(textFont, "( " + settlement.Coordinate.X + "," + settlement.Coordinate.Y + " )", globalCoordinates, Alignment.Center, Color.White);

            //draw the properties
            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.SLUM_HOUSING), poorRect, Color.Black);
            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.MIDDLE_HOUSING), middleRect, Color.Black);
            batch.Draw(content, SpriteManager.GetSprite(InterfaceSpriteName.RICH_HOUSING), richRect, Color.Black);

            //Put the person icons
            int lastI = -1;

            for (int i = 0; i < settlement.PoorPercentage/10; i += 1)
            {
                batch.Draw(content,person,new Rectangle(x+60+(10*i),poorRect.Y+10,10,10),Color.Black);
                lastI = i;
            }

            if (settlement.PoorPercentage%10 >= 5)
            {
                //and a bit
                batch.Draw(content, halfPerson, new Rectangle(x + 60 + (10 * ++lastI), poorRect.Y+10, 5, 10), Color.Black);
            }

            lastI = -1;

            //Put the person icons - middle
            for (int i = 0; i < settlement.MiddlePercentage / 10; i += 1)
            {
                batch.Draw(content, person, new Rectangle(x + 60 + (10 * i), middleRect.Y + 10, 10, 10), Color.Black);
                lastI = i;
            }

            if (settlement.MiddlePercentage % 10 >= 5)
            {
                //and a bit
                batch.Draw(content, halfPerson, new Rectangle(x + 60 + (10 * ++lastI), middleRect.Y + 10, 5, 10), Color.Black);
            }

            lastI = -1;

            //Put the person icons - rich
            for (int i = 0; i < settlement.RichPercentage / 10; i += 1)
            {
                batch.Draw(content, person, new Rectangle(x + 60 + (10 * i), richRect.Y + 10, 10, 10), Color.Black);
                lastI = i;
            }

            if (settlement.PoorPercentage % 10 >= 5)
            {
                //and a bit
                batch.Draw(content, halfPerson, new Rectangle(x + 60 + (10 * ++lastI), richRect.Y + 10, 5, 10), Color.Black);
            }

            var box = SpriteManager.GetSprite(InterfaceSpriteName.DISTRICT_BOX);
            var star = SpriteManager.GetSprite(InterfaceSpriteName.DISTRICT_STAR);

            //Let's draw the districts
            for (int i = 0; i < settlement.Districts.Count; i++)
            {
                //Put a box in the rectangle
                //batch.Draw(content, box, districtRectangles[i], Color.Black);

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

            this.rect = new Rectangle(x, y, 170, 450);

            this.bannerRect = new Rectangle(x, y, 170, 100);
            this.titleRect = new Rectangle(x, y+10, 170, 25);
            this.nameRect = new Rectangle(x, y+40, 170, 25);
            this.globalCoordinates = new Rectangle(x, y + 65, 170, 10);
            this.borderRect = new Rectangle(rect.X - 2, rect.Y - 2, rect.Width + 4, rect.Height + 4);

            poorRect = new Rectangle(x+10, y+100, 30, 30);
            middleRect = new Rectangle(x+10, y + 130, 30, 30);
            richRect = new Rectangle(x+10,y+160,30,30);

            this.districtRectangles = new List<Rectangle>();

            int districtX= 0;
            int districtY = 210; 

            foreach (District district in settlement.Districts)
            {
                //Create the boxes
                districtRectangles.Add(new Rectangle(districtX+x+10, districtY+y, 50, 50));

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
