// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the autocomplete mode for ComboBox and TextBox AutoComplete Feature.
    /// </summary>
    public enum AutoCompleteMode
    {
        /// <summary>
        ///  Disables the AutoComplete Feature for ComboBox and TextBox.
        /// </summary>
        None = (int)Shell32.AUTOCOMPLETEOPTIONS.NONE,

        /// <summary>
        ///  Displays the auxiliary drop-down list associated with the edit control,
        ///  this drop-down is populated with one or more suggested completed strings.
        /// </summary>
        Suggest = (int)Shell32.AUTOCOMPLETEOPTIONS.AUTOSUGGEST,

        /// <summary>
        ///  Appends the remainder of the most likely candidate string to the existing
        ///  characters, hightlighting the appended characters.
        /// </summary>
        Append = (int)Shell32.AUTOCOMPLETEOPTIONS.AUTOAPPEND,

        /// <summary>
        ///  The AutoSuggest and AutoAppend are applied in conjuction.
        /// </summary>
        SuggestAppend = Suggest | Append
    }
}
