using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightGame.Managers
{
    public class WorldGenerationManager
    {
        /// <summary>
        /// How many tiles of Width
        /// </summary>
        public static const int WIDTH = 100;
        /// <summary>
        /// How many tiles of Height
        /// </summary>
        public static const int HEIGHT = 100;
        
        protected static bool isGenerating = false;
        /// <summary>
        /// Determines whether the code is generating or not. This will be used to tell the interface to do something else.
        /// All other things (saving et cetera) should be done by the time this flag is triggered
        /// </summary>
        public static bool IsGenerating
        {
            get
            {
                lock (GlobalMap.lockMe)
                {
                    return isGenerating;
                }
            }
        }
        /// <summary>
        /// Generates the global map
        /// </summary>
        public static void GenerateWorld()
        {
            isGenerating = true;

            GlobalMap globalMap = GameState.GlobalMap;

            //Hey Byro - put your code in here.
            //Before you modify the global map, lock the code as seen below.
            //Also if you need to create any new tiles or objects ( you will) put them in DRObjects/Items - and be sure to extend correctly
            //You could lock EVERYTHING, but then the world generation won't show anything and will hang instead :(
            //Instead try to lock/unlock in a loop
            //Don't change any part of the global map outside of the lock or we'll have a race condition

            lock (GlobalMap.lockMe)
            {



            }

        }

    }
}
