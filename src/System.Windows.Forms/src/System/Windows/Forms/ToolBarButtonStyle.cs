// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\ToolBarButtonStyle.uex' path='docs/doc[@for="ToolBarButtonStyle"]/*' />
    /// <devdoc>
    ///    <para>Specifies the button style within a toolbar.</para>
    /// </devdoc>
    public enum ToolBarButtonStyle {

        /// <include file='doc\ToolBarButtonStyle.uex' path='docs/doc[@for="ToolBarButtonStyle.PushButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A
        ///       standard, three-dimensional
        ///       button.
        ///    </para>
        /// </devdoc>
        PushButton     = 1,

        /// <include file='doc\ToolBarButtonStyle.uex' path='docs/doc[@for="ToolBarButtonStyle.ToggleButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A toggle button that appears sunken when clicked
        ///       and retains the sunken appearance until
        ///       clicked again.
        ///    </para>
        /// </devdoc>
        ToggleButton   = 2,

        /// <include file='doc\ToolBarButtonStyle.uex' path='docs/doc[@for="ToolBarButtonStyle.Separator"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A space or
        ///       line between toolbar buttons. The appearance depends on the
        ///       value of the <see cref='System.Windows.Forms.ToolBar.Appearance'/>
        ///       property.
        ///    </para>
        /// </devdoc>
        Separator      = 3,

        /// <include file='doc\ToolBarButtonStyle.uex' path='docs/doc[@for="ToolBarButtonStyle.DropDownButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       A drop down control that displays a menu or other window
        ///       when
        ///       clicked.
        ///    </para>
        /// </devdoc>
        DropDownButton = 4,

    }
}
