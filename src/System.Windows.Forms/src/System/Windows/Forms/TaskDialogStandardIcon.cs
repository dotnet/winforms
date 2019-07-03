// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Contains constants for predefined icons of a task dialog.
    /// </summary>
    public enum TaskDialogStandardIcon : int
    {
        /// <summary>
        /// The task dialog does not display an icon.
        /// </summary>
        None = 0,

        /// <summary>
        /// The task dialog contains a symbol consisting of a lowercase letter i in a circle.
        /// </summary>
        Information = ushort.MaxValue - 2, // TD_INFORMATION_ICON

        /// <summary>
        /// The task dialog contains an icon consisting of an exclamation point in a triangle with a yellow background.
        /// </summary>
        Warning = ushort.MaxValue, // TD_WARNING_ICON

        /// <summary>
        /// The task dialog contains an icon consisting of white X in a circle with a red background.
        /// </summary>
        Error = ushort.MaxValue - 1, // TD_ERROR_ICON

        /// <summary>
        /// The task dialog contains an icon consisting of an user account control (UAC) shield.
        /// </summary>
        SecurityShield = ushort.MaxValue - 3, // TD_SHIELD_ICON

        /// <summary>
        /// The task dialog contains an icon consisting of an user account control (UAC) shield and shows a blue bar around the icon.
        /// </summary>
        SecurityShieldBlueBar = ushort.MaxValue - 4,

        /// <summary>
        /// The task dialog contains an icon consisting of an user account control (UAC) shield and shows a gray bar around the icon.
        /// </summary>
        SecurityShieldGrayBar = ushort.MaxValue - 8,

        /// <summary>
        /// The task dialog contains an icon consisting of an exclamation point in a yellow shield and shows a yellow bar around the icon.
        /// </summary>
        SecurityWarningYellowBar = ushort.MaxValue - 5,

        /// <summary>
        /// The task dialog contains an icon consisting of white X in a red shield and shows a red bar around the icon.
        /// </summary>
        SecurityErrorRedBar = ushort.MaxValue - 6,

        /// <summary>
        /// The task dialog contains an icon consisting of white tick in a green shield and shows a green bar around the icon.
        /// </summary>
        SecuritySuccessGreenBar = ushort.MaxValue - 7,
    }
}
