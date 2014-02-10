using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.ActorHandling
{
    public abstract class ActorMission
    {
        public abstract ActorMissionType MissionType {get;}
        public abstract EnemyThought EnemyThought { get; }

    }
}
