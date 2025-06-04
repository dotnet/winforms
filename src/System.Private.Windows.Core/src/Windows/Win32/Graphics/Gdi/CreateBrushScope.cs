// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Helper to scope the lifetime of a <see cref="HBRUSH"/>.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass
///   by <see langword="ref" /> to avoid duplicating the handle and risking a double delete.
///  </para>
/// </remarks>
#if DEBUG
internal class CreateBrushScope : DisposalTracking.Tracker, IDisposable
#else
internal readonly ref struct CreateBrushScope
#endif
{
    public HBRUSH HBRUSH { get; }

    /// <summary>
    ///  Creates a solid brush based on the <paramref name="color"/> using <see cref="PInvokeCore.CreateSolidBrush(COLORREF)"/>.
    /// </summary>
    public CreateBrushScope(Color color)
    {
        HBRUSH = color.IsSystemColor
            ? PInvokeCore.GetSysColorBrush(color)
            : PInvokeCore.CreateSolidBrush(color);

        ValidateBrushHandle();
    }

    public static implicit operator HBRUSH(in CreateBrushScope scope) => scope.HBRUSH;
    public static implicit operator HGDIOBJ(in CreateBrushScope scope) => scope.HBRUSH;

    public bool IsNull => HBRUSH.IsNull;

    public void Dispose()
    {
        if (!HBRUSH.IsNull)
        {
            // Note that this is a no-op if the original brush was a system brush
            PInvokeCore.DeleteObject(HBRUSH);
        }

#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }

    [Conditional("DEBUG")]
    private void ValidateBrushHandle()
    {
        if (HBRUSH.IsNull)
        {
            // Take LastError with a grain of salt here as it may not have been set.
#if DEBUG
            GC.SuppressFinalize(this);
#endif
            throw new Win32Exception("Could not create a GDI brush.");
        }
    }
}
