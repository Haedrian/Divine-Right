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
    public class InventoryItem :
        MapItem
    {
        /// <summary>
        /// The total amount of Inventory Item in this 'stack'
        /// </summary>
        public int TotalAmount { get; set; }

        /// <summary>
        /// Whether this Item may represent a stack of items. Items which aren't stackable may only contain a single amount
        /// </summary>
        public bool Stackable { get; set; }

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

        private int _baseSingleValue;

        public int BaseValue
        {
            get
            {
                //Multiply the base single value by how many we have of the item
                return _baseSingleValue * TotalAmount;
            }
            set
            {
                _baseSingleValue = value;
            }
        }

        /// <summary>
        /// How useful the device is as armour
        /// </summary>
        public int ArmourRating { get; set; }
        /// <summary>
        /// How much damage this item does
        /// </summary>
        public int DamageDice { get; set; }

        /// <summary>
        /// The chance of stunning your opponent
        /// </summary>
        public int StunAmount { get; set; }

        /// <summary>
        /// The chance of wounding your opponent
        /// </summary>
        public int WoundPotential { get; set; }

        public string WeaponType { get; set; }

        /// <summary>
        /// Defines where exactly this item may be equipped. If any
        /// </summary>
        public EquipmentLocation? EquippableLocation { get; set; }

        public InventoryCategory Category { get; set; }

        public override string Description
        {
            get
            {
                if (InInventory)
                {
                    string desc = base.Description;

                    //Add the value and armour/damage rating
                    desc += "\nValue " + BaseValue;

                    if (ArmourRating > 0)
                    {
                        desc += " Armr " + ArmourRating;
                    }
                    if (DamageDice > 0)
                    {
                        desc += " Dmg: 3d" + DamageDice + " WP :" + WoundPotential + " Stun: " + StunAmount;
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

        public override List<SpriteData> Graphics
        {
            get
            {
                var graphics = base.Graphics;

                if (Stackable && TotalAmount > 1)
                {
                    //Remove any old text sprite data - not sure of a better way to do this without refactoring the code :(
                    graphics.Remove(graphics.FirstOrDefault(f => f.GetType().Equals(typeof(TextSpriteData))));

                    graphics.Add(new TextSpriteData() { Text = TotalAmount.ToString(), Colour = Color.Black });
                }

                return graphics;
            }
            set
            {
                base.Graphics = value;
            }
        }

        public override Enums.ActionTypeEnum[] GetPossibleActions(Actor actor)
        {
            List<ActionTypeEnum> actions = base.GetPossibleActions(actor).ToList();

            if (InInventory)
            {
                actions = new List<ActionTypeEnum>(); //Clear them
                if (IsEquipped)
                {
                    actions.Add(ActionTypeEnum.UNEQUIP);
                }
                else
                    if (IsEquippable)
                    {
                        actions.Add(ActionTypeEnum.EQUIP);
                        actions.Add(ActionTypeEnum.DROP);
                    }
                    else
                    {
                        actions.Add(ActionTypeEnum.DROP);
                    }

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
            if (actionType == ActionTypeEnum.TAKE)
            {
                if (Math.Abs(actor.MapCharacter.Coordinate - this.Coordinate) < 2)
                {
                    if (actionType == ActionTypeEnum.TAKE)
                    {
                        //take it
                        this.Coordinate = new MapCoordinate(999, 999, 0, MapType.CONTAINER); //Dummy - this will cause the block to reject and delete it

                        //Is the item stackable ?
                        if (this.Stackable)
                        {
                            //Do we have an item with the same name in the inventory?
                            var item = actor.Inventory.Inventory.GetObjectsByGroup(this.Category).Where(g => g.Name.Equals(this.Name)).FirstOrDefault();

                            if (item != null)
                            {
                                //Instead we increment the total in that item in the inventory
                                item.TotalAmount++;
                            }
                            else
                            {
                                actor.Inventory.Inventory.Add(this.Category, this);
                                this.InInventory = true;
                            }
                        }
                        else
                        {
                            actor.Inventory.Inventory.Add(this.Category, this);
                            this.InInventory = true;
                        }

                        return new ActionFeedback[1] { new CurrentLogFeedback(InterfaceSpriteName.MAN, Color.Black, "You pick up the " + this.Name) };

                    }
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
            else if (actionType == ActionTypeEnum.EQUIP && this.IsEquippable && this.EquippableLocation.HasValue)
            {
                //Equip it
                this.IsEquipped = true;

                //Equip the item
                List<EquipmentLocation> possibleLocation = new List<EquipmentLocation>();

                //Is this a ring ? - they have two places where they can fit
                if (this.EquippableLocation == EquipmentLocation.RING)
                {
                    possibleLocation.Add(EquipmentLocation.RING1);
                    possibleLocation.Add(EquipmentLocation.RING2);
                }
                else
                {
                    possibleLocation.Add(this.EquippableLocation.Value);
                }

                EquipmentLocation? clearLocation = null;

                //So, do we have a clear area?
                foreach (var possible in possibleLocation)
                {
                    if (!actor.Inventory.EquippedItems.ContainsKey(possible))
                    {
                        clearLocation = possible;
                        break;
                    }
                }

                if (!clearLocation.HasValue)
                {
                    //We need to move the existing item
                    actor.Inventory.EquippedItems[possibleLocation[0]].IsEquipped = false;
                    actor.Inventory.EquippedItems.Remove(possibleLocation[0]);

                    clearLocation = possibleLocation[0];
                }

                //And put the item there
                this.IsEquipped = true;

                actor.Inventory.EquippedItems.Add(clearLocation.Value, this);

            }
            else if (actionType == ActionTypeEnum.UNEQUIP && this.IsEquipped)
            {
                this.IsEquipped = false;

                //Find the item and remove it from the inventory
                var item = actor.Inventory.EquippedItems.First(kvp => kvp.Value == this);

                actor.Inventory.EquippedItems.Remove(item.Key);
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }

            return new ActionFeedback[0] { };
        }
    }
}
