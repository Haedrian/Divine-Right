using DRObjects.ActorHandling.CharacterSheet.Enums;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Feedback;

namespace DRObjects.Items.Archetypes.Local
{
    public class Altar
        : MapItem
    {
        /// <summary>
        /// Whether it's a cursed altar or not
        /// </summary>
        private bool IsCursed { get; set; }
        /// <summary>
        /// Whether the altar has been used or not
        /// </summary>
        private bool IsUsed { get; set; }

        public override string Description
        {
            get
            {
                if (this.IsCursed)
                {
                    return "An altar to an unholy deity";
                }
                else
                {
                    if (this.IsUsed)
                    {
                        return "An altar you have already prayed at";
                    }
                    else
                    {
                        return "An altar consecrated to your deity";
                    }
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
                if (this.IsCursed)
                {
                    return "Unholy Altar";
                }
                else
                {
                    return "Altar";
                }
            }
            set
            {
                base.Name = value;
            }
        }

        public override List<Graphics.SpriteData> Graphics
        {
            get
            {
                if (this.IsCursed)
                {
                    return new List<SpriteData>(){SpriteManager.GetSprite(LocalSpriteName.ALTAR_CURSED)};
                }
                else if (this.IsUsed)
                {
                    return new List<SpriteData>() { SpriteManager.GetSprite(LocalSpriteName.ALTAR_USED) };
                }
                else
                {
                    return new List<SpriteData>() { SpriteManager.GetSprite(LocalSpriteName.ALTAR) };
                }
            }
            set
            {
                base.Graphics = value;
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

        public override Enums.ActionType[] GetPossibleActions(Actor actor)
        {
            if (actor.MapCharacter.Coordinate - this.Coordinate < 2)
            {
                List<ActionType> actions = new List<ActionType>();
                actions.AddRange(base.GetPossibleActions(actor));

                if (this.IsCursed)
                {
                    actions.Add(ActionType.BLESS);
                }
                else if (this.IsUsed)
                {

                }
                else
                {
                    actions.Add(ActionType.PRAY);
                }

                return actions.ToArray();
            }

            return base.GetPossibleActions(actor);
        }

        public override GraphicsEngineObjects.Abstract.ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType == ActionType.BLESS)
            {
                //Learn some skill
                actor.Attributes.IncreaseSkill(SkillName.RITUALIST);

                //Add a message and uncurse it
                this.IsCursed = false;

                return new ActionFeedback[] { new LogFeedback(InterfaceSpriteName.SUN,Color.Black,"You invoke a blessing upon the altar and claim it for your deity")};
            }
            else if (actionType == ActionType.PRAY)
            {
                this.IsUsed = true;
                //Since it's out of the scope, we'll handle it somewhere else
                return new ActionFeedback[] { new ReceiveBlessingFeedback() };
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }
        }

        public Altar()
        {
            IsCursed = true;
            IsUsed = false;
        }

    }
}
