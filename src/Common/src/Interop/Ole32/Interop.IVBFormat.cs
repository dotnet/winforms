// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Ole32
    {
        [ComImport]
        [Guid("9849FD60-3768-101B-8D72-AE6164FFE3CF")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public unsafe interface IVBFormat
        {
            [PreserveSig]
            HRESULT Format(
                IntPtr vData,
                IntPtr pszFormat,
                IntPtr lpBuffer,
                ushort cb,
                int lcid,
                VarFormatFirstDayOfWeek sFirstDayOfWeek,
                VarFormatFirstWeekOfYear sFirstWeekOfYear,
                ushort* rcb);
        }
    }
}
