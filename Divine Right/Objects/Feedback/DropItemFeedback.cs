using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.Items.Archetypes.Local;

namespace DRObjects.GraphicsEngineObjects
{
    public class DropItemFeedback:
        ActionFeedback
    {
        public InventoryItem ItemToDrop { get; set; }
    }
}
