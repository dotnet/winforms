// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\StatusBarPanelAutoSize.uex' path='docs/doc[@for="StatusBarPanelAutoSize"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies how a panel on a status bar changes when the
    ///       status bar resizes.
    ///    </para>
    /// </devdoc>
    public enum StatusBarPanelAutoSize {

        /// <include file='doc\StatusBarPanelAutoSize.uex' path='docs/doc[@for="StatusBarPanelAutoSize.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The panel does not change
        ///       its size when the status bar resizes.
        ///    </para>
        /// </devdoc>
        None        = 1,

        /// <include file='doc\StatusBarPanelAutoSize.uex' path='docs/doc[@for="StatusBarPanelAutoSize.Spring"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The panel shares the available status bar space (the
        ///       space not taken up by panels with the <see langword='None'/> and
        ///    <see langword='Contents'/> settings) with other panels that have the 
        ///    <see langword='Spring'/>
        ///    setting.
        /// </para>
        /// </devdoc>
        Spring      = 2,

        /// <include file='doc\StatusBarPanelAutoSize.uex' path='docs/doc[@for="StatusBarPanelAutoSize.Contents"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The width of the panel is determined by its contents.
        ///    </para>
        /// </devdoc>
        Contents    = 3,

    }
}
