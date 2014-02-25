﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Divine_Right.GraphicalObjects;
using Microsoft.Xna.Framework.Input;
using Divine_Right.InterfaceComponents.Objects;
using Divine_Right.HelperFunctions;
using DRObjects;
using DivineRightGame.Managers;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using Divine_Right.InterfaceComponents;
using Divine_Right.InterfaceComponents.Objects.Enums;
using Divine_Right.InterfaceComponents.Components;
using DRObjects.Graphics;
using Divine_Right.GameScreens.Components;
using DivineRightGame.LocalMapGenerator;

namespace Divine_Right.GameScreens
{
    /// <summary>
    /// The Actual Game Interface that the user plays in
    /// </summary>
    class PlayableInterface :
        DrawableGameComponent
    {

        public int TILEWIDTH = 50;
        public int TILEHEIGHT = 50;

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
        #region Constants
        const int MAXTILEWIDTH = 50;
        const int MAXTILEHEIGHT = 50;

        const int MINTILEWIDTH = 20;
        const int MINTILEHEIGHT = 20;

        const int WINDOWWIDTH = 900;
        const int WINDOWHEIGHT = 500;

        /// <summary>
        /// The time in miliseconds for a key press to be considered
        /// </summary>
        const int GAMEINPUTDELAY = 100;

        #endregion

        #region Members

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<InterfaceBlock> blocks = new List<InterfaceBlock>();
        Game game;
        private List<IGameInterfaceComponent> interfaceComponents = new List<IGameInterfaceComponent>();
        private object[] parameters;

        /// <summary>
        /// Stores how the left and right button were last update
        /// </summary>
        bool lastLeftButtonClicked = false;
        bool lastRightButtonClicked = false;

        /// <summary>
        /// Stores the previous Game time in order to limit the amount of movement a player can make if he holds the key down
        /// </summary>
        int previousGameTime = 0;

        #endregion

        public PlayableInterface(Game game, GraphicsDeviceManager gr, object[] parameters) :
            base(game)
        {
            this.game = game;
            graphics = gr;
            this.parameters = parameters;


            game.Components.Add(new FPSCounter(game));
        }

        public override void Initialize()
        {
            base.Initialize();
            if (parameters.Length == 0)
            {
                TestFunctions.PrepareFileTestMap();
            }
            else if (parameters[0].ToString().Equals("Village"))
            {
                TestFunctions.ParseXML();
            }
            else
            {
                TestFunctions.GenerateDungeon();
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        public override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                game.Exit();

            //Lets see if there are any keyboard keys being pressed

            KeyboardState keyboardState = Keyboard.GetState();

            //Has the user pressed esc?
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                //Go back to the main menu for now
                BaseGame.requestedInternalAction = InternalActionEnum.EXIT;
                BaseGame.requestedArgs = new object[0];
            }

            bool shiftHeld = keyboardState.IsKeyDown(Keys.LeftShift) || keyboardState.IsKeyDown(Keys.RightShift);

            //has the user pressed one of the directional keys?
            //If yes, try to move

            //If shift was held, then we wait for less, so game moves faster
            if (gameTime.TotalGameTime.TotalMilliseconds - previousGameTime < GAMEINPUTDELAY / (shiftHeld ? 2 : 1))
            {
                //do nothing
            }
            else
            {
                //is there a component which wants to handle the keyboard movement?
                ActionTypeEnum? kAction = null;
                object[] kArgs = null;
                MapCoordinate kTargetCoord = null;

                bool keyHandled = false;
                bool destroy = false;

                //Do it upside down, so the stuff which appears on top happens first
                for (int i = interfaceComponents.Count - 1; i >= 0; i--)
                {
                    var interfaceComponent = interfaceComponents[i];

                    keyHandled = interfaceComponent.HandleKeyboard(keyboardState, out kAction, out kArgs, out kTargetCoord, out destroy);

                    //do we destroy?

                    if (destroy)
                    {
                        interfaceComponents.RemoveAt(i);
                        i++; //increment by 1
                    }

                    if (keyHandled)
                    {
                        //Break out of the loop - someone handled it
                        break;
                    }
                }

                if (kAction != null)
                {
                    this.PerformAction(kTargetCoord, kAction.Value, kArgs);
                }

                //did we do anything?
                if (keyHandled)
                {


                }
                else
                {
                    //Lets see if tab is held down
                    if (keyboardState.IsKeyDown(Keys.Tab))
                    {
                        //Zoom out a bit
                        TILEWIDTH = MINTILEWIDTH;
                        TILEHEIGHT = MINTILEHEIGHT;
                    }
                    else
                    {
                        //Let's animate it :)

                        //Leave it zoomed in
                        TILEWIDTH = MAXTILEWIDTH;
                        TILEHEIGHT = MAXTILEHEIGHT;
                    }

                    //not handled, lets walk
                    //where is the user?
                    MapCoordinate coord = UserInterfaceManager.GetPlayerActor().MapCharacter.Coordinate;
                    MapCoordinate difference = new MapCoordinate(0, 0, 0, coord.MapType);
                    //we will either increase or decrease it by an amount depending on the directional key pressed

                    if (keyboardState.IsKeyDown(Keys.OemPeriod))
                    {
                        //Just waste time
                        UserInterfaceManager.PerformLocalTick();
                    }
                    else if (keyboardState.IsKeyDown(Keys.Up))
                    {
                        difference = new MapCoordinate(0, 1, 0, coord.MapType);
                    }
                    else if (keyboardState.IsKeyDown(Keys.Down))
                    {
                        difference = new MapCoordinate(0, -1, 0, coord.MapType);
                    }
                    else if (keyboardState.IsKeyDown(Keys.Left))
                    {
                        difference = new MapCoordinate(-1, 0, 0, coord.MapType);
                    }
                    else if (keyboardState.IsKeyDown(Keys.Right))
                    {
                        difference = new MapCoordinate(1, 0, 0, coord.MapType);
                    }


                    //The fact that they'er not the same means the user pressed a key, lets move
                    if (!difference.Equals(new MapCoordinate(0, 0, 0, coord.MapType)))
                    {
                        //add the difference to the coordinate
                        coord += difference;

                        //send a move message to that coordinate
                        this.PerformAction(coord, DRObjects.Enums.ActionTypeEnum.MOVE, null);

                    }

                }
                //mark the current time
                previousGameTime = (int)gameTime.TotalGameTime.TotalMilliseconds;
            }

            #region Mouse Handling

            //See what the mouse is doing
            MouseState mouse = Mouse.GetState();

            MouseActionEnum? mouseAction = this.DetermineMouseAction(mouse);

            //this is a potential mouse action
            ActionTypeEnum? action = null;
            object[] args = null;
            MapCoordinate targetCoord = null;

            //see if there is a component which will handle it instead

            if (mouseAction != null)
            {
                bool mouseHandled = false;

                for (int i = interfaceComponents.Count - 1; i >= 0; i--)
                {
                    var interfaceComponent = interfaceComponents[i];

                    //is the click within this interface's scope? or is it modal?

                    Point mousePoint = new Point(mouse.X, mouse.Y);

                    if (interfaceComponent.IsModal() || interfaceComponent.ReturnLocation().Contains(mousePoint))
                    {
                        bool destroy;

                        //see if the component can handle it
                        mouseHandled = interfaceComponent.HandleClick(mouse.X, mouse.Y, mouseAction.Value, out action, out args, out targetCoord, out destroy);

                        if (destroy)
                        {
                            //destroy the component
                            interfaceComponents.RemoveAt(i);
                            i++;
                        }

                        if (mouseHandled)
                        {
                            //Get out of the loop - someone has handled it
                            break;
                        }
                    }
                }

                //dispatch the action, if any
                if (action != null)
                {
                    this.PerformAction(targetCoord, action.Value, args);
                }


                //do we continue?
                if (mouseHandled)
                {

                }
                else
                {
                    //the grid will handle it

                    //determine where we clicked
                    int xCord = (int)(mouse.X / TILEWIDTH);
                    int yCord = (int)(mouse.Y / TILEHEIGHT);

                    //get the game coordinate from the interface coordinate

                    InterfaceBlock iBlock = blocks.FirstOrDefault(b => b.InterfaceX.Equals(xCord) && b.InterfaceY.Equals(yCord));

                    //if it was a left click, we look

                    if (mouseAction.Value == MouseActionEnum.LEFT_CLICK)
                    {
                        if (iBlock != null)
                        {
                            this.PerformAction(iBlock.MapCoordinate, DRObjects.Enums.ActionTypeEnum.LOOK, null);
                        }
                    }
                    else if (mouseAction.Value == MouseActionEnum.RIGHT_CLICK)
                    {
                        if (iBlock != null)
                        {
                            ActionTypeEnum[] actions = UserInterfaceManager.GetPossibleActions(iBlock.MapCoordinate);

                            //we are going to get a context menu
                            
                            //Check for other context menus and remove them - we can only have one
                            for (int i = 0; i < interfaceComponents.Count; i++)
                            {
                                if (interfaceComponents[i].GetType().Equals(typeof(ContextMenuComponent)))
                                {
                                    //remove it
                                    interfaceComponents.RemoveAt(i);
                                    i--;
                                }
                            }

                            ContextMenuComponent comp = new ContextMenuComponent(mouse.X + 10, mouse.Y, iBlock.MapCoordinate);

                            foreach (ActionTypeEnum act in actions)
                            {
                                comp.AddContextMenuItem(act, null, this.game.Content);
                            }

                            //And add it
                            interfaceComponents.Add(comp);
                        }
                    }
                }

            } //end mouse handling

            this.lastLeftButtonClicked = (mouse.LeftButton == ButtonState.Pressed);
            this.lastRightButtonClicked = (mouse.RightButton == ButtonState.Pressed);
            #endregion

        }

        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //get the current state of the game
            //11,4,0
            GraphicalBlock[] blocks = UserInterfaceManager.GetBlocksAroundPlayer((TotalTilesWidth / 2), (TotalTilesHeight / 2), 0);
            //clean the blocks up

            List<InterfaceBlock> iBlocks = this.PrepareGrid(blocks.ToList<GraphicalBlock>());

            this.blocks = iBlocks; //copy them

            //draw them
            spriteBatch.Begin();
            this.DrawGrid(iBlocks);

            //any interface components to draw?

            foreach (var interfaceComponent in interfaceComponents)
            {
                interfaceComponent.Draw(this.game.Content, this.spriteBatch);
            }


            spriteBatch.End();

            base.Draw(gameTime);

            foreach (var comp in this.game.Components)
            {
                if (comp.GetType().Equals(typeof(FPSCounter)))
                {
                    (comp as FPSCounter).Draw(gameTime);
                }
            }
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
                    foreach (SpriteData tileGraphic in block.TileGraphics)
                    {
                        if (tileGraphic != null)
                        {
                            if (tileGraphic.sourceRectangle == null)
                            {
                                spriteBatch.Draw(this.game.Content.Load<Texture2D>(tileGraphic.path), rec, Color.White);
                            }
                            else
                            { //part of a tileset
                                spriteBatch.Draw(this.game.Content.Load<Texture2D>(tileGraphic.path), rec, tileGraphic.sourceRectangle, Color.White);
                            }
                        }
                    }
                }
                catch
                {
                    //texture not found, lets draw the default
                    spriteBatch.Draw(defTex, rec, Color.Green);
                }

