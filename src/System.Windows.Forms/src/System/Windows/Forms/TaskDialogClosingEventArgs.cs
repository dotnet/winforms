// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskDialogClosingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        internal TaskDialogClosingEventArgs(TaskDialogButton closeButton)
            : base()
        {
            CloseButton = closeButton;
        }

        /// <summary>
        /// Gets the <see cref="TaskDialogButton"/> that is causing the task dialog
        /// to close.
        /// </summary>
        public TaskDialogButton CloseButton
        {
            get;
        }
    }
}
