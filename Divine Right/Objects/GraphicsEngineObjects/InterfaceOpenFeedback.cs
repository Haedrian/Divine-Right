using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.Enums;

namespace DRObjects.GraphicsEngineObjects
{
    public class InterfaceOpenFeedback :
        PlayerFeedback
    {
        public InternalActionEnum InterfaceComponent { get; set; }
        public object Argument { get; set; }

        public InterfaceOpenFeedback(InternalActionEnum action,object argument)
        {
            this.InterfaceComponent = action;
            this.Argument = argument;
        }
    }
}
