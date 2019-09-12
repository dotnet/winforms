// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static partial class Interop
{
    internal static partial class Shlwapi
    {
        [DllImport(Libraries.Shlwapi, ExactSpelling = true)]
        public static extern HRESULT SHAutoComplete(IntPtr hwndEdit, SHACF flags);

        public static HRESULT SHAutoComplete(IHandle hwndEdit, SHACF flags)
        {
            HRESULT result = SHAutoComplete(hwndEdit.Handle, flags);
            GC.KeepAlive(hwndEdit);
            return result;
        }

        public static HRESULT SHAutoComplete(HandleRef hwndEdit, SHACF flags)
        {
            HRESULT result = SHAutoComplete(hwndEdit.Handle, flags);
            GC.KeepAlive(hwndEdit.Wrapper);
            return result;
        }
    }
}
