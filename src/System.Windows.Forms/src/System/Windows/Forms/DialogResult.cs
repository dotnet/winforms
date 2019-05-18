﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies identifiers to indicate the return value of a dialog box.
    /// </devdoc>
    [ComVisible(true)]
    public enum DialogResult
    {
        /// <summary>
        /// Nothing is returned from the dialog box. This means that the modal
        /// dialog continues running.
        /// </devdoc>
        None = 0,

        /// <summary>
        /// The dialog box return value is OK (usually sent from a button labeled OK).
        /// </devdoc>
        OK = 1,

        /// <summary>
        /// The dialog box return value is Cancel (usually sent from a button
        /// labeled Cancel).
        /// </devdoc>
        Cancel = 2,

        /// <summary>
        /// The dialog box return value is Abort (usually sent from a button
        /// labeled Abort).
        /// </devdoc>
        Abort = 3,

        /// <summary>
        /// The dialog box return value is Retry (usually sent from a button
        /// labeled Retry).
        /// </devdoc>
        Retry = 4,

        /// <summary>
        /// The dialog box return value is Ignore (usually sent from a button
        /// labeled Ignore).
        /// </devdoc>
        Ignore = 5,

        /// <summary>
        /// The dialog box return value is Yes (usually sent from a button
        /// labeled Yes).
        /// </devdoc>
        Yes = 6,

        /// <summary>
        /// The dialog box return value is No (usually sent from a button
        /// labeled No).
        /// </devdoc>
        No = 7,
    }
}
