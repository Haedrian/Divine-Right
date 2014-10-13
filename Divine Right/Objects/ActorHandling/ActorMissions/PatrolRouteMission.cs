using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.ActorHandling.ActorMissions
{
    /// <summary>
    /// A Mission where the user patrols around a set of points.
    /// Use this to get the next point to walk towards, and then generate a WalkToMission
    /// </summary>
    [Serializable]
    public class PatrolRouteMission:
        ActorMission
    {
        private List<MapCoordinate> patrolRoute;
        /// <summary>
        /// 
        /// </summary>
        public List<MapCoordinate> PatrolRoute
        {
            get
            {
                return patrolRoute;
            }
            set
            {
                this.patrolRoute = value;
                this.PointID = 0;
            }
        }
        private int PointID { get; set; }

        /// <summary>
        /// Gets the next point the patroller has to visit. Will change the Next Point.
        /// </summary>
        /// <returns></returns>
        public MapCoordinate GetNextPoint()
        {
            PointID ++;

            if (PatrolRoute.Count > PointID)
            {
                //Patrol ready, start over
                PointID = 0;
            }

            return PatrolRoute[PointID];
        }


        public override ActorMissionType MissionType
        {
            get { return ActorMissionType.PATROL_ROUTE; }
        }

        public override EnemyThought EnemyThought
        {
            get { return EnemyThought.WAIT; }
        }
    }
}
