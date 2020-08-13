// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Form.OnClosed'/> event.
    /// </summary>
    public class FormClosedEventArgs : EventArgs
    {
        public FormClosedEventArgs(CloseReason closeReason)
        {
            CloseReason = closeReason;
        }

        /// <summary>
        ///  Provides the reason for the Form Close.
        /// </summary>
        public CloseReason CloseReason { get; }
    }
}
