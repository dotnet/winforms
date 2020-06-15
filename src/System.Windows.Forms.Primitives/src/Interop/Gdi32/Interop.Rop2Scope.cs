// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope selecting a given foreground mix mode into a HDC. Restores the original
        ///  mix mode into the HDC when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and resetting multiple times.
        /// </remarks>
        internal ref struct Rop2Scope
        {
            private readonly R2 _previousRop;
            private readonly IntPtr _hdc;

            /// <summary>
            ///  Selects <paramref name="rop2"/> into the given <paramref name="hdc"/>.
            /// </summary>
            public Rop2Scope(IntPtr hdc, R2 rop2)
            {
                _hdc = hdc;
                _previousRop = SetROP2(hdc, rop2);
            }

            public void Dispose()
            {
                SetROP2(_hdc, _previousRop);
            }
        }
    }
}
