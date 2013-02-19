using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects;
using Microsoft.Xna.Framework.Graphics;
using DivineRightGame.Managers;
using System.Threading;

namespace Divine_Right.GameScreens
{
    class WorldGenerationScreen : 
        DrawableGameComponent
    {
        #region Constants
        const int TILEWIDTH = 25;
        const int TILEHEIGHT = 25;

        const int PLAYABLEWIDTH = 950;
        const int PLAYABLEHEIGHT = 450;

        const int TOTALTILESWIDTH = (PLAYABLEWIDTH / TILEWIDTH) - 1;
        const int TOTALTILESHEIGHT = (PLAYABLEHEIGHT / TILEHEIGHT) - 1;
        #endregion

        #region Members

        protected Game game;
        protected GraphicsDeviceManager graphics;
        protected MapCoordinate centrePoint = new MapCoordinate();
        protected SpriteBatch spriteBatch;

        protected int secondsSinceLastRefresh = 0;
        protected int secondsSinceLastMove = 0;

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

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);

            

        }

        

    }
}
