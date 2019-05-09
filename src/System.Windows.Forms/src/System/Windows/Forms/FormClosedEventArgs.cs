// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.Form.OnClosed'/> event.
    /// </devdoc>
    public class FormClosedEventArgs : EventArgs
    {
        public FormClosedEventArgs(CloseReason closeReason)
        {
            CloseReason = closeReason;
        }

        /// <devdoc>
        /// Provides the reason for the Form Close.
        /// </devdoc>
        public CloseReason CloseReason { get; }
    }
}
