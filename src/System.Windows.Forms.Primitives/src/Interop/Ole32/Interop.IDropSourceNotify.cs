﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("0000012B-0000-0000-C000-000000000046")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDropSourceNotify
        {
            [PreserveSig]
            HRESULT DragEnterTarget(
                IntPtr hwndTarget);

            [PreserveSig]
            HRESULT DragLeaveTarget();
        }
    }
}
