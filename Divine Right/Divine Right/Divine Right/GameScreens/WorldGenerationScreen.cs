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

namespace Divine_Right.GameScreens
{
    class WorldGenerationScreen : 
        DrawableGameComponent
    {
        #region Constants
        const int TILEWIDTH = 5;
        const int TILEHEIGHT = 5;

        const int PLAYABLEWIDTH = 950;
        const int PLAYABLEHEIGHT = 450;

        const int TOTALTILESWIDTH = (PLAYABLEWIDTH / TILEWIDTH) - 1;
        const int TOTALTILESHEIGHT = (PLAYABLEHEIGHT / TILEHEIGHT) - 1;

        /// <summary>
        /// The time in miliseconds for a key press to be considered
        /// </summary>
        const int GAMEINPUTDELAY = 100;


        #endregion

        #region Members

        protected Game game;
        protected GraphicsDeviceManager graphics;
        protected MapCoordinate centrePoint = new MapCoordinate();
        protected SpriteBatch spriteBatch;

        //The locations we're looking at right now
        int locationX = WorldGenerationManager.WORLDSIZE/2;
        int locationY = WorldGenerationManager.WORLDSIZE/2;
        int previousGameTime = 0;

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
                locationY+=5;
            }
            else if (keyboardState.IsKeyDown(Keys.Down))
            {
                locationY-=5;
            }
            else if (keyboardState.IsKeyDown(Keys.Left))
            {
                locationX-=5;
            }
            else if (keyboardState.IsKeyDown(Keys.Right))
            {
                locationX+=5;
            }


        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            GraphicalBlock[] blocks = null;

            //lock so we can access the map
            lock (GlobalMap.lockMe)
            {
                blocks = UserInterfaceManager.GetBlocksAroundPoint(new MapCoordinate(locationX, locationY, 0, DRObjects.Enums.MapTypeEnum.GLOBAL), TOTALTILESWIDTH / 2, TOTALTILESHEIGHT / 2, 0);
            }

            List<InterfaceBlock> iBlocks = this.PrepareGrid(blocks.ToList<GraphicalBlock>());

            //draw them
            spriteBatch.Begin();
            this.DrawGrid(iBlocks);

            //lets also draw the current step and the location of the cursor
            spriteBatch.DrawString(this.game.Content.Load<SpriteFont>("Fonts/TextFeedbackFont"), this.locationX + "," + this.locationY, new Vector2(0, 0), Color.WhiteSmoke);

            spriteBatch.DrawString(this.game.Content.Load<SpriteFont>("Fonts/TextFeedbackFont"), WorldGenerationManager.CurrentStep, new Vector2(0, PLAYABLEHEIGHT), Color.White);


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

                if (yCount > TOTALTILESHEIGHT)
                {
                    return returnList; //we're done
                }

                if (xCount <= TOTALTILESWIDTH)
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
                    foreach (string tileGraphic in block.TileGraphics.Reverse())
                    {
                        if (tileGraphic != string.Empty)
                        {
                            spriteBatch.Draw(this.game.Content.Load<Texture2D>(tileGraphic), rec, Color.White);
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
                        foreach (string itemGraphic in block.ItemGraphics.Reverse())
                        {
                            spriteBatch.Draw(this.game.Content.Load<Texture2D>(itemGraphic), rec, Color.White);
                        }
                    }
                }
                catch
                {
                    //texture not found, lets draw the default
                    spriteBatch.Draw(defTex, rec, Color.MediumPurple);
                }
            }

        }

        #endregion

    }
}
