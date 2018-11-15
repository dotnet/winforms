// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;



    /// <include file='doc\DialogResult.uex' path='docs/doc[@for="DialogResult"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies identifiers to
    ///       indicate the return value of a dialog box.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public enum DialogResult{

        /// <include file='doc\DialogResult.uex' path='docs/doc[@for="DialogResult.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       
        ///       Nothing is returned from the dialog box. This
        ///       means that the modal dialog continues running.
        ///       
        ///    </para>
        /// </devdoc>
        None = 0,

        /// <include file='doc\DialogResult.uex' path='docs/doc[@for="DialogResult.OK"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The
        ///       dialog box return value is
        ///       OK (usually sent from a button labeled OK).
        ///       
        ///    </para>
        /// </devdoc>
        OK = 1,

        /// <include file='doc\DialogResult.uex' path='docs/doc[@for="DialogResult.Cancel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The
        ///       dialog box return value is Cancel (usually sent
        ///       from a button labeled Cancel).
        ///       
        ///    </para>
        /// </devdoc>
        Cancel = 2,

        /// <include file='doc\DialogResult.uex' path='docs/doc[@for="DialogResult.Abort"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The dialog box return value is
        ///       Abort (usually sent from a button labeled Abort).
        ///       
        ///    </para>
        /// </devdoc>
        Abort = 3,

        /// <include file='doc\DialogResult.uex' path='docs/doc[@for="DialogResult.Retry"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The dialog box return value is
        ///       Retry (usually sent from a button labeled Retry).
        ///       
        ///    </para>
        /// </devdoc>
        Retry = 4,

        /// <include file='doc\DialogResult.uex' path='docs/doc[@for="DialogResult.Ignore"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The
        ///       dialog box return value is Ignore (usually sent
        ///       from a button labeled Ignore).
        ///       
        ///    </para>
        /// </devdoc>
        Ignore = 5,

        /// <include file='doc\DialogResult.uex' path='docs/doc[@for="DialogResult.Yes"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The dialog box return value is
        ///       Yes (usually sent from a button labeled Yes).
        ///       
        ///    </para>
        /// </devdoc>
        Yes = 6,

        /// <include file='doc\DialogResult.uex' path='docs/doc[@for="DialogResult.No"]/*' />
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
