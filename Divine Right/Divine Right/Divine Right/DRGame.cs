using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using DRObjects.GraphicsEngineObjects;
using Divine_Right.GraphicalObjects;
using DRObjects;
using DivineRightGame.Managers;
using Divine_Right.HelperFunctions;
using Divine_Right.InterfaceComponents.Objects;
using DRObjects.GraphicsEngineObjects.Abstract;
using Divine_Right.InterfaceComponents.Managers;
using DRObjects.Enums;

namespace Divine_Right
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class DRGame : Microsoft.Xna.Framework.Game
    {
        #region Constants

        const int WIDTH = 950;
        const int HEIGHT = 500;

        const int TILEWIDTH = 50;
        const int TILEHEIGHT = 50;

        const int PLAYABLEWIDTH = 950;
        const int PLAYABLEHEIGHT = 450;

        const int TOTALTILESWIDTH = (PLAYABLEWIDTH / TILEWIDTH)-1;
        const int TOTALTILESHEIGHT = (PLAYABLEHEIGHT / TILEHEIGHT)-1;

        const int WINDOWWIDTH = 900;
        const int WINDOWHEIGHT = 500;

        public const int TEXTFEEDBACKDISPLAYTIMESECONDS = 2;

        #endregion

        #region Members

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<InterfaceBlock> blocks = new List<InterfaceBlock>();
        Keys[] lastKeys = new Keys[0];
        ContextMenu contextMenu = null;

        /// <summary>
        /// Stores how the left and right button were last update
        /// </summary>
        bool lastLeftButtonClicked = false;
        bool lastRightButtonClicked = false;

        /// <summary>
        /// Holds the interface text feedback which needs to be displayed
        /// </summary>
        List<InterfaceTextFeedback> interfaceTextFeedback = new List<InterfaceTextFeedback>();

        #endregion

        public DRGame()
        {
            this.Window.Title = "Divine Right Milestone 0 Version 4";
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = WINDOWWIDTH;
            graphics.PreferredBackBufferHeight = WINDOWHEIGHT;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            
            //TODO: for now just create a hard coded map
            //TODO: REMOVE AFTER TESTING

            //TestFunctions.PrepareHardCodedTestMap();

            TestFunctions.PrepareFileTestMap();

            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Lets see if there are any keyboard keys being pressed

            KeyboardState keyboardState = Keyboard.GetState();

            //has the user pressed one of the directional keys?
            //If yes, try to move

            if (keyboardState.IsKeyDown(Keys.Up) && !lastKeys.Contains(Keys.Up))
            {
                //get the coordinates from the gamestate
                MapCoordinate coord = UserInterfaceManager.GetPlayerActor().MapCharacter.Coordinate + new MapCoordinate(0, 1, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                //move to that coordinate
                this.PerformAction(coord, DRObjects.Enums.ActionTypeEnum.MOVE, null);
            }
            else if (keyboardState.IsKeyDown(Keys.Down) && !lastKeys.Contains(Keys.Down))
            {
                //get the coordinates from the gamestate
                MapCoordinate coord = UserInterfaceManager.GetPlayerActor().MapCharacter.Coordinate + new MapCoordinate(0, -1, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                //move to that coordinate
                this.PerformAction(coord, DRObjects.Enums.ActionTypeEnum.MOVE, null);
            }
            else if (keyboardState.IsKeyDown(Keys.Left) && !lastKeys.Contains(Keys.Left))
            {
                //get the coordinates from the gamestate
                MapCoordinate coord = UserInterfaceManager.GetPlayerActor().MapCharacter.Coordinate + new MapCoordinate(-1, 0, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                //move to that coordinate
                this.PerformAction(coord, DRObjects.Enums.ActionTypeEnum.MOVE, null);
            }
            else if (keyboardState.IsKeyDown(Keys.Right) && !lastKeys.Contains(Keys.Right))
            {
                //get the coordinates from the gamestate
                MapCoordinate coord = UserInterfaceManager.GetPlayerActor().MapCharacter.Coordinate + new MapCoordinate(1, 0, 0, DRObjects.Enums.MapTypeEnum.LOCAL);
                //move to that coordinate
                this.PerformAction(coord, DRObjects.Enums.ActionTypeEnum.MOVE, null);
            }
            lastKeys = keyboardState.GetPressedKeys();

            #region Mouse Handling

            //See what the mouse is doing
            MouseState mouse = Mouse.GetState();

            
            if (mouse.LeftButton == ButtonState.Released && this.lastLeftButtonClicked)
            {
                //left mouse button was clicked and released
                
                //if there is a context menu open, then we need to check whether the click was on it
                if (contextMenu != null)
                {
                    ActionTypeEnum actionType = ActionTypeEnum.IDLE;
                    MapCoordinate coord = null;
                    object[] args = null;

                    if (contextMenu.HandleClick(mouse.X, mouse.Y, out actionType, out args, out coord))
                    {
                        //The click was on the menu -perform the action

                        PerformAction(coord, actionType, args);

                        //note the mouse
                        lastLeftButtonClicked = mouse.LeftButton == ButtonState.Pressed;
                        lastRightButtonClicked = mouse.RightButton == ButtonState.Pressed;

                        return;
                    }
                    //it wasn't - carry on
                }
                
                
                //determine where we clicked

                if (mouse.X < PLAYABLEWIDTH)
                {
                    if (mouse.Y < PLAYABLEHEIGHT)
                    {
                        //within grid

                        int xCord = (int) (mouse.X / TILEWIDTH);
                        int yCord = (int)(mouse.Y / TILEHEIGHT);

                        //get the game coordinate from the interface coordinate

                        InterfaceBlock iBlock = blocks.FirstOrDefault(b => b.InterfaceX.Equals(xCord) && b.InterfaceY.Equals(yCord));

                        if (iBlock != null)
                        {
                            PerformAction(iBlock.MapCoordinate, DRObjects.Enums.ActionTypeEnum.LOOK, null);
                        }
                    }
                }

            }

            //what about the right mouse button?
            if (mouse.RightButton == ButtonState.Released && this.lastRightButtonClicked)
            {
                //determine where we clicked
                if (mouse.X < PLAYABLEWIDTH)
                {
                    if (mouse.Y < PLAYABLEHEIGHT)
                    {
                        //within grid
                        int xCord = (int)(mouse.X / TILEWIDTH);
                        int yCord = (int)(mouse.Y / TILEHEIGHT);

                        //get the game coordinate from the interface
                        InterfaceBlock iBlock = blocks.FirstOrDefault(b => b.InterfaceX.Equals(xCord) && b.InterfaceY.Equals(yCord));

                        if (iBlock != null)
                        {
                            ActionTypeEnum[] actions = UserInterfaceManager.GetPossibleActions(iBlock.MapCoordinate);

                            //do we have more than 1 item
                            if (actions.Length == 0)
                            {
                                //clear the menu
                                contextMenu = null;
                            }
                            else
                            {
                                //create a new one
                                contextMenu = new ContextMenu(mouse.X+10, mouse.Y,iBlock.MapCoordinate);
                                foreach (ActionTypeEnum act in actions)
                                {
                                    contextMenu.AddContextMenuItem(act, null, Content);
                                }
                            }
                        }

                    }
                }


            } //end mouse handling

            this.lastLeftButtonClicked = (mouse.LeftButton == ButtonState.Pressed);
            this.lastRightButtonClicked = (mouse.RightButton == ButtonState.Pressed);
            #endregion

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //get the current state of the game
            //11,4,0
            GraphicalBlock[] blocks = UserInterfaceManager.GetBlocksAroundPlayer((TOTALTILESWIDTH/2), (TOTALTILESHEIGHT/2), 0);
            //clean the blocks up

            List<InterfaceBlock> iBlocks = this.PrepareGrid(blocks.ToList<GraphicalBlock>());

            this.blocks = iBlocks; //copy them

            //draw them
            spriteBatch.Begin();
            this.DrawGrid(iBlocks);

            //Draw the current log

            //spriteBatch.DrawString(Content.Load<SpriteFont>(@"Fonts/TextFeedbackFont"), "Hello \n World", new Vector2(PLAYABLEWIDTH, PLAYABLEHEIGHT), Color.White);

            //draw the text feedback
            InterfaceTextManager.DrawTextFeedback(spriteBatch, Content, GraphicsDevice, this.interfaceTextFeedback);

            //do we have a contextmenu to draw?

            if (contextMenu != null)
            {
                contextMenu.DrawMenu(Content, spriteBatch);
            }

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
            defTex.SetData(new[] { Color.White});

            foreach (InterfaceBlock block in blocks)
            {
                //find the location and put it in there

                rec.X = block.InterfaceX*TILEWIDTH;
                rec.Y = block.InterfaceY*TILEHEIGHT;

                //Start with the tile
                try
                {
                    if (block.TileGraphic != string.Empty)
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>(block.TileGraphic), rec, Color.White);
                    }
                }
                catch
                {
                    //texture not found, lets draw the default
                    spriteBatch.Draw(defTex, rec, Color.Green);
                }

                //now draw the item

                try
                {
                    if (block.ItemGraphic != string.Empty)
                    {
                        spriteBatch.Draw(Content.Load<Texture2D>(block.ItemGraphic), rec, Color.White);
                    }
                }
                catch
                {
                    //texture not found, lets draw the default
                    spriteBatch.Draw(defTex, rec, Color.Blue);
                }
            }

        }

        /// <summary>
        /// Performs the action and handles feedback
        /// </summary>
        /// <param name="?"></param>
        public void PerformAction(MapCoordinate coord, DRObjects.Enums.ActionTypeEnum actionType, object[] args)
        {
            //wipe the context menu
            contextMenu = null;

            PlayerFeedback[] fb = UserInterfaceManager.PerformAction(coord, actionType, args);

            //go through all the feedback

            foreach (PlayerFeedback feedback in fb)
            {
                if (feedback.GetType().Equals(typeof(TextFeedback)))
                {
                    MouseState mouse = Mouse.GetState();

                    //add it to the list
                    interfaceTextFeedback.Add(new InterfaceTextFeedback((TextFeedback) feedback,mouse.X + 25,mouse.Y));
                }
                //TODO: THE REST
            }

        }

        #endregion
    }
}
