// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.Form.OnClosing'/> event.
    /// </devdoc>
    public class FormClosingEventArgs : CancelEventArgs
    {
        public FormClosingEventArgs(CloseReason closeReason, bool cancel) : base(cancel)
        {
            CloseReason = closeReason;                                           
        }

        /// <devdoc>
        /// Provides the reason for the Form close.
        /// </devdoc>
        public CloseReason CloseReason { get; }
    }
}
