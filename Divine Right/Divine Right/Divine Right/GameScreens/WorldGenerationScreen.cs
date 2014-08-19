using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects;
using Microsoft.Xna.Framework.Graphics;
using DivineRightGame.Managers;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using DivineRightGame;
using DRObjects.GraphicsEngineObjects;
using Divine_Right.GraphicalObjects;
using DRObjects.Enums;
using DRObjects.Graphics;

namespace Divine_Right.GameScreens
{
    class WorldGenerationScreen :
        DrawableGameComponent
    {
        #region Changable Values
        private int TILEWIDTH = 10;
        private int TILEHEIGHT = 10;
        private GlobalOverlay OVERLAY = GlobalOverlay.NONE;

        #endregion
        /// <summary>
        /// The time in miliseconds for a key press to be considered
        /// </summary>
        const int GAMEINPUTDELAY = 100;
        #region Members

        protected Game game;
        protected GraphicsDeviceManager graphics;
        protected MapCoordinate centrePoint = new MapCoordinate();
        protected SpriteBatch spriteBatch;

        //The locations we're looking at right now
        int locationX = WorldGenerationManager.WORLDSIZE / 2;
        int locationY = WorldGenerationManager.WORLDSIZE / 2;
        int previousGameTime = 0;
        int dotCount = 0;

        int PlayableWidth
        {
            get
            {
                return GraphicsDevice.Viewport.Width;
            }

        }
        int PlayableHeight
        {
            get
            {
                return GraphicsDevice.Viewport.Height - 50;

            }

        }
        int TotalTilesWidth
        {
            get
            {
                return (PlayableWidth / TILEWIDTH) - 1; ;
            }


        }
        int TotalTilesHeight
        {
            get
            {
                return (PlayableHeight / TILEHEIGHT) - 1;
            }

        }

        #endregion
        #region Constructor

        public WorldGenerationScreen(Game game, GraphicsDeviceManager graphics)
            : base(game)
        {
            this.game = game;
            this.graphics = graphics;

        }

        #endregion

        public override void Initialize()
        {
            base.Initialize();

            ThreadStart function = new ThreadStart(WorldGenerationManager.GenerateWorld);
            Thread thread = new Thread(function);

            //Start the thread
            thread.Start();

        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (gameTime.TotalGameTime.TotalMilliseconds - previousGameTime < GAMEINPUTDELAY)
            {
                //do nothing
                return;
            }

            previousGameTime = (int)gameTime.TotalGameTime.TotalMilliseconds;

            //Lets see if there are any keyboard keys being pressed

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                locationY += 5;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                locationY -= 5;
            }
            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                locationX -= 5;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                locationX += 5;
            }

            if (keyboardState.IsKeyDown(Keys.OemPlus))
            {
                TILEWIDTH++;
                TILEHEIGHT++;
            }
            else if (keyboardState.IsKeyDown(Keys.OemMinus))
            {
                if (TILEHEIGHT != 1) //this would cause a div by zero
                {
                    TILEHEIGHT--;
                    TILEWIDTH--;
                }
            }

            if (keyboardState.IsKeyDown(Keys.R))
            {
                OVERLAY = GlobalOverlay.REGION;
            }
            else if (keyboardState.IsKeyDown(Keys.T))
            {
                OVERLAY = GlobalOverlay.TEMPERATURE;
            }
            else if (keyboardState.IsKeyDown(Keys.N))
            {
                OVERLAY = GlobalOverlay.NONE;
            }
            else if (keyboardState.IsKeyDown(Keys.P))
            {
                OVERLAY = GlobalOverlay.RAINFALL;
            }
            else if (keyboardState.IsKeyDown(Keys.E))
            {
                OVERLAY = GlobalOverlay.ELEVATION;
            }
            else if (keyboardState.IsKeyDown(Keys.D))
            {
                OVERLAY = GlobalOverlay.DESIRABILITY;
            }
            else if (keyboardState.IsKeyDown(Keys.O))
            {
                OVERLAY = GlobalOverlay.OWNER;
            }
        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicalBlock[] blocks = null;

            //lock so we can access the map

            lock (GlobalMap.lockMe)
            {
                blocks = UserInterfaceManager.GetBlocksAroundPoint(new MapCoordinate(locationX, locationY, 0, DRObjects.Enums.MapTypeEnum.GLOBAL), TotalTilesWidth / 2, TotalTilesHeight / 2, 0, OVERLAY);
            }

            List<InterfaceBlock> iBlocks = this.PrepareGrid(blocks.ToList<GraphicalBlock>());

            //draw them
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            this.DrawGrid(iBlocks);

