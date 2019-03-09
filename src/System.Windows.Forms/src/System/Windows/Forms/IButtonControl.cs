﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Allows a control to act like a button on a form.
    /// </devdoc>
    public interface IButtonControl
    {

        /// <devdoc>
        /// Gets and sets the dialog result of the Button control. This is used as the result
        /// for the dialog on which the button is set to be an "accept" or "cancel" button.
        ///       
        /// </devdoc>
        DialogResult DialogResult { get; set; }

        /// <devdoc>
        /// Notifies a control that it is the default button so that its appearance and behavior
        /// is adjusted accordingly.
        ///       
        /// </devdoc>
        void NotifyDefault(bool value);

        /// <devdoc>
        /// Generates a <see cref='System.Windows.Forms.Control.Click'/> event for the control.
        /// </devdoc>
        void PerformClick();
    }
}
