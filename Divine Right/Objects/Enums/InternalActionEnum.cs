using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Enums
{
    /// <summary>
    /// These are internal actions given to the game engine
    /// </summary>
    public enum InternalActionEnum
    {
        GENERATE,
        NEW,
        SAVE,
        LOAD,
        EXIT,

        DIE,

        OPEN_HEALTH,
        OPEN_ATTRIBUTES,
        OPEN_LOG,
        OPEN_ATTACK,
        LOSE,
        TOGGLE_SETTLEMENT,

    }
}
