using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;

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

        public override SpriteData Graphic
        {
            get
            {
                switch (SettlementType)
                {
                    case SettlementType.HAMLET:
                        return SpriteManager.GetSprite(GlobalSpriteName.HAMLET);
                    case SettlementType.TOWN:
                        return SpriteManager.GetSprite(GlobalSpriteName.TOWN);
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
