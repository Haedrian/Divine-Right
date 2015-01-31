using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.ActorHandling.ActorMissions
{
    [Serializable]
    public class WaitMission:ActorMission
    {
        /// <summary>
        /// How long to wait
        /// </summary>
        public int WaitTime{get;set;}

        public WaitMission(int waitTime)
        {
            this.WaitTime = waitTime;
        }

        public void Wait()
        {
            this.WaitTime--;
        }

        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.WAIT; }
        }

        public override EnemyThought EnemyThought
        {
            get { return EnemyThought.WAIT; }
        }

        public override bool IsRetainable
        {
            get { return true; }
        }
    }
}
