using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Global
{
    public class MapSiteItem:
        MapItem
    {
        /// <summary>
        /// The sprite to display
        /// </summary>
        private List<SpriteData> sprites = null;
        
        public MapSite Camp { get; set; }

        /// <summary>
        /// Creates a bandit camp item.
        /// </summary>
        public MapSiteItem()
        {
            this.MayContainItems = true;
            sprites = new List<SpriteData>();

            //Show a graphic depending on the type of the site (and later on the owner)
            switch(Camp.SiteData.SiteTypeData.SiteType)
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
                    //VisitCamp = this.Camp
                }
            };
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
