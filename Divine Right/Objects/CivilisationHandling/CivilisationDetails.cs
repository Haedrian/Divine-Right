using DRObjects.Enums;
using DRObjects.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.CivilisationHandling
{
    [Serializable]
    /// <summary>
    /// A particular civilisation
    /// </summary>
    public class Civilisation
    {
        /// <summary>
        /// The name of the civilisation
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The ID of the civilisation
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The faction this civilisation falls under
        /// </summary>
        public OwningFactions Faction { get; set; }

        /// <summary>
        /// The flag to display
        /// </summary>
        public GlobalSpriteName Flag
        {
            get
            {
                switch (ID)
                {
                    case 0: return GlobalSpriteName.FLAG_BROWN;
                    case 1: return GlobalSpriteName.FLAG_GREEN;
                    case 2: return GlobalSpriteName.FLAG_ORANGE;
                    case 3: return GlobalSpriteName.FLAG_PINK;
                    case 4: return GlobalSpriteName.FLAG_PURPLE;
                    case 5: return GlobalSpriteName.FLAG_RED;
                    case 6: return GlobalSpriteName.FLAG_YELLOW;
                    case 50: return GlobalSpriteName.FLAG_BLACK;
                    case 100: return GlobalSpriteName.FLAG_ORC;
                }

                return GlobalSpriteName.HAMLET;
            }
        }
    }
}
