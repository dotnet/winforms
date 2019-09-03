// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        [Flags]
        public enum VarFormatFlags : uint
        {
            VAR_CALENDAR_HIJRI = 0x008,
            VAR_FORMAT_NOSUBSTITUTE = 0x020,
        }

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        public static extern HRESULT VarFormat(
            IntPtr pvarIn,
            IntPtr pstrFormat,
            Ole32.VarFormatFirstDayOfWeek iFirstDay,
            Ole32.VarFormatFirstWeekOfYear iFirstWeek,
            VarFormatFlags dwFlags,
            ref IntPtr pbstrOut);
    }
}
