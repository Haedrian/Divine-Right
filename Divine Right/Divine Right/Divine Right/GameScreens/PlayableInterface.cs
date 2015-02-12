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
using DRObjects.Items.Tiles.Global;
using DivineRightGame.RayTracing;
using DRObjects.Feedback;
using DivineRightGame.Deity;
using DivineRightGame.CombatHandling;
using DRObjects.ActorHandling.SpecialAttacks;

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

        private Color greyedOutColour = new Color(225, 225, 225, 75);

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
        /// This is used to determine when to blink
        /// </summary>
        private int BlinkDrawCounter = 0;
        /// <summary>
        /// This is used to determine what to blink
        /// </summary>
        private int BlinkLargeCounter = 0;

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

                //Give him random stats so he won't completly suck
                CharacterCreation.ProcessParameters(CharacterCreation.GenerateRandom());

                //Also give him a bow of some sort
                InventoryItemManager iim = new InventoryItemManager();

                
                InventoryItem item = iim.GetBestCanAfford("WEAPON", 500);

                GameState.PlayerCharacter.SpecialAttacks[0] = SpecialAttacksGenerator.GenerateSpecialAttack(1);
                GameState.PlayerCharacter.SpecialAttacks[1] = SpecialAttacksGenerator.GenerateSpecialAttack(2);
                GameState.PlayerCharacter.SpecialAttacks[2] = SpecialAttacksGenerator.GenerateSpecialAttack(3);

                GameState.PlayerCharacter.SpecialAttacks[1].TimeOutLeft = 5;

              //  GameState.PlayerCharacter.SpecialAttacks[3] = SpecialAttacksGenerator.GenerateSpecialAttack(4);
               // GameState.PlayerCharacter.SpecialAttacks[4] = SpecialAttacksGenerator.GenerateSpecialAttack(5);

                item.InInventory = true;


                CombatManualComponent cmc = new CombatManualComponent(GameState.PlayerCharacter.SpecialAttacks[1]);

                GameState.PlayerCharacter.Inventory.Inventory.Add(item.Category, item);

                interfaceComponents.Add(cmc);
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

            if (GameState.LocalMap.Location as Settlement != null)
            {
                menuButtons.Add(new AutoSizeGameButton(" Settlement ", this.game.Content, InternalActionEnum.TOGGLE_SETTLEMENT, new object[] { }, 270, GraphicsDevice.Viewport.Height - 35));
                LocationDetailsComponent ldc = new LocationDetailsComponent(GameState.LocalMap.Location as Settlement, PlayableWidth - 170, 0);
                ldc.Visible = true;
                interfaceComponents.Add(ldc);
            }

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


            //Only check if the game is actually active and not busy
            if (Game.IsActive && !GameState.IsRunningHeavyProcessing)
            {


                //Lets see if there are any keyboard keys being pressed

                KeyboardState keyboardState = Keyboard.GetState();

                //Has the user pressed esc?
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    GameState.NewLog.Add(new LogFeedback(InterfaceSpriteName.BANNER_GREEN, Color.White, "Saving Game Please Wait..."));

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

            if (GameState.IsRunningHeavyProcessing)
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor;
            }
            else
            {
                System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default;
            }

            GraphicalBlock[] blocks = UserInterfaceManager.GetBlocksAroundPlayer((TotalTilesWidth / 2), (TotalTilesHeight / 2), 0);

            BlinkDrawCounter++;

            if (BlinkDrawCounter > 40)
            {
                //Restart
                BlinkDrawCounter = 0;
                BlinkLargeCounter++;

                if (BlinkLargeCounter > 100)
                {
                    //restart
                    BlinkLargeCounter = 0;
                }
            }

            //Go through each block ( :( ) and if their coordinates match any temporary graphic, add to it
            foreach (var block in blocks)
            {
                if (BlinkDrawCounter <= 20)
                {
                    var temps = (GameState.LocalMap.TemporaryGraphics.Where(tg => tg.Coord.Equals(block.MapCoordinate))).ToArray();

                    if (temps.Count() > 0)
                    {
                        //Put them in actor graphics 
                        var tempList = block.ActorGraphics.ToList();

                        //To go through them one at a time in order, we'll use % and the blink large counter
                        tempList.Add(temps[BlinkLargeCounter % temps.Length].Graphic);

                        block.ActorGraphics = tempList.ToArray();
                    }
                }
            }

            //get the current state of the game
            //11,4,0
            if (GameState.LocalMap.IsUnderground)
            {
                //Blank out the blocks except the ones next to the player character
                var blankBlocks = blocks.Where(b => Math.Abs(GameState.PlayerCharacter.MapCharacter.Coordinate - b.MapCoordinate) > GameState.PlayerCharacter.LineOfSight).ToArray();

                //Grab the nonblank ones and raycast them to see whether we can see anything
                var nonBlankBlocks = blocks.Where(b => Math.Abs(GameState.PlayerCharacter.MapCharacter.Coordinate - b.MapCoordinate) <= GameState.PlayerCharacter.LineOfSight).ToArray();

                RayTracingHelper.RayTrace(nonBlankBlocks, GameState.PlayerCharacter.MapCharacter.Coordinate);

                for (int i = 0; i < blankBlocks.Length; i++)
                {
                    if (!blankBlocks[i].WasVisited)
                    {
                        blankBlocks[i].TileGraphics = new SpriteData[] { };
                        blankBlocks[i].ItemGraphics = new SpriteData[] { };
                        blankBlocks[i].ActorGraphics = new SpriteData[] { };
                    }
                    else
                    {
                        blankBlocks[i].IsOld = true;
                        blankBlocks[i].ActorGraphics = new SpriteData[] { };
                    }
                }
            }

            //clean the blocks up

            List<InterfaceBlock> iBlocks = this.PrepareGrid(blocks.ToList<GraphicalBlock>());

            this.blocks = iBlocks; //copy them

            //draw them
            spriteBatch.Begin();

            this.DrawGrid(iBlocks);


            // nighttime 
            if (GameState.LocalMap.IsUnderground || GameState.UniverseTime.GetTimeComponent(DRTimeComponent.HOUR) >= 5)
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
                                spriteBatch.Draw(this.game.Content.Load<Texture2D>(tileGraphic.path), rec, block.IsOld ? Color.DarkGray : (tileGraphic.ColorFilter.HasValue ? tileGraphic.ColorFilter.Value : Color.White));
                            }
                            else
                            { //part of a tileset
                                spriteBatch.Draw(this.game.Content.Load<Texture2D>(tileGraphic.path), rec, tileGraphic.sourceRectangle, block.IsOld ? Color.DarkGray : (tileGraphic.ColorFilter.HasValue ? tileGraphic.ColorFilter.Value : Color.White));
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
                    SpriteData[] graphics = new SpriteData[block.ItemGraphics.Length + block.ActorGraphics.Length];
                    block.ItemGraphics.CopyTo(graphics, 0);
                    block.ActorGraphics.CopyTo(graphics, block.ItemGraphics.Length);

                    if (graphics.Length != 0)
                    {
                        foreach (SpriteData itemGraphic in graphics)
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
                                        spriteBatch.Draw(this.game.Content.Load<Texture2D>(itemGraphic.path), rec, block.IsOld ? greyedOutColour : itemGraphic.ColorFilter.HasValue ? itemGraphic.ColorFilter.Value : Color.White);
                                    }
                                    else
                                    { //part of a tileset
                                        spriteBatch.Draw(this.game.Content.Load<Texture2D>(itemGraphic.path), rec, itemGraphic.sourceRectangle, block.IsOld ? greyedOutColour : (itemGraphic.ColorFilter.HasValue ? itemGraphic.ColorFilter.Value : Color.White));
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

            for (int i = 0; i < fb.Length; i++)
            {
                ActionFeedback feedback = fb[i];

                if (feedback == null)
                {
                    continue;
                }

                if (feedback.GetType().Equals(typeof(AttackFeedback)))
                {
                    AttackFeedback af = feedback as AttackFeedback;

                    var combatAf = CombatManager.Attack(af.Attacker, af.Defender, AttackLocation.CHEST); //always attack the chest

                    var tempFBList = fb.ToList();
                    tempFBList.AddRange(combatAf);

                    fb = tempFBList.ToArray();

                }
                else 
                if (feedback.GetType().Equals(typeof(TextFeedback)))
                {
                    MouseState mouse = Mouse.GetState();

                    //Display it
                    interfaceComponents.Add(new ViewTileTextComponent(mouse.X + 15, mouse.Y, (feedback as TextFeedback).Text));
                }
                else if (feedback.GetType().Equals(typeof(LogFeedback)))
                {
                    GameState.NewLog.Add(feedback as LogFeedback);
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
                    else if (iop.InterfaceComponent == InternalActionEnum.OPEN_LOOT)
                    {
                        //Open Loot
                        TreasureChest lootContainer = (iop.Argument as object[])[0] as TreasureChest;

                        LootComponent lc = new LootComponent(100, 100, lootContainer);

                        interfaceComponents.Add(lc);
                    }
                }
                else if (feedback.GetType().Equals(typeof(CreateEventFeedback)))
                {
                    CreateEventFeedback eventFeedback = feedback as CreateEventFeedback;

                    var gameEvent = EventHandlingManager.CreateEvent(eventFeedback.EventName);

                    //Create the actual control
                    interfaceComponents.Add(new DecisionPopupComponent(PlayableWidth / 2 - 150, PlayableHeight / 2 - 150, gameEvent));

                }
                else if (feedback.GetType().Equals(typeof(ReceiveEffectFeedback)))
                {
                    ReceiveEffectFeedback recFeed = feedback as ReceiveEffectFeedback;

                    EffectsManager.PerformEffect(recFeed.Effect.Actor, recFeed.Effect);
                }
                else if (feedback.GetType().Equals(typeof(ReceiveBlessingFeedback)))
                {
                    ReceiveBlessingFeedback blessFeedback = feedback as ReceiveBlessingFeedback;

                    LogFeedback lg = null;

                    //Bless him!
                    //Later we're going to want to do this properly so other characters can get blessed too
                    BlessingManager.GetAndApplyBlessing(GameState.PlayerCharacter, out lg);

                    if (lg != null)
                    {
                        //Log it
                        GameState.NewLog.Add(lg);
                    }
                }
                else if (feedback.GetType().Equals(typeof(ReceiveItemFeedback)))
                {
                    ReceiveItemFeedback receiveFeedback = feedback as ReceiveItemFeedback;

                    //Determine which item we're going to generate
                    InventoryItemManager iim = new InventoryItemManager();

                    InventoryItem itm = iim.GetBestCanAfford(receiveFeedback.Category.ToString(), receiveFeedback.MaxValue);

                    if (itm != null)
                    {
                        itm.InInventory = true;

                        GameState.PlayerCharacter.Inventory.Inventory.Add(itm.Category, itm);

                        GameState.NewLog.Add(new LogFeedback(InterfaceSpriteName.SUN, Color.DarkGreen, "You throw in your offering. You then see something glimmer and take it out"));
                    }
                    else
                    {
                        GameState.NewLog.Add(new LogFeedback(InterfaceSpriteName.MOON, Color.DarkBlue, "You throw in your offering. Nothing appears to be there. Hmm..."));
                    }

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

                    if (lce.Location != null)
                    {
                        LoadLocation(lce.Location);

                        if (lce.Location is Settlement)
                        {

                            //Makde the components visible
                            LocationDetailsComponent ldc = new LocationDetailsComponent(GameState.LocalMap.Location as Settlement, PlayableWidth - 170, 0);
                            ldc.Visible = true;
                            interfaceComponents.Add(ldc);
                            menuButtons.Add(new AutoSizeGameButton(" Settlement ", this.game.Content, InternalActionEnum.TOGGLE_SETTLEMENT, new object[] { }, 270, GraphicsDevice.Viewport.Height - 35));
                            Window_ClientSizeChanged(null, null); //button is in the wrong position for some reason
                        }


                        GameState.LocalMap.IsGlobalMap = false;
                    }
                    else if (lce.VisitMainMap)
                    {
                        //If it's a bandit camp or a site, update the values of the members
                        if (GameState.LocalMap.Location as MapSite != null || GameState.LocalMap.Location as BanditCamp != null)
                        {
                            if (GameState.LocalMap.Location as BanditCamp != null)
                            {
                                var banditCamp = GameState.LocalMap.Location as BanditCamp;

                                banditCamp.BanditTotal = GameState.LocalMap.Actors.Count(a => a.IsActive && a.IsAlive && !a.IsPlayerCharacter
                                    && a.EnemyData != null && a.EnemyData.Profession == ActorProfession.WARRIOR);

                                //Has it been cleared?
                                if (banditCamp.BanditTotal == 0)
                                {
                                    //Find the item
                                    var campItem = GameState.GlobalMap.CampItems.FirstOrDefault(ci => ci.Camp == (GameState.LocalMap.Location as BanditCamp));

                                    if (campItem != null)
                                    {
                                        campItem.IsActive = false;

                                        GameState.GlobalMap.CampItems.Remove(campItem);

                                        //Also find the coordinate of the camp, grab a circle around it and remove the owner
                                        var mapblocks = GameState.GlobalMap.GetBlocksAroundPoint(campItem.Coordinate, WorldGenerationManager.BANDIT_CLAIMING_RADIUS);

                                        foreach (var block in mapblocks)
                                        {
                                            var tile = (block.Tile as GlobalTile);

                                            //Owned by bandit
                                            if (tile.Owner == 50)
                                            {
                                                tile.RemoveOwner();
                                            }
                                        }

                                        //Yes. Let's clear the camp
                                        GameState.NewLog.Add(new LogFeedback(InterfaceSpriteName.SWORD, Color.Black, "You drive the bandits away from the camp"));
                                    }

                                }

                            }
                            else if (GameState.LocalMap.Location as MapSite != null)
                            {
                                var site = GameState.LocalMap.Location as MapSite;

                                site.SiteData.ActorCounts.Clear();

                                foreach (var actorProfession in (ActorProfession[])Enum.GetValues(typeof(ActorProfession)))
                                {
                                    int count = GameState.LocalMap.Actors.Count(a => a.IsActive && a.IsAlive && !a.IsPlayerCharacter
                                    && a.EnemyData != null && a.EnemyData.Profession == actorProfession);

                                    site.SiteData.ActorCounts.Add(actorProfession, count);
                                }

                                if (site.SiteData.ActorCounts[ActorProfession.WARRIOR] == 0)
                                {
                                    //Out of warriors, abandon it. We'll decide who really owns it later
                                    site.SiteData.OwnerChanged = true;
                                    site.SiteData.MapRegenerationRequired = true;
                                    site.SiteData.Owners = OwningFactions.ABANDONED;
                                    site.SiteData.ActorCounts = new Dictionary<ActorProfession, int>();
                                }
                            }
                        }
                        //Serialise the old map
                        GameState.LocalMap.SerialiseLocalMap();

                        //Clear the stored location items
                        GameState.LocalMap.Location = null;

                        LoadGlobalMap(GameState.PlayerCharacter.GlobalCoordinates);

                        GameState.LocalMap.IsGlobalMap = true;

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
                else if (feedback.GetType().Equals(typeof(VisitedBlockFeedback)))
                {
                    VisitedBlockFeedback vbf = feedback as VisitedBlockFeedback;

                    //Visit a region equal to the line of sight of the player character - 
                    var blocks = GameState.LocalMap.GetBlocksAroundPoint(vbf.Coordinate, GameState.PlayerCharacter.LineOfSight);

                    //Only do the ones which can be ray traced
                    foreach (var block in RayTracingHelper.RayTraceForExploration(blocks, GameState.PlayerCharacter.MapCharacter.Coordinate))
                    {
                        block.WasVisited = true;
                    }
                }
                else if (feedback.GetType().Equals(typeof(DescendDungeonFeedback)))
                {
                    DescendDungeonFeedback ddf = feedback as DescendDungeonFeedback;

                    (GameState.LocalMap.Location as Dungeon).DifficultyLevel++;

                    this.LoadLocation(GameState.LocalMap.Location, true);
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

        /// <summary>
        /// Load the location
        /// </summary>
        /// <param name="location"></param>
        /// <param name="forceRegenerate">If set to true, will regenerate the item anyway</param>
        private void LoadLocation(Location location, bool forceRegenerate = false)
        {
            if (LocalMap.MapGenerated(location.UniqueGUID) && !forceRegenerate)
            {
                //Reload the map
                var savedMap = LocalMap.DeserialiseLocalMap(location.UniqueGUID);

                GameState.LocalMap = new LocalMap(savedMap.localGameMap.GetLength(0), savedMap.localGameMap.GetLength(1), 1, 0);

                GameState.LocalMap.Location = savedMap.Location;

                //GameState.LocalMap.Location = location;

                if (GameState.LocalMap.Location is MapSite && (GameState.LocalMap.Location as MapSite).SiteData.MapRegenerationRequired)
                {
                    Actor[] newActors = null;

                    //Before we do anything, check that we don't need to regenerate it
                    MapBlock[,] savedMap2D = new MapBlock[savedMap.localGameMap.GetLength(0), savedMap.localGameMap.GetLength(1)];

                    for (int x = 0; x < savedMap.localGameMap.GetLength(0); x++)
                    {
                        for (int y = 0; y < savedMap.localGameMap.GetLength(1); y++)
                        {
                            savedMap2D[x, y] = savedMap.localGameMap[x, y, 0]; //NB: CHANGE IF WE GO 3D
                        }
                    }

                    var blocks = SiteGenerator.RegenerateSite((GameState.LocalMap.Location as MapSite).SiteData, savedMap2D, savedMap.Actors.ToArray(), out newActors);

                    List<MapBlock> cM = new List<MapBlock>();

                    foreach (MapBlock block in blocks)
                    {
                        cM.Add(block);
                    }

                    GameState.LocalMap.AddToLocalMap(cM.ToArray());
                    GameState.LocalMap.Actors = newActors.ToList();

                    GameState.LocalMap.Tick(); //Tick to remove the dead actors
                }

                GameState.LocalMap.Actors = new List<Actor>();

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (MapBlock block in savedMap.localGameMap)
                {
                    collapsedMap.Add(block);
                }

                GameState.LocalMap.AddToLocalMap(collapsedMap.ToArray());
                GameState.LocalMap.PathfindingMap = savedMap.PathfindingMap;
                GameState.LocalMap.PointsOfInterest = savedMap.PointsOfInterest;
                GameState.LocalMap.IsUnderground = savedMap.IsUnderground;

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

                //GameState.LocalMap.Location = location;
            }
            else
            {
                Actor[] actors = null;

                MapCoordinate startPoint = null;
                List<PointOfInterest> pointsOfInterest = null;

                MapBlock[,] gennedMap = null;

                if (location is BanditCamp)
                {
                    gennedMap = CampGenerator.GenerateCamp((location as BanditCamp).BanditTotal, out startPoint, out actors);
                }
                else if (location is Citadel)
                {
                    var citadel = location as Citadel;

                    CitadelGenerator gen = new CitadelGenerator();
                    gennedMap = gen.GenerateDungeon(citadel.TierCount, citadel.TrapRooms, citadel.GuardRooms, citadel.TreasureRoom, citadel.OwnerCreatureType, (decimal)citadel.PercentageOwned, citadel.MaxWildPopulation, citadel.MaxOwnedPopulation, out startPoint, out actors, out pointsOfInterest);
                }
                else if (location is MapSite)
                {
                    var mapSite = location as MapSite;

                    gennedMap = SiteGenerator.GenerateSite(mapSite.SiteData, out actors);
                    startPoint = new MapCoordinate(gennedMap.GetLength(0) / 2, 0, 0, MapType.LOCAL);
                }
                else if (location is Settlement)
                {
                    List<Actor> settlementActors = null;
                    PointOfInterest sp = null;

                    gennedMap = SettlementGenerator.GenerateMap((location as Settlement), out settlementActors, out sp);

                    actors = settlementActors.ToArray();
                    startPoint = sp.Coordinate;
                }
                else if (location is Dungeon)
                {
                    Dungeon dungeon = null;

                    gennedMap = DungeonGenerator.GenerateDungeonLevel((location as Dungeon).DifficultyLevel, 80, out startPoint, out actors, out dungeon);

                    //Copy the changes, that way we retain the object reference and the guid for serialization
                    (location as Dungeon).Rooms = dungeon.Rooms;
                    (location as Dungeon).SummoningCircles = dungeon.SummoningCircles;

                }

                GameState.LocalMap = new LocalMap(gennedMap.GetLength(0), gennedMap.GetLength(1), 1, 0);
                GameState.LocalMap.Actors = new List<Actor>();

                List<MapBlock> collapsedMap = new List<MapBlock>();

                foreach (MapBlock block in gennedMap)
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
                GameState.LocalMap.Location = location;

                GameState.LocalMap.IsUnderground = (location is Dungeon);

                if (location is Dungeon) //Spawn at least 10 enemies
                {
                    for (int i = 0; i < 10; i++)
                    {
                        GameState.LocalMap.MinuteChanged(null, null); //Summon!
                    }
                }

            }
        }
        #endregion
    }
}
