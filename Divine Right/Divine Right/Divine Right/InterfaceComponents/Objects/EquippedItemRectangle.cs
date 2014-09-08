using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects.Items.Archetypes.Local;
using DRObjects.Enums;

namespace Divine_Right.InterfaceComponents.Objects
{
    /// <summary>
    /// Represents a rectangle holding information on an item which has been equipped
    /// </summary>
    public class EquippedItemRectangle
    {
        public Rectangle Rect { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public EquipmentLocation Location { get; set; }
    }
}
