// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.Dialogs;

namespace Windows.Win32
{
    // https://github.com/microsoft/win32metadata/issues/1300
    internal static partial class PInvoke
    {
        [DllImport("COMDLG32.dll", EntryPoint = "GetOpenFileNameW", ExactSpelling = true, PreserveSig = false)]
        public static extern unsafe BOOL GetOpenFileName([In][Out] OPENFILENAME* param0);

        [DllImport("COMDLG32.dll", EntryPoint = "GetSaveFileNameW", ExactSpelling = true, PreserveSig = false)]
        public static extern unsafe BOOL GetSaveFileName([In][Out] OPENFILENAME* param0);
    }
}
