// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    

    /// <devdoc>
    ///    <para>
    ///       Specifies the autocomplete mode for ComboBox and TextBox AutoComplete Feature.
    ///    </para>
    /// </devdoc>
    public enum AutoCompleteMode {


        /// <devdoc>
        ///    <para>
        ///       Disables the AutoComplete Feature for ComboBox and TextBox.
        ///    </para>
        /// </devdoc>
        None = 0,


        /// <devdoc>
        ///    <para>
        ///       Displays the auxiliary drop-down list associated with the edit control, this drop-down is populated 
        ///       with one or more suggested completed strings.
        ///    </para>
        /// </devdoc>
        Suggest = 0x1,


        /// <devdoc>
        ///    <para>
        ///       Appends the remainder of the most likely candidate string to the existing characters,
        ///       hightlighting the appended characters.
        ///    </para>
        /// </devdoc>
        Append = 0x2,


        /// <devdoc>
        ///    <para>
        ///       The AutoSuggest and AutoAppend are applied in conjuction.
        ///    </para>
        /// </devdoc>
        SuggestAppend = Suggest | Append
    }
}

