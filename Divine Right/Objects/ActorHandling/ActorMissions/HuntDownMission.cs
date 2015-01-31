using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.ActorHandling.ActorMissions
{
    [Serializable]
    public class HuntDownMission:ActorMission
    {
        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.HUNTDOWN; }
        }

        public override EnemyThought EnemyThought
        {
            get { return EnemyThought.WALK; }
        }

        public bool isRetainable = true;

        public override bool IsRetainable
        {
            get { return isRetainable; }
        }

        public Stack<MapCoordinate> Coordinates { get; set; }
        public MapCoordinate TargetCoordinate { get; set; }
        public Actor Target { get; set; }
    }
}
