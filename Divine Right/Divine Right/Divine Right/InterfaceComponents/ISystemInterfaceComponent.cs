using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using DRObjects.Enums;
using Microsoft.Xna.Framework;

namespace Divine_Right.InterfaceComponents
{
    /// <summary>
    /// This is similar to the IGameInterfaceComponent, except that it only responds to mouse clicks, and it returns a string and a number of objects
    /// </summary>
    public interface ISystemInterfaceComponent
    {
        /// <summary>
        /// Handle the drawing of this component
        /// </summary>
        /// <param name="content"></param>
        /// <param name="batch"></param>
        void Draw(ContentManager content, SpriteBatch batch);

        /// <summary>
        /// Handles being clicked on
        /// </summary>
        /// <param name="x">The x coordinate the mouse was at</param>
        /// <param name="y">The y coordinate the mouse was at</param>
        /// <param name="instruction">The instruction to return</param>
        /// <param name="args">Any arguments</param>
        /// <returns></returns>
        bool HandleClick(int x, int y, out InternalActionEnum? instruction, out object[] args);

        /// <summary>
        /// The Rectangle that this component is within
        /// </summary>
        /// <returns></returns>
        Rectangle ReturnLocation();


    }
}
