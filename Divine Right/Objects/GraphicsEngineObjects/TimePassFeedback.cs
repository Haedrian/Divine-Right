using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects.Abstract;

namespace DRObjects.GraphicsEngineObjects
{
    /// <summary>
    /// Represents some time passing
    /// </summary>
    public class TimePassFeedback:
        ActionFeedback
    {
        public int TimePassInMinutes { get; set; }
    }
}
