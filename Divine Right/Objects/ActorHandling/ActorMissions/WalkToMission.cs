using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.ActorHandling.ActorMissions
{
    [Serializable]
    public class WalkToMission:ActorMission
    {
        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.WALKTO; }
        }

        public override EnemyThought EnemyThought
        {
            get { return EnemyThought.WALK; }
        }

        public Stack<MapCoordinate> Coordinates { get; set; }
        public MapCoordinate TargetCoordinate { get; set; }
    }
}
