using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;

namespace DRObjects.Items.Archetypes.Local
{
    /// <summary>
    /// This item will allow the user to leave town when used
    /// </summary>
    public class LeaveTownItem:
        MapItem
    {
        private List<SpriteData> sprites = new List<SpriteData>();

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

        public LeaveTownItem()
        {
            this.sprites = new List<SpriteData>();
            sprites.Add(SpriteManager.GetSprite(InterfaceSpriteName.GOTO_WORLD_MAP));
        }
    }
}
