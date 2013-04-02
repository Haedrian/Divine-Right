using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;

namespace DRObjects.Items.Tiles
{
    /// <summary>
    /// Represents an empty tile. The default tile used when there is nothing at the location
    /// </summary>
    public class Air:
        MapItem
    {
        #region Constructor

        /// <summary>
        /// Constructor. Sets the default values for this tile at a particular coordinate
        /// </summary>
        public Air(MapCoordinate coord):base
            (coord)
        {
            this.Description = "";
            this.Graphic = null;
            this.MayContainItems = false;
            this.Name = "";
            this.InternalName = "Air";
        }

        /// <summary>
        /// Constructor, sets the default values for this tile
        /// </summary>
        public Air()
        {
            this.Description = "";
            this.Graphic = null;
            this.MayContainItems = false;
            this.Name = "";
            this.InternalName = "";
        }

        #endregion

        #region Overriden Functions

        /// <summary>
        /// Gets all possible actions for this air tile. That is, nothing
        /// </summary>
        /// <returns></returns>
        public override ActionTypeEnum[] GetPossibleActions(Actor actor)
        {
            return new ActionTypeEnum[] { };
        }

        public override PlayerFeedback[] PerformAction(ActionTypeEnum actionType, Actor actor, object[] args)
        {
            //none of the actions are valid for this tile
            return new PlayerFeedback[] { };
        }

        #endregion
    }
}
