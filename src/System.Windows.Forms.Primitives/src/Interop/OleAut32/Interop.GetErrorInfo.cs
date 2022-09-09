// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [DllImport(Libraries.Oleaut32)]
        private static extern HRESULT GetErrorInfo(uint dwReserved, out IntPtr pperrinfo);

        public static void GetErrorInfo(out WinFormsComWrappers.ErrorInfoWrapper? errinfo)
        {
            HRESULT result = GetErrorInfo(0, out IntPtr pperrinfo);
            errinfo = null;
            if (result.Succeeded && pperrinfo != IntPtr.Zero)
            {
                errinfo = (WinFormsComWrappers.ErrorInfoWrapper)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(pperrinfo, CreateObjectFlags.Unwrap);
            }
        }
    }
}
