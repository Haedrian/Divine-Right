using System;
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
using DivineRightGame;
using DRObjects.Items.Archetypes.Local;
using DivineRightGame.EventHandling;
using DivineRightGame.SettlementHandling;
using DivineRightGame.ActorHandling;
using DRObjects.LocalMapGeneratorObjects;
using DRObjects.Items.Archetypes.Global;
using DRObjects.Database;
using DivineRightGame.ItemFactory.ItemFactoryManagers;
using DRObjects.DataStructures.Enum;
using System.Threading;
using DivineRightGame.CharacterCreation;
using DRObjects.ActorHandling.CharacterSheet.Enums;

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
                return GraphicsDevice.Viewport.Height;

            }

        }
        int TotalTilesWidth
        {
            get
            {
                return (PlayableWidth / TILEWIDTH);
            }


        }
        int TotalTilesHeight
        {
            get
            {
                return (PlayableHeight / TILEHEIGHT);
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

        /// <summary>
        /// This is a dirty hack to allow the draw to run at least once before it blocks for saving
        /// </summary>
        private bool saveAndQuit = false;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<InterfaceBlock> blocks = new List<InterfaceBlock>();
        Game game;

        /// <summary>
        /// The font to draw text in
        /// </summary>
        private SpriteFont font;

        private List<IGameInterfaceComponent> interfaceComponents = new List<IGameInterfaceComponent>();
        TextLogComponent log;
        List<ISystemInterfaceComponent> menuButtons = new List<ISystemInterfaceComponent>();

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
            this.game.Window.ClientSizeChanged += new EventHandler<EventArgs>(Window_ClientSizeChanged);

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
                //TestFunctions.ParseXML();
                TestFunctions.GenerateSettlement();

                InventoryItemManager mgr = new InventoryItemManager();

                for (int i = 0; i < 500; i++)
                {
                    var block = GameState.LocalMap.GetBlockAtCoordinate(new MapCoordinate(GameState.Random.Next(GameState.LocalMap.localGameMap.GetLength(0)), GameState.Random.Next(GameState.LocalMap.localGameMap.GetLength(1)), 0, MapType.LOCAL));

                    if (block.MayContainItems)
                    {
                        var item = mgr.CreateItem(DatabaseHandling.GetItemIdFromTag(Archetype.INVENTORYITEMS, "loot")) as InventoryItem;
                        item.Coordinate = new MapCoordinate(block.Tile.Coordinate);
                        block.ForcePutItemOnBlock(item);
                    }
                }

                //Create the new character stuff
                MultiDecisionComponent mdc = new MultiDecisionComponent(PlayableWidth / 2 - 250, 150, CharacterCreation.GenerateCharacterCreation());
                mdc.Visible = true;

                interfaceComponents.Add(mdc);

                GameState.LocalMap.IsGlobalMap = false;

            }
            else if (parameters[0].ToString().Equals("Continue"))
            {
                GameState.LoadGame();
            }
            else if (parameters[0].ToString().Equals("WorldMap"))
            {
                //Load from the world map
                var worldMap = GameState.GlobalMap.globalGameMap;

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (var block in worldMap)
                {
                    collapsedMap.Add(block);
                }

                GameState.LocalMap = new LocalMap(GameState.GlobalMap.globalGameMap.GetLength(0), GameState.GlobalMap.globalGameMap.GetLength(1), 1, 0);

                //Go through each of them, add them to the local map
                GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

                //Let's start the player off at the first capital
                var coordinate = GameState.GlobalMap.WorldSettlements.Where(w => w.IsCapital).Select(w => w.Coordinate).FirstOrDefault();

                //Create the player character. For now randomly. Later we'll start at a capital
                MapItem player = new MapItem();
                player.Coordinate = new MapCoordinate(coordinate);
                player.Description = "The player character";
                player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR_MALE);
                player.InternalName = "Player Char";
                player.MayContainItems = false;
                player.Name = "Player";

                MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(player.Coordinate);
                playerBlock.PutItemOnBlock(player);

                GameState.PlayerCharacter = new Actor();
                GameState.PlayerCharacter.MapCharacter = player;
                GameState.PlayerCharacter.IsPlayerCharacter = true;

                GameState.PlayerCharacter.Attributes = ActorGeneration.GenerateAttributes("human", DRObjects.ActorHandling.CharacterSheet.Enums.ActorProfession.WARRIOR, 10, GameState.PlayerCharacter);
                GameState.PlayerCharacter.Anatomy = ActorGeneration.GenerateAnatomy("human");

                GameState.PlayerCharacter.Attributes.Health = GameState.PlayerCharacter.Anatomy;

                GameState.PlayerCharacter.Inventory.EquippedItems = ActorGeneration.GenerateEquippedItems(250); //give him 250 worth of stuff

                foreach (var item in GameState.PlayerCharacter.Inventory.EquippedItems.Values)
                {
                    GameState.PlayerCharacter.Inventory.Inventory.Add(item.Category, item);
                }

                GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);

                //What attributes do we want?
                MultiDecisionComponent mdc = new MultiDecisionComponent(PlayableWidth / 2 - 250, 150, CharacterCreation.GenerateCharacterCreation());
                mdc.Visible = true;

                interfaceComponents.Add(mdc);

                GameState.LocalMap.IsGlobalMap = true;

            }
            else if (parameters[0].ToString().Equals("Camp"))
            {

                TestFunctions.ParseXML();

                //MapCoordinate coo = new MapCoordinate();
                //Actor[] arr = null;
                ////var gennedMap = CampGenerator.GenerateCamp(15,out coo, out arr);


                //var gennedMap = WildernessGenerator.GenerateMap(GlobalBiome.ARID_DESERT,3, 0, out arr, out coo);

                //GameState.LocalMap = new LocalMap(100, 100, 1, 0);

                //List<MapBlock> collapsedMap = new List<MapBlock>();

                //foreach (MapBlock block in gennedMap)
                //{
                //    collapsedMap.Add(block);
                //}

                //GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

                //GameState.LocalMap.Actors.AddRange(arr);

                //MapItem player = new MapItem();
                //player.Coordinate = coo;
                //player.Description = "The player character";
                //player.Graphic = SpriteManager.GetSprite(LocalSpriteName.PLAYERCHAR_MALE);
                //player.InternalName = "Player Char";
                //player.MayContainItems = false;
                //player.Name = "Player";

                //MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(player.Coordinate);
                //playerBlock.PutItemOnBlock(player);

                //GameState.PlayerCharacter = new Actor();
                //GameState.PlayerCharacter.MapCharacter = player;
                //GameState.PlayerCharacter.IsPlayerCharacter = true;

                //GameState.PlayerCharacter.Attributes = ActorGeneration.GenerateAttributes("human", DRObjects.ActorHandling.CharacterSheet.Enums.ActorProfession.WARRIOR, 10, GameState.PlayerCharacter);

                //GameState.PlayerCharacter.Anatomy = ActorGeneration.GenerateAnatomy("human");

                //GameState.PlayerCharacter.Attributes.Health = GameState.PlayerCharacter.Anatomy;

                //GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);
                //// GameState.LocalMap.Actors.AddRange(actors

                //GameState.LocalMap.IsGlobalMap = false;

            }
            else
            {
                TestFunctions.GenerateDungeon();
            }

            //Add the health control
            HealthDisplayComponent hdc = new HealthDisplayComponent(50, 50, GameState.LocalMap.Actors.Where(a => a.IsPlayerCharacter).FirstOrDefault());
            hdc.Visible = false;
            interfaceComponents.Add(hdc);

            CharacterSheetComponent csc = new CharacterSheetComponent(50, 50, GameState.LocalMap.Actors.Where(a => a.IsPlayerCharacter).FirstOrDefault());
            csc.Visible = false;
            interfaceComponents.Add(csc);

            InventoryDisplayComponent ivt = new InventoryDisplayComponent(50, 50, GameState.LocalMap.Actors.Where(a => a.IsPlayerCharacter).FirstOrDefault());
            ivt.Visible = false;
            interfaceComponents.Add(ivt);

            TextLogComponent tlc = new TextLogComponent(10, GraphicsDevice.Viewport.Height - 50, GameState.NewLog);
            tlc.Visible = true;
            interfaceComponents.Add(tlc);

            log = tlc;

            AdventureDisplayComponent tdc = new AdventureDisplayComponent(GraphicsDevice.Viewport.Width / 2 - 100, 0);
            interfaceComponents.Add(tdc);

            var cemetry = SpriteManager.GetSprite(InterfaceSpriteName.DEAD);

            //Create the menu buttons
            menuButtons.Add(new AutoSizeGameButton("  Health  ", this.game.Content, InternalActionEnum.OPEN_HEALTH, new object[] { }, 50, GraphicsDevice.Viewport.Height - 35));
            menuButtons.Add(new AutoSizeGameButton(" Attributes ", this.game.Content, InternalActionEnum.OPEN_ATTRIBUTES, new object[] { }, 150, GraphicsDevice.Viewport.Height - 35));
            //menuButtons.Add(new AutoSizeGameButton(" Settlement ", this.game.Content, InternalActionEnum.TOGGLE_SETTLEMENT, new object[] { }, 270, GraphicsDevice.Viewport.Height - 35));
            menuButtons.Add(new AutoSizeGameButton(" Inventory ", this.game.Content, InternalActionEnum.OPEN_INVENTORY, new object[] { }, 350, GraphicsDevice.Viewport.Height - 35));

            //Invoke a size change
            Window_ClientSizeChanged(null, null);

        }

        void Window_ClientSizeChanged(object sender, EventArgs e)
        {
            //Resizing
            foreach (AutoSizeGameButton button in menuButtons)
            {
                button.drawRect.Y = GraphicsDevice.Viewport.Height - 35;
            }

            foreach (var interfaceComponent in interfaceComponents)
            {
                var textLog = interfaceComponent as TextLogComponent;

                if (textLog != null)
                {
                    textLog.Move(textLog.ReturnLocation().X, GraphicsDevice.Viewport.Height - 70);
                }
            }
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        bool isDragging = false;
        int dragPointX = -1;
        int dragPointY = -1;
        IGameInterfaceComponent dragItem = null;

        public override void Update(GameTime gameTime)
        {
            //This fixes an issue in monogame with resizing
            graphics.ApplyChanges();

            //Is the user asking to quit?
            if (saveAndQuit)
            {
                //Go to loading. It'll open the main menu when it's done
                BaseGame.requestedInternalAction = InternalActionEnum.CONTINUE;
                BaseGame.requestedArgs = new object[1] { "Save" };

                saveAndQuit = false;
            }

            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                game.Exit();


            //Only check if the game is actually active
            if (Game.IsActive)
            {


                //Lets see if there are any keyboard keys being pressed

                KeyboardState keyboardState = Keyboard.GetState();

                //Has the user pressed esc?
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    GameState.NewLog.Add(new CurrentLogFeedback(InterfaceSpriteName.BANNER_GREEN, Color.White, "Saving Game Please Wait..."));

                    saveAndQuit = true;
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
                    ActionType? kAction = null;
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
                        this.PerformAction(kTargetCoord, null, kAction.Value, kArgs);
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
                            this.PerformAction(null, null, ActionType.IDLE, null);
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
                            this.PerformAction(coord, null, DRObjects.Enums.ActionType.MOVE, null);

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
                ActionType? action = null;
                object[] args = null;
                MapCoordinate targetCoord = null;
                MapItem item = null;

                //see if there is a component which will handle it instead

                //Is the mouse over a particular component ?
                foreach (var component in interfaceComponents.Where(ic => ic.Visible))
                {
                    if (component.ReturnLocation().Contains(mouse.X, mouse.Y))
                    {
                        //Mouse over trigger
                        component.HandleMouseOver(mouse.X, mouse.Y);
                    }
                }


                if (mouseAction != null)
                {
                    bool mouseHandled = false;

                    InternalActionEnum? internalAction = null;
                    object[] arg = null;

                    foreach (var menuButton in menuButtons)
                    {
                        if (menuButton.ReturnLocation().Contains(new Point(mouse.X, mouse.Y)) && mouseAction == MouseActionEnum.LEFT_CLICK && menuButton.HandleClick(mouse.X, mouse.Y, out internalAction, out arg))
                        {
                            mouseHandled = true; //don't get into the other loop
                            break; //break out
                        }
                    }

                    //Are we dragging something?

                    if (isDragging)
                    {
                        //Check the location we're at
                        int deltaX = mouse.X - dragPointX;
                        int deltaY = mouse.Y - dragPointY;

                        //Drag it
                        dragItem.PerformDrag(deltaX, deltaY);

                        //Update the dragpoints

                        dragPointX = deltaX;
                        dragPointY = deltaY;

                        //are we still dragging?
                        if (mouseAction.Value != MouseActionEnum.DRAG)
                        {
                            //Nope
                            isDragging = false;
                        }
                    }

                    //Do we have a MODAL interface component?
                    var modalComponent = interfaceComponents.Where(ic => ic.IsModal()).FirstOrDefault();

                    if (modalComponent != null)
                    {
                        //Force it to handle it
                        bool destroy = false;

                        modalComponent.HandleClick(mouse.X, mouse.Y, mouseAction.Value, out action, out internalAction, out args, out item, out targetCoord, out destroy);

                        if (destroy)
                        {
                            //Destroy it
                            interfaceComponents.Remove(modalComponent);
                        }

                        //It's handled
                        mouseHandled = true;
                    }

                    for (int i = interfaceComponents.Count - 1; i >= 0; i--)
                    {
                        if (mouseHandled)
                        {
                            break;
                        }

                        var interfaceComponent = interfaceComponents[i];

                        //is the click within this interface's scope? or is it modal?

                        Point mousePoint = new Point(mouse.X, mouse.Y);

                        if (interfaceComponent.IsModal() || interfaceComponent.ReturnLocation().Contains(mousePoint) && interfaceComponent.Visible)
                        {
                            bool destroy;

                            //Are we dragging?
                            if (mouseAction.Value == MouseActionEnum.DRAG)
                            {
                                //Yes
                                this.dragPointX = mouse.X;
                                this.dragPointY = mouse.Y;
                                isDragging = true;
                                dragItem = interfaceComponent;

                                mouseHandled = true;

                                //insert the item again
                                interfaceComponents.RemoveAt(i);

                                interfaceComponents.Add(interfaceComponent);

                                break; //break out
                            }

                            //see if the component can handle it
                            mouseHandled = interfaceComponent.HandleClick(mouse.X, mouse.Y, mouseAction.Value, out action, out internalAction, out args, out item, out targetCoord, out destroy);

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
                    if (action.HasValue)
                    {
                        this.PerformAction(targetCoord, item, action.Value, args);
                    }

                    //Dispatch the internal action, if any
                    if (internalAction.HasValue)
                    {
                        //Let's do it here
                        switch (internalAction.Value)
                        {
                            case InternalActionEnum.OPEN_HEALTH:
                                //Toggle the health
                                var health = this.interfaceComponents.Where(ic => ic.GetType().Equals(typeof(HealthDisplayComponent))).FirstOrDefault();

                                health.Visible = !health.Visible;

                                break;
                            case InternalActionEnum.OPEN_ATTRIBUTES:
                                //Toggle the attributes
                                var att = this.interfaceComponents.Where(ic => ic.GetType().Equals(typeof(CharacterSheetComponent))).FirstOrDefault();
                                att.Visible = !att.Visible;

                                break;

                            case InternalActionEnum.TOGGLE_SETTLEMENT:
                                //Toggle the settlements
                                var loc = this.interfaceComponents.Where(ic => ic.GetType().Equals(typeof(LocationDetailsComponent))).FirstOrDefault();
                                loc.Visible = !loc.Visible;

                                break;

                            case InternalActionEnum.OPEN_LOG:
                                //Toggle the log
                                var log = this.interfaceComponents.Where(ic => ic.GetType().Equals(typeof(TextLogComponent))).FirstOrDefault();
                                log.Visible = !log.Visible;
                                break;

                            case InternalActionEnum.OPEN_INVENTORY:
                                //Toggle inventory
                                var inv = this.interfaceComponents.Where(ic => ic.GetType().Equals(typeof(InventoryDisplayComponent))).FirstOrDefault();
                                inv.Visible = !inv.Visible;
                                break;

                            case InternalActionEnum.LOSE:
                                //For now, just go back to the main menu
                                BaseGame.requestedInternalAction = InternalActionEnum.EXIT;
                                BaseGame.requestedArgs = new object[0];
                                break;
                            case InternalActionEnum.MULTIDECISION:
                                //The only thing that has multidecisions right now is char creation
                                //So let's do it dirty for now
                                CharacterCreation.ProcessParameters(args[1] as List<string>);
                                break;

                            //TODO: THE REST
                        }
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
                                this.PerformAction(iBlock.MapCoordinate, null, DRObjects.Enums.ActionType.LOOK, null);
                            }
                        }
                        else if (mouseAction.Value == MouseActionEnum.RIGHT_CLICK)
                        {
                            if (iBlock != null)
                            {
                                ActionType[] actions = UserInterfaceManager.GetPossibleActions(iBlock.MapCoordinate);

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

                                foreach (ActionType act in actions)
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
        }

        public override void Draw(GameTime gameTime)
        {
            if (font == null)
            {
                //Load the font
                font = game.Content.Load<SpriteFont>(@"Fonts/LightText");
            }

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

            // nighttime 
            if (GameState.UniverseTime.GetTimeComponent(DRTimeComponent.HOUR) >= 5)
            {
                spriteBatch.Draw(this.game.Content, SpriteManager.GetSprite(ColourSpriteName.MARBLEBLUE), new Rectangle(0, 0, 10000, 10000), new Color(0, 0, 0, 150));
            }

            foreach (AutoSizeGameButton button in menuButtons)
            {
                button.Draw(this.game.Content, this.spriteBatch);
            }

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
                    foreach (SpriteData tileGraphic in block.TileGraphics.Reverse())
                    {
                        if (tileGraphic != null)
                        {
                            if (tileGraphic.sourceRectangle == null)
                            {
                                spriteBatch.Draw(this.game.Content.Load<Texture2D>(tileGraphic.path), rec, tileGraphic.ColorFilter.HasValue ? tileGraphic.ColorFilter.Value : Color.White);
                            }
                            else
                            { //part of a tileset
                                spriteBatch.Draw(this.game.Content.Load<Texture2D>(tileGraphic.path), rec, tileGraphic.sourceRectangle, tileGraphic.ColorFilter.HasValue ? tileGraphic.ColorFilter.Value : Color.White);
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
                                if (itemGraphic.GetType().Equals(typeof(TextSpriteData)))
                                {
                                    TextSpriteData data = (TextSpriteData)itemGraphic;

                                    //Write it in the screen
                                    spriteBatch.DrawString(font, data.Text, rec, Alignment.Right | Alignment.Bottom, data.Colour);
                                }
                                else
                                {

                                    if (itemGraphic.sourceRectangle == null)
                                    {
                                        spriteBatch.Draw(this.game.Content.Load<Texture2D>(itemGraphic.path), rec, itemGraphic.ColorFilter.HasValue ? itemGraphic.ColorFilter.Value : Color.White);
                                    }
                                    else
                                    { //part of a tileset
                                        spriteBatch.Draw(this.game.Content.Load<Texture2D>(itemGraphic.path), rec, itemGraphic.sourceRectangle, itemGraphic.ColorFilter.HasValue ? itemGraphic.ColorFilter.Value : Color.White);
                                    }
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
        public void PerformAction(MapCoordinate coord, MapItem item, DRObjects.Enums.ActionType actionType, object[] args)
        {
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

            ActionFeedback[] fb = UserInterfaceManager.PerformAction(coord, item, actionType, args);

            //go through all the feedback

            foreach (ActionFeedback feedback in fb)
            {
                if (feedback == null)
                {
                    continue;
                }

                if (feedback.GetType().Equals(typeof(TextFeedback)))
                {
                    MouseState mouse = Mouse.GetState();

                    //Display it
                    interfaceComponents.Add(new ViewTileTextComponent(mouse.X + 15, mouse.Y, (feedback as TextFeedback).Text));
                }
                else if (feedback.GetType().Equals(typeof(CurrentLogFeedback)))
                {
                    GameState.NewLog.Add(feedback as CurrentLogFeedback);
                }
                else if (feedback.GetType().Equals(typeof(InterfaceToggleFeedback)))
                {
                    InterfaceToggleFeedback iop = feedback as InterfaceToggleFeedback;

                    if (iop.InterfaceComponent == InternalActionEnum.OPEN_ATTACK && iop.Open)
                    {
                        //Open the attack interface for a particular actor. If one is not open already
                        //Identify the actor in question
                        var actorMapItem = iop.Argument as LocalCharacter;

                        //Locate the actual actor
                        Actor actor = GameState.LocalMap.Actors.Where(lm => lm.MapCharacter == actorMapItem).FirstOrDefault(); //Yep, it's a pointer equals

                        bool openAlready = false;

                        //Do we have one open already?
                        foreach (AttackActorComponent aac in interfaceComponents.Where(ic => ic.GetType().Equals(typeof(AttackActorComponent))))
                        {
                            if (aac.TargetActor.Equals(actor))
                            {
                                openAlready = true;
                                break;
                            }
                        }

                        if (!openAlready)
                        {
                            //Open it. Otherwise don't do anything
                            interfaceComponents.Add(new AttackActorComponent(150, 150, GameState.PlayerCharacter, actor) { Visible = true });
                        }

                    }
                    else if (iop.InterfaceComponent == InternalActionEnum.OPEN_ATTACK && !iop.Open)
                    {
                        //Close it
                        var actor = iop.Argument as Actor;

                        AttackActorComponent component = null;

                        foreach (AttackActorComponent aac in interfaceComponents.Where(ic => ic.GetType().Equals(typeof(AttackActorComponent))))
                        {
                            if (aac.TargetActor.Equals(actor))
                            {
                                component = aac;
                            }
                        }

                        //Did we have a match?
                        if (component != null)
                        {
                            //remove it
                            interfaceComponents.Remove(component);
                        }

                    }
                    else if (iop.InterfaceComponent == InternalActionEnum.OPEN_TRADE && iop.Open)
                    {
                        //Open trade
                        var arguments = iop.Argument as object[];

                        TradeDisplayComponent tdc = new TradeDisplayComponent(100, 100, arguments[1] as Actor, arguments[0] as Actor);

                        interfaceComponents.Add(tdc);
                    }
                }
                else if (feedback.GetType().Equals(typeof(CreateEventFeedback)))
                {
                    CreateEventFeedback eventFeedback = feedback as CreateEventFeedback;

                    var gameEvent = EventHandlingManager.CreateEvent(eventFeedback.EventName);

                    //Create the actual control
                    interfaceComponents.Add(new DecisionPopupComponent(PlayableWidth / 2 - 150, PlayableHeight / 2 - 150, gameEvent));

                }
                else if (feedback.GetType().Equals(typeof(LocationChangeFeedback)))
                {
                    //Remove settlement button and interface
                    var locDetails = this.interfaceComponents.Where(ic => ic.GetType().Equals(typeof(LocationDetailsComponent))).FirstOrDefault();

                    if (locDetails != null)
                    {
                        this.interfaceComponents.Remove(locDetails);
                    }

                    var button = this.menuButtons.Where(mb => (mb as AutoSizeGameButton).Action == InternalActionEnum.TOGGLE_SETTLEMENT).FirstOrDefault();

                    if (button != null)
                    {
                        this.menuButtons.Remove(button);
                    }

                    LocationChangeFeedback lce = feedback as LocationChangeFeedback;

                    if (lce.VisitSettlement != null)
                    {
                        LoadSettlement(lce.VisitSettlement);

                        //Makde the components visible
                        LocationDetailsComponent ldc = new LocationDetailsComponent(GameState.LocalMap.Settlement, PlayableWidth - 170, 0);
                        ldc.Visible = false;
                        interfaceComponents.Add(ldc);
                        menuButtons.Add(new AutoSizeGameButton(" Settlement ", this.game.Content, InternalActionEnum.TOGGLE_SETTLEMENT, new object[] { }, 270, GraphicsDevice.Viewport.Height - 35));
                        Window_ClientSizeChanged(null, null); //button is in the wrong position for some reason

                        GameState.LocalMap.IsGlobalMap = false;
                    }
                    else if (lce.VisitDungeon != null)
                    {
                        LoadDungeon(lce.VisitDungeon);
                        GameState.LocalMap.IsGlobalMap = false;
                    }
                    else if (lce.VisitCamp != null)
                    {
                        LoadCamp(lce.VisitCamp);
                        GameState.LocalMap.IsGlobalMap = false;
                    }
                    else if (lce.VisitMainMap)
                    {
                        //If it's a bandit camp or a site, update the values of the members
                        if (GameState.LocalMap.Site != null || GameState.LocalMap.Camp != null)
                        {
                            ////Count the amount of actors which aren't the player character or animals and update the counts
                            //TO USE LATER ON WHEN WE WANT A SCOUTING REPORT

                            //int warriors = 0;
                            //int civilians = 0;
                            //int priests = 0;

                            //foreach (var actor in GameState.LocalMap.Actors)
                            //{
                            //    if (actor.EnemyData != null)
                            //    {
                            //        var prof = actor.EnemyData.Profession;

                            //        if (prof == ActorProfession.MERCHANT || prof == ActorProfession.RICH || prof == ActorProfession.WORKER)
                            //        {
                            //            civilians++;
                            //        }
                            //        else if (prof == ActorProfession.WARRIOR)
                            //        {
                            //            warriors++;
                            //        }
                            //        else if (prof == ActorProfession.PRIEST)
                            //        {
                            //            priests++;
                            //        }
                            //    }
                            //}

                            if (GameState.LocalMap.Camp != null)
                            {
                                GameState.LocalMap.Camp.BanditTotal = GameState.LocalMap.Actors.Count(a => a.IsActive && a.IsAlive && !a.IsPlayerCharacter
                                    && a.EnemyData != null && a.EnemyData.Profession == ActorProfession.WARRIOR);
                            }
                            else if (GameState.LocalMap.Site != null)
                            {
                                GameState.LocalMap.Site.SiteData.ActorCounts.Clear();

                                foreach (var actorProfession in (ActorProfession[])Enum.GetValues(typeof(ActorProfession)))
                                {
                                    int count = GameState.LocalMap.Actors.Count(a => a.IsActive && a.IsAlive && !a.IsPlayerCharacter
                                    && a.EnemyData != null && a.EnemyData.Profession == actorProfession);

                                    GameState.LocalMap.Site.SiteData.ActorCounts.Add(actorProfession, count);
                                }

                                if (GameState.LocalMap.Site.SiteData.ActorCounts[ActorProfession.WARRIOR] == 0)
                                {
                                    //Out of warriors, abandon it. We'll decide who really owns it later
                                    GameState.LocalMap.Site.SiteData.OwnerChanged = true;
                                    GameState.LocalMap.Site.SiteData.MapRegenerationRequired = true;
                                    GameState.LocalMap.Site.SiteData.Owners = OwningFactions.ABANDONED;
                                    GameState.LocalMap.Site.SiteData.ActorCounts = new Dictionary<ActorProfession, int>();
                                }
                            }
                        }
                        //Serialise the old map
                        GameState.LocalMap.SerialiseLocalMap();

                        //Clear the stored location items
                        GameState.LocalMap.Camp = null;
                        GameState.LocalMap.Dungeon = null;
                        GameState.LocalMap.Settlement = null;
                        GameState.LocalMap.Site = null;

                        LoadGlobalMap(GameState.PlayerCharacter.GlobalCoordinates);

                        GameState.LocalMap.IsGlobalMap = true;

                    }
                    else if (lce.VisitSite != null)
                    {
                        LoadSite(lce.VisitSite);
                        GameState.LocalMap.IsGlobalMap = false;
                    }
                    else if (lce.RandomEncounter != null)
                    {
                        //Get the biome
                        LoadRandomEncounter(lce.RandomEncounter.Value);
                    }
                }
                else if (feedback.GetType().Equals(typeof(DropItemFeedback)))
                {
                    DropItemFeedback dif = feedback as DropItemFeedback;

                    //Drop the item underneath the player
                    GameState.LocalMap.GetBlockAtCoordinate(dif.ItemToDrop.Coordinate).PutItemUnderneathOnBlock(dif.ItemToDrop);

                    //Remove from inventory
                    dif.ItemToDrop.InInventory = false;

                    GameState.PlayerCharacter.Inventory.Inventory.Remove(dif.ItemToDrop.Category, dif.ItemToDrop);
                }
                else if (feedback.GetType().Equals(typeof(TimePassFeedback)))
                {
                    TimePassFeedback tpf = feedback as TimePassFeedback;

                    //Move time forth
                    GameState.IncrementGameTime(DRTimeComponent.MINUTE, tpf.TimePassInMinutes);

                    //Is the character dead?
                    if (!GameState.PlayerCharacter.IsAlive)
                    {
                        var gameEvent = EventHandlingManager.CreateEvent("Hunger Death");

                        //Create the actual control
                        interfaceComponents.Add(new DecisionPopupComponent(PlayableWidth / 2 - 150, PlayableHeight / 2 - 150, gameEvent));
                    }
                }

            }

            //Update the log control
            log.UpdateLog();

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

            if (this.lastLeftButtonClicked && state.LeftButton == ButtonState.Pressed)
            {
                //Starting to drag
                return MouseActionEnum.DRAG;
            }

            if (this.lastRightButtonClicked && (state.RightButton == ButtonState.Released))
            {
                //right click
                return MouseActionEnum.RIGHT_CLICK;
            }

            //TODO: DO THE REST - WE DON'T NEED THEM FOR NOW
            return null;
        }

        /// <summary>
        /// Loads the Global Map, and drops the player at a particular coordinate
        /// </summary>
        private void LoadGlobalMap(MapCoordinate coordinate)
        {
            //Load from the world map
            var worldMap = GameState.GlobalMap.globalGameMap;

            List<MapBlock> collapsedMap = new List<MapBlock>();

            foreach (var block in worldMap)
            {
                collapsedMap.Add(block);
            }

            GameState.LocalMap = new LocalMap(GameState.GlobalMap.globalGameMap.GetLength(0), GameState.GlobalMap.globalGameMap.GetLength(1), 1, 0);

            //Go through each of them, add them to the local map
            GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

            //Create the player character. For now randomly. Later we'll start at a capital
            GameState.PlayerCharacter.MapCharacter.Coordinate = coordinate;

            MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(coordinate);
            playerBlock.PutItemOnBlock(GameState.PlayerCharacter.MapCharacter);

            GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);
        }

        /// <summary>
        /// Loads a Random encounter at a particular biome
        /// </summary>
        /// <param name="biome"></param>
        private void LoadRandomEncounter(GlobalBiome biome)
        {
            Actor[] actors = null;

            MapCoordinate startPoint = null;
            List<PointOfInterest> pointsOfInterest = null;

            var gennedCamp = WildernessGenerator.GenerateMap(biome, GameState.Random.Next(1, 5), GameState.Random.Next(0, 2), out actors, out startPoint);

            GameState.LocalMap = new LocalMap(gennedCamp.GetLength(0), gennedCamp.GetLength(1), 1, 0);
            GameState.LocalMap.Actors = new List<Actor>();

            List<MapBlock> collapsedMap = new List<MapBlock>();

            foreach (MapBlock block in gennedCamp)
            {
                collapsedMap.Add(block);
            }

            GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

            GameState.PlayerCharacter.MapCharacter.Coordinate = startPoint;

            MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(startPoint);
            playerBlock.PutItemOnBlock(GameState.PlayerCharacter.MapCharacter);
            GameState.LocalMap.Actors.AddRange(actors);
            GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);
            GameState.LocalMap.PointsOfInterest = pointsOfInterest;
        }


        private void LoadCamp(BanditCamp camp)
        {
            if (LocalMap.MapGenerated(camp.UniqueGUID))
            {
                //Reload the map
                var savedMap = LocalMap.DeserialiseLocalMap(camp.UniqueGUID);

                GameState.LocalMap = new LocalMap(savedMap.localGameMap.GetLength(0), savedMap.localGameMap.GetLength(1), 1, 0);

                GameState.LocalMap.Actors = new List<Actor>();

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (MapBlock block in savedMap.localGameMap)
                {
                    collapsedMap.Add(block);
                }

                GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());
                GameState.LocalMap.PathfindingMap = savedMap.PathfindingMap;
                GameState.LocalMap.PointsOfInterest = savedMap.PointsOfInterest;

                GameState.LocalMap.Actors = savedMap.Actors;

                //Find the player character item
                var playerActor = GameState.LocalMap.Actors.Where(a => a.IsPlayerCharacter).FirstOrDefault();

                GameState.PlayerCharacter.MapCharacter.Coordinate = playerActor.MapCharacter.Coordinate;
                GameState.PlayerCharacter.MapCharacter = playerActor.MapCharacter;

                GameState.LocalMap.Camp = camp;
            }
            else
            {
                Actor[] actors = null;

                MapCoordinate startPoint = null;
                List<PointOfInterest> pointsOfInterest = null;

                var gennedCamp = CampGenerator.GenerateCamp(camp.BanditTotal, out startPoint, out actors);

                GameState.LocalMap = new LocalMap(gennedCamp.GetLength(0), gennedCamp.GetLength(1), 1, 0);
                GameState.LocalMap.Actors = new List<Actor>();

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (MapBlock block in gennedCamp)
                {
                    collapsedMap.Add(block);
                }

                GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

                GameState.PlayerCharacter.MapCharacter.Coordinate = startPoint;

                MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(startPoint);
                playerBlock.PutItemOnBlock(GameState.PlayerCharacter.MapCharacter);
                GameState.LocalMap.Actors.AddRange(actors);
                GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);
                GameState.LocalMap.PointsOfInterest = pointsOfInterest;
                GameState.LocalMap.Camp = camp;
            }
        }

        private void LoadDungeon(Dungeon dungeon)
        {

            if (LocalMap.MapGenerated(dungeon.UniqueGUID))
            {
                //Reload the map
                var savedMap = LocalMap.DeserialiseLocalMap(dungeon.UniqueGUID);

                GameState.LocalMap = new LocalMap(savedMap.localGameMap.GetLength(0), savedMap.localGameMap.GetLength(1), 1, 0);

                GameState.LocalMap.Actors = new List<Actor>();

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (MapBlock block in savedMap.localGameMap)
                {
                    collapsedMap.Add(block);
                }

                GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());
                GameState.LocalMap.PathfindingMap = savedMap.PathfindingMap;
                GameState.LocalMap.PointsOfInterest = savedMap.PointsOfInterest;

                GameState.LocalMap.Actors = savedMap.Actors;

                //Find the player character item
                var playerActor = GameState.LocalMap.Actors.Where(a => a.IsPlayerCharacter).FirstOrDefault();

                GameState.PlayerCharacter.MapCharacter.Coordinate = playerActor.MapCharacter.Coordinate;
                GameState.PlayerCharacter.MapCharacter = playerActor.MapCharacter;

                GameState.LocalMap.Dungeon = dungeon;
            }
            else
            {
                Actor[] actors = null;

                DungeonGenerator gen = new DungeonGenerator();
                MapCoordinate startPoint = null;
                List<PointOfInterest> pointsOfInterest = null;

                var gennedDungeon = gen.GenerateDungeon(dungeon.TierCount, dungeon.TrapRooms, dungeon.GuardRooms, dungeon.TreasureRoom, dungeon.OwnerCreatureType, (decimal)dungeon.PercentageOwned, dungeon.MaxWildPopulation, dungeon.MaxOwnedPopulation, out startPoint, out actors, out pointsOfInterest);

                GameState.LocalMap = new LocalMap(gennedDungeon.GetLength(0), gennedDungeon.GetLength(1), 1, 0);
                GameState.LocalMap.Actors = new List<Actor>();

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (MapBlock block in gennedDungeon)
                {
                    collapsedMap.Add(block);
                }

                GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

                GameState.PlayerCharacter.MapCharacter.Coordinate = startPoint;

                MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(startPoint);
                playerBlock.PutItemOnBlock(GameState.PlayerCharacter.MapCharacter);
                GameState.LocalMap.Actors.AddRange(actors);
                GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);
                GameState.LocalMap.PointsOfInterest = pointsOfInterest;
                GameState.LocalMap.Dungeon = dungeon;
            }
        }

        private void LoadSite(MapSite site)
        {
            //Do we already have the map generated ?

            if (LocalMap.MapGenerated(site.UniqueGUID))
            {
                //Reload the map
                var savedMap = LocalMap.DeserialiseLocalMap(site.UniqueGUID);

                GameState.LocalMap = new LocalMap(savedMap.localGameMap.GetLength(0), savedMap.localGameMap.GetLength(1), 1, 0);

                GameState.LocalMap.Site = site;

                GameState.LocalMap.Actors = new List<Actor>();

                Actor[] newActors = null;

                //Before we do anything, check that we don't need to regenerate it
                if (GameState.LocalMap.Site.SiteData.MapRegenerationRequired)
                {
                    MapBlock[,] savedMap2D = new MapBlock[savedMap.localGameMap.GetLength(0), savedMap.localGameMap.GetLength(1)];

                    for(int x =0; x < savedMap.localGameMap.GetLength(0);x++)
                    {
                        for(int y=0; y < savedMap.localGameMap.GetLength(1); y++)
                        {
                            savedMap2D[x, y] = savedMap.localGameMap[x, y, 0]; //NB: CHANGE IF WE GO 3D
                        }
                    }

                    var blocks = SiteGenerator.RegenerateSite(GameState.LocalMap.Site.SiteData, savedMap2D, savedMap.Actors.ToArray(), out newActors);

                    List<MapBlock> collapsedMap = new List<MapBlock>();

                    foreach (MapBlock block in blocks)
                    {
                        collapsedMap.Add(block);
                    }

                    GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());
                    GameState.LocalMap.Actors = newActors.ToList();

                    GameState.LocalMap.Tick(); //Tick to remove the dead actors
                }
                else
                {
                    List<MapBlock> collapsedMap = new List<MapBlock>();

                    foreach (MapBlock block in savedMap.localGameMap)
                    {
                        collapsedMap.Add(block);
                    }

                    GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

                    GameState.LocalMap.Actors = savedMap.Actors;
                }

                LocalMapGenerator lmg = new LocalMapGenerator();

                //Go through the actors
                foreach (var actor in GameState.LocalMap.Actors)
                {
                    //Do we have any vendors ?
                    if (actor.VendorDetails != null)
                    {
                        if (Math.Abs((actor.VendorDetails.GenerationTime - GameState.UniverseTime).GetTimeComponent(DRTimeComponent.MONTH)) > 1)
                        {
                            //More than a month old
                            //Regenerate it
                            lmg.UpdateVendorStock(actor);
                        }
                    }
                }

                //Find the player character item
                var playerActor = GameState.LocalMap.Actors.Where(a => a.IsPlayerCharacter).FirstOrDefault();

                GameState.PlayerCharacter.MapCharacter.Coordinate = playerActor.MapCharacter.Coordinate;
                GameState.PlayerCharacter.MapCharacter = playerActor.MapCharacter;
            }
            else
            {
                Actor[] actors = null;

                var gennedMap = SiteGenerator.GenerateSite(site.SiteData, out actors);

                //Wipe the old map
                GameState.LocalMap = new LocalMap(gennedMap.GetLength(0), gennedMap.GetLength(1), 1, 0);
                GameState.LocalMap.Actors = new List<Actor>();

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (MapBlock block in gennedMap)
                {
                    collapsedMap.Add(block);
                }

                GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

                GameState.PlayerCharacter.MapCharacter.Coordinate = new MapCoordinate(5, 0, 0, MapType.LOCAL);

                MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(new MapCoordinate(5, 0, 0, MapType.LOCAL));
                playerBlock.PutItemOnBlock(GameState.PlayerCharacter.MapCharacter);
                GameState.LocalMap.Actors.AddRange(actors);
                GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);

                GameState.LocalMap.Site = site;
            }
        }

        private void LoadSettlement(Settlement settlement)
        {
            //Do we already have the map generated ?

            if (LocalMap.MapGenerated(settlement.UniqueGUID))
            {
                //Reload the map
                var savedMap = LocalMap.DeserialiseLocalMap(settlement.UniqueGUID);

                GameState.LocalMap = new LocalMap(savedMap.localGameMap.GetLength(0), savedMap.localGameMap.GetLength(1), 1, 0);

                GameState.LocalMap.Actors = new List<Actor>();

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (MapBlock block in savedMap.localGameMap)
                {
                    collapsedMap.Add(block);
                }

                GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

                GameState.LocalMap.Actors = savedMap.Actors;

                LocalMapGenerator lmg = new LocalMapGenerator();

                //Go through the actors
                foreach (var actor in GameState.LocalMap.Actors)
                {
                    //Do we have any vendors ?
                    if (actor.VendorDetails != null)
                    {
                        if (Math.Abs((actor.VendorDetails.GenerationTime - GameState.UniverseTime).GetTimeComponent(DRTimeComponent.MONTH)) > 1)
                        {
                            //More than a month old
                            //Regenerate it
                            lmg.UpdateVendorStock(actor);
                        }
                    }
                }

                //Find the player character item
                var playerActor = GameState.LocalMap.Actors.Where(a => a.IsPlayerCharacter).FirstOrDefault();

                GameState.PlayerCharacter.MapCharacter.Coordinate = playerActor.MapCharacter.Coordinate;
                GameState.PlayerCharacter.MapCharacter = playerActor.MapCharacter;

                GameState.LocalMap.Settlement = settlement;
            }
            else
            {
                List<Actor> actors = null;
                PointOfInterest startPoint = null;

                var gennedMap = SettlementGenerator.GenerateMap(settlement, out actors, out startPoint);

                //Wipe the old map
                GameState.LocalMap = new LocalMap(gennedMap.GetLength(0), gennedMap.GetLength(1), 1, 0);
                GameState.LocalMap.Actors = new List<Actor>();

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (MapBlock block in gennedMap)
                {
                    collapsedMap.Add(block);
                }

                GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());

                GameState.PlayerCharacter.MapCharacter.Coordinate = startPoint.Coordinate;

                MapBlock playerBlock = GameState.LocalMap.GetBlockAtCoordinate(startPoint.Coordinate);
                playerBlock.PutItemOnBlock(GameState.PlayerCharacter.MapCharacter);
                GameState.LocalMap.Actors.AddRange(actors);
                GameState.LocalMap.Actors.Add(GameState.PlayerCharacter);

                GameState.LocalMap.Settlement = settlement;
            }
        }
        #endregion
    }
}
