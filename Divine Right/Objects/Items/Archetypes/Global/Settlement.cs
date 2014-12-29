using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.Settlements.Districts;
using System.IO;
using DRObjects.CivilisationHandling;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    /// <summary>
    /// This item repesents a human settlement 
    /// </summary>
    public class Settlement:
        Location
    {
    
        #region Members

        public SettlementType SettlementType
        {
            get
            {
                if (this.SettlementSize < 3)
                {
                    return SettlementType.HAMLET;
                }
                else if (this.SettlementSize < 5)
                {
                    return SettlementType.VILLAGE;
                }
                else
                {
                    return SettlementType.CAPITAL;
                }
            }
        }
        public string Name { get; set; }
        public int SettlementSize { get; set; }
        public List<SettlementBuilding> Districts { get; set; }
        public int RichPercentage { get; set; }
        public int MiddlePercentage { get; set; }
        public int PoorPercentage { get; set; }
        public bool IsCapital { get; set; }
        public Civilisation Civilisation { get; set; }
        #endregion

        public Settlement()
        {
            IsCapital = false;
        }

    }
}
