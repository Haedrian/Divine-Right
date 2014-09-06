using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using DRObjects.Items.Archetypes.Local;

namespace Divine_Right.InterfaceComponents.Objects
{
    /// <summary>
    /// Represents a rectangle and an assigned InventoryItem
    /// </summary>
    public class InventoryItemRectangle
    {
        public Rectangle Rect { get; set; }
        public InventoryItem Item { get; set; }
    }
}
