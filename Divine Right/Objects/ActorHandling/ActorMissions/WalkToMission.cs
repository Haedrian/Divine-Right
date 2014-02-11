﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.ActorMissions
{
    public class WalkToMission:ActorMission
    {
        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.WALKTO; }
        }

        public override Enums.EnemyThought EnemyThought
        {
            get { return Enums.EnemyThought.WALK; }
        }

        public Stack<MapCoordinate> coordinates { get; set; }
        public MapCoordinate targetCoordinate { get; set; }
    }
}
