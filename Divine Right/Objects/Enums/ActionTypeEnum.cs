using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Enums
{
    public enum ActionType
    {
        IDLE = 0,
        LOOK = 1,
        EXAMINE = 2,
        USE = 3,
        MOVE = 4,
        PREPARE_ATTACK = 5,
        ATTACK = 6,
        EXPLORE,
        LEAVE,
        TAKE,
        EQUIP,
        DROP,
        UNEQUIP,
        TRADE,
        MULTIDECISION,
        CONSUME,
        SHOVE,
        HUNT,
        ASCEND_TO_SURFACE,
        DESCEND,
        DISABLE,
        LOOT,
        BLESS,
        PRAY,
        TOSS_IN_10_COINS,
        TOSS_IN_100_COINS,
        TOSS_IN_50_COINS,
        READ,
    }
}
