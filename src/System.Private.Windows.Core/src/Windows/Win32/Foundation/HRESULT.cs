// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Diagnostics.Debug;

namespace Windows.Win32.Foundation;

internal readonly partial struct HRESULT
{
    /// <summary>
    ///  Extracts the facility code of the HRESULT. [HRESULT_FACILITY]
    /// </summary>
    public FACILITY_CODE Facility
        // https://learn.microsoft.com/windows/win32/api/winerror/nf-winerror-hresult_facility
        // #define HRESULT_FACILITY(hr)  (((hr) >> 16) & 0x1fff)
        => (FACILITY_CODE)((Value >> 16) & 0x1fff);

    /// <summary>
    ///  Extracts the code portion of the HRESULT. [HRESULT_CODE]
    /// </summary>
    public int Code
        // https://learn.microsoft.com/windows/win32/api/winerror/nf-winerror-hresult_code
        // #define HRESULT_CODE(hr)    ((hr) & 0xFFFF)
        => Value & 0xFFFF;

    /// <summary>
    ///  Converts a Win32 error code into an HRESULT. [HRESULT_FROM_WIN32]
    /// </summary>
    public static HRESULT FromWin32(WIN32_ERROR error)
        // https://learn.microsoft.com/windows/win32/api/winerror/nf-winerror-hresult_from_win32
        // return (HRESULT)(x) <= 0 ? (HRESULT)(x) : (HRESULT) (((x) & 0x0000FFFF) | (FACILITY_WIN32 << 16) | 0x80000000);
        => new(((int)error & 0x0000FFFF) | unchecked((int)0x80070000));

    public static implicit operator HRESULT(Exception ex)
    {
        Debug.WriteLine(ex);
        return (HRESULT)ex.HResult;
    }

    public void AssertSuccess() => Debug.Assert(Succeeded, $"Result failed: {this}");

#pragma warning disable format
#pragma warning disable IDE1006 // Naming Styles

    // COR_* HRESULTs are .NET HRESULTs
    public static readonly HRESULT COR_E_ARGUMENT               = (HRESULT)unchecked((int)0x80070057);
    public static readonly HRESULT TLBX_E_LIBNOTREGISTERED      = (HRESULT)unchecked((int)0x80131165);
    public static readonly HRESULT COR_E_MISSINGFIELD           = (HRESULT)unchecked((int)0x80131511);
    public static readonly HRESULT COR_E_MISSINGMEMBER          = (HRESULT)unchecked((int)0x80131512);
    public static readonly HRESULT COR_E_MISSINGMETHOD          = (HRESULT)unchecked((int)0x80131513);
    public static readonly HRESULT COR_E_NOTSUPPORTED           = (HRESULT)unchecked((int)0x80131515);
    public static readonly HRESULT COR_E_OVERFLOW               = (HRESULT)unchecked((int)0x80131516);
    public static readonly HRESULT COR_E_INVALIDOLEVARIANTTYPE  = (HRESULT)unchecked((int)0x80131531);
    public static readonly HRESULT COR_E_SAFEARRAYTYPEMISMATCH  = (HRESULT)unchecked((int)0x80131533);
    public static readonly HRESULT COR_E_TARGETINVOCATION       = (HRESULT)unchecked((int)0x80131604);
    public static readonly HRESULT COR_E_OBJECTDISPOSED         = (HRESULT)unchecked((int)0x80131622);
    public static readonly HRESULT COR_E_SERIALIZATION          = (HRESULT)unchecked((int)0x8013150c);

#pragma warning restore format
#pragma warning restore IDE1006
}
