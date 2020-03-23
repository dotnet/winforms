// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the <see cref='ComboBox'/> style.
    /// </summary>
    public enum ComboBoxStyle
    {
        /// <summary>
        ///  The text portion is editable. The list portion is always visible.
        /// </summary>
        Simple = 0,

        /// <summary>
        ///  The text portion is editable. The user must click the arrow button to
        ///  display the list portion.
        /// </summary>
        DropDown = 1,

        /// <summary>
        ///  The user cannot directly edit the text portion. The user must click
        ///  the arrow button to display the list portion.
        /// </summary>
        DropDownList = 2,
    }
}
