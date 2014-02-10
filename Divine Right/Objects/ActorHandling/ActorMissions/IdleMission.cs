using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.ActorMissions
{
    /// <summary>
    /// For this mission, do nothing
    /// </summary>
    public class IdleMission:
        ActorMission
    {

        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.IDLE; }
        }

        public override Enums.EnemyThought EnemyThought
        {
            get { return Enums.EnemyThought.WAIT; }
        }
    }
}
