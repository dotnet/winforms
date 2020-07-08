﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope selecting a given background mix mode into a HDC. Restores the original
        ///  mix mode into the HDC when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass by
        ///  <see langword="ref" /> to avoid duplicating the handle and resetting multiple times.
        /// </remarks>
        internal readonly ref struct SetBkModeScope
        {
            private readonly BKMODE _previousMode;
            private readonly HDC _hdc;

            /// <summary>
            ///  Selects <paramref name="bkmode"/> into the given <paramref name="hdc"/>.
            /// </summary>
            public SetBkModeScope(HDC hdc, BKMODE bkmode)
            {
                _previousMode = SetBkMode(hdc, bkmode);

                // If we didn't actually change the mode, don't keep the HDC so we skip putting back the same state.
                _hdc = _previousMode == bkmode ? default : hdc;
            }

            public void Dispose()
            {
                if (!_hdc.IsNull)
                {
                    SetBkMode(_hdc, _previousMode);
                }
            }
        }
    }
}
