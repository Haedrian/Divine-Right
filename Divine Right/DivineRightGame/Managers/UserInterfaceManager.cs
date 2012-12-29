using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects;
using DRObjects;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;

namespace DivineRightGame.Managers
{
    /// <summary>
    /// Used to communicate between the User Interface and the Game State
    /// </summary>
    public static class UserInterfaceManager
    {
        /// <summary>
        /// Performs an action on a particular block
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="actionType"></param>
        /// <param name="args"></param>
        public static PlayerFeedback[] PerformAction(MapCoordinate coordinate, ActionTypeEnum actionType, object[] args)
        {
            switch (coordinate.MapType)
            {
                case MapTypeEnum.LOCAL:
                    return GameState.LocalMap.GetBlockAtCoordinate(coordinate).PerformAction(actionType, GameState.PlayerCharacter, args);
                case MapTypeEnum.GLOBAL:
                    return GameState.GlobalMap.GetBlockAtCoordinate(coordinate).PerformAction(actionType, GameState.PlayerCharacter, args);
                default:
                    throw new NotImplementedException("There is no support for that particular maptype");
            }
        }

        /// <summary>
        /// Performs a particular internal action such as saving, loading or quitting
        /// </summary>
        /// <param name="actionType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static PlayerFeedback[] PerformInternalAction(InternalActionEnum actionType, object[] args)
        {
            return GameState.PerformInternalAction(actionType, args);
        }

        /// <summary>
        /// Gets possible actions for a particular block
        /// </summary>
        /// <param name="coordinate"></param>
        /// <param name="actionType"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static ActionTypeEnum[] GetPossibleActions(MapCoordinate coordinate, ActionTypeEnum actionType, object[] args)
        {
            switch (coordinate.MapType)
            {
                case MapTypeEnum.LOCAL:
                    return GameState.LocalMap.GetBlockAtCoordinate(coordinate).GetActions(GameState.PlayerCharacter);
                case MapTypeEnum.GLOBAL:
                    return GameState.GlobalMap.GetBlockAtCoordinate(coordinate).GetActions(GameState.PlayerCharacter);
                default:
                    throw new NotImplementedException("There is no support for that particular maptype");
            }
        }

        /// <summary>
        /// Gets a number of graphical blocks around a particular point.
        /// The order will be as follows:
        /// first max z, then max x, then max y.
        /// </summary>
        /// <param name="centrePoint">The center point around which the tiles will be calculated</param>
        /// <param name="xRange">How many tiles away from the centre point on the x axis will be obtained</param>
        /// <param name="yCount">How many tiles away from the centre point on the y axis will be obtained</param>
        /// <param name="zCount">How many tiles away from the centre point on the y axis will be obtained</param>
        /// <returns></returns>
        public static GraphicalBlock[] GetBlocksAroundPoint(MapCoordinate centrePoint, int xRange, int yRange, int zRange)
        {
            int minZ = centrePoint.Z - Math.Abs(zRange);
            int maxZ = centrePoint.Z + Math.Abs(zRange);

            int minY = centrePoint.Y - Math.Abs(yRange);
            int maxY = centrePoint.Y + Math.Abs(yRange);

            int minX = centrePoint.X - Math.Abs(xRange);
            int maxX = centrePoint.X + Math.Abs(xRange);

            List<GraphicalBlock> returnList = new List<GraphicalBlock>();

            //go through all of them


            for (int zLoop = maxZ; zLoop >= minZ; zLoop--)
            {
                for (int yLoop = maxY; yLoop >= minY; yLoop--)
                {
                    for (int xLoop = minX; xLoop <= maxX; xLoop++)
                    {
                        MapCoordinate coord = new MapCoordinate(xLoop, yLoop, zLoop, centrePoint.MapType);
                        returnList.Add(GetBlockAtPoint(coord));
                    }
                }

            }

            return returnList.ToArray();
        }

        /// <summary>
        /// Gets a number of graphical blocks around the player.
        /// The order will be as follows:
        /// first max z, then max x, then max y.
        /// </summary>
        /// <param name="xRange">How many tiles away from the centre point on the x axis will be obtained</param>
        /// <param name="yCount">How many tiles away from the centre point on the y axis will be obtained</param>
        /// <param name="zCount">How many tiles away from the centre point on the y axis will be obtained</param>
        /// <returns></returns>
        public static GraphicalBlock[] GetBlocksAroundPlayer(int xRange, int yRange, int zRange)
        {
                return GetBlocksAroundPoint(GameState.PlayerCharacter.MapCharacter.Coordinate, xRange, yRange, zRange);   
        }

        /// <summary>
        /// Gets a graphical block whcih exists on a particular point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static GraphicalBlock GetBlockAtPoint(MapCoordinate point)
        {
            switch (point.MapType)
            {
                case (DRObjects.Enums.MapTypeEnum.GLOBAL)
                :
                        return GameState.GlobalMap.GetBlockAtCoordinate(point).ConvertToGraphicalBlock();   
                case (DRObjects.Enums.MapTypeEnum.LOCAL)
                :
                        return GameState.LocalMap.GetBlockAtCoordinate(point).ConvertToGraphicalBlock();
                default:
                        throw new NotImplementedException("There is no map manager for that type");
            }
        }

    }
}
