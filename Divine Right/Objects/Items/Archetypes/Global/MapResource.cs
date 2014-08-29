using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    /// <summary>
    /// A resource which is positioned on the map
    /// </summary>
    public class MapResource :
        MapItem
    {
        public GlobalResourceType ResourceType { get; set; }
        /// <summary>
        /// How much this resource is desirable
        /// </summary>
        public int Desirability { get; set; }
        private List<SpriteData> sprites;

        public MapResource(GlobalResourceType resourceType)
        {
            this.ResourceType = resourceType;

            //pick the right graphic to show
            sprites = new List<SpriteData>();

            switch(ResourceType)
            {
                case GlobalResourceType.FISH:
                    sprites.Add(SpriteManager.GetSprite(LocalSpriteName.FISH_4));
                    this.Name = "Fish";
                    this.Description = "A source of plentiful fish";
                    this.Desirability = 3;
                    break;
                case GlobalResourceType.GAME:
                    sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.GAME));
                    this.Name = "Game";
                    this.Description = "A source of Game and other huntable creatures";
                    this.Desirability = 3;
                    break;
                case GlobalResourceType.GOLD:
                    sprites.Add(SpriteManager.GetSprite(LocalSpriteName.GOLD_BARS_2));
                    this.Name = "Gold";
                    this.Description = "There's gold in them hills";
                    this.Desirability = 5;
                    break;
                case GlobalResourceType.HORSE:
                    sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.HORSES));
                    this.Name = "Horses";
                    this.Description = "Wild horses live here";
                    this.Desirability = 3;
                    break;
                case GlobalResourceType.IRON:
                    sprites.Add(SpriteManager.GetSprite(LocalSpriteName.IRON_BARS_2));
                    this.Name = "Iron";
                    this.Description = "Iron is found here";
                    this.Desirability = 4;
                    break;
                case GlobalResourceType.STONE:
                    sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.STONES));
                    this.Name = "Stone";
                    this.Description = "Good stone is found here";
                    this.Desirability = 3;
                    break;
                case GlobalResourceType.FARMLAND:
                    sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FARMLAND));
                    this.Name = "Farmland";
                    this.Description = "This land is very fertile";
                    this.Desirability = 4;
                    break;
                case GlobalResourceType.WOOD:
                    sprites.Add(SpriteManager.GetSprite(LocalSpriteName.WOOD_LOGS_THREE));
                    this.Name = "Wood";
                    this.Description = "Wood here grows well";
                    this.Desirability = 3;
                    break;
                default:
                    throw new NotImplementedException("No code for resource type " + ResourceType);
            }
        }

        public override bool MayContainItems
        {
            get
            {
                return true;
            }
            set
            {
                base.MayContainItems = value;
            }
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
