// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  The various tab controls will let you configure their appearance.
    ///  This enumeration contains the possible values.
    /// </summary>
    public enum TabAppearance
    {
        /// <summary>
        ///  Indicates that the tabs look like normal tabs typically seen in Property
        ///  page type situations.
        /// </summary>
        Normal = 0,

        /// <summary>
        ///  Indicates that the tabs look like buttons as seen on the taskbar found
        ///  in Windows 95 or Windows NT.
        /// </summary>
        Buttons = 1,

        /// <summary>
        ///  Indicates that buttons should be draw flat instead of like regular
        ///  windows pushbuttons.
        /// </summary>
        FlatButtons = 2,
    }
}
