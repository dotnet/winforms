// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms;

internal static class UnsafeNativeMethods
{
    [DllImport(Libraries.Comdlg32, SetLastError = true, CharSet = CharSet.Auto)]
    public static extern HRESULT PrintDlgEx([In, Out] NativeMethods.PRINTDLGEX lppdex);
}
