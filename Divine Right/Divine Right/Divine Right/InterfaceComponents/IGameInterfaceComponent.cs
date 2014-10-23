using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DRObjects.Enums;
using DRObjects;
using Microsoft.Xna.Framework.Input;
using Divine_Right.InterfaceComponents.Objects.Enums;

namespace Divine_Right.InterfaceComponents
{
    /// <summary>
    /// This is an interface for all components.
    /// </summary>
    interface IGameInterfaceComponent
    {
        /// <summary>
        /// Handle the drawing of this component
        /// </summary>
        /// <param name="content"></param>
        /// <param name="batch"></param>
        void Draw(ContentManager content, SpriteBatch batch);
        /// <summary>
        /// Handle the mouse being clicked on the control
        /// Return true if it was handled, false if you want something else to handle it
        /// </summary>
        /// <param name="x">The x coordinate where the mouse clicked</param>
        /// <param name="y">The y coordinate where the mouse clicked</param>
        /// <param name="mouseAction">The action which has been performed by the mouse</param>
        /// <param name="actionType">The type of action to perform</param>
        /// <param name="internalActionType">The type of internal action to perform</param>
        /// <param name="args">The arguments of the action to perform</param>
        /// <param name="coord">The map coordinate to perform the actions on</param>
        /// <param name="item">If this is not null, coordinates will be ignored and mapitem will instead handle the object</param>
        /// <param name="destroy">Whether to destroy the component after we're done</param>
        /// <returns></returns>
        bool HandleClick(int x, int y, MouseActionEnum mouseAction, out ActionType? actionType,out InternalActionEnum? internalActionType, out object[] args, out MapItem item, out MapCoordinate coord, out bool destroy);

        /// <summary>
        /// Triggers when the mouse is over the component.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void HandleMouseOver(int x, int y);

        /// <summary>
        /// Handle the keyboard being pressed
        /// Return true if it was handled, false if you want something else to handle it
        /// </summary>
        /// <param name="keyboard">The state of the keyboard</param>
        /// <param name="actionType">The action type to perform - may be null</param>
        /// <param name="args">A list of arguments the action takes</param>
        /// <param name="coord">The coordinate to perform the action on</param>
        /// <param name="destroy">Whether to destroy the component after we're done</param>
        /// <returns></returns>
        bool HandleKeyboard(KeyboardState keyboard, out ActionType? actionType, out object[] args, out MapCoordinate coord, out bool destroy);
        /// <summary>
        /// Returns the location of the component
        /// </summary>
        /// <returns></returns>
        Rectangle ReturnLocation();

        /// <summary>
        /// What happens when there is an attempt to drag the component. Could be ignored.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        void PerformDrag(int deltaX, int deltaY);

        /// <summary>
        /// Determines whether the current component is modal (blocks all interface controls or not)
        /// </summary>
        /// <returns></returns>
        bool IsModal();

        /// <summary>
        /// Whether the control is visible
        /// </summary>
        bool Visible { get; set; }
    }
}
