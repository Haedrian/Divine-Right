using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;

namespace DRObjects.Items.Archetypes.Global
{
    /// <summary>
    /// This item repesents a settlement of some sort, be it a human settlement, a dungeon or whatever
    /// </summary>
    public class Settlement:
        MapItem
    {
        #region Members

        public SettlementType SettlementType { get; set; }

        #endregion

        public override string Graphic
        {
            get
            {
                const string HAMLET = @"Graphics/World/Settlements/SmallSettlement";
                const string VILLAGE = @"Graphics/World/Settlements/MediumSettlement" ;
                const string TOWN = @"Graphics/World/Settlements/LargeSettlement";

                switch (SettlementType)
                {
                    case SettlementType.HAMLET:
                        return HAMLET;
                    case SettlementType.TOWN:
                        return TOWN;
                    case SettlementType.VILLAGE:
                        return VILLAGE;
                }

                return "";

            }
            set
            {
                return; //dummy
            }
        }
    }
}
