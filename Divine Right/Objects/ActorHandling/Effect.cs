using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.Enums;
using DRObjects.GraphicsEngineObjects;

namespace DRObjects.ActorHandling
{
    /// <summary>
    /// Represents a temporary effect on a character.
    /// Will run out when "Minutes Left" runs out
    /// </summary>
    public class Effect
    {
        public EffectName Name { get; set; }
        public int EffectAmount { get; set; }
        public int MinutesLeft { get; set; }

        /// <summary>
        /// The Log Feedback to display when the effect disappears
        /// </summary>
        public LogFeedback EffectDisappeared { get; set; }
    }
}
