using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.GraphicsEngineObjects;
using DRObjects.Graphics;
using Microsoft.Xna.Framework;

namespace DRObjects.Items.Archetypes.Local
{
    [Serializable]
    /// <summary>
    /// An item which can be picked up, or can be carried in the inventory
    /// </summary>
    public class InventoryItem:
        MapItem
    {
        /// <summary>
        /// Whether the item is held in the inventory
        /// </summary>
        public bool InInventory { get; set; }

        /// <summary>
        /// Whether the item is equipped or not
        /// </summary>
        public bool IsEquipped { get; set; }

        /// <summary>
        /// Whether the item can be equipped or not
        /// </summary>
        public bool IsEquippable { get; set; }

        /// <summary>
        /// The base value of this item if it is sold
        /// </summary>
        public int BaseValue { get; set; }
        
        /// <summary>
        /// How useful the device is as armour
        /// </summary>
        public int ArmourRating { get; set; }
        /// <summary>
        /// How much damage this item does
        /// </summary>
        public int DamageRating { get; set; }

        public InventoryCategory Category { get; set; }

        public override string Description
        {
            get
            {
                if (InInventory)
                {
                    string desc = base.Description;

                    //Add the value and armour/damage rating
                    desc += "\nBaseVal " + BaseValue;

                    if (ArmourRating > 0)
                    {
                        desc += " Armr " + ArmourRating;
                    }
                    if (DamageRating > 0)
                    {
                        desc += " Dmg " + DamageRating;
                    }

                    return desc;
                }
                else
                {
                    return base.Description;
                }
            }
            set
            {
                base.Description = value;
            }
        }


        public override Enums.ActionTypeEnum[] GetPossibleActions(Actor actor)
        {
            List<ActionTypeEnum> actions = base.GetPossibleActions(actor).ToList() ;

            if (InInventory)
            {
                actions = new List<ActionTypeEnum>(); //Clear them
                if (IsEquippable)
                {
                    actions.Add(ActionTypeEnum.EQUIP);
                }

                actions.Add(ActionTypeEnum.DROP);
            }
            else 
            {
                if (Math.Abs(actor.MapCharacter.Coordinate - this.Coordinate) < 2)
                {
                    actions.Add(ActionTypeEnum.TAKE);
                }
            }

            return actions.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.ActionFeedback[] PerformAction(ActionTypeEnum actionType, Actor actor, object[] args)
        {
            if (actionType == ActionTypeEnum.EQUIP || actionType == ActionTypeEnum.TAKE)
            {
                if (Math.Abs(actor.MapCharacter.Coordinate - this.Coordinate) < 2)
                {
                    if (actionType == ActionTypeEnum.TAKE)
                    {
                        //take it
                        this.Coordinate = new MapCoordinate(999, 999, 0, MapTypeEnum.CONTAINER); //Dummy - this will cause the block to reject and delete it
                        actor.Inventory.Add(this.Category,this);
                        this.InInventory = true;

                        return new ActionFeedback[1] { new CurrentLogFeedback(InterfaceSpriteName.MAN,Color.Black,"You pick up the " + this.Name) };
                        
                    }
                    //TODO: EQUIP 
                }
            }
            else if (actionType == ActionTypeEnum.DROP)
            {
                //Drop it.
                //Since we can't access GameState from out here, we'll need to use an ActionFeedback
                this.Coordinate = actor.MapCharacter.Coordinate;
                this.InInventory = false;
                return new ActionFeedback[1] { new DropItemFeedback() { ItemToDrop = this } };
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }

            return new ActionFeedback[0] { };
        }
    }
}
