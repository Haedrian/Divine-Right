using DivineRightGame;
using DRObjects;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Items.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Divine_Right.HelperFunctions;

namespace Divine_Right.InterfaceComponents.Components
{
    [Serializable]
    /// <summary>
    /// Component for choosing which tile to throw the item upon
    /// </summary>
    public class ThrowItemComponent
        : IGameInterfaceComponent
    {

        private int locationX;
        private int locationY;

        private int centerX;
        private int centerY;

        private InventoryItem item;

        private Rectangle rect;
        private List<Tuple<Rectangle, MapCoordinate, bool>> rectangles;

        public ThrowItemComponent(int X, int Y, InventoryItem item)
        {
            locationX = X;
            locationY = Y;
            this.item = item;

            centerX = locationX + 250;
            centerY = locationY + 250;

            //Since this can't be dragged, tis safe to put the designing code in 'ere

            rect = new Rectangle(locationX, locationY, 500, 500);

            //This component will draw a number of rectangles over areas where you're allowed to throw something
            //So go through each tile in a 10x10 square and see if 
            //a) They're within the site range you have
            //b) You have a direct line of site to them
            //c) Yeah I'm putting a limit at 5 squares. Sue me.

            rectangles = new List<Tuple<Rectangle, MapCoordinate, bool>>();

            MapCoordinate center = GameState.PlayerCharacter.MapCharacter.Coordinate;

            for (int x = -5; x < 6; x++)
            {
                for (int y = -5; y < 6; y++)
                {
                    MapCoordinate mc = new MapCoordinate(center.X + x, center.Y - y, 0, MapType.LOCAL);

                    var block = GameState.LocalMap.GetBlockAtCoordinate(mc);

                    //Is it an airblock ?
                    if (block == null || block.GetType().Equals(typeof(Air)))
                    {
                        continue;
                    }

                    //Is it within our sight range?

                    if ((mc - center) > GameState.PlayerCharacter.LineOfSight)
                    {
                        //Nope
                        continue;
                    }

                    if (mc.Equals(center))
                    {
                        continue;
                        //don't throw it on yourself
                    }

                    //Do we have LoS to it?
                    if (GameState.LocalMap.HasDirectPath(center, mc))
                    {
                        //Yes!
                        Rectangle rectangle = new Rectangle(centerX + (x * 50), centerY + (y * 50), 50, 50);
                        rectangles.Add(new Tuple<Rectangle, MapCoordinate, Boolean>(rectangle, mc, false));
                    }

                }
            }
        }

        public void Draw(Microsoft.Xna.Framework.Content.ContentManager content, Microsoft.Xna.Framework.Graphics.SpriteBatch batch)
        {
            SpriteData box = SpriteManager.GetSprite(ColourSpriteName.GREEN_BOXED);

            //Go through each rectangle and draw it
            foreach (var rC in rectangles)
            {
                if (rC.Item3)
                {
                    batch.Draw(content, box, rC.Item1, Color.White * 0.5f);
                }
                else
                {
                    batch.Draw(content, box, rC.Item1, Color.Blue * 0.15f);
                }
            }
        }

        public bool HandleClick(int x, int y, Objects.Enums.MouseActionEnum mouseAction, out DRObjects.Enums.ActionType? actionType, out DRObjects.Enums.InternalActionEnum? internalActionType, out object[] args, out DRObjects.MapItem item, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            item = null;
            args = null;
            coord = null;
            destroy = false;
            actionType = null;
            internalActionType = null;

            return true; //modal!
        }

        public void HandleMouseOver(int x, int y)
        {
            for (int i = 0; i < rectangles.Count; i++)
            {
                //Apparently they're read only. I don't know

                rectangles[i] = new Tuple<Rectangle, MapCoordinate, bool>(rectangles[i].Item1, rectangles[i].Item2, false);

                if (rectangles[i].Item1.Contains(x, y))
                {
                    rectangles[i] = new Tuple<Rectangle, MapCoordinate, bool>(rectangles[i].Item1, rectangles[i].Item2, true); //moused over
                }

            }
        }

        public bool HandleKeyboard(Microsoft.Xna.Framework.Input.KeyboardState keyboard, out DRObjects.Enums.ActionType? actionType, out object[] args, out DRObjects.MapCoordinate coord, out bool destroy)
        {
            item = null;
            args = null;
            coord = null;
            destroy = false;
            actionType = null;

            return true; //modal!
        }

        public Microsoft.Xna.Framework.Rectangle ReturnLocation()
        {
            return rect;
        }

        public void PerformDrag(int deltaX, int deltaY)
        {
            return; //NO DRAGGING
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
                return;
            }
        }
    }
}
