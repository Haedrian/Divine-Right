using System;

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
            using (DRGame game = new DRGame())
            {
                game.Run();
            }
        }
    }
#endif
}

