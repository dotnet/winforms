// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Graphics.Gdi;

/// <summary>
///  Helper to scope the lifetime of a <see cref="Gdi.HPEN"/>.
/// </summary>
/// <remarks>
///  <para>
///   Use in a <see langword="using" /> statement. If you must pass this around, always pass
///   by <see langword="ref" /> to avoid duplicating the handle and risking a double delete.
///  </para>
/// </remarks>
#if DEBUG
internal class CreatePenScope : DisposalTracking.Tracker, IDisposable
#else
internal readonly ref struct CreatePenScope
#endif
{
    public HPEN HPEN { get; }

    /// <summary>
    ///  Creates a solid pen based on the <paramref name="color"/> and <paramref name="width"/> using
    ///  <see cref="PInvokeCore.CreatePen(PEN_STYLE, int, COLORREF)" />.
    /// </summary>
    public CreatePenScope(Color color, int width = 1) =>
        HPEN = PInvokeCore.CreatePen(PEN_STYLE.PS_SOLID, width, color);

    public static implicit operator HPEN(in CreatePenScope scope) => scope.HPEN;
    public static unsafe implicit operator HGDIOBJ(in CreatePenScope scope) => scope.HPEN;

    public bool IsNull => HPEN.IsNull;

    public void Dispose()
    {
        if (!HPEN.IsNull)
        {
            PInvokeCore.DeleteObject(HPEN);
        }

#if DEBUG
        GC.SuppressFinalize(this);
#endif
    }
}
