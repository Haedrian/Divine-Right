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
    public class DungeonItem
        :MapItem
    {
        public Dungeon Dungeon { get; set; }

        private List<SpriteData> sprites = null;

        public DungeonItem()
        {
            MayContainItems = true;
            sprites = new List<SpriteData>();
            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.RUINS));
        }

        public override string Description
        {
            get
            {
                return "An evil place fraught with danger";
            }
            set
            {
                base.Description = value;
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

        public override string Name
        {
            get
            {
                return "Dungeon";
            }
            set
            {
                base.Name = value;
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

            //Otherwise, visit the dungeon

            return new GraphicsEngineObjects.Abstract.ActionFeedback[1] {
                new LocationChangeFeedback()
                {
                    VisitMainMap = false,
                    Location = this.Dungeon
                }
            };
        }

    }
}
