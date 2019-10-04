﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///   Provides data for the <see cref="TaskDialog.Closing"/> event.
    /// </summary>
    public class TaskDialogClosingEventArgs : CancelEventArgs
    {
        /// <summary>
        ///   Initializes a new instance of the <see cref="TaskDialogClosingEventArgs"/> class.
        /// </summary>
        internal TaskDialogClosingEventArgs(TaskDialogButton closeButton)
        {
            CloseButton = closeButton;
        }

        /// <summary>
        ///   Gets the <see cref="TaskDialogButton"/> that is causing the task dialog
        ///   to close.
        /// </summary>
        /// <value>
        ///   The <see cref="TaskDialogButton"/> that is causing the task dialog
        ///   to close.
        /// </value>
        public TaskDialogButton CloseButton
        {
            get;
        }
    }
}
