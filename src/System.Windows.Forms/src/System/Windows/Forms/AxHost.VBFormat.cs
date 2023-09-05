// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    private unsafe class VBFormat : IVBFormat.Interface, IManagedWrapper<IVBFormat>
    {
        HRESULT IVBFormat.Interface.Format(
            VARIANT* vData,
            BSTR bstrFormat,
            void* lpBuffer,
            ushort cb,
            int lcid,
            short sFirstDayOfWeek,
            ushort sFirstWeekOfYear,
            ushort* rcb)
        {
            if (rcb is null)
            {
                return HRESULT.E_INVALIDARG;
            }

            *rcb = 0;
            if (lpBuffer == null || cb < sizeof(char))
            {
                return HRESULT.E_INVALIDARG;
            }

            using BSTR pbstr = default;

            const int VAR_FORMAT_NOSUBSTITUTE = 0x00000020;
            HRESULT hr = PInvoke.VarFormat(
                vData,
                bstrFormat.Value,
                (VARFORMAT_FIRST_DAY)sFirstDayOfWeek,
                (VARFORMAT_FIRST_WEEK)sFirstWeekOfYear,
                VAR_FORMAT_NOSUBSTITUTE,
                &pbstr);

            if (hr.Failed)
            {
                return hr;
            }

            Span<char> buffer = new(lpBuffer, cb / sizeof(char));
            ReadOnlySpan<char> format = pbstr.AsSpan();
            if (format.Length >= buffer.Length)
            {
                // Only want to copy what will fit and leave room for a null terminator.
                format = format[..(buffer.Length - 1)];
            }

            format.CopyTo(buffer);
            buffer[format.Length] = '\0';
            *rcb = (ushort)(format.Length * sizeof(char));

            return HRESULT.S_OK;
        }
    }
}
