using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.ActorMissions
{
    public class WanderMission:
        ActorMission
    {
        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.WANDER; }
        }

        public override Enums.EnemyThought EnemyThought
        {
            get { return Enums.EnemyThought.WALK; }
        }

        /// <summary>
        /// The point to center wandering around
        /// </summary>
        public MapCoordinate WanderPoint { get; set; }

        /// <summary>
        /// The range in which to wander
        /// </summary>
        public int WanderRange { get; set; }

    }
}
