﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Enums;

namespace DRObjects
{
    /// <summary>
    /// Represents a block on the map. A tile and a number of Map items upon it.
    /// </summary>
    public class MapBlock
    {
        #region Members

        /// <summary>
        /// A 'stack' of the map items. The top is the largest value
        /// </summary>
        private List<MapItem> mapItems;

        #endregion

        #region Properties

        /// <summary>
        /// The tile at the bottom of the local block.
        /// </summary>
        public MapItem Tile { get; set; }

        public bool MayContainItems
        {
            get
            {
                return GetTopItem().MayContainItems;
            }

        }

        #endregion

        #region Constructors

        public MapBlock()
        {
            this.mapItems = new List<MapItem>();
        }

        #endregion

        #region Functions

        /// <summary>
        /// Adds a local map item in the current block.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public PlayerFeedback[] PutItemOnBlock(MapItem item)
        {
            //First check whether the top item in question allows items to be placed upon it

            MapItem top = this.GetTopItem();

            if (!top.MayContainItems)
            {
                //can't do it
                return new PlayerFeedback[] { new TextFeedback("Can't do that") };
            }
            else
            {
                //move the item - we'll remove it from the list lazily later
                item.Coordinate = Tile.Coordinate;
                this.mapItems.Add(item);

                return new PlayerFeedback[0];
            }

        }

        /// <summary>
        /// Gets the top mapitem in the stack, or null if there is no item
        /// </summary>
        /// <returns></returns>
        public MapItem GetTopMapItem()
        {
            MapItem item = null;

            while (item == null)
            {
                if (this.mapItems.Count == 0)
                {
                    return null;
                }
                else
                {
                    //This is to do moving items lazily
                    item = this.mapItems[mapItems.Count];
                    
                    //Do the coordinates match?
                    if (!item.Coordinate.Equals(Tile.Coordinate))
                    {
                        //remove it from the list
                        mapItems.Remove(item);
                        item = null; 
                    }
                }
            }

            return item;
        }

        /// <summary>
        /// Gets the top Item in the block. Could either be a tile, or it could be the top item
        /// </summary>
        /// <returns></returns>
        public MapItem GetTopItem()
        {
            if (this.mapItems.Count == 0)
            {
                return Tile;
            }
            else
            {
                return GetTopMapItem();
            }
        }

 
        /// <summary>
        /// Gets the actions which can be performed on this Block
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public ActionTypeEnum[] GetActions(Actor actor)
        {
            MapItem item = GetTopItem();
            ActionTypeEnum[] actions = item.GetPossibleActions(actor);

            if (this.MayContainItems)
            {
                List<ActionTypeEnum> actionList = actions.ToList();
                actionList.Add(ActionTypeEnum.MOVE);

                return actionList.ToArray();
            }
            else
            {
                return actions;
            }
        }

        public PlayerFeedback[] PerformAction(ActionTypeEnum actionType, Actor actor, object[] args)
        {
            if (actionType == ActionTypeEnum.MOVE)
            {
                //the block must handle this
                //TODO: Future - the block can transmit 'moved on' action on the top item or the tile for things like traps

                if (this.MayContainItems)
                {
                    //it is possible to move there

                    //you can only move to a particular block if its only 1 square away

                    int distance = this.Tile.Coordinate - actor.MapCharacter.Coordinate;

                    if (distance > 1)
                    {
                        return new PlayerFeedback[] { new TextFeedback("Can't move there") };
                    }

                    actor.MapCharacter.Coordinate = this.Tile.Coordinate;
                    this.mapItems.Add(actor.MapCharacter);
                    return new PlayerFeedback[0];
                }
                else
                {
                    //not possible
                    return new PlayerFeedback[] { new TextFeedback("Not possible to move there") };
                }
            }

            MapItem item = GetTopItem();
            return item.PerformAction(actionType, actor, args);
        }

        /// <summary>
        /// Converts a particular block into a graphical block
        /// </summary>
        /// <returns></returns>
        public GraphicalBlock ConvertToGraphicalBlock()
        {
            GraphicalBlock block = new GraphicalBlock();
            block.TileGraphic = this.Tile.Graphic;
            block.ItemGraphic = string.Empty;

            if (this.GetTopMapItem() != null)
            {
                block.ItemGraphic = this.GetTopMapItem().Graphic;
            }

            block.MapCoordinate = this.Tile.Coordinate;

            return block;
        }

        #endregion
    }
}
