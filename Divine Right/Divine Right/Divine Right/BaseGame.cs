using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Divine_Right.GameScreens;

namespace Divine_Right
{
    public class BaseGame:
        Game
    {

        #region Constants
        
        const int WINDOWWIDTH = 900;
        const int WINDOWHEIGHT = 500;
        #endregion

        #region members

        public GraphicsDeviceManager graphics;

        #endregion

        public BaseGame()
        {
            this.Window.Title = "Divine Right Milestone 0 Version 5";
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = WINDOWWIDTH;
            graphics.PreferredBackBufferHeight = WINDOWHEIGHT;

            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";

            PlayableInterface pI = new PlayableInterface(this,graphics);
            this.Components.Add(pI);
        }

        }
    }
