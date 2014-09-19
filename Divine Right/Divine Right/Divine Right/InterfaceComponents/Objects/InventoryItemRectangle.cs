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
        /// <summary>
        /// Used for trading. Whether it is selected or not
        /// </summary>
        public bool Selected { get; set; }

        public InventoryItemRectangle()
        {
            Selected = false;
        }
    }
}
