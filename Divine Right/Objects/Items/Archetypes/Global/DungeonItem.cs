﻿using System;
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
    /// A map item representing a dungeon on the map.
    /// </summary>
    public class DungeonItem
        :MapItem
    {
        /// <summary>
        /// The sprite to display
        /// </summary>
        private List<SpriteData> sprites = null;
        /// <summary>
        /// The owning race of the dungeon
        /// </summary>
        public int? OwnerID { get; set; }

        /// <summary>
        /// The Corner of the Dungeon - determines which graphic to draw
        /// </summary>
        public int DungeonCorner { get; set; }

        public Dungeon Dungeon { get; set; }

        /// <summary>
        /// Creates a dungeon item. For now we only support mazes, not ruins
        /// </summary>
        public DungeonItem(int corner)
        {
            DungeonCorner = corner;
            this.MayContainItems = true;
            sprites = new List<SpriteData>();

            sprites.Add(SpriteManager.GetSprite((GlobalSpriteName)Enum.Parse(typeof(GlobalSpriteName), "DUNGEON" + "_" + DungeonCorner)));
        }

        public override Enums.ActionTypeEnum[] GetPossibleActions(Actor actor)
        {
            List<ActionTypeEnum> actionTypes = new List<ActionTypeEnum>();
            actionTypes.AddRange(base.GetPossibleActions(actor));

            if (Math.Abs(this.Coordinate - actor.MapCharacter.Coordinate) < 2)
            {
                actionTypes.Add(ActionTypeEnum.EXPLORE);
            }

            return actionTypes.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.PlayerFeedback[] PerformAction(ActionTypeEnum actionType, Actor actor, object[] args)
        {
            if (actionType != ActionTypeEnum.EXPLORE)
            {
                return base.PerformAction(actionType, actor, args);
            }

            //Otherwise, visit the dungeon

            return new GraphicsEngineObjects.Abstract.PlayerFeedback[1] {
                new LocationChangeFeedback()
                {
                    VisitMainMap = false,
                    VisitDungeon = Dungeon
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
