using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Divine_Right.GameScreens;
using DRObjects.Enums;
using Microsoft.Xna.Framework.Graphics;
using DivineRightGame;

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
            this.Window.Title = "Divine Right Milestone 5 Version 0";
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = WINDOWWIDTH;
            graphics.PreferredBackBufferHeight = WINDOWHEIGHT;

            this.IsMouseVisible = true;
            this.Window.AllowUserResizing = true;
           
            Content.RootDirectory = "Content";

            //PlayableInterface pI = new PlayableInterface(this,graphics);
            MainMenuScreen mI = new MainMenuScreen(this, graphics, "");
            this.Components.Add(mI);

            GameState.NewLog = new List<DRObjects.GraphicsEngineObjects.CurrentLogFeedback>();
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
                    this.Components.Add (new PlayableInterface(this,graphics,requestedArgs));
                }
                else if (requestedInternalAction.Value == InternalActionEnum.GENERATE)
                {
                    //load the world gen
                    this.Components.Clear();
                    this.Components.Add(new WorldGenerationScreen(this, graphics));
                }
                else if (requestedInternalAction.Value == InternalActionEnum.EXIT)
                {
                    //Load the main menu
                    this.Components.Clear();
                    this.Components.Add(new MainMenuScreen(this, graphics, ""));
                }
                else if (requestedInternalAction.Value == InternalActionEnum.DIE)
                {
                    //Load the death
                    this.Components.Clear();
                    this.Components.Add(new DeathScreen(this, graphics));
                }

                requestedInternalAction = null; //set it back to 
            }
        }

        #region Control Events
        #endregion

    }
    }
