using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;

namespace DRObjects.Items.Archetypes.Global
{
    /// <summary>
    /// A map item representing a dungeon on the map.
    /// </summary>
    public class DungeonItem
        :MapItem
    {
        /// <summary>
        /// The sprite to display
        /// </summary>
        private List<SpriteData> sprites = null;
        /// <summary>
        /// The owning race of the dungeon
        /// </summary>
        public int? OwnerID { get; set; }

        /// <summary>
        /// The Corner of the Dungeon - determines which graphic to draw
        /// </summary>
        public int DungeonCorner { get; set; }

        /// <summary>
        /// Creates a dungeon item. For now we only support mazes, not ruins
        /// </summary>
        public DungeonItem(int corner)
        {
            DungeonCorner = corner;
            this.MayContainItems = true;
            sprites = new List<SpriteData>();

            sprites.Add(SpriteManager.GetSprite((GlobalSpriteName)Enum.Parse(typeof(GlobalSpriteName), "DUNGEON" + "_" + DungeonCorner)));
        }

        public override List<SpriteData> Graphics
        {
            get
            {
                return sprites;
            }
            set
            {
                base.Graphics = value;
            }
        }
    }
}
