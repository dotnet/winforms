// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the position that a task dialog will be shown in when it is first opened.
    /// </summary>
    public enum TaskDialogStartupLocation
    {
        /// <summary>
        ///   The startup location of the task dialog is the center of the screen.
        /// </summary>
        CenterScreen = 0,

        /// <summary>
        ///   The startup location of the task dialog is the center of the window that owns it,
        ///   as specified by the <c>owner</c> parameter.
        /// </summary>
        /// <remarks>
        /// <para>
        ///   This value will only have an effect when showing a modal dialog.
        /// </para>
        /// </remarks>
        CenterOwner = 1
    }
}
