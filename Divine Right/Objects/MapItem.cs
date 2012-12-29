using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.GraphicsEngineObjects;

namespace DRObjects
{
    /// <summary>
    /// Represents a tile, or an item on a map
    /// </summary>
    public class MapItem
    {
        #region Properties

        /// <summary>
        /// Defines whether the tile or item may have items placed on it.
        /// A value of false means that this tile can't have MapItems placed on it - this includes Actors walking on it
        /// </summary>
        public bool MayContainItems { get; set; }

        /// <summary>
        /// Represents the name of the tile or item
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the description to be shown when the user examines the tile or item
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Represents the graphic that this tile or item will be represented as
        /// </summary>
        public string Graphic { get; set; }

        /// <summary>
        /// Represents the current coordinate
        /// </summary>
        public MapCoordinate Coordinate { get; set; }

        /// <summary>
        /// Represents the internal name of the item - used for debugging purposes
        /// </summary>
        public string InternalName { get; set; }

        #endregion Properties

        #region Actions Possible

        /// <summary>
        /// Defines what happens when an action is performed upon the tile or item. These are the default values. 
        /// </summary>
        public virtual PlayerFeedback[] PerformAction(ActionTypeEnum actionType, Actor actor, object[] args)
        {

            if (GetPossibleActions(actor).Contains(actionType))
            {
                switch (actionType)
                {
                    case ActionTypeEnum.EXAMINE :
                        return new PlayerFeedback[] { new CurrentLogFeedback(this.Description) };
                    case ActionTypeEnum.LOOK:
                        return new PlayerFeedback[] { new TextFeedback(this.Name) };
                    default:
                        //TODO: ERROR HANDLING
                        throw new NotImplementedException("The following action : " + actionType + " has no code");
                }
            }
            else
            {
                //TODO: ERROR HANDLING
                return new PlayerFeedback[0];
            }

        }

        /// <summary>
        /// Returns the list of actions which are possible upon this tile or item
        /// For mundane tiles  and items, the following are default for the player character
        /// Other details to be added later.
        /// </summary>
        /// <returns></returns>
        public virtual ActionTypeEnum[] GetPossibleActions(Actor actor)
        {
            if (actor.IsPlayerCharacter)
            {
                return new ActionTypeEnum[] { ActionTypeEnum.EXAMINE, ActionTypeEnum.LOOK };
            }
            else
            {
                //todo
                return new ActionTypeEnum[0] { };
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Creates a default map item
        /// </summary>
        public MapItem()
        {

        }

        /// <summary>
        /// Creates a default map item at a current location
        /// </summary>
        /// <param name="coordinate"></param>
        public MapItem(MapCoordinate coordinate)
        {
            this.Coordinate = coordinate;
        }

        #endregion

        #region Overridden Methods

        public override string ToString()
        {
            return InternalName + " at " + Coordinate.ToString();
        }

        #endregion
    }
}
