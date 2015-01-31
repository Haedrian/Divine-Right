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

        public bool isRetainable = true;

        public override bool IsRetainable
        {
            get { return isRetainable; }
        }

        /// <summary>
        /// How close it is acceptable to be to count as having reached the point
        /// </summary>
        public int AcceptableRadius { get; set; }

        public WalkToMission()
        {
            AcceptableRadius = 1;
        }
    }
}
