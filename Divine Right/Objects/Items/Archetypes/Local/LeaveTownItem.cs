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

        public override Enums.ActionTypeEnum[] GetPossibleActions(Actor actor)
        {
            List<ActionTypeEnum> actions = new List<ActionTypeEnum>();
            actions.AddRange(base.GetPossibleActions(actor));

            if (actor.MapCharacter.Coordinate - this.Coordinate < 2)
            {
                actions.Add(ActionTypeEnum.LEAVE);
            }
            return actions.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.PlayerFeedback[] PerformAction(ActionTypeEnum actionType, Actor actor, object[] args)
        {
            if (actionType != ActionTypeEnum.LEAVE)
            {
                return base.PerformAction(actionType, actor, args);
            }
            else
            {
                if (actor.IsPlayerCharacter && (actor.MapCharacter.Coordinate - this.Coordinate < 2))
                {
                    return new DRObjects.GraphicsEngineObjects.Abstract.PlayerFeedback[1] { new LocationChangeFeedback() { VisitMainMap = true  } };
                }
                else
                {
                    return new GraphicsEngineObjects.Abstract.PlayerFeedback[0] { };
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
