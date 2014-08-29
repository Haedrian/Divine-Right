using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using DivineRightGame;
using DRObjects.Enums;

namespace Divine_Right.GameScreens
{
    /// <summary>
    /// The screen displaying credits.
    /// Pretty simple. Just display a single image and click anywhere to go back
    /// </summary>
   public class CreditsScreen
       : DrawableGameComponent
   {

       #region Members
       protected Game game;
       protected GraphicsDeviceManager graphics;
       protected SpriteBatch sprites;
       #endregion


       public CreditsScreen(Game game, GraphicsDeviceManager graphics)
            : base(game)
        {
            this.game = game;
            this.graphics = graphics;
        }

       protected override void LoadContent()
       {
           base.LoadContent();
           sprites = new SpriteBatch(GraphicsDevice);
       }

       public override void Draw(GameTime gameTime)
       {
           base.Draw(gameTime);

           GraphicsDevice.Clear(Color.Black);
           sprites.Begin();

           sprites.Draw(game.Content.Load<Texture2D>("Graphics/Interface/creditsPage"), new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height),Color.White);

           sprites.End();
       }

       public override void Update(GameTime gameTime)
       {
           MouseState mouse = Mouse.GetState();

           if (mouse.LeftButton == ButtonState.Pressed)
           {
               //Go back
               BaseGame.requestedInternalAction = InternalActionEnum.EXIT;
               BaseGame.requestedArgs = new object[0];
           }
       }
    }
}
