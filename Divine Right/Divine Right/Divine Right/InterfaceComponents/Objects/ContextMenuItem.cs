using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.Enums;
using Microsoft.Xna.Framework;

namespace Divine_Right.InterfaceComponents.Objects
{
    /// <summary>
    /// Represents a context menu item in the list
    /// </summary>
    public class ContextMenuItem
    {
        #region Properties
        
        /// <summary>
        /// The type of the action to display and send
        /// </summary>
        public ActionType Action{get;set; }
        /// <summary>
        /// The rectangle which encompasses this context menu item
        /// </summary>
        public Rectangle Rect { get; set; }
        /// <summary>
        /// Any arguments which need to be passed for an action to occur
        /// </summary>
        public object[] Args { get; set; }

        /// <summary>
        /// The text to display
        /// </summary>
        public string Text
        {
            get
            {
                string text = this.Action.ToString();
                text = text.ToLowerInvariant();
                text = text.Replace('_', ' ');

                //capitalise first character
                string firstChar = text[0].ToString().ToUpperInvariant();
                text = string.Concat(firstChar, text.Substring(1));
                return text;

            }
        }

        #endregion Properties
    }
}
