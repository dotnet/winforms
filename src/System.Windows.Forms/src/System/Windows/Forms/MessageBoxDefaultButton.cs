// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.User32;

namespace System.Windows.Forms
{
    public enum MessageBoxDefaultButton
    {
        /// <summary>
        ///  Specifies that the first button on the message box should be the
        ///  default button.
        /// </summary>
        Button1 = (int)MB.DEFBUTTON1,

        /// <summary>
        ///  Specifies that the second button on the message box should be the
        ///  default button.
        /// </summary>
        Button2 = (int)MB.DEFBUTTON2,

        /// <summary>
        ///  Specifies that the third button on the message box should be the
        ///  default button.
        /// </summary>
        Button3 = (int)MB.DEFBUTTON3,
    }
}
