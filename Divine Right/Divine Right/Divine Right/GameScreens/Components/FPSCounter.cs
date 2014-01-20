using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Divine_Right.GameScreens.Components
{
    public class FPSCounter : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        Game game;

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;


        public FPSCounter(Game game)
            : base(game)
        {
            this.game = game;
            //content = new ContentManager(game.Services);
        }


        protected override void LoadContent()
        {

            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = this.game.Content.Load<SpriteFont>("Fonts/TextFeedbackFont");

        }


        protected override void UnloadContent()
        {
            if (content != null)
            {
                content.Unload();
            }
        }


        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }


        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, fps, new Vector2(33, 10), Color.CornflowerBlue);
            spriteBatch.DrawString(spriteFont, fps, new Vector2(32, 10), Color.White);

            spriteBatch.End();
        }
    }
}
