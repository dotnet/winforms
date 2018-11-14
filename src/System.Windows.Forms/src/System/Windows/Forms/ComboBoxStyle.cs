// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\ComboBoxStyle.uex' path='docs/doc[@for="ComboBoxStyle"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the <see cref='System.Windows.Forms.ComboBox'/>
    ///       style.
    ///
    ///    </para>
    /// </devdoc>
    public enum ComboBoxStyle {

        /// <include file='doc\ComboBoxStyle.uex' path='docs/doc[@for="ComboBoxStyle.Simple"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The text portion is editable. The list portion is
        ///       always visible.
        ///
        ///    </para>
        /// </devdoc>
        Simple       = 0,

        /// <include file='doc\ComboBoxStyle.uex' path='docs/doc[@for="ComboBoxStyle.DropDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///
        ///       The text portion is editable. The user must click the arrow button to display
        ///       the list portion.
        ///
        ///    </para>
        /// </devdoc>
        DropDown     = 1,

        /// <include file='doc\ComboBoxStyle.uex' path='docs/doc[@for="ComboBoxStyle.DropDownList"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The
        ///       user cannot directly edit the text portion. The user must click the arrow button to
        ///       display the list portion.
        ///
        ///    </para>
        /// </devdoc>
        DropDownList = 2,

    }
}
