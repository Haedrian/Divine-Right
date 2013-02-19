using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Divine_Right.GameScreens;
using DRObjects.Enums;

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
        public static InternalActionEnum? requestedInternalAction = null;
        public static object[] requestedArgs;


        #endregion

        public BaseGame()
        {
            this.Window.Title = "Divine Right Milestone 0 Version 7";
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = WINDOWWIDTH;
            graphics.PreferredBackBufferHeight = WINDOWHEIGHT;

            this.IsMouseVisible = true;

            Content.RootDirectory = "Content";

            //PlayableInterface pI = new PlayableInterface(this,graphics);
            MainMenuScreen mI = new MainMenuScreen(this, graphics, "");
            this.Components.Add(mI);
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //check whetehr the requestedInternalAction is null or not

            if (requestedInternalAction !=null)
            {
                //not null, change the screen

                if (requestedInternalAction.Value == InternalActionEnum.LOAD)
                {
                    //todo: cleaner implementation later

                    //load the main game
                    this.Components.Clear();
                    this.Components.Add (new PlayableInterface(this,graphics));
                }

                requestedInternalAction = null; //set it back to null
            }
        }

        }
    }
