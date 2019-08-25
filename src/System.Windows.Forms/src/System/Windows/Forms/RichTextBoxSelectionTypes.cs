// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Defines the possible kinds selection types in a RichTextBox control.
    ///  The actual vale returned by RichTextBox.getSelType() is a combination
    ///  of any of the below options.
    /// </summary>
    [Flags]
    public enum RichTextBoxSelectionTypes
    {
        /// <summary>
        ///  The current selection is empty.
        /// </summary>
        Empty = 0,

        /// <summary>
        ///  The current selection is text only.
        /// </summary>
        Text = 1,

        /// <summary>
        ///  The current selection contains atleast one OLE object.
        /// </summary>
        Object = 2,

        /// <summary>
        ///  The current selection contains more than one character.
        /// </summary>
        MultiChar = 4,

        /// <summary>
        ///  The current selection contains more than one OLE object.
        /// </summary>
        MultiObject = 8,
    }
}
