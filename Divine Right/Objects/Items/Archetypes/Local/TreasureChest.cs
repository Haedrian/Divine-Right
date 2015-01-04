using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Items.Archetypes.Local
{
    public class TreasureChest:
        MapItem
    {
        /// <summary>
        /// The items this chest contains
        /// </summary>
        public List<InventoryItem> Contents { get; set; }

        public TreasureChest()
        {
            this.Contents = new List<InventoryItem>();
        }

        public override string Description
        {
            get
            {
                if (this.Contents.Count == 0)
                {
                    return "An empty treasure chest";
                }
                else
                {
                    return "A chest with treasure inside";
                }
            }
            set
            {
                base.Description = value;
            }
        }

        public override string Name
        {
            get
            {
                if (this.Contents.Count == 0)
                {
                    return "Empty Chest";
                }
                else
                {
                    return "Treasure Chest";
                }
            }            
            set
            {
                base.Name = value;
            }
        }

        public override ActionType[] GetPossibleActions(Actor actor)
        {
            if (actor.MapCharacter.Coordinate - this.Coordinate > 2)
            {
                return base.GetPossibleActions(actor);
            }
            else
            {
                List<ActionType> actions = base.GetPossibleActions(actor).ToList();

                actions.Add(ActionType.LOOT);

                return actions.ToArray();
            }
        }

        public override bool MayContainItems
        {
            get
            {
                return false;
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
                if (this.Contents.Count > 0)
                {
                    return new List<Graphics.SpriteData> { SpriteManager.GetSprite(LocalSpriteName.TREASURE_CLOSED) };
                }
                else 
                {
                    return new List<Graphics.SpriteData> { SpriteManager.GetSprite(LocalSpriteName.TREASURE_EMPTY) };
                }
            }
            set
            {
                base.Graphics = value;
            }
        }

        public override ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType == ActionType.LOOT)
            {
                return new ActionFeedback[] { new InterfaceToggleFeedback(InternalActionEnum.OPEN_LOOT, true, new object[1] { this}) };
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }
        }

    }
}
