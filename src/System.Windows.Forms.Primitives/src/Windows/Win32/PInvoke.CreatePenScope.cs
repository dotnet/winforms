// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <summary>
    ///  Helper to scope the lifetime of a <see cref="HPEN"/>.
    /// </summary>
    /// <remarks>
    ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
    ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double delete.
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
        ///  <see cref="CreatePen(PEN_STYLE, int, COLORREF)" />.
        /// </summary>
        public CreatePenScope(Color color, int width = 1)
        {
            // From MSDN: if width > 1, the style must be PS_NULL, PS_SOLID, or PS_INSIDEFRAME.
            HPEN = CreatePen(
                width > 1 ? (PEN_STYLE.PS_GEOMETRIC | PEN_STYLE.PS_SOLID) : default,
                width,
                color);
        }

        public static implicit operator HPEN(in CreatePenScope scope) => scope.HPEN;
        public static implicit operator HGDIOBJ(in CreatePenScope scope) => (HGDIOBJ)scope.HPEN.Value;

        public bool IsNull => HPEN.IsNull;

        public void Dispose()
        {
            if (!HPEN.IsNull)
            {
                DeleteObject(HPEN);
            }

#if DEBUG
            GC.SuppressFinalize(this);
#endif
        }
    }
}
