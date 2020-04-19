// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///   Specifies where the expanded area of the
    ///   task dialog is to be displayed.
    /// </summary>
    public enum TaskDialogExpanderPosition : int
    {
        /// <summary>
        ///   The expanded area is to be displayed immediately after the
        ///   dialog's text.
        /// </summary>
        AfterText = 0,

        /// <summary>
        ///   The expanded area is to be displayed at the bottom of the dialog's
        ///   footnote area.
        /// </summary>
        AfterFootnote = 1
    }
}
