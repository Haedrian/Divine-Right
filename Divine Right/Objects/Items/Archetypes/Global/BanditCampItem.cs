﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    /// <summary>
    /// A representation of a bandit camp on the map
    /// </summary>
    public class BanditCampItem: 
        MapItem
    {
        /// <summary>
        /// The sprite to display
        /// </summary>
        private List<SpriteData> sprites = null;
        
        public BanditCamp Camp { get; set; }

        /// <summary>
        /// Creates a bandit camp item.
        /// </summary>
        public BanditCampItem()
        {
            this.MayContainItems = true;
            sprites = new List<SpriteData>();

            sprites.Add(SpriteManager.GetSprite(GlobalSpriteName.BANDIT_CAMP));
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

        public override GraphicsEngineObjects.Abstract.ActionFeedback[] PerformAction(ActionTypeEnum actionType, Actor actor, object[] args)
        {
            if (actionType != ActionTypeEnum.EXPLORE)
            {
                return base.PerformAction(actionType, actor, args);
            }

            //Otherwise, visit the camp

            return new GraphicsEngineObjects.Abstract.ActionFeedback[1] {
                new LocationChangeFeedback()
                {
                    VisitMainMap = false,
                    VisitCamp = this.Camp
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
