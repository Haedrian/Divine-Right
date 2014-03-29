using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Items.Archetypes.Global;

namespace DivineRightGame
{
    /// <summary>
    /// Represents the state of the game
    /// </summary>
    public static class GameState
    {
        /// <summary>
        /// The state of the global map
        /// </summary>
        public static GlobalMap GlobalMap { get; set; }
        /// <summary>
        /// The state of the currently loaded local map
        /// </summary>
        public static LocalMap LocalMap { get; set; }

        /// <summary>
        /// The list of Very Important People
        /// </summary>
        public static List<Actor> VIPs { get; set; }

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
        /// Performs an internal action - TODO
        /// </summary>
        /// <param name="action"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static PlayerFeedback[] PerformInternalAction(InternalActionEnum action, object[] args)
        {
            throw new NotImplementedException();

        }
    }
}
