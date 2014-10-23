﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;
using Microsoft.Xna.Framework;

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
                        default:
                            throw new NotImplementedException("There is no graphic for the thought " + EnemyThought);
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

        public override ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actions = new List<ActionType>();

            actions.AddRange(base.GetPossibleActions(actor));

            //Are we next to the target? Is the actor aggressive
            if (this.Coordinate - actor.MapCharacter.Coordinate < 2 && this.Actor.IsAggressive)
            {
                //Add the attack one too
                actions.Add(ActionType.PREPARE_ATTACK);
            }

            //Is he a vendor?
            if (this.Coordinate - actor.MapCharacter.Coordinate < 2 && this.Actor.VendorDetails != null)
            {
                actions.Add(ActionType.TRADE);
            }

            return actions.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType == ActionType.PREPARE_ATTACK)
            {
                return new InterfaceToggleFeedback[] { new InterfaceToggleFeedback(InternalActionEnum.OPEN_ATTACK,true,this) };
            }
            else if (actionType == ActionType.TRADE)
            {
                return new InterfaceToggleFeedback[] { new InterfaceToggleFeedback(InternalActionEnum.OPEN_TRADE,true,new object[2] {this.Actor,actor})};
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
