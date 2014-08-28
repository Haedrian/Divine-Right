using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.ActorMissions
{
    [Serializable]
    public class HuntDownMission:ActorMission
    {
        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.HUNTDOWN; }
        }

        public override Enums.EnemyThought EnemyThought
        {
            get { return Enums.EnemyThought.WALK; }
        }

        public Stack<MapCoordinate> Coordinates { get; set; }
        public MapCoordinate TargetCoordinate { get; set; }
        public Actor Target { get; set; }
    }
}
