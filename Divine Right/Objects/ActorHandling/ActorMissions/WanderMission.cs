﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

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
        public Rectangle WanderRectangle { get; set; }

        /// <summary>
        /// The percentage chance that the character loiters instead of taking a step
        /// </summary>
        public int LoiterPercentage { get; set; }

        public WanderMission()
        {
            LoiterPercentage = 0;
        }
    }
}
