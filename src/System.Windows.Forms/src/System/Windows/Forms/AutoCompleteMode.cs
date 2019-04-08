// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the autocomplete mode for ComboBox and TextBox AutoComplete Feature.
    /// </devdoc>
    public enum AutoCompleteMode
    {
        /// <devdoc>
        /// Disables the AutoComplete Feature for ComboBox and TextBox.
        /// </devdoc>
        None = 0,

        /// <devdoc>
        /// Displays the auxiliary drop-down list associated with the edit control,
        /// this drop-down is populated with one or more suggested completed strings.
        /// </devdoc>
        Suggest = 0x1,

        /// <devdoc>
        /// Appends the remainder of the most likely candidate string to the existing
        /// characters, hightlighting the appended characters.
        /// </devdoc>
        Append = 0x2,

        /// <devdoc>
        /// The AutoSuggest and AutoAppend are applied in conjuction.
        /// </devdoc>
        SuggestAppend = Suggest | Append
    }
}
