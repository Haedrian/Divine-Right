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
using DRObjects.DataStructures;
using Microsoft.Xna.Framework;
using DRObjects.DataStructures.Enum;
using DivineRightGame.CombatHandling;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Graphics;

namespace DivineRightGame
{
    [Serializable]
    /// <summary>
    /// Represents the state of the game
    /// </summary>
    public static class GameState
    {
        public static readonly string SAVEPATH = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/DivineRight/";
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

        private static DivineRightDateTime _universeTime = null;

        /// <summary>
        /// Returns an object having the same value as the Universe Time
        /// Modifying this value has no effect. Use IncremementGameTime instead
        /// </summary>
        public static DivineRightDateTime UniverseTime
        {
            get
            {
                return new DivineRightDateTime(_universeTime);
            }
        }

        /// <summary>
        /// Incremements the Game Time by an amount of minutes, and does any processing that needs to be done
        /// </summary>
        /// <param name="minutes"></param>
        public static void IncrementGameTime(DRTimeComponent timeComponent, int value)
        {
            int lastDay = _universeTime.GetTimeComponent(DRTimeComponent.DAY);

            _universeTime.Add(timeComponent, value);

            if (lastDay != _universeTime.GetTimeComponent(DRTimeComponent.DAY))
            {
                //A day has passed. Healing if needs be
                HealthCheckManager.HealCharacter(GameState.PlayerCharacter, 1);

                //Get somewhat hungrier too
                GameState.PlayerCharacter.FeedingLevel--;

                //Do we have any food?
                foreach (var item in GameState.PlayerCharacter.Inventory.Inventory.GetObjectsByGroup(InventoryCategory.SUPPLY))
                {
                    //Do we have anything to feed him ?
                    if ((int)GameState.PlayerCharacter.FeedingLevel < 4)
                    {
                        var cons = item as ConsumableItem;

                        //It's a flag, but if we later have stuff which feeds and does something else, we don't want it. So just take those which only feed
                        if (cons != null && cons.Effects == (ConsumableEffect.FEED))
                        {
                            //Nom Nom Nom!
                           var feedback =  cons.PerformAction(ActionType.CONSUME, GameState.PlayerCharacter, null);
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                //So, is he dead ?
                if (GameState.PlayerCharacter.FeedingLevel <= 0 )
                {
                    //Died of hunger. Silly Billy
                    CombatManager.KillCharacter(GameState.PlayerCharacter);
                }
                else if ((int)GameState.PlayerCharacter.FeedingLevel <= 2)
                {
                    GameState.NewLog.Add(new CurrentLogFeedback(InterfaceSpriteName.MOON, Color.DarkRed, "You are hungry and out of food"));
                }

            }
        }

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

        static GameState()
        {
            //Start off at the 1/1/210
            _universeTime = new DivineRightDateTime(210, 1, 1);
        }

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
            saveFile.UniverseTime = GameState.UniverseTime;

            IFormatter formatter = new BinaryFormatter();

            SurrogateSelector ss = new SurrogateSelector();

            ss.AddSurrogate(typeof(Rectangle), new StreamingContext(StreamingContextStates.All), new RectSerializationSurrogate());
            ss.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), new ColorSerializationSurrogate());

            formatter.SurrogateSelector = ss;

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

            SurrogateSelector ss = new SurrogateSelector();

            ss.AddSurrogate(typeof(Rectangle), new StreamingContext(StreamingContextStates.All), new RectSerializationSurrogate());
            ss.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), new ColorSerializationSurrogate());

            formatter.SurrogateSelector = ss;

            Stream stream = new FileStream(GameState.SAVEPATH + "GameState.dvg", FileMode.Open, FileAccess.Read, FileShare.Read);
            GameStateSaveFile obj = (GameStateSaveFile)formatter.Deserialize(stream);
            stream.Close();

            //Load it
            GameState.LocalMap = obj.LocalMap;
            GameState.GlobalMap = obj.GlobalMap;
            GameState.PlayerCharacter = obj.PlayerCharacter;
            GameState._universeTime = obj.UniverseTime;
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
