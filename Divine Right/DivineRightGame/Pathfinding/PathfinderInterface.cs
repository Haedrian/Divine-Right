using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects;
using System.Drawing;

namespace DivineRightGame.Pathfinding
{
    /// <summary>
    /// An interface for using the Pathfinding algorithm
    /// </summary>
    public static class PathfinderInterface
    {
        private static byte[,] nodes;
        private static IPathFinder pathFinder;

        /// <summary>
        /// The Nodes
        /// </summary>
        public static byte[,] Nodes
        {
            get { return nodes; }
            set
            {
                nodes = value;
                pathFinder = null; //clear the pathfinder
            }
        }

        public static bool HasLoadedNodes
        {
            get
            {
                return nodes != null;
            }
        }

        /// <summary>
        /// Gets a path from the startPoint to the endPoint. Or null if there are no possible points
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public static Stack<MapCoordinate> GetPath(MapCoordinate startPoint, MapCoordinate endPoint)
        {
            if (pathFinder == null)
            {
                if (nodes == null)
                {
                    GameState.LocalMap.GeneratePathfindingMap();
                    nodes = GameState.LocalMap.PathfindingMap;
                }

                //Create the new pathfinder with the map - add the settings
                pathFinder = new PathFinderFast(nodes);

                pathFinder.Formula = HeuristicFormula.Manhattan;
                pathFinder.Diagonals = false;
                pathFinder.HeavyDiagonals = false;
                pathFinder.HeuristicEstimate = 2;
                pathFinder.PunishChangeDirection = true;
                pathFinder.TieBreaker = false;
                pathFinder.SearchLimit = 5000;
            }

            List<PathFinderNode> path = pathFinder.FindPath(new Point(startPoint.X, startPoint.Y), new Point(endPoint.X, endPoint.Y));

            Stack<MapCoordinate> coordStack = new Stack<MapCoordinate>();

            if (path == null || nodes[path[0].X, path[0].Y] == 255)
            {
                Console.WriteLine("No path found :( ");
                return null;

            }

            foreach (PathFinderNode node in path)
            {
                coordStack.Push(new MapCoordinate(node.X, node.Y, 0, startPoint.MapType));
            }

            if (coordStack.Count == 0)
            {
                return null;
            }
            else
            {
                coordStack.Pop(); //remove the start node
                return coordStack;
            }
        }

        public static int CeilToPower2(int number)
        {
            uint n = 1;

            while (number > n)
            {
                n <<= 1;
            }

            return (int)n;
        }
    }
}
