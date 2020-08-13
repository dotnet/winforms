// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The event handler service provides a unified way to handle
    ///  the various events that our form designer must process.  What
    ///  we want to be able to do is to write code in one place that
    ///  handles events of a certain type.  We also may need to globally
    ///  change the behavior of these events for modal functions like
    ///  the tab order UI.  Our designer, however, is in many pieces
    ///  so we must somehow funnel these events to a common place.
    ///  This service implements an "event stack" that contains the
    ///  current set of event handlers.  There can be different
    ///  types of handlers on the stack.  For example, we may push
    ///  a keyboard handler and a mouse handler.  When you request
    ///  a handler, we will find the topmost handler on the stack
    ///  that fits the class you requested.  This way the service
    ///  can be extended to any eventing scheme, and it also allows
    ///  sections of a handler to be replaced (eg, you can replace
    ///  mouse handling without effecting menus or the keyboard).
    /// </summary>
    internal interface IEventHandlerService
    {
        /// <summary>
            ///  Gets the control that handles focus changes
        ///  for this event handler service.
            /// </summary>
        Control FocusWindow { get; }

        event EventHandler EventHandlerChanged;

        /// <summary>
        ///  Gets the currently active event handler of the specified type.
        /// </summary>
        object GetHandler(Type handlerType);

        /// <summary>
            ///  Pops
        ///  the given handler off of the stack.
            /// </summary>
        void PopHandler(object handler);

        /// <summary>
        ///  Pushes a new event handler on the stack.
        /// </summary>
        void PushHandler(object handler);
    }
}
