using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.GraphicsEngineObjects.Abstract;
using DRObjects.Enums;

namespace DRObjects.GraphicsEngineObjects
{
    public class InterfaceToggleFeedback :
        ActionFeedback
    {
        public InternalActionEnum InterfaceComponent { get; set; }
        public object Argument { get; set; }
        public bool Open { get; set; }

        public InterfaceToggleFeedback(InternalActionEnum action,bool open,object argument)
        {
            this.InterfaceComponent = action;
            this.Argument = argument;
            this.Open = open;
        }
    }
}
