using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;

namespace DRObjects.Items.Archetypes.Local
{
    public class LocalEnemy: MapItem
    {
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
                //The bottom one will always be the base sprite
                List<SpriteData> sprites = new List<SpriteData> { this.Graphic };

                switch (EnemyThought)
                {
                    case Enums.EnemyThought.ATTACK:
                        sprites.Insert(0,AttackSprite);  
                        break;
                    case Enums.EnemyThought.WAIT:
                        sprites.Insert(0, WaitSprite);
                        break;
                    case Enums.EnemyThought.WALK:
                        sprites.Insert(0, WalkSprite);
                        break;
                    default:
                        throw new NotImplementedException("There is no graphic for the thought " + EnemyThought);
                }

                return sprites;
            }
            set
            {
                //dummy :)
            }
        }

        /// <summary>
        /// How far this enemy can see
        /// </summary>
        public int LineOfSightRange { get; set; }
    }
}
