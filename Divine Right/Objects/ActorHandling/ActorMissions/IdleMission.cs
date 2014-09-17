using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.ActorHandling.ActorMissions
{
    [Serializable]
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

        public override EnemyThought EnemyThought
        {
            get { return EnemyThought.WAIT; }
        }
    }
}
