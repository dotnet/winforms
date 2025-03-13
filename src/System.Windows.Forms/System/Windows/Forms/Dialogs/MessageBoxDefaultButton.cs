// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public enum MessageBoxDefaultButton
{
    /// <summary>
    ///  Specifies that the first button on the message box should be the default button.
    /// </summary>
    Button1 = (int)MESSAGEBOX_STYLE.MB_DEFBUTTON1,

    /// <summary>
    ///  Specifies that the second button on the message box should be the default button.
    /// </summary>
    Button2 = (int)MESSAGEBOX_STYLE.MB_DEFBUTTON2,

    /// <summary>
    ///  Specifies that the third button on the message box should be the default button.
    /// </summary>
    Button3 = (int)MESSAGEBOX_STYLE.MB_DEFBUTTON3,

    /// <summary>
    ///  Specifies that the Help button on the message box should be the default button.
    /// </summary>
    Button4 = (int)MESSAGEBOX_STYLE.MB_DEFBUTTON4,
}
