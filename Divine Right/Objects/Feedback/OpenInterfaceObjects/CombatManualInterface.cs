using DRObjects.Items.Archetypes.Local;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Feedback.OpenInterfaceObjects
{
    /// <summary>
    /// Information necessary to open a Combat Manual Interface
    /// </summary>
    public class CombatManualInterface
        : OpenInterfaceObject
    {

        public CombatManualInterface(CombatManual manual):
            base ()
        {
            Manual = manual;
        }

        /// <summary>
        /// The manual this is referring to
        /// </summary>
        public CombatManual Manual { get; set; }
    }
}
