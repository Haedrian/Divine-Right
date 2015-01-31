using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.ActorHandling
{
    [Serializable]
    public abstract class ActorMission
    {
        public abstract ActorMissionType MissionType {get;}
        public abstract EnemyThought EnemyThought { get; }

        /// <summary>
        /// Whether this Mission is retained in case it is interrupted or whether it is lost
        /// </summary>
        public abstract bool IsRetainable { get; }

    }
}
