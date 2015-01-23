using DRObjects.GraphicsEngineObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Feedback
{
    /// <summary>
    /// For attacking something
    /// </summary>
    public class AttackFeedback
        : ActionFeedback
    {
        public Actor Attacker { get; set; }
        public Actor Defender { get; set; }
    }
}
