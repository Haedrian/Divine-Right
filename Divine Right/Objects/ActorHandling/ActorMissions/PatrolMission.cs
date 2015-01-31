using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.LocalMapGeneratorObjects;
using DRObjects.Enums;

namespace DRObjects.ActorHandling.ActorMissions
{
    [Serializable]
    /// <summary>
    /// A mission where we walk to a target point. This is a dummy mission of sorts. It has no data.
    /// </summary>
    public class PatrolMission: ActorMission
    {
        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.PATROL; }
        }

        public override EnemyThought EnemyThought
        {
            get { return EnemyThought.WALK; }
        }

        public override bool IsRetainable
        {
            get { return true; }
        }
    }
}
