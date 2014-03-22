using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.ActorHandling.ActorMissions
{
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

        public override Enums.EnemyThought EnemyThought
        {
            get { return Enums.EnemyThought.WAIT; }
        }
    }
}
