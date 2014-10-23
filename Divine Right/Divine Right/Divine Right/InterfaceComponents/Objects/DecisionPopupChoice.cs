using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using Microsoft.Xna.Framework;

namespace Divine_Right.InterfaceComponents.Objects
{
    /// <summary>
    /// A choice in a decision popup
    /// </summary>
    public class DecisionPopupChoice
    {
        /// <summary>
        /// The Text to display for the choice
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// The internal action to trigger when this choice is selected
        /// </summary>
        public InternalActionEnum? InternalAction { get; set; }
        /// <summary>
        /// The type of the action to trigger when this choice is selected.
        /// </summary>
        public ActionType? ActionType { get; set; }
        /// <summary>
        /// The arguments to send with the action
        /// </summary>
        public object[] Args { get; set; }

        /// <summary>
        /// The rectangle that the decision is saved in. Populated when it's drawn on the screen
        /// </summary>
        public Rectangle Rect { get; set; }

        public DecisionPopupChoice(string text, InternalActionEnum? internalAction, ActionType? gameAction,object[] arguments)
        {
            this.Text = text;
            this.InternalAction = internalAction;
            this.ActionType = gameAction;
            this.Args = arguments;
        }
    }
}
