using DRObjects.GraphicsEngineObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.GraphicsEngineObjects
{
    [Serializable]
    public class VisitedBlockFeedback:
        ActionFeedback
    {
        /// <summary>
        /// The coordinate we visited
        /// </summary>
        public MapCoordinate Coordinate { get; set; }
    }
}
