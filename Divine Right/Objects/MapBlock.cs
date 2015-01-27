using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Enums;
using DRObjects.Items.Tiles.Global;
using DRObjects.Graphics;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Feedback;

namespace DRObjects
{
    [Serializable]
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

        /// <summary>
        /// Whether the player has visited this tile. This is used for underground areas
        /// </summary>
        public bool WasVisited { get; set; }

        public bool MayContainItems
        {
            get
            {

                return GetTopItem() == null ? false : GetTopItem().MayContainItems;
            }

        }

        /// <summary>
        /// Whether this block can be seen through
        /// It can be seen through if it has no items or the item on top can be walked upon
        /// </summary>
        public bool IsSeeThrough
        {
            get
            {
                if (GetTopMapItem() == null)
                {
                    return true;
                }
                else
                {
                    return GetTopMapItem().MayContainItems;
                }
            }
        }

        #endregion

        #region Constructors

        public MapBlock()
        {
            this.mapItems = new List<MapItem>();
            this.WasVisited = false;
        }

        #endregion

        #region Functions

        /// <summary>
        /// Adds a local map item in the current block.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ActionFeedback[] PutItemOnBlock(MapItem item)
        {
            //First check whether the top item in question allows items to be placed upon it

            MapItem top = this.GetTopItem();

            if (top == null || !top.MayContainItems)
            {
                //can't do it
                return new ActionFeedback[] { new TextFeedback("Can't do that") };
            }
            else
            {
                //move the item - we'll remove it from the list lazily later
                item.Coordinate = Tile.Coordinate;
                this.mapItems.Add(item);

                return new ActionFeedback[0];
            }

        }

        /// <summary>
        /// Puts a local map item int he current block. Whether the tile lets it or not.
        /// </summary>
        /// <param name="item"></param>
        public void ForcePutItemOnBlock(MapItem item)
        {
            item.Coordinate = new MapCoordinate(Tile.Coordinate);
            this.mapItems.Add(item);
        }

        /// <summary>
        /// Puts a local map item in the current block. Whether the tile lets it or not.
        /// Will put it in second place (underneath the top item)
        /// </summary>
        /// <param name="item"></param>
        public void PutItemUnderneathOnBlock(MapItem item)
        {
            item.Coordinate = new MapCoordinate(Tile.Coordinate);
            if (this.mapItems.Count > 0)
            {
                this.mapItems.Insert(0, item);
            }
            else
            {
                this.mapItems.Add(item);
            }
        }

        /// <summary>
        /// Removes the Nth item in the block
        /// </summary>
        /// <param name="index"></param>
        public void RemoveItemAt(int index)
        {
            this.mapItems.RemoveAt(index);
        }

        /// <summary>
        /// Removes a particular item in the block
        /// </summary>
        /// <param name="item"></param>
        public void RemoveItem(MapItem item)
        {
            this.mapItems.Remove(item);
        }

        /// <summary>
        /// Removes the top item. If it is empty - do nothing
        /// </summary>
        public void RemoveTopItem()
        {
            if (this.mapItems.Count == 0)
            {
                return;
            }

            this.mapItems.RemoveAt(this.mapItems.Count - 1);
        }

        /// <summary>
        /// Removes all items from the MapBlock. THIS INCLUDES THE ONES WHICH ARE INACTIVE
        /// </summary>
        public void RemoveAllItems()
        {
            this.mapItems = new List<MapItem>();
        }

