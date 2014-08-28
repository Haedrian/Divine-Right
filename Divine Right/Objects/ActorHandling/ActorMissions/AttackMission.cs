using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public override Enums.EnemyThought EnemyThought
        {
            get { return Enums.EnemyThought.ATTACK; }
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
