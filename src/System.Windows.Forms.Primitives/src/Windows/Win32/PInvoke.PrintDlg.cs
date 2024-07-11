// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;

namespace Windows.Win32;

internal static partial class PInvoke
{
    [DllImport("COMDLG32.dll", ExactSpelling = true, EntryPoint = "PrintDlgW")]
    internal static extern unsafe BOOL PrintDlg(PRINTDLGW_64* pPD);

    internal static unsafe BOOL PrintDlg(PRINTDLGW_32* pPD)
    {
        Debug.Assert(RuntimeInformation.ProcessArchitecture == Architecture.X86);
        return PrintDlg((PRINTDLGW_64*)pPD);
    }
}
