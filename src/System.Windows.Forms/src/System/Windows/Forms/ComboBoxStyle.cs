// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the <see cref='System.Windows.Forms.ComboBox'/> style.
    /// </devdoc>
    public enum ComboBoxStyle
    {
        /// <devdoc>
        /// The text portion is editable. The list portion is always visible.
        /// </devdoc>
        Simple = 0,

        /// <devdoc>
        /// The text portion is editable. The user must click the arrow button to
        /// display the list portion.
        /// </devdoc>
        DropDown = 1,

        /// <devdoc>
        /// The user cannot directly edit the text portion. The user must click
        /// the arrow button to display the list portion.
        /// </devdoc>
        DropDownList = 2,
    }
}