                //now draw the items

                try
                {
                    if (block.ItemGraphics.Length != 0)
                    {

                        foreach (SpriteData itemGraphic in block.ItemGraphics)
                        {
                            if (itemGraphic != null)
                            {
                                if (itemGraphic.sourceRectangle == null)
                                {
                                    spriteBatch.Draw(this.game.Content.Load<Texture2D>(itemGraphic.path), rec, Color.White);
                                }
                                else
                                { //part of a tileset
                                    spriteBatch.Draw(this.game.Content.Load<Texture2D>(itemGraphic.path), rec, itemGraphic.sourceRectangle, Color.White);
                                }
                            }
                        }
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
            //remove any interface component
            
            //remove any viewtiletext components or contextmenu components
            for (int i = 0; i < interfaceComponents.Count; i++)
            {
                var type = interfaceComponents[i].GetType();
                if (type.Equals(typeof(ViewTileTextComponent)) || type.Equals(typeof(ContextMenuComponent)))
                {
                    //delete
                    interfaceComponents.RemoveAt(i);
                    i--;
                }
            }


            PlayerFeedback[] fb = UserInterfaceManager.PerformAction(coord, actionType, args);

            //go through all the feedback

            foreach (PlayerFeedback feedback in fb)
            {
                if (feedback.GetType().Equals(typeof(TextFeedback)))
                {
                    MouseState mouse = Mouse.GetState();

                    //Display it
                    interfaceComponents.Add(new ViewTileTextComponent(mouse.X + 15, mouse.Y, (feedback as TextFeedback).Text));
                }
                //TODO: THE REST
            }

        }
        /// <summary>
        /// Translate the state of the mouse and the last actions to determine what the mouse is doing
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public MouseActionEnum? DetermineMouseAction(MouseState state)
        {
            if (this.lastLeftButtonClicked && (state.LeftButton == ButtonState.Released))
            {
                //left click
                return MouseActionEnum.LEFT_CLICK;
            }

            if (this.lastRightButtonClicked && (state.RightButton == ButtonState.Released))
            {
                //right click
                return MouseActionEnum.RIGHT_CLICK;
            }

            //TODO: DO THE REST - WE DON'T NEED THEM FOR NOW
            return null;
        }

        #endregion

    }
}
