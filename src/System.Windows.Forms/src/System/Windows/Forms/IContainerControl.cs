// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides functionality for a control to parent other controls.
    /// </devdoc>
    public interface IContainerControl
    {
        /// <summary>
        /// Indicates the control that is currently active on the container control.
        /// </devdoc>
        Control ActiveControl { get; set; }

        /// <summary>
        /// Activates the specified control.
        /// </devdoc>
        bool ActivateControl(Control active);
    }
}
