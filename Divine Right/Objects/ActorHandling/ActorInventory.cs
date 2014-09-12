using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.DataStructures;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Enums;

namespace DRObjects.ActorHandling
{
    /// <summary>
    /// Holds data on the actor's entire inventory, including money and items which are equipped.
    /// </summary>
    [Serializable]
    public class ActorInventory
    {
        /// <summary>
        /// The List of items that this actor is holding
        /// </summary>
        public GroupedList<InventoryItem> Inventory { get; set; }

        /// <summary>
        /// How much money this actor has
        /// </summary>
        public int TotalMoney { get; set; }

        public Dictionary<EquipmentLocation,InventoryItem> EquippedItems { get; set; }

        public ActorInventory()
        {
            Inventory = new GroupedList<InventoryItem>();
            TotalMoney = 0;
            EquippedItems = new Dictionary<EquipmentLocation, InventoryItem>();
        }

    }
}
