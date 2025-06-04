// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Helper to scope selecting a given background mix mode into a HDC. Restores the original
///  mix mode into the HDC when disposed.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass by
///   <see langword="ref" /> to avoid duplicating the handle and resetting multiple times.
///  </para>
/// </remarks>
#if DEBUG
internal class SetBkModeScope : DisposalTracking.Tracker, IDisposable
#else
internal readonly ref struct SetBkModeScope
#endif
{
    private readonly BACKGROUND_MODE _previousMode;
    private readonly HDC _hdc;

    /// <summary>
    ///  Selects <paramref name="bkmode"/> into the given <paramref name="hdc"/>.
    /// </summary>
    public SetBkModeScope(HDC hdc, BACKGROUND_MODE bkmode)
    {
        _previousMode = (BACKGROUND_MODE)PInvokeCore.SetBkMode(hdc, bkmode);

        // If we didn't actually change the mode, don't keep the HDC so we skip putting back the same state.
        _hdc = _previousMode == bkmode ? default : hdc;
    }

    public void Dispose()
    {
        if (!_hdc.IsNull)
        {
            PInvokeCore.SetBkMode(_hdc, _previousMode);
        }

#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }
}
