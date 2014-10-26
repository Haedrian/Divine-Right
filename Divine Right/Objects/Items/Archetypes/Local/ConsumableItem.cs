using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using Microsoft.Xna.Framework;

namespace DRObjects.Items.Archetypes.Local
{
    [Serializable]
    /// <summary>
    /// An item that can be consumed.
    /// A consumed item is a special kind of inventory item which may be consumed to provide some benefit
    /// </summary>
    public class ConsumableItem:
        InventoryItem
    {
        /// <summary>
        /// The effects which occur when this is consumed
        /// </summary>
        public ConsumableEffect Effects { get; set; }
        /// <summary>
        /// The total amount of effect this consumed item has
        /// </summary>
        public int EffectPower { get; set; }

        public override ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actions = base.GetPossibleActions(actor).ToList();

            if (base.InInventory)
            {
                actions.Add(ActionType.CONSUME);
            }

            return actions.ToArray();
        }

        public override ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType == ActionType.CONSUME)
            {
                if (Effects.HasFlag(ConsumableEffect.FEED))
                {
                    //Eat
                    
                    //TODO - LATER
                }
                //How many have we got?
                if (this.TotalAmount > 1)
                {
                    this.TotalAmount--; //reduce by one
                }
                else
                {
                    //remove from inventory
                    actor.Inventory.Inventory.Remove(this.Category, this);
                }

                return new ActionFeedback[] { new CurrentLogFeedback(null, Color.Blue, "You consume the " + this.Name) };
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }
        }
    }
}
