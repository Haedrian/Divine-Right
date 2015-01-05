using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.DataStructures;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using Microsoft.Xna.Framework;

namespace DRObjects.Items.Archetypes
{
    public class SummoningCircle
        :MapItem
    {
        /// <summary>
        /// Whether this item is summoning or has been disabled
        /// </summary>
        public bool IsSummoning { get; set; }

        /// <summary>
        /// The last time summoning took place.
        /// </summary>
        public DivineRightDateTime LastSummonTime { get; set; }

        public override string Description
        {
            get
            {
                if (IsSummoning)
                {
                    return "An active summoning circle. You should disable this";
                }
                else
                {
                    return "A disabled summoning circle";
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
                if (IsSummoning)
                {
                    return "An active summoning circle";
                }
                else
                {
                    return "A disabled summoning circle";
                }
            }
            set
            {
                base.Name = value;
            }
        }

        public override List<SpriteData> Graphics
        {
            get
            {
                if (IsSummoning)
                {
                    return new List<SpriteData>() { SpriteManager.GetSprite(LocalSpriteName.SUMMONING_CIRCLE) };
                }
                else
                {
                    return new List<SpriteData>() { SpriteManager.GetSprite(LocalSpriteName.SUMMONING_CIRCLE_OFF) };
                }
            }
            set
            {
                base.Graphics = value;
            }
        }

        public override ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actions = new List<ActionType>();

            actions.AddRange(base.GetPossibleActions(actor));

            if (IsSummoning)
            {
                actions.Add(ActionType.DISABLE);
            }

            return actions.ToArray();
        }

        public override ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType == ActionType.DISABLE)
            {
                //Disable it
                this.IsSummoning = false;

                //Increase skill in ritualism
                actor.Attributes.IncreaseSkill(SkillName.RITUALIST);

                return new ActionFeedback[]{new LogFeedback(InterfaceSpriteName.SUN,Color.Black,"You invoke a blessing upon the summoning circle and disable it")};
            }
            return base.PerformAction(actionType, actor, args);
        }

        public SummoningCircle()
        {
            this.IsSummoning = true;
            this.LastSummonTime = new DivineRightDateTime(0, 0, 0); //lowest possible date
        }

    }
}
