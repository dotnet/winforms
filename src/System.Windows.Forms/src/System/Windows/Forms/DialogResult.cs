// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;



    /// <devdoc>
    ///    <para>
    ///       Specifies identifiers to
    ///       indicate the return value of a dialog box.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum DialogResult{

        /// <devdoc>
        ///    <para>
        ///       
        ///       Nothing is returned from the dialog box. This
        ///       means that the modal dialog continues running.
        ///       
        ///    </para>
        /// </devdoc>
        None = 0,

        /// <devdoc>
        ///    <para>
        ///       The
        ///       dialog box return value is
        ///       OK (usually sent from a button labeled OK).
        ///       
        ///    </para>
        /// </devdoc>
        OK = 1,

        /// <devdoc>
        ///    <para>
        ///       The
        ///       dialog box return value is Cancel (usually sent
        ///       from a button labeled Cancel).
        ///       
        ///    </para>
        /// </devdoc>
        Cancel = 2,

        /// <devdoc>
        ///    <para>
        ///       The dialog box return value is
        ///       Abort (usually sent from a button labeled Abort).
        ///       
        ///    </para>
        /// </devdoc>
        Abort = 3,

        /// <devdoc>
        ///    <para>
        ///       The dialog box return value is
        ///       Retry (usually sent from a button labeled Retry).
        ///       
        ///    </para>
        /// </devdoc>
        Retry = 4,

        /// <devdoc>
        ///    <para>
        ///       The
        ///       dialog box return value is Ignore (usually sent
        ///       from a button labeled Ignore).
        ///       
        ///    </para>
        /// </devdoc>
        Ignore = 5,

        /// <devdoc>
        ///    <para>
        ///       The dialog box return value is
        ///       Yes (usually sent from a button labeled Yes).
        ///       
        ///    </para>
        /// </devdoc>
        Yes = 6,

        /// <devdoc>
        ///    <para>
        ///       The dialog box return value is
        ///       No (usually sent from a button labeled No).
        ///       
        ///    </para>
        /// </devdoc>
        No = 7,

    }
}
