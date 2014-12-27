using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;

namespace DRObjects.Items.Archetypes.Local
{
    /// <summary>
    /// Represents stairs in a dungeon. Either for going in deeper, or for returning to the surface
    /// </summary>
    public class DungeonStairs :
        MapItem
    {
        /// <summary>
        /// Whether the stairs go up or down
        /// </summary>
        public bool StairsUp { get; set; }

        public DungeonStairs(bool stairsUp)
        {
            this.StairsUp = stairsUp;
        }

        public DungeonStairs()
        {
            StairsUp = false;
        }

        private List<SpriteData> sprites = null;

        public override List<SpriteData> Graphics
        {
            get
            {
                if (sprites != null)
                {
                    return sprites;
                }
                else
                {
                    sprites = new List<SpriteData>();

                    if (this.StairsUp)
                    {
                        Graphics.Add(SpriteManager.GetSprite(LocalSpriteName.STAIRS_UP));
                    }
                    else
                    {
                        Graphics.Add(SpriteManager.GetSprite(LocalSpriteName.STAIRS_DOWN));
                    }

                    return sprites;
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

            if (Math.Abs( actor.MapCharacter.Coordinate - this.Coordinate) < 2)
            {
                if (this.StairsUp)
                {
                    actions.Add(ActionType.ASCEND_TO_SURFACE);
                }
                else
                {
                    actions.Add(ActionType.DESCEND);
                }
            }

            return actions.ToArray();
        }

        public override string Description
        {
            get
            {
                if (this.StairsUp)
                {
                    return "Stairs for returning to the surface";
                }
                else
                {
                    return "Stairs for going in deeper...";
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
                if (this.StairsUp)
                {
                    return "To the surface";
                }
                else
                {
                    return "Go in deeper";
                }
            }
            set
            {
                base.Name = value;
            }
        }



    }
}
