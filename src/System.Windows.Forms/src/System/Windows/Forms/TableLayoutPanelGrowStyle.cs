// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\TableLayoutPanelGrowStyle.uex' path='docs/doc[@for="TableLayoutPanelGrowStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies if a TableLayoutPanel will gain additional rows or columns once its existing cells
    ///       become full.  If the value is 'None' then the TableLayoutPanel will throw an exception
    ///       when the TableLayoutPanel is over-filled.
    ///    </para>
    /// </devdoc>
    public enum TableLayoutPanelGrowStyle {

        /// <include file='doc\TableLayoutGrowStyle.uex' path='docs/doc[@for="TableLayoutPanelGrowStyle.FixedSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The TableLayoutPanel will not allow additional rows or columns once it is full.
        ///    </para>
        /// </devdoc>
        FixedSize       = 0,

        /// <include file='doc\TableLayoutPanelGrowStyle.uex' path='docs/doc[@for="TableLayoutPanelGrowStyle.AddRows"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The TableLayoutPanel will gain additional rows once it becomes full.
        ///    </para>
        /// </devdoc>
        AddRows         = 1,

        /// <include file='doc\TableLayoutPanelGrowStyle.uex' path='docs/doc[@for="TableLayoutPanelGrowStyle.AddColumns"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The TableLayoutPanel will gain additional columns once it becomes full.
        ///    </para>
        /// </devdoc>
        AddColumns      = 2

    }
}
