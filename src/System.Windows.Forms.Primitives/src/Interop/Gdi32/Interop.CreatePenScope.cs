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
            public HPEN HPen { get; }

            /// <summary>
            ///  Creates a solid pen based on the <paramref name="color"/> and <paramref name="width"/> using
            ///  <see cref="CreatePen(PS, int, int)" />.
            /// </summary>
            public CreatePenScope(Color color, int width = 1)
            {
                // From MSDN: if width > 1, the style must be PS_NULL, PS_SOLID, or PS_INSIDEFRAME.
                HPen = CreatePen(
                    width > 1 ? PS.GEOMETRIC | PS.SOLID : default,
                    width,
                    ColorTranslator.ToWin32(color));
            }

            public static implicit operator HPEN(in CreatePenScope scope) => scope.HPen;
            public static implicit operator HGDIOBJ(in CreatePenScope scope) => scope.HPen;

            public bool IsNull => HPen.IsNull;

            public void Dispose()
            {
                if (!HPen.IsNull)
                {
                    DeleteObject(HPen);
                }

#if DEBUG
                GC.SuppressFinalize(this);
#endif
            }
        }
    }
}
