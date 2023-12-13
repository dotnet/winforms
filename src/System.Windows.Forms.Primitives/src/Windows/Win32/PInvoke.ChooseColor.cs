// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;

namespace Windows.Win32;

internal static partial class PInvoke
{
    [DllImport("COMDLG32.dll", ExactSpelling = true, EntryPoint = "ChooseColorW")]
    public static extern unsafe BOOL ChooseColor(CHOOSECOLORW* param0);
}
