using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.ActorMissions
{
    public class PatrolMission:
        ActorMission
    {
        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.PATROL; }
        }

        public override Enums.EnemyThought EnemyThought
        {
            get { return Enums.EnemyThought.WALK; }
        }

        /// <summary>
        /// The point to center patrol around
        /// </summary>
        public MapCoordinate PatrolPoint { get; set; }

        /// <summary>
        /// The range in which to patrol
        /// </summary>
        public int PatrolRange { get; set; }

    }
}
