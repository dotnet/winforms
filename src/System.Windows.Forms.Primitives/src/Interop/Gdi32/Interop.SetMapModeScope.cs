// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope selecting a given mapping mode into a HDC. Restores the original mapping mode into the HDC
        ///  when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass by
        ///  <see langword="ref" /> to avoid duplicating the handle and resetting multiple times.
        /// </remarks>
#if DEBUG
        internal class SetMapModeScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct SetMapModeScope
#endif
        {
            private readonly MM _previousMapMode;
            private readonly HDC _hdc;

            /// <summary>
            ///  Sets the <paramref name="mapMode"/> in the given <paramref name="hdc"/> using
            ///  <see cref="SetMapMode(HDC, MM)"/>.
            /// </summary>
            public SetMapModeScope(HDC hdc, MM mapMode)
            {
                _previousMapMode = SetMapMode(hdc, mapMode);

                // If we didn't actually change the map mode, don't keep the HDC so we skip putting back the same state.
                _hdc = mapMode == _previousMapMode ? default : hdc;
            }

            public void Dispose()
            {
                if (!_hdc.IsNull)
                {
                    SetMapMode(_hdc, _previousMapMode);
                }

#if DEBUG
                GC.SuppressFinalize(this);
#endif
            }
        }
    }
}
