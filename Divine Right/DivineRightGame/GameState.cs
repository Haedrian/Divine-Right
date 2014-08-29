using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Items.Archetypes.Global;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace DivineRightGame
{
    [Serializable]
    /// <summary>
    /// Represents the state of the game
    /// </summary>
    public static class GameState
    {
        public static readonly string SAVEPATH =  Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/DivineRight/";
        /// <summary>
        /// The state of the global map
        /// </summary>
        public static GlobalMap GlobalMap { get; set; }
        /// <summary>
        /// The state of the currently loaded local map
        /// </summary>
        public static LocalMap LocalMap { get; set; }

        /// <summary>
        /// The player character
        /// </summary>
        public static Actor PlayerCharacter { get; set; }

        /// <summary>
        /// A log for storing feedback. Only holds one tick's worth.
        /// </summary>
        public static List<CurrentLogFeedback> NewLog { get; set; }
        /// <summary>
        /// A random to use across
        /// </summary>
        public static Random Random
        {
            get
            {

                return _random;

            }
        }
        private static Random _random = new Random();

        /// <summary>
        /// Saves the Game into the folder. There will only be one such savegame at a time.
        /// </summary>
        public static void SaveGame()
        {
            //Create this file so we don't try to serialise a static class
            GameStateSaveFile saveFile = new GameStateSaveFile();
            saveFile.GlobalMap = GameState.GlobalMap;
            saveFile.LocalMap = GameState.LocalMap;
            saveFile.PlayerCharacter = GameState.PlayerCharacter;

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(GameState.SAVEPATH + "GameState.dvg", FileMode.Create, FileAccess.Write, FileShare.None);
            formatter.Serialize(stream, saveFile);
            stream.Close();
        }
        /// <summary>
        /// Loads the game from the folder. There will only ever be one such savegame at a time.
        /// </summary>
        /// <returns></returns>
        public static void LoadGame()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(GameState.SAVEPATH + "GameState.dvg", FileMode.Open, FileAccess.Read, FileShare.Read);
            GameStateSaveFile obj = (GameStateSaveFile)formatter.Deserialize(stream);
            stream.Close();

            //Load it
            GameState.LocalMap = obj.LocalMap;
            GameState.GlobalMap = obj.GlobalMap;
            GameState.PlayerCharacter = obj.PlayerCharacter;
        }

        /// <summary>
        /// Returns true if the save file exists
        /// </summary>
        /// <returns></returns>
        public static bool SaveFileExists()
        {
            return File.Exists(GameState.SAVEPATH + "GameState.dvg");
        }

    }
}
