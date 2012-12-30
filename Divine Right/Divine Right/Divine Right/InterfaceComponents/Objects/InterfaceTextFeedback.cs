using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects;

namespace Divine_Right.InterfaceComponents.Objects
{
    /// <summary>
    /// Text feedback items with some additional information allowing it to be displayed on the interface
    /// </summary>
   public class InterfaceTextFeedback
        : TextFeedback
    {
        #region Properties
        /// <summary>
        /// The X coordinate on the interface
        /// </summary>
        public int InterfaceX { get; set; }
        /// <summary>
        /// The Y coordinate on the interface
        /// </summary>
        public int InterfaceY { get; set; }

        /// <summary>
        /// The datetime upon which this will stop appearing
        /// </summary>
        public DateTime TimeToDestroy { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new InterfaceTextFeedback based on the textfeedback passed
        /// </summary>
        /// <param name="feedback"></param>
        public InterfaceTextFeedback(TextFeedback feedback,int xCoord, int yCoord)
        {
            this.Text = feedback.Text;
            this.InterfaceX = xCoord;
            this.InterfaceY = yCoord;

            TimeToDestroy = DateTime.Now.AddSeconds(DRGame.TEXTFEEDBACKDISPLAYTIMESECONDS);
        }

        #endregion
    }
}
