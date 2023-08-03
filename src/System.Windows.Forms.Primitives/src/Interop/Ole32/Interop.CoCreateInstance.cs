// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern HRESULT CoCreateInstance(
            in Guid rclsid,
            IntPtr punkOuter,
            CLSCTX dwClsContext,
            in Guid riid,
            [MarshalAs(UnmanagedType.Interface)] out object ppv);
    }
}
