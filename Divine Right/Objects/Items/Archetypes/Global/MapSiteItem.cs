using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    public class MapSiteItem:
        MapItem
    {
        /// <summary>
        /// The sprite to display
        /// </summary>
        private List<SpriteData> sprites = null;
        
        public MapSite Site { get; set; }

        /// <summary>
        /// Creates a map site item
        /// </summary>
        public MapSiteItem()
        {
            this.MayContainItems = true;
        }

        public override string Name
        {
            get
            {
                switch(Site.SiteData.SiteTypeData.SiteType)
                {
                    case SiteType.FARM: return "Farm";
                    case SiteType.FISHING_VILLAGE: return "Fishing Village";
                    case SiteType.GOLD_MINE: return "Gold Mine";
                    case SiteType.HUNTER: return "Hunting Lodge";
                    case SiteType.IRON_MINE: return "Iron Mine";
                    case SiteType.STABLES: return "Stables";
                    case SiteType.WOODCUTTER: return "Woodcutter";
                    default:
                        return "";
                }
            }
            set
            {
                base.Name = value;
            }
        }

        public override string Description
        {
            get
            {
                switch(Site.SiteData.SiteTypeData.SiteType)
                {
                    case SiteType.FARM: return "A farm";
                    case SiteType.FISHING_VILLAGE: return "A village of fishermen";
                    case SiteType.GOLD_MINE: return "A mine where gold ore may be found";
                    case SiteType.HUNTER: return "A hunting lodge";
                    case SiteType.IRON_MINE: return "A mine where ferrous metals may be found";
                    case SiteType.STABLES: return "A stable for breeding of horses";
                    case SiteType.WOODCUTTER: return "A woodcutter's hut";
                    default:
                        return "";
                }
            }
            set
            {
                base.Description = value;
            }
        }

        public override Enums.ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actionTypes = new List<ActionType>();
            actionTypes.AddRange(base.GetPossibleActions(actor));

            if (Math.Abs(this.Coordinate - actor.MapCharacter.Coordinate) < 2)
            {
                actionTypes.Add(ActionType.EXPLORE);
            }

            return actionTypes.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType != ActionType.EXPLORE)
            {
                return base.PerformAction(actionType, actor, args);
            }

            //Otherwise, visit the camp

            return new GraphicsEngineObjects.Abstract.ActionFeedback[1] {
                new LocationChangeFeedback()
                {
                    VisitMainMap = false,
                    VisitSite = this.Site
                }
            };
        }

        public override List<SpriteData> Graphics
        {
            get
            {
                if (sprites == null)
                {
                    sprites = new List<SpriteData>();

                    //who's the owner?
                    switch(Site.SiteData.Owners)
                    {
                        case OwningFactions.ABANDONED:
                        case OwningFactions.UNDEAD:
                            //todo
                            break;
                        case OwningFactions.BANDITS:
                            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FLAG_BLACK));
                            break;
                        case OwningFactions.HUMANS:

                            switch (this.Site.SiteData.OwnerID)
                            {
                                case 0: sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FLAG_BROWN)); break;
                                case 1: sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FLAG_GREEN)); break;
                                case 2: sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FLAG_ORANGE)); break;
                                case 3: sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FLAG_PINK)); break;
                                case 4: sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FLAG_PURPLE)); break;
                                case 5: sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FLAG_RED)); break;
                                case 6: sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FLAG_YELLOW)); break;
                            }

                            break;
                        case OwningFactions.ORCS:
                            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FLAG_ORC));
                            break;
                    }

                    //Show a graphic depending on the type of the site (and later on the owner)
                    switch (Site.SiteData.SiteTypeData.SiteType)
                    {
                        case SiteType.FARM:
                            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FARM));
                            break;
                        case SiteType.FISHING_VILLAGE:
                            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.FISHING_HUT));
                            break;
                        case SiteType.GOLD_MINE:
                            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.GOLD_MINE));
                            break;
                        case SiteType.HUNTER:
                            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.HUNTER));
                            break;
                        case SiteType.IRON_MINE:
                            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.IRON_MINE));
                            break;
                        case SiteType.STABLES:
                            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.STABLES));
                            break;
                        case SiteType.WOODCUTTER:
                            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.WOODCUTTER));
                            break;
                    }
                }

                return sprites;
            }
            set
            {
                base.Graphics = value;
            }
        }
    }
}
