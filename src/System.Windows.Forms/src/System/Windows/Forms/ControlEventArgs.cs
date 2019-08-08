// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  A ControlEventArgs is an event that has a control as a property.
    /// </summary>
    public class ControlEventArgs : EventArgs
    {
        /// <summary>
        ///  Creates a new ControlEventArgs.
        /// </summary>
        public ControlEventArgs(Control control)
        {
            Control = control;
        }

        /// <summary>
        ///  Retrieves the control object stored in this event.
        /// </summary>
        public Control Control { get; }
    }
}
