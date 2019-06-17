// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public enum TaskDialogStandardIcon : int
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        Information = ushort.MaxValue - 2, // TD_INFORMATION_ICON

        /// <summary>
        /// 
        /// </summary>
        Warning = ushort.MaxValue, // TD_WARNING_ICON

        /// <summary>
        /// 
        /// </summary>
        Error = ushort.MaxValue - 1, // TD_ERROR_ICON

        /// <summary>
        /// 
        /// </summary>
        SecurityShield = ushort.MaxValue - 3, // TD_SHIELD_ICON

        /// <summary>
        /// 
        /// </summary>
        SecurityShieldBlueBar = ushort.MaxValue - 4,

        /// <summary>
        /// 
        /// </summary>
        SecurityShieldGrayBar = ushort.MaxValue - 8,

        /// <summary>
        /// 
        /// </summary>
        SecurityWarningYellowBar = ushort.MaxValue - 5,

        /// <summary>
        /// 
        /// </summary>
        SecurityErrorRedBar = ushort.MaxValue - 6,

        /// <summary>
        /// 
        /// </summary>
        SecuritySuccessGreenBar = ushort.MaxValue - 7,
    }
}
