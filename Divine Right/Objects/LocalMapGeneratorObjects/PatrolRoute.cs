using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.ActorMissions;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Enums;
using Microsoft.Xna.Framework;

namespace DRObjects.LocalMapGeneratorObjects
{
    [Serializable]
    public class PatrolRoute
    {
        public string PatrolName { get; set; }
        public OwningFactions Owners { get; set; }
        public ActorProfession Profession { get; set; }

        /// <summary>
        /// The collection of points that make up the route, as well as the acceptable radius of each
        /// </summary>
        public List<PatrolPoint> Route { get; set; }

        /// <summary>
        /// Produces a list of grouped patrol routes from a collection of points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static List<PatrolRoute> GetPatrolRoutes(IEnumerable<MapletPatrolPoint> points)
        {
            List<PatrolRoute> routes = new List<PatrolRoute>();

            //Clean up the points, give those with a null name an empty string instead
            foreach(var point in points)
            {
                point.PatrolName = point.PatrolName ?? String.Empty;

                var parentRoute = routes.Where(r => r.PatrolName.Equals(point.PatrolName) && r.Owners.Equals(point.Factions) && r.Profession.Equals(point.Profession)).FirstOrDefault();

                if (parentRoute == null)
                {
                    //Create a new one
                    routes.Add(new PatrolRoute()
                        {
                            Owners = point.Factions,
                            PatrolName = point.PatrolName,
                            Profession = point.Profession,
                            Route = new List<PatrolPoint>() { new PatrolPoint() { AcceptableRadius = point.PointRadius, Coordinate = point.Point } }
                        });
                }
                else
                {
                    //Add it
                    parentRoute.Route.Add(new PatrolPoint() { AcceptableRadius = point.PointRadius, Coordinate = point.Point });
                }
            }

            return routes;


        }
    }
}
