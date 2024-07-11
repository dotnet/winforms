// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    [DllImport("COMDLG32.dll", ExactSpelling = true, EntryPoint = "PrintDlgExW")]
    internal static extern unsafe HRESULT PrintDlgEx(PRINTDLGEXW* pPD);
}
