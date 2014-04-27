using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;

namespace DRObjects.Items.Archetypes.Local
{
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

                if (graphics == null || graphics.Count == 0)
                {
                    //The bottom one will always be the base sprite
                    sprites.Add(this.Graphic);
                }
                else
                {
                    sprites.AddRange(graphics);
                }
                

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
                        default:
                            throw new NotImplementedException("There is no graphic for the thought " + EnemyThought);
                    }
                }

                return sprites;
            }
            set
            {
                graphics = value;
            }
        }

        public override ActionTypeEnum[] GetPossibleActions(Actor actor)
        {
            List<ActionTypeEnum> actions = new List<ActionTypeEnum>();

            actions.AddRange(base.GetPossibleActions(actor));

            //Are we next to the target?
            if (this.Coordinate - actor.MapCharacter.Coordinate < 2)
            {
                //Add the attack one too
                actions.Add(ActionTypeEnum.PREPARE_ATTACK);
            }

            return actions.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.PlayerFeedback[] PerformAction(ActionTypeEnum actionType, Actor actor, object[] args)
        {
            if (actionType == ActionTypeEnum.PREPARE_ATTACK)
            {
                return new InterfaceToggleFeedback[] { new InterfaceToggleFeedback(InternalActionEnum.OPEN_ATTACK,true,this) };
            }
            else
            {
                return base.PerformAction(actionType, actor, args);
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
    }
}
