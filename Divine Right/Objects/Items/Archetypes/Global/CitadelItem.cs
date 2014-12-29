using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;
using DRObjects.Enums;
using DRObjects.GraphicsEngineObjects;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    /// <summary>
    /// A map item representing a Citadel on the map.
    /// </summary>
    public class CitadelItem
        :MapItem
    {
        /// <summary>
        /// The sprite to display
        /// </summary>
        private List<SpriteData> sprites = null;
        /// <summary>
        /// The owning race of the Citadel
        /// </summary>
        public int? OwnerID { get; set; }

        /// <summary>
        /// The Corner of the Dungeon - determines which graphic to draw
        /// </summary>
        public int DungeonCorner { get; set; }

        public Citadel Citadel { get; set; }

        /// <summary>
        /// Creates a citadel item. For now we only support mazes, not ruins
        /// </summary>
        public CitadelItem(int corner)
        {
            DungeonCorner = corner;
            this.MayContainItems = true;
            sprites = new List<SpriteData>();

            sprites.Add(SpriteManager.GetSprite((GlobalSpriteName)Enum.Parse(typeof(GlobalSpriteName), "DUNGEON" + "_" + DungeonCorner)));
        }

        public override Enums.ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actionTypes = new List<ActionType>();
            actionTypes.AddRange(base.GetPossibleActions(actor));

            if (Math.Abs(this.Coordinate - actor.MapCharacter.Coordinate) < 2)
            {
                actionTypes.Add(ActionType.EXPLORE);
            }

            return actionTypes.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            if (actionType != ActionType.EXPLORE)
            {
                return base.PerformAction(actionType, actor, args);
            }

            //Otherwise, visit the citadel

            return new GraphicsEngineObjects.Abstract.ActionFeedback[1] {
                new LocationChangeFeedback()
                {
                    VisitMainMap = false,
                    Location = Citadel
                }
            };
        }

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
    }
}
