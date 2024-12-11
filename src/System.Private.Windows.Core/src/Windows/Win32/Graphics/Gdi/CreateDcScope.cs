// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Helper to scope lifetime of an HDC retrieved via CreateDC/CreateCompatibleDC.
///  Deletes the HDC (if any) when disposed.
/// </summary>
/// <remarks>
///  <para>
///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
///  by <see langword="ref" /> to avoid duplicating the handle and risking a double delete.
///  </para>
/// </remarks>
#if DEBUG
internal class CreateDcScope : DisposalTracking.Tracker, IDisposable
#else
internal readonly ref struct CreateDcScope
#endif
{
    public HDC HDC { get; }

    /// <summary>
    ///  Creates a compatible HDC for <paramref name="hdc"/> using <see cref="PInvokeCore.CreateCompatibleDC(HDC)"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Passing a <see langword="null"/> HDC will use the current screen.
    ///  </para>
    /// </remarks>
    public CreateDcScope(HDC hdc)
    {
        HDC = PInvokeCore.CreateCompatibleDC(hdc);
    }

    public unsafe CreateDcScope(
        string driverName,
        string? deviceName = null,
        DEVMODEW* lpInitData = default,
        bool informationOnly = true)
    {
        fixed (char* driver = driverName)
        fixed (char* device = deviceName)
        {
            HDC = informationOnly
                ? PInvokeCore.CreateICW(driver, device, null, lpInitData)
                : PInvokeCore.CreateDCW(driver, device, null, lpInitData);
        }
    }

    public static implicit operator HDC(in CreateDcScope scope) => scope.HDC;
    public static unsafe implicit operator HGDIOBJ(in CreateDcScope scope) => (HGDIOBJ)(scope.HDC.Value);
    public static implicit operator nint(in CreateDcScope scope) => scope.HDC;
    public static explicit operator WPARAM(in CreateDcScope scope) => (WPARAM)(nuint)(nint)scope.HDC;

    public bool IsNull => HDC.IsNull;

    public void Dispose()
    {
        if (!HDC.IsNull)
        {
            PInvokeCore.DeleteDC(HDC);
        }

#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }
}
