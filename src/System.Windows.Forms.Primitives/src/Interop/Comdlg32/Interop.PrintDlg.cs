// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Comdlg32
    {
        [DllImport(Libraries.Comdlg32, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern BOOL PrintDlgW(ref PRINTDLGW_32 lppd);

        [DllImport(Libraries.Comdlg32, ExactSpelling = true, SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern BOOL PrintDlgW(ref PRINTDLGW_64 lppd);

        public static BOOL PrintDlg(ref PRINTDLGW lppd)
        {
            if (IntPtr.Size == 4)
            {
                if (lppd is PRINTDLGW_32 lppd32)
                {
                    BOOL result = PrintDlgW(ref lppd32);
                    lppd = lppd32;
                    return result;
                }

                throw new InvalidOperationException($"Expected {nameof(PRINTDLGW_32)} data struct");
            }

            if (lppd is PRINTDLGW_64 lppd64)
            {
                BOOL result = PrintDlgW(ref lppd64);
                lppd = lppd64;
                return result;
            }

            throw new InvalidOperationException($"Expected {nameof(PRINTDLGW_64)} data struct");
        }
    }
}
