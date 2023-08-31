// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;

namespace Windows.Win32;

// https://github.com/microsoft/win32metadata/issues/1300
internal static partial class PInvoke
{
    [DllImport("COMDLG32.dll", EntryPoint = "GetOpenFileNameW", ExactSpelling = true)]
    public static extern unsafe BOOL GetOpenFileName(OPENFILENAME* param0);

    [DllImport("COMDLG32.dll", EntryPoint = "GetSaveFileNameW", ExactSpelling = true)]
    public static extern unsafe BOOL GetSaveFileName(OPENFILENAME* param0);
}
