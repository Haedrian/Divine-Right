using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Divine_Right.InterfaceComponents;
using Divine_Right.InterfaceComponents.MainMenuComponents;
using Microsoft.Xna.Framework.Graphics;

namespace Divine_Right.GameScreens
{
    class MainMenuScreen:
        DrawableGameComponent
    {
        #region members

        protected Game game;
        protected GraphicsDeviceManager graphics;
        protected string baseGameMessage;
        protected SpriteBatch sprites;
        protected List<ISystemInterfaceComponent> components= new List<ISystemInterfaceComponent>();

        #endregion

        #region Constructor

        public MainMenuScreen(Game game, GraphicsDeviceManager graphics,string baseGameMessage)
            : base(game)
        {
            this.game = game;
            this.graphics = graphics;
            this.baseGameMessage = baseGameMessage;

        }

        public override void Initialize()
        {
            base.Initialize();
            MainMenuButton button = new MainMenuButton("I love my Gabza", game.Content, DRObjects.Enums.InternalActionEnum.NEW, new object[0], 450, 250);

            components.Add(button);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            sprites = new SpriteBatch(GraphicsDevice);
        }

        public override void  Draw(GameTime gameTime)
        {

            GraphicsDevice.Clear(Color.Black);
            sprites.Begin();

            

            foreach (ISystemInterfaceComponent component in components)
            {
                component.Draw(game.Content, sprites);
            }

            sprites.End();

 	        base.Draw(gameTime);
        }

        #endregion

    }
}
