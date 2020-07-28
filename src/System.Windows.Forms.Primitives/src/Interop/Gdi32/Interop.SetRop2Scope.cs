// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope selecting a given foreground mix mode into a HDC. Restores the original mix mode into the
        ///  HDC when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass by
        ///  <see langword="ref" /> to avoid duplicating the handle and resetting multiple times.
        /// </remarks>
#if DEBUG
        internal class SetRop2Scope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct SetRop2Scope
#endif
        {
            private readonly R2 _previousRop;
            private readonly HDC _hdc;

            /// <summary>
            ///  Selects <paramref name="rop2"/> into the given <paramref name="hdc"/> using <see cref="SetROP2(HDC, R2)"/>.
            /// </summary>
            public SetRop2Scope(HDC hdc, R2 rop2)
            {
                _previousRop = SetROP2(hdc, rop2);

                // If we didn't actually change the ROP, don't keep the HDC so we skip putting back the same state.
                _hdc = _previousRop == rop2 ? default : hdc;
            }

            public void Dispose()
            {
                if (!_hdc.IsNull)
                {
                    SetROP2(_hdc, _previousRop);
                }

#if DEBUG
                GC.SuppressFinalize(this);
#endif
            }
        }
    }
}
