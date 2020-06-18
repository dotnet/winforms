// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        private class VBFormat : Ole32.IVBFormat
        {
            unsafe HRESULT Ole32.IVBFormat.Format(
                IntPtr vData,
                IntPtr bstrFormat,
                IntPtr lpBuffer,
                ushort cb,
                int lcid,
                Ole32.VarFormatFirstDayOfWeek sFirstDayOfWeek,
                Ole32.VarFormatFirstWeekOfYear sFirstWeekOfYear,
                ushort* rcb)
            {
                Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in Format");
                if (rcb is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                *rcb = 0;
                if (lpBuffer == IntPtr.Zero || cb < 2)
                {
                    return HRESULT.E_INVALIDARG;
                }

                IntPtr pbstr = IntPtr.Zero;
                HRESULT hr = Oleaut32.VarFormat(
                    vData,
                    bstrFormat,
                    sFirstDayOfWeek,
                    sFirstWeekOfYear,
                    Oleaut32.VarFormatOptions.FORMAT_NOSUBSTITUTE,
                    ref pbstr);
                try
                {
                    ushort i = 0;
                    if (pbstr != IntPtr.Zero)
                    {
                        short ch = 0;
                        cb--;
                        for (; i < cb && (ch = Marshal.ReadInt16(pbstr, i * 2)) != 0; i++)
                        {
                            Marshal.WriteInt16(lpBuffer, i * 2, ch);
                        }
                    }
                    Marshal.WriteInt16(lpBuffer, i * 2, (short)0);
                    *rcb = i;
                }
                finally
                {
                    Oleaut32.SysFreeString(pbstr);
                }

                return HRESULT.S_OK;
            }
        }
    }
}
