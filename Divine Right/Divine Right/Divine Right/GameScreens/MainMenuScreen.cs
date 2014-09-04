using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Divine_Right.InterfaceComponents;
using Divine_Right.InterfaceComponents.MainMenuComponents;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DRObjects.Enums;
using DivineRightGame;

namespace Divine_Right.GameScreens
{
    class MainMenuScreen :
        DrawableGameComponent
    {
        #region members

        protected Game game;
        protected GraphicsDeviceManager graphics;
        protected string baseGameMessage;
        protected SpriteBatch sprites;
        protected List<ISystemInterfaceComponent> components = new List<ISystemInterfaceComponent>();

        #endregion

        #region Constructor

        public MainMenuScreen(Game game, GraphicsDeviceManager graphics, string baseGameMessage)
            : base(game)
        {
            this.game = game;
            this.graphics = graphics;
            this.baseGameMessage = baseGameMessage;

        }
        #endregion
        #region Function

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            sprites = new SpriteBatch(GraphicsDevice);
        }

        public override void Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);
            sprites.Begin();

            //draw the title

            SpriteFont titleFont = game.Content.Load<SpriteFont>(@"Fonts/TitleFont");

            Vector2 titleSize = titleFont.MeasureString("Divine Right");

            //We want it to be in the centre
            Rectangle titleRect = new Rectangle((int)(GraphicsDevice.Viewport.Width / 2 - titleSize.X), (int)(50 - titleSize.Y), (int)titleSize.X * 2, (int)titleSize.Y * 2);

            Vector2 stringDraw = new Vector2(titleRect.Center.X - (titleSize.X / 2), titleRect.Center.Y - (titleSize.Y / 2));

            sprites.DrawString(titleFont, "Divine Right", stringDraw, Color.SlateGray);

            components.Clear();

            //add the buttons

            components.Add(new AutoSizeButton("Generate Test Local Map", game.Content, InternalActionEnum.LOAD, new object[1] { "Village" }, (GraphicsDevice.Viewport.Width / 2), 200));

            /* These are the old buttons
            components.Add(new AutoSizeButton("Generate Test World", game.Content, DRObjects.Enums.InternalActionEnum.GENERATE, new object[0], (GraphicsDevice.Viewport.Width / 2), 150));

            components.Add(new AutoSizeButton("Generate Test Dungeon", game.Content, InternalActionEnum.LOAD, new object[1] { "Dungeon" }, (GraphicsDevice.Viewport.Width / 2), 250));
            */

            components.Add(new AutoSizeButton("Start New Game",game.Content,InternalActionEnum.GENERATE, new object[0], (GraphicsDevice.Viewport.Width/2),150));
            if (GameState.SaveFileExists())
            {
                components.Add(new AutoSizeButton("Continue Game", game.Content, InternalActionEnum.CONTINUE, new object[1] { "Continue" }, (GraphicsDevice.Viewport.Width / 2), 200));
            }

            components.Add(new AutoSizeButton("Credits",game.Content,InternalActionEnum.CREDITS,new object[0],(GraphicsDevice.Viewport.Width/2),250));

            foreach (ISystemInterfaceComponent component in components)
            {
                component.Draw(game.Content, sprites);
            }

            Rectangle logoRect = new Rectangle(5,(int)(GraphicsDevice.Viewport.Height - 65),50,60);

            sprites.Draw(game.Content.Load<Texture2D>(@"Graphics/Interface/epicLlama"), logoRect, Color.White);
            sprites.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //this screen only supports left mouse clicks

            MouseState mouse = Mouse.GetState();
            InternalActionEnum? action = null;
            object[] args = null;

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                Point mousePoint = new Point(mouse.X, mouse.Y);

                foreach (ISystemInterfaceComponent component in components)
                {
                    if (component.ReturnLocation().Contains(mousePoint))
                    {
                        //handle it
                        if (component.HandleClick(mouse.X, mouse.Y, out action, out args))
                        {
                            break;

                        }
                    }

                }
            }

            if (action != null)
            {
                //give it to the base game
                BaseGame.requestedInternalAction = action;
                BaseGame.requestedArgs = args;

            }
        }

        #endregion

    }
}
