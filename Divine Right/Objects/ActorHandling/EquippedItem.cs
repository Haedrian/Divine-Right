using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Enums;

namespace DRObjects.ActorHandling
{
    [Serializable]
    /// <summary>
    /// An item which is equipped at a particular location
    /// </summary>
    public class EquippedItem
    {
        public InventoryItem Item { get; set; }
        public EquipmentLocation Location { get; set; }
    }
}
