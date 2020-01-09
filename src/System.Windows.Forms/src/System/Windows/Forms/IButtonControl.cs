// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    /// <summary>
    ///  Allows a control to act like a button on a form.
    /// </summary>
    public interface IButtonControl
    {
        /// <summary>
        ///  Gets and sets the dialog result of the Button control. This is used as the result
        ///  for the dialog on which the button is set to be an "accept" or "cancel" button.
        /// </summary>
        DialogResult DialogResult { get; set; }

        /// <summary>
        ///  Notifies a control that it is the default button so that its appearance and behavior
        ///  is adjusted accordingly.
        /// </summary>
        void NotifyDefault(bool value);

        /// <summary>
        ///  Generates a <see cref='Control.Click'/> event for the control.
        /// </summary>
        void PerformClick();
    }
}
