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
using System.Threading;
using DRObjects.Items.Tiles.Global;
using DRObjects.ActorHandling;

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
        /// Event fires whenever the minute changes.
        /// </summary>
        public static EventHandler MinuteValueChanged;

        /// <summary>
        /// If the game is running heavy processing we store it here, so we can ignore user updates and show him something
        /// </summary>
        public static bool IsRunningHeavyProcessing = false;

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
        /// Incremements the Game Time by an amount of minutes
        /// </summary>
        /// <param name="minutes"></param>
        public static void IncrementGameTime(DRTimeComponent timeComponent, int value)
        {
            _universeTime.Add(timeComponent, value);                        
        }

        /// <summary>
        /// A log for storing feedback. Only holds one tick's worth.
        /// </summary>
        public static List<LogFeedback> NewLog { get; set; }
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

            //Attach the handlers
            _universeTime.DayChanged += new System.EventHandler(DayChanged);
            _universeTime.MonthChanged += new System.EventHandler(MonthChanged);
            _universeTime.MinuteChanged += new EventHandler(MinuteChanged);
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

            //Reattach handlers
            _universeTime.DayChanged += new System.EventHandler(DayChanged);
            _universeTime.MonthChanged += new System.EventHandler(MonthChanged);
            _universeTime.MinuteChanged += new EventHandler(MinuteChanged);
        }

        /// <summary>
        /// Returns true if the save file exists
        /// </summary>
        /// <returns></returns>
        public static bool SaveFileExists()
        {
            return File.Exists(GameState.SAVEPATH + "GameState.dvg");
        }

        #region Time Handlers


        /// <summary>
        /// When the day changes we need to do some things
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void DayChanged(object sender, EventArgs e)
        {
            //A day has passed. Healing if needs be
            HealthCheckManager.HealCharacter(GameState.PlayerCharacter, 1);

            //Get somewhat hungrier too
            GameState.PlayerCharacter.FeedingLevel--;

            //We hungry?
            while ((int)GameState.PlayerCharacter.FeedingLevel < 4)
            {
                //Do we have any food?
                //It's a flag, but if we later have stuff which feeds and does something else, we don't want it. So just take those which only feed
                var food = GameState.PlayerCharacter.Inventory.Inventory.GetObjectsByGroup(InventoryCategory.SUPPLY).Where(f => (f as ConsumableItem).Effects == ConsumableEffect.FEED).FirstOrDefault();

                if (food != null)
                {
                    //Nom it
                    food.PerformAction(ActionType.CONSUME, GameState.PlayerCharacter, null);
                }
                else
                {
                    //We're out of food
                    break;
                }
            }

            //So, is he dead ?
            if (GameState.PlayerCharacter.FeedingLevel <= 0)
            {
                //Died of hunger. Silly Billy
                CombatManager.KillCharacter(GameState.PlayerCharacter);
            }
            else if ((int)GameState.PlayerCharacter.FeedingLevel <= 2)
            {
                GameState.NewLog.Add(new LogFeedback(InterfaceSpriteName.MOON, Color.DarkRed, "You are hungry and out of food"));
            }
        }

        public static void MonthChanged(object sender, EventArgs e)
        {
            //A month has passed. Go through each site and either reclaim them, or reinforce them

            (new Thread(() =>
            {
                GameState.IsRunningHeavyProcessing = true;

                //Go through each site
                foreach (MapSiteItem item in GameState.GlobalMap.MapSiteItems)
                {
                    //Is the site abandoned?
                    if (item.Site.SiteData.Owners == OwningFactions.ABANDONED)
                    {
                        //Is it reasonable for someone to reclaim it?
                        if ((GameState.GlobalMap.GetBlockAtCoordinate(item.Coordinate).Tile as GlobalTile).Owner.HasValue)
                        {
                            int tileOwner = (GameState.GlobalMap.GetBlockAtCoordinate(item.Coordinate).Tile as GlobalTile).Owner.Value;

                            //Reclaim it!
                            item.Site.SiteData.Owners = tileOwner < 10 ? OwningFactions.HUMANS : tileOwner == 100 ? OwningFactions.ORCS : OwningFactions.BANDITS;

                            item.Site.SiteData.Civilisation = GameState.GlobalMap.Civilisations.First(c => c.ID.Equals(tileOwner));
                            item.Site.SiteData.LoadAppropriateActorCounts();

                            item.Site.SiteData.MapRegenerationRequired = true;
                            item.Site.SiteData.OwnerChanged = true;
                        }
                    }
                    else
                        //Is the tile owned by the same person ?
                        if (item.Site.SiteData.Civilisation.ID == (GlobalMap.GetBlockAtCoordinate(item.Coordinate).Tile as GlobalTile).Owner)
                        {
                            item.Site.SiteData.IncrementActorCounts();
                        }
                }

                GameState.IsRunningHeavyProcessing = false;

            })).Start();

        }

        public static void MinuteChanged(object sender, EventArgs e)
        {
            if (MinuteValueChanged != null)
            {
                //Bubble it up
                MinuteValueChanged(sender, e);
            }
        }


        #endregion

    }
}
