using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.Settlements.Districts;

namespace DRObjects.Items.Archetypes.Global
{
    /// <summary>
    /// This item repesents a settlement of some sort, be it a human settlement, a dungeon or whatever
    /// </summary>
    public class Settlement:
        MapItem
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
                    return SettlementType.TOWN;
                }
            }
        }
        public int SettlementSize { get; set; }
        public List<SettlementBuilding> Districts { get; set; }
        public int RichPercentage { get; set; }
        public int MiddlePercentage { get; set; }
        public int PoorPercentage { get; set; }

        #endregion

        public override List<SpriteData> Graphics
        {
            get
            {
                return new List<SpriteData>(){Graphic};
            }
            set
            {
                base.Graphics = value;
            }
        }

        public override SpriteData Graphic
        {
            get
            {
                switch (SettlementType)
                {
                    case SettlementType.HAMLET:
                        return SpriteManager.GetSprite(GlobalSpriteName.HAMLET);
                    case SettlementType.TOWN:
                        return SpriteManager.GetSprite(GlobalSpriteName.CAPITAL);
                    case SettlementType.VILLAGE:
                        return SpriteManager.GetSprite(GlobalSpriteName.VILLAGE);
                }

                return null;

            }
            set
            {
                return; //dummy
            }
        }
    }
}
