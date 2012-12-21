using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;

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
    }
}
