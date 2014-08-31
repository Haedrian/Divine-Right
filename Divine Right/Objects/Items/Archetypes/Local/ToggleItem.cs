using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Graphics;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.GraphicsEngineObjects;

namespace DRObjects.Items.Archetypes.Local
{
    /// <summary>
    /// This is a particular item which has two states. State A and State B.
    /// Each state can have a different graphic, different walkability.
    /// The item can have its state toggled by using it
    /// </summary>
    public class ToggleItem:
        MapItem
    {
        /// <summary>
        /// The state the item is in
        /// </summary>
        protected bool stateA = true;

        /// <summary>
        /// The Graphic to show when State A is the state
        /// </summary>
        public SpriteData GraphicStateA { get; set; }
        /// <summary>
        /// The Graphic to show when State B is the state
        /// </summary>
        public SpriteData GraphicStateB { get; set; }

        public string DescriptionStateA { get; set; }
        public string DescriptionStateB { get; set; }
        public bool AllowItemsStateA { get; set; }
        public bool AllowItemsStateB { get; set; }
        public string MessageToStateA{get;set;}
        public string MessageToStateB{get;set;}

        public override List<SpriteData> Graphics
        {
            get
            {
                return new List<SpriteData>(){Graphic};
            }
            set
            {
                base.Graphics = value;
            }
        }

        /// <summary>
        /// Returns the Graphic based on the state of the object
        /// </summary>
        public override Graphics.SpriteData Graphic
        {
            get
            {
                if (stateA)
                {
                    base.Graphics = new List<SpriteData>();
                    base.Graphics.Add(GraphicStateA);
                    return GraphicStateA;
                }
                else
                {
                    base.Graphics = new List<SpriteData>();
                    base.Graphics.Add(GraphicStateB);
                    return GraphicStateB;
                }
            }
            set
            {
                //dummy
            }
        }

        public override bool MayContainItems
        {
            get
            {
                if (stateA)
                {
                    return AllowItemsStateA;
                }
                else
                {
                    return AllowItemsStateB;
                }
            }
            set
            {
                //dummy
            }
        }

        public override string Description
        {
            get
            {
                if (stateA)
                {
                    return DescriptionStateA;
                }
                else
                {
                    return DescriptionStateB;
                }
            }
            set
            {
                //dummy
            }
        }

        public override Enums.ActionTypeEnum[]  GetPossibleActions(Actor actor)
        {
 	        List<Enums.ActionTypeEnum> enums = base.GetPossibleActions(actor).ToList();

            //We also want to add support for the "Use" enum - if they're one tile away

            if (Math.Abs(actor.MapCharacter.Coordinate - this.Coordinate) < 2)
            {
                enums.Add(Enums.ActionTypeEnum.USE);
            }

            return enums.ToArray();
        }

        public override GraphicsEngineObjects.Abstract.ActionFeedback[]  PerformAction(Enums.ActionTypeEnum actionType, Actor actor, object[] args)
        {
            if (actionType == Enums.ActionTypeEnum.USE)
            {
                if (Math.Abs(actor.MapCharacter.Coordinate - this.Coordinate) < 2)
                {
                    //if the actor is one tile away
                    stateA = !stateA; //toggle the state

                    if (stateA)
                    {
                        return new ActionFeedback[] { new TextFeedback(MessageToStateA)};
                    }
                    else 
                    {
                        return new ActionFeedback[] { new TextFeedback(MessageToStateB)};
                    }

                }
                else 
                {
                    return new ActionFeedback[] { new TextFeedback("Too far away")};
                }
            }
            else 
            { //let base handle it
 	            return base.PerformAction(actionType, actor, args);
            }
        }

    }
}
