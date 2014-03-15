using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Divine_Right.InterfaceComponents;
using DRObjects.Graphics;
using Divine_Right.HelperFunctions;
using Microsoft.Xna.Framework.Input;
using DRObjects.Enums;

namespace Divine_Right.GameScreens
{
    /// <summary>
    /// Screen describing that the player died, their score or whatever and leading them back to the main menu
    /// </summary>
    public class DeathScreen
        : DrawableGameComponent
    {
        #region Members
        protected Game game;
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch sprites;
        protected List<ISystemInterfaceComponent> components = new List<ISystemInterfaceComponent>();

        private Rectangle rect;
        #endregion

        #region Constructors
        public DeathScreen(Game game, GraphicsDeviceManager graphics)
            : base(game)
        {
            this.game = game;
            this.graphics = graphics;
        }
        #endregion

        #region Functions

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
            rect = new Rectangle(10,10,GraphicsDevice.Viewport.Width-20,GraphicsDevice.Viewport.Height-20);

            GraphicsDevice.Clear(Color.Black);
            sprites.Begin();

            //draw the deadness
            var graveyard = SpriteManager.GetSprite(InterfaceSpriteName.DEAD);

            sprites.Draw(game.Content,graveyard,rect,Color.White);

            sprites.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            MouseState mouse = Mouse.GetState();

            if (mouse.LeftButton == ButtonState.Pressed)
            {
                //Back to the main menu
                BaseGame.requestedInternalAction = InternalActionEnum.EXIT;
                BaseGame.requestedArgs = new object[0];
            }
        }

        #endregion
    }
}
