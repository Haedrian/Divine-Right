using DRObjects.Feedback.OpenInterfaceObjects;
using DRObjects.GraphicsEngineObjects.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DRObjects.Feedback
{
    /// <summary>
    /// A generic class which contains a particular instance of a IGameInterfaceComponent which the system is to load and display 
    /// </summary>
    public class OpenInterfaceFeedback
        : ActionFeedback
    {
        public OpenInterfaceFeedback(OpenInterfaceObject inter)
        {
            Interface = inter;
        }

        public OpenInterfaceObject Interface { get; set; }
    }
}