        /// <summary>
        /// Gets the top mapitem in the stack, or null if there is no item.
        /// It will check if the map item is active or not before showing it.
        /// </summary>
        /// <returns></returns>
        public MapItem GetTopMapItem()
        {
            MapItem item = null;

            //This is used due to the active thing. The 'top' item might be inactive
            int counter = 1;

            while (item == null)
            {
                if (this.mapItems.Count == 0 || mapItems.Count - counter < 0)
                {
                    return null;
                }
                else
                {
                    //This is to do moving items lazily
                    item = this.mapItems[mapItems.Count - counter];

                    //Do the coordinates match?
                    if (!item.Coordinate.Equals(Tile.Coordinate))
                    {
                        //remove it from the list
                        mapItems.Remove(item);
                        item = null;
                        continue;
                    }

                    //Is the item active?
                    if (!item.IsActive)
                    {
                        item = null;
                        counter++;
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
            if (this.mapItems.Where(mi => mi.IsActive).Count() == 0)
            {
                return Tile;
            }
            else
            {
                return GetTopMapItem();
            }
        }

        /// <summary>
        /// Gets all items. This includes inactive ones - you might want to check that
        /// </summary>
        /// <returns></returns>
        public MapItem[] GetItems()
        {
            return this.mapItems.ToArray();
        }

        /// <summary>
        /// Gets the actions which can be performed on this Block
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public ActionType[] GetActions(Actor actor)
        {
            MapItem item = GetTopItem();
            ActionType[] actions = item.GetPossibleActions(actor);

            if (this.MayContainItems)
            {
                List<ActionType> actionList = actions.ToList();
                actionList.Add(ActionType.MOVE);

                return actionList.ToArray();
            }
            else
            {
                return actions;
            }
        }

        public ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType == ActionType.MOVE)
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
                        return new ActionFeedback[] { new TextFeedback("Can't move there") };
                    }

                    actor.MapCharacter.Coordinate = this.Tile.Coordinate;
                    this.mapItems.Add(actor.MapCharacter);

                    //Are we moving the player character on the world map?
                    if (actor.IsPlayerCharacter && this.Tile.GetType() == typeof(GlobalTile))
                    {
                        //So, how about a random encounter?

                        Random random = new Random();

                        int randomValue = random.Next(100);

                        List<ActionFeedback> feedback = new List<ActionFeedback>();

                        //1%
                        if (randomValue > 99)
                        {
                            //Are we next to a bandit camp ?
                            feedback.Add(new LocationChangeFeedback() { RandomEncounter = (this.Tile as GlobalTile).Biome });
                        }

                        //Change the location of the character
                        actor.GlobalCoordinates = new MapCoordinate(this.Tile.Coordinate);
                        actor.GlobalCoordinates.MapType = MapType.GLOBAL;

                        //Make some time pass
                        feedback.Add(new TimePassFeedback() { TimePassInMinutes = (this.Tile as GlobalTile).TraverseTimeInMinutes(actor) });

                        return feedback.ToArray();
                    }

                    if (actor.IsPlayerCharacter)
                    {
                        //Mark all tiles around him as having been visited
                        return new ActionFeedback[1] { new VisitedBlockFeedback { Coordinate = this.Tile.Coordinate } };
                    }

                    return new ActionFeedback[0];
                }
                else
                {
                    //Check whether it's the player character trying to move on a tile upon which there is an enemy
                    if (actor.IsPlayerCharacter)
                    {
                        if (this.GetTopItem().GetType() == typeof(LocalCharacter))

                        {
                            //Attack him instead - randomly
                            LocalCharacter lc = (LocalCharacter)this.GetTopItem();

                            return new ActionFeedback[] { new AttackFeedback() { Attacker = actor, Defender = lc.Actor } };
                        }
                    }

                    //not possible
                    return new ActionFeedback[] { new TextFeedback("Not possible to move there") };
                }
            }

            MapItem item = GetTopItem();

            if (item == null)
            {
                return new ActionFeedback[0] { };
            }

            return item.PerformAction(actionType, actor, args);
        }

        /// <summary>
        /// Converts a particular block into a graphical block
        /// </summary>
        /// <returns></returns>
        public GraphicalBlock ConvertToGraphicalBlock()
        {
            GraphicalBlock block = new GraphicalBlock();
            block.TileGraphics = this.Tile.Graphics.ToArray();
            List<SpriteData> itemGraphics = new List<SpriteData>();
            List<SpriteData> actorGraphics = new List<SpriteData>();

            if (this.GetTopMapItem() != null)
            {
                //go through all the items and add them to the list in order
                foreach (MapItem item in this.mapItems.Where(mi => mi.IsActive))
                {
                    if (item.GetType() == typeof(LocalCharacter))
                    {
                        actorGraphics.AddRange(item.Graphics);
                    }
                    else
                    {
                        itemGraphics.AddRange(item.Graphics);
                    }
                }

            }

            block.ActorGraphics = actorGraphics.ToArray();
            block.ItemGraphics = itemGraphics.ToArray();
            block.MapCoordinate = this.Tile.Coordinate;
            block.WasVisited = this.WasVisited;
            block.IsSeeThrough = this.IsSeeThrough;

            return block;
        }

        public GraphicalBlock ConvertToGraphicalBlock(GlobalOverlay overlay)
        {
            GraphicalBlock block = new GraphicalBlock();
            block.TileGraphics = this.Tile.Graphics.ToArray();
            List<SpriteData> itemGraphics = new List<SpriteData>();

            if (this.GetTopItem() != null)
            {
                //go through all the items and add them to the list in order
                foreach (MapItem item in this.mapItems)
                {
                    itemGraphics.Add(item.Graphic);
                }

            }

            block.ItemGraphics = itemGraphics.ToArray();
            block.MapCoordinate = this.Tile.Coordinate;
            block.WasVisited = this.WasVisited;

            //get the overlay images if its a global tile

            if (this.Tile is GlobalTile)
            {
                block.OverlayGraphic = (this.Tile as GlobalTile).GetGraphicsByOverlay(overlay);
            }

            return block;

        }

        #endregion

        #region Overridden functions

        public override string ToString()
        {
            return "Block at " + this.Tile.Coordinate + " " + this.GetTopItem().InternalName;
        }

        #endregion
    }
}
