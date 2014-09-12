using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Enums
{
    public enum ActionTypeEnum
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
    }
}
