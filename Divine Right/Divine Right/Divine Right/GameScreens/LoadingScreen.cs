using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Enums;
using DivineRightGame;
using System.Threading.Tasks;

namespace Divine_Right.GameScreens
{
    public class LoadingScreen
        :DrawableGameComponent
    {
          #region Members
       protected Game game;
       protected GraphicsDeviceManager graphics;
       protected SpriteBatch sprites;
       bool isSave = false;
       bool saveTaskStarted = false;
       #endregion


       public LoadingScreen(Game game, GraphicsDeviceManager graphics, object[] parameters)
            : base(game)
        {
            this.game = game;
            this.graphics = graphics;
            isSave = (parameters[0].Equals("Save"));
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

           if (isSave)
           {
               sprites.Draw(game.Content.Load<Texture2D>("Graphics/Interface/savingImage"), new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), Color.White);

               sprites.End();

               if (!saveTaskStarted)
               {
                   saveTaskStarted = true;

                   Task.Factory.StartNew(() => { SaveAndQuit(); });
               }
               
           }
           else
           {
               sprites.Draw(game.Content.Load<Texture2D>("Graphics/Interface/loadingImage"), new Rectangle(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height), Color.White);

               sprites.End();

               //Do the actual loading that we really want
               BaseGame.requestedInternalAction = InternalActionEnum.LOAD;
               BaseGame.requestedArgs = new object[1] { "Continue" };
           }
       }

        private void SaveAndQuit()
       {
           GameState.SaveGame();

           //We're done? Go back to the main menu
           BaseGame.requestedInternalAction = InternalActionEnum.EXIT;
           BaseGame.requestedArgs = new object[0];
       }

    }
}
