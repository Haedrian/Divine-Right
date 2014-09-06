using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;

namespace DRObjects.Items.Archetypes.Local
{
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
                if (IsEquippable)
                {
                    actions.Add(ActionTypeEnum.EQUIP);
                }
            }
            else 
            {
                if (Math.Abs(actor.GlobalCoordinates - this.Coordinate) < 2)
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
                if (Math.Abs(actor.GlobalCoordinates - this.Coordinate) < 2)
                {
                    if (actionType == ActionTypeEnum.TAKE)
                    {
                        //take it
                        this.Coordinate = new MapCoordinate(999, 999, 0, MapTypeEnum.CONTAINER); //Dummy - this will cause the block to reject and delete it
                        actor.Inventory.Add("",this); //TODO - CATEGORY!
                        this.InInventory = true;
                    }
                    //TODO: EQUIP 
                }
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }

            return new ActionFeedback[0] { };
        }
    }
}
