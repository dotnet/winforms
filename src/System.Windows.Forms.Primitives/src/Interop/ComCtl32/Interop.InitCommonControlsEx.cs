// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class ComCtl32
    {
        [DllImport(Libraries.Comctl32, ExactSpelling = true, EntryPoint = "InitCommonControlsEx")]
        private static extern BOOL InitCommonControlsExInternal(ref INITCOMMONCONTROLSEX picce);

        public static BOOL InitCommonControlsEx(ref INITCOMMONCONTROLSEX picce)
        {
            picce.dwSize = (uint)Marshal.SizeOf<INITCOMMONCONTROLSEX>();
            return InitCommonControlsExInternal(ref picce);
        }

        public struct INITCOMMONCONTROLSEX
        {
            public uint dwSize;
            public ICC dwICC;
        }
    }
}
