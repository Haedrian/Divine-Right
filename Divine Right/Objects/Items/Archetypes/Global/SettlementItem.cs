﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using DRObjects.Graphics;
using Microsoft.Xna.Framework;
using DRObjects.GraphicsEngineObjects;

namespace DRObjects.Items.Archetypes.Global
{
    [Serializable]
    /// <summary>
    /// This represents an item which is part of a settlement
    /// </summary>
    public class SettlementItem :
        MapItem
    {
        #region Members
        public int SettlementSize { get; set; }
        public bool IsCapital { get; set; }
        public int OwnerID { get; set; }
        /// <summary>
        /// A reference to the settlement to be visited when used
        /// </summary>
        public Settlement Settlement { get; set; }
        /// <summary>
        /// The Corner of the settlement - determines which graphic to draw
        /// </summary>
        public int SettlementCorner { get; set; }
        public SettlementType SettlementType
        {
            get
            {
                if (IsCapital)
                {
                    return SettlementType.CAPITAL;
                }

                if (this.SettlementSize < 3)
                {
                    return SettlementType.HAMLET;
                }
                else
                {
                    return SettlementType.VILLAGE;
                }
            }
        }
        #endregion

        #region Private Members
        private List<SpriteData> sprites;
        #endregion

        #region Overridden functions
        public override ActionType[] GetPossibleActions(Actor actor)
        {
            List<ActionType> actions = new List<ActionType>();

            actions.AddRange(base.GetPossibleActions(actor));

            if (Math.Abs(actor.MapCharacter.Coordinate - this.Coordinate) < 2)
            {
                actions.Add(ActionType.EXPLORE);
            }

            return actions.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.ActionFeedback[] PerformAction(ActionType actionType, Actor actor, object[] args)
        {
            //Use base for everything that's not visit
            if (actionType != ActionType.EXPLORE)
            {
                return base.PerformAction(actionType, actor, args);
            }

            if (actor.IsPlayerCharacter && Math.Abs(actor.MapCharacter.Coordinate - this.Coordinate) < 2)
            {
                return new DRObjects.GraphicsEngineObjects.Abstract.ActionFeedback[1] { new LocationChangeFeedback() { Location = Settlement } };
            }
            else
            {
                return new GraphicsEngineObjects.Abstract.ActionFeedback[0] { };
            }
        }

        public override List<Graphics.SpriteData> Graphics
        {
            get
            {
                if (sprites == null)
                {
                    //Create it
                    sprites = new List<SpriteData>();

                    sprites.Add(SpriteManager.GetSprite((GlobalSpriteName)Enum.Parse(typeof(GlobalSpriteName), this.SettlementType.ToString() + "_" + SettlementCorner)));

                    //Are we in the top corner?
                    if (SettlementCorner == 1)
                    {
                        //Flag it

                       sprites.Add(SpriteManager.GetSprite(this.Settlement.Civilisation.Flag));
                    }

                }

                return sprites;
            }
            set
            {
                base.Graphics = value;
            }
        }
        #endregion
    }
}
