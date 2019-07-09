// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref="TaskDialogButton.Click"/> event.
    /// </summary>
    public class TaskDialogButtonClickedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='TaskDialogButtonClickedEventArgs'/> class.
        /// </summary>
        internal TaskDialogButtonClickedEventArgs()
        {
        }

        /// <summary>
        /// Gets or sets a value that indicates if the dialog should not be closed
        /// after the event handler returns.
        /// </summary>
        /// <value>
        /// <see langword="true"/> to indicate that the dialog should stay open after the event handler
        /// returns; <see langword="false"/> to indicate that the dialog should close.
        /// The default value is <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// When you don't set this property to <see langword="true"/>, the
        /// <see cref="TaskDialog.Closing"/> event will occur afterwards.
        /// </remarks>
        public bool CancelClose
        {
            get;
            set;
        }
    }
}
