// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Helper to scope selecting a given foreground mix mode into a HDC. Restores the original mix mode into the
///  HDC when disposed.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass by
///   <see langword="ref" /> to avoid duplicating the handle and resetting multiple times.
///  </para>
/// </remarks>
#if DEBUG
internal class SetRop2Scope : DisposalTracking.Tracker, IDisposable
#else
internal readonly ref struct SetRop2Scope
#endif
{
    private readonly R2_MODE _previousRop;
    private readonly HDC _hdc;

    /// <summary>
    ///  Selects <paramref name="rop2"/> into the given <paramref name="hdc"/> using <see cref="PInvokeCore.SetROP2(HDC, R2_MODE)"/>.
    /// </summary>
    public SetRop2Scope(HDC hdc, R2_MODE rop2)
    {
        _previousRop = (R2_MODE)PInvokeCore.SetROP2(hdc, rop2);

        // If we didn't actually change the ROP, don't keep the HDC so we skip putting back the same state.
        _hdc = _previousRop == rop2 ? default : hdc;
    }

    public void Dispose()
    {
        if (!_hdc.IsNull)
        {
            PInvokeCore.SetROP2(_hdc, _previousRop);
        }

#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }
}
