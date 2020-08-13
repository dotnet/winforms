// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Diagnostics;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope the lifetime of a <see cref="HBRUSH"/>.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double delete.
        /// </remarks>
#if DEBUG
        internal class CreateBrushScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct CreateBrushScope
#endif
        {
            public HBRUSH HBrush { get; }

            /// <summary>
            ///  Creates a solid brush based on the <paramref name="color"/> using <see cref="CreateSolidBrush(int)"/>.
            /// </summary>
            public CreateBrushScope(Color color)
            {
                HBrush = color.IsSystemColor
                    ? User32.GetSysColorBrush(color)
                    : CreateSolidBrush(ColorTranslator.ToWin32(color));
              }

            public static implicit operator HBRUSH(in CreateBrushScope scope) => scope.HBrush;
            public static implicit operator HGDIOBJ(in CreateBrushScope scope) => scope.HBrush;

            public bool IsNull => HBrush.IsNull;

            public void Dispose()
            {
                if (!HBrush.IsNull)
                {
                    // Note that this is a no-op if the original brush was a system brush
                    DeleteObject(HBrush);
                }

#if DEBUG
                GC.SuppressFinalize(this);
#endif
            }
        }
    }
}
