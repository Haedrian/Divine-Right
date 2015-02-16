using DRObjects.ActorHandling.SpecialAttacks;
using DRObjects.Enums;
using DRObjects.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.ActorHandling.CharacterSheet;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.Feedback;
using DRObjects.Feedback.OpenInterfaceObjects;

namespace DRObjects.Items.Archetypes.Local
{
    [Serializable]
    /// <summary>
    /// A scroll holding information about a particular special attack
    /// </summary>
    public class CombatManual:
        InventoryItem
    {
        /// <summary>
        /// The Special Attack this scroll teaches when it is consumed
        /// </summary>
        public SpecialAttack SpecialAttack { get; set; }

        private List<SpriteData> graphics = null;

        public override InventoryCategory Category
        {
            get
            {
                return InventoryCategory.DOCUMENTS;
            }
            set
            {
                base.Category = value;
            }
        }

        public override string Description
        {
            get
            {
                return "A combat manual. Required Level : " + ActorSkill.DisplayLevels[SpecialAttack.SkillLevelRequired];
            }
            set
            {
                base.Description = value;
            }
        }

        public override ActionType[] GetPossibleActions(Actor actor)
        {
            if (InInventory)
            {
                List<ActionType> actions = new List<ActionType>();

                actions.AddRange(base.GetPossibleActions(actor));

                actions.Add(ActionType.READ);

                return actions.ToArray();
            }
            else
            {
                return base.GetPossibleActions(actor);
            }
        }

        public override List<Graphics.SpriteData> Graphics
        {
            get
            {
                if (graphics == null)
                {
                    graphics = new List<SpriteData>();
                    //Generate it
                    graphics.Add(SpriteManager.GetSprite(LocalSpriteName.SCROLL));

                    switch(SpecialAttack.Level)
                    {
                        case 1:
                            graphics.Add(SpriteManager.GetSprite(LocalSpriteName.SA_GREEN)); break;
                        case 2:
                            graphics.Add(SpriteManager.GetSprite(LocalSpriteName.SA_YELLOW));break;
                        case 3:
                            graphics.Add(SpriteManager.GetSprite(LocalSpriteName.SA_ORANGE)); break;
                        case 4:
                            graphics.Add(SpriteManager.GetSprite(LocalSpriteName.SA_RED)); break;
                        case 5:
                            graphics.Add(SpriteManager.GetSprite(LocalSpriteName.SA_DARKRED)); break;
                        default:
                            throw new NotImplementedException("No code for Special attack of level " + SpecialAttack.Level);
                    }
                }

                return graphics;
            }
            set
            {
                base.Graphics = value;
            }
        }

        public CombatManual(SpecialAttack attack)
        {
            this.SpecialAttack = attack;
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

        public override string Name
        {
            get
            {
                return "Special Attack Scroll";
            }
            set
            {
                base.Name = value;
            }
        }

        public override ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {

            if (actionType == ActionType.READ)
            {
                //Open the CombatManualComponent
                return new ActionFeedback[] { new OpenInterfaceFeedback(new CombatManualInterface(this)) };
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }
        }
    }
}
