// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Helper to scope selecting a given foreground text color into a HDC. Restores the original text color into
///  into the HDC when disposed.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass by
///   <see langword="ref" /> to avoid duplicating the handle and resetting multiple times.
///  </para>
/// </remarks>
#if DEBUG
internal class SetTextColorScope : DisposalTracking.Tracker, IDisposable
#else
internal readonly ref struct SetTextColorScope
#endif
{
    private readonly COLORREF _previousColor;
    private readonly HDC _hdc;

    /// <summary>
    ///  Sets text color <paramref name="color"/> in the given <paramref name="hdc"/> using
    ///  <see cref="PInvokeCore.SetTextColor(HDC, COLORREF)"/>.
    /// </summary>
    public SetTextColorScope(HDC hdc, COLORREF color)
    {
        _previousColor = PInvokeCore.SetTextColor(hdc, color);

        // If we didn't actually change the color, don't keep the HDC so we skip putting back the same state.
        _hdc = color == _previousColor ? default : hdc;
    }

    public void Dispose()
    {
        if (!_hdc.IsNull)
        {
            PInvokeCore.SetTextColor(_hdc, _previousColor);
        }

#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }
}
