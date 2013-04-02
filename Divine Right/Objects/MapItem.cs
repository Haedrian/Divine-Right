using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Graphics;

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
        /// Represents the graphics that this tile will use to represent itself. 
        /// </summary>
        public virtual List<SpriteData> Graphics { get; set; }

        /// <summary>
        /// Represents the top graphic that this tile uses to represent itself. Assigning this WILL OVERWRITE the top graphic
        /// </summary>
        public virtual SpriteData Graphic
        {
            get
            {
                if (Graphics.Count == 0)
                {
                    return null;
                }
                else
                {
                    return Graphics[0];
                }

            }

            set
            {
                if (Graphics.Count == 0)
                {
                    Graphics.Add(value);
                }
                else
                {
                    Graphics[0] = value;
                }

            }
        }

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
                        return new PlayerFeedback[] { new TextFeedback("You see " + this.Description) };
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
            this.Graphics = new List<SpriteData>();
        }

        /// <summary>
        /// Creates a default map item at a current location
        /// </summary>
        /// <param name="coordinate"></param>
        public MapItem(MapCoordinate coordinate)
        {
            this.Coordinate = coordinate;
            this.Graphics = new List<SpriteData>();
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
