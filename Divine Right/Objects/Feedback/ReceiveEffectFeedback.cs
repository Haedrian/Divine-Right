using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling;
using DRObjects.GraphicsEngineObjects.Abstract;

namespace DRObjects.Feedback
{
    [Serializable]
    /// <summary>
    /// For receiving the effect of an effect (heh)
    /// </summary>
    public class ReceiveEffectFeedback
        :ActionFeedback
    {
        public Effect Effect { get; set; }
    }
}