            //lets also draw the current step and the location of the cursor
            spriteBatch.DrawString(this.game.Content.Load<SpriteFont>("Fonts/TextFeedbackFont"), this.locationX + "," + this.locationY + " - " + OVERLAY.ToString(), new Vector2(0, 0), Color.WhiteSmoke);

            dotCount++;

            if (dotCount > 50)
            {
                dotCount = 0;
            }

            string text = WorldGenerationManager.CurrentStep;

            if (WorldGenerationManager.IsGenerating)
            {
                text += "[";

                for (int i = 0; i < dotCount / 10; i++)
                {
                    text += ".";
                }

                for (int i = 0; i < 5 - (dotCount / 10) ; i++)
                {
                    text += " "; //so they always have the same size
                }

                    text += "]";
            }

            spriteBatch.DrawString(this.game.Content.Load<SpriteFont>("Fonts/TextFeedbackFont"), text, new Vector2(0, PlayableHeight), Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Helper Functions

        /// <summary>
        /// Prepares the grid for drawing by converting the graphical blocks into interface blocks and giving them the coordinates required.
        /// </summary>
        /// <param name="blocks"></param>
        /// <returns></returns>
        List<InterfaceBlock> PrepareGrid(List<GraphicalBlock> blocks)
        {
            //FOR NOW ONLY HANDLES 1 Z LEVEL

            //order them and limit them
            List<InterfaceBlock> returnList = new List<InterfaceBlock>();

            int currentYCoord = 0;
            if (blocks.Count != 0)
            {
                currentYCoord = blocks[0].MapCoordinate.Y;
            }

            int xCount = 0;
            int yCount = 0;

            foreach (GraphicalBlock block in blocks)
            {
                if (currentYCoord != block.MapCoordinate.Y)
                {
                    //we started a new line
                    xCount = 0;
                    yCount++;
                    currentYCoord = block.MapCoordinate.Y;
                }

                if (yCount > TotalTilesHeight)
                {
                    return returnList; //we're done
                }

                if (xCount <= TotalTilesWidth)
                {
                    //add it
                    InterfaceBlock iBlock = new InterfaceBlock(block);
                    iBlock.InterfaceX = xCount;
                    iBlock.InterfaceY = yCount;

                    returnList.Add(iBlock);

                    xCount++;

                }
                else
                {
                    //we need to increase the y amount
                    continue; //go through the loop, eventually the ycoordinate won't match
                }

            }

            return returnList;


        }
        /// <summary>
        /// Draws the prepared grid onto the screen
        /// </summary>
        /// <param name="blocks"></param>
        void DrawGrid(IEnumerable<InterfaceBlock> blocks)
        {
            Rectangle rec = new Rectangle(0, 0, TILEWIDTH, TILEHEIGHT);
            Rectangle itemRec = new Rectangle(0, 0, TILEWIDTH - 5, TILEHEIGHT - 5);

            //Set the default texture in case we don't find it
            Texture2D defTex = new Texture2D(GraphicsDevice, 1, 1);
            defTex.SetData(new[] { Color.White });

            foreach (InterfaceBlock block in blocks)
            {
                //find the location and put it in there

                rec.X = block.InterfaceX * TILEWIDTH;
                rec.Y = block.InterfaceY * TILEHEIGHT;

                //Start with the tile
                try
                {
                    //reverse it so what's on 0 gets to be on top
                    foreach (SpriteData tileGraphic in block.TileGraphics.Reverse())
                    {
                        if (tileGraphic != null)
                        {
                            spriteBatch.Draw(this.game.Content.Load<Texture2D>(tileGraphic.path), rec, tileGraphic.sourceRectangle, Color.White);
                        }
                    }
                }
                catch
                {
                    //texture not found, lets draw the default
                    spriteBatch.Draw(defTex, rec, Color.HotPink);
                }

                //now draw the items

                try
                {
                    if (block.ItemGraphics.Length != 0)
                    {
                        //we're using reverse here so items on the top get drawn last
                        foreach (SpriteData itemGraphic in block.ItemGraphics.Reverse())
                        {
                            if (itemGraphic != null)
                            {
                                spriteBatch.Draw(this.game.Content.Load<Texture2D>(itemGraphic.path), rec, itemGraphic.sourceRectangle, Color.White);
                            }

                        }
                    }
                }
                catch
                {
                    //texture not found, lets draw the default
                    spriteBatch.Draw(defTex, rec, Color.MediumPurple);
                }

                //now draw the overlay

                try
                {
                    if (block.OverlayGraphic != null)
                    {
                        //semi-transparent

                        spriteBatch.Draw(this.game.Content.Load<Texture2D>(block.OverlayGraphic.path), rec, block.OverlayGraphic.sourceRectangle, Color.White * 0.75f);
                    }
                }
                catch
                {
                    //texutre not found, skip it
                }
            }

        }

        #endregion

    }
}
