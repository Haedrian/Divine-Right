using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using Microsoft.Xna.Framework;
using DRObjects.ActorHandling.CharacterSheet.Enums;

namespace DRObjects.Items.Archetypes.Local
{
    [Serializable]
    public class LocalCharacter: MapItem
    {
        private List<SpriteData> graphics = null;

        private static SpriteData WalkSprite
        {
            get
            {
                return SpriteManager.GetSprite(LocalSpriteName.ENEMY_THOUGHT_WALK);
            }
        }
        private static SpriteData WaitSprite
        {
            get
            {
                return SpriteManager.GetSprite(LocalSpriteName.ENEMY_THOUGHT_WAIT);
            }
        }
        private static SpriteData AttackSprite
        {
            get
            {
                return SpriteManager.GetSprite(LocalSpriteName.ENEMY_THOUGHT_ATTACK);
            }
        }
        private static SpriteData StunnedSprite
        {
            get
            {
                return SpriteManager.GetSprite(LocalSpriteName.ENEMY_THOUGH_CONFUSED);
            }
        }

        private static SpriteData ProneSprite
        {
            get
            {
                return SpriteManager.GetSprite(LocalSpriteName.ENEMY_THOUGHT_PRONE);
            }
        }

        private static SpriteData RogueSprite
        {
            get
            {
                return SpriteManager.GetSprite(LocalSpriteName.ROGUE_CLASS);
            }
        }

        private static SpriteData WarriorSprite
        {
            get
            {
                return SpriteManager.GetSprite(LocalSpriteName.WARRIOR_CLASS);
            }
        }

        private static SpriteData BruteSprite
        {
            get
            {
                return SpriteManager.GetSprite(LocalSpriteName.BRUTE_CLASS);
            }
        }

        private static SpriteData RangedSprite
        {
            get
            {
                return SpriteManager.GetSprite(LocalSpriteName.RANGED_CLASS);
            }
        }

        private SpriteData baseGraphic;

        /// <summary>
        /// What the enemy is thinking
        /// </summary>
        public EnemyThought EnemyThought { get; set; }


        public override SpriteData Graphic
        {
            get
            {
                return this.baseGraphic;
            }
            set
            {
                this.baseGraphic = value;
            }
        }

        public override List<SpriteData> Graphics
        {
            get
            {
                List<SpriteData> sprites = new List<SpriteData>();                

                if (IsStunned)
                {
                    sprites.Insert(0, StunnedSprite);
                }
                else
                {
                    switch (EnemyThought)
                    {
                        case Enums.EnemyThought.ATTACK:
                            //sprites.Insert(0, AttackSprite);
                            sprites.Add(AttackSprite);
                            break;
                        case Enums.EnemyThought.WAIT:
                            //sprites.Insert(0, WaitSprite);
                            sprites.Add(WaitSprite);
                            break;
                        case Enums.EnemyThought.WALK:
                            //sprites.Insert(0, WalkSprite);
                            sprites.Add(WalkSprite);
                            break;
                        case Enums.EnemyThought.PRONE:
                            sprites.Add(ProneSprite);
                            break;
                        default:
                            throw new NotImplementedException("There is no graphic for the thought " + EnemyThought);
                    }
                }

                if (this.Actor.EnemyData != null)
                {
                    //Add the class
                    switch (this.Actor.EnemyData.Profession)
                    {
                        case ActorProfession.BRUTE:
                            sprites.Add(BruteSprite);
                            break;
                        case ActorProfession.RANGED:
                            sprites.Add(RangedSprite);
                            break;
                        case ActorProfession.DEFENDER:
                            sprites.Add(RogueSprite);
                            break;
                        case ActorProfession.WARRIOR:
                            sprites.Add(WarriorSprite);
                            break;
                    }
                }

                if (graphics == null || graphics.Count == 0)
                {
                    //The base sprite goes on top
                    sprites.Add(this.Graphic);
                }
                else
                {
                    sprites.AddRange(graphics);
                }

                return sprites;
            }
            set
            {
                graphics = value;
            }
        }

        public override bool MayContainItems
        {
            get
            {
                return Actor.IsProne;
            }
            set
            {
                base.MayContainItems = value;
            }
        }

        public override ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actions = new List<ActionType>();

            actions.AddRange(base.GetPossibleActions(actor));

            //Are we next to the target? Is the actor aggressive, or an animal?
            if ( (this.Coordinate - actor.MapCharacter.Coordinate < 2 || actor.Inventory.EquippedItems.ContainsKey(EquipmentLocation.BOW)) && (this.Actor.IsAggressive || (this.Actor.IsAnimal && !this.Actor.IsDomesticatedAnimal)))
            {
                //Add the attack one too
                actions.Add(ActionType.PREPARE_ATTACK);
            }

            //Is he a vendor?
            if (this.Coordinate - actor.MapCharacter.Coordinate < 2 && this.Actor.VendorDetails != null)
            {
                actions.Add(ActionType.TRADE);
            }

            //Can we shove them ?
            if (this.Coordinate - actor.MapCharacter.Coordinate <2 && !this.Actor.IsAggressive)
            {
                actions.Add(ActionType.SHOVE);
            }

            return actions.ToArray();
        }

        public override ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType == ActionType.PREPARE_ATTACK)
            {
                return new InterfaceToggleFeedback[] { new InterfaceToggleFeedback(InternalActionEnum.OPEN_ATTACK,true,this) };
            }
            else if (actionType == ActionType.TRADE)
            {
                return new InterfaceToggleFeedback[] { new InterfaceToggleFeedback(InternalActionEnum.OPEN_TRADE,true,new object[2] {this.Actor,actor})};
            }
            else if (actionType == ActionType.SHOVE)
            {
                this.Actor.IsProne = true;
                return new LogFeedback[] { new LogFeedback( InterfaceSpriteName.CHA, Color.Black, "You shove " + this.Name)};
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
            }
        }

        public override string Name
        {
            get
            {
                if (Actor != null && !String.IsNullOrEmpty(Actor.Name))
                {
                    return Actor.Name;
                }
                else
                {
                    return base.Name;
                }
            }
            set
            {
                base.Name = value;
            }
        }

        /// <summary>
        /// How far this enemy can see
        /// </summary>
        public int LineOfSightRange { get; set; }

        /// <summary>
        /// The actor being linked to
        /// </summary>
        public bool IsStunned { get; set; }

        /// <summary>
        /// The actor this character is linked to
        /// </summary>
        public Actor Actor { get; set; }
    }
}
