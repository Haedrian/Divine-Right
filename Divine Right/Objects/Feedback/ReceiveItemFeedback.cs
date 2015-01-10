using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Feedback
{
    /// <summary>
    /// For receiving items
    /// </summary>
    public class ReceiveItemFeedback:
        ActionFeedback
    {
        public InventoryCategory Category { get; set; }
        public int MaxValue { get; set; }
    }
}
