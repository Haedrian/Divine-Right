using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects;

namespace DRObjects.Items.Archetypes.Local
{
    [Serializable]
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

        public override Enums.ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actions = new List<ActionType>();
            actions.AddRange(base.GetPossibleActions(actor));

            if (actor.MapCharacter.Coordinate - this.Coordinate < 2)
            {
                actions.Add(ActionType.LEAVE);
            }
            return actions.ToArray();
        }

        public override bool MayContainItems
        {
            get
            {
                return true;
            }
            set
            {
                
            }
        }

        public override GraphicsEngineObjects.Abstract.ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType != ActionType.LEAVE)
            {
                return base.PerformAction(actionType, actor, args);
            }
            else
            {
                if (actor.IsPlayerCharacter && (actor.MapCharacter.Coordinate - this.Coordinate < 2))
                {
                    return new DRObjects.GraphicsEngineObjects.Abstract.ActionFeedback[1] { new LocationChangeFeedback() { VisitMainMap = true  } };
                }
                else
                {
                    return new GraphicsEngineObjects.Abstract.ActionFeedback[0] { };
                }
            }
        }

        public LeaveTownItem()
        {
            this.sprites = new List<SpriteData>();
            sprites.Add(SpriteManager.GetSprite(InterfaceSpriteName.GOTO_WORLD_MAP));
        }
    }
}
