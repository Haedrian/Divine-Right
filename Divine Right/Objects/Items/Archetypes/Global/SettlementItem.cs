using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;

namespace DRObjects.Items.Archetypes.Global
{
    /// <summary>
    /// This represents an item which is part of a settlement
    /// </summary>
    public class SettlementItem:
        MapItem
    {
        #region Members
        public int SettlementSize { get; set; }
        public bool IsCapital { get; set; }
        /// <summary>
        /// The Corner of the settlement - determines which graphic to draw
        /// </summary>
        public int SettlementCorner { get; set; }
        public SettlementType SettlementType
        {
            get
            {
                if (IsCapital)
                {
                    return SettlementType.CAPITAL;
                }

                if (this.SettlementSize < 3)
                {
                    return SettlementType.HAMLET;
                }
                else
                {
                    return SettlementType.VILLAGE;
                }
            }
        }
        #endregion

        #region Private Members
        private List<SpriteData> sprites;
        #endregion

        #region Overridden functions
        public override List<Graphics.SpriteData> Graphics
        {
            get
            {
                if (sprites == null)
                {
                    //Create it
                    sprites = new List<SpriteData>();
                    sprites.Add(SpriteManager.GetSprite((GlobalSpriteName)Enum.Parse(typeof(GlobalSpriteName),this.SettlementType.ToString() + "_" + SettlementCorner)));
                }
                
                return sprites;
            }
            set
            {
                base.Graphics = value;
            }
        }
        #endregion
    }
}
