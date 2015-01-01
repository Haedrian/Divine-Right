using System;
using System.Collections.Generic;
using System.IO;

namespace Divine_Right
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (BaseGame game = new BaseGame())
            {
                game.Run();
                game.IsFixedTimeStep = false;
            }

        }
    }
#endif
}

