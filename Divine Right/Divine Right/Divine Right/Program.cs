using System;
using Microsoft.Xna.Framework.GamerServices;
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
            try
            {
                using (BaseGame game = new BaseGame())
                {
                    game.Run();
                }
            }
            catch (Exception ex)
            {
               // Guide.BeginShowMessageBox("Error Has Occured", ex.Message, new List<string>(), 0, MessageBoxIcon.Error, null, null);
                TextWriter writer = new StreamWriter(System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "error.txt", true);
                writer.WriteLine(DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss") + ex.Message);
                writer.Flush();
                writer.Close();
            }
        }
    }
#endif
}

