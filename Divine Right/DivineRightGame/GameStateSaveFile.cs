using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using DRObjects.DataStructures;

namespace DivineRightGame
{
    [Serializable]
    /// <summary>
    /// This is a non static version of GameState. For saving and loading
    /// </summary>
    public class GameStateSaveFile
    {
        public GlobalMap GlobalMap { get; set; }
        /// <summary>
        /// The state of the currently loaded local map
        /// </summary>
        public LocalMap LocalMap { get; set; }

        /// <summary>
        /// The player character
        /// </summary>
        public Actor PlayerCharacter { get; set; }

        /// <summary>
        /// The Universe time
        /// </summary>
        public DivineRightDateTime UniverseTime { get; set; }
    }
}
