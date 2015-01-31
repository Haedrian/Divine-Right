using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.ActorHandling.ActorMissions
{
    [Serializable]
    public class AttackMission:
        ActorMission
    {
        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.ATTACK; }
        }

        public override EnemyThought EnemyThought
        {
            get { return EnemyThought.ATTACK; }
        }

        public override bool IsRetainable
        {
            get { return true; }
        }

        /// <summary>
        /// Who is getting attacked
        /// </summary>
        public Actor AttackTarget { get; set; }

        public AttackMission(Actor target)
        {
            this.AttackTarget = target;
        }
    }
}
