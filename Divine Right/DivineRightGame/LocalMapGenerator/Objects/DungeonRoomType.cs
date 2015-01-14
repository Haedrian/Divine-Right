using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DivineRightGame.LocalMapGenerator.Objects
{
    /// <summary>
    /// The type of Dungeon Room
    /// </summary>
    public enum DungeonRoomType
    {
        ENTRANCE,
        EXIT,
        SUMMONING,
        TREASURE, //DONE
        ARMOURY,
        TEMPLE,
        POOL,
        DESTROYED_CAMP,
        WISHING_WELL,
        /*POTIONS,
        LIBRARY,
        CRYPT,
        ABANDONED_FARM,
        COMBAT_PIT,
        PRISON,
        TORTURE_CHAMBER,
        MAP,
        EMPTY,
        ALTAR,
        THRONE
         */
    }
}
