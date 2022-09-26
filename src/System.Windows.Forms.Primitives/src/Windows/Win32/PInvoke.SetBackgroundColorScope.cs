// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        /// <summary>
        ///  Helper to scope selecting a given background color into a HDC. Restores the original background color into
        ///  the HDC when disposed.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass by
        ///  <see langword="ref" /> to avoid duplicating the handle and resetting multiple times.
        ///  </para>
        /// </remarks>
        internal readonly ref struct SetBackgroundColorScope
        {
            private readonly COLORREF _previousColor;
            private readonly HDC _hdc;

            /// <summary>
            ///  Sets text color <paramref name="color"/> in the given <paramref name="hdc"/> using
            ///  <see cref="SetBkColor(HDC, COLORREF)"/>.
            /// </summary>
            public SetBackgroundColorScope(HDC hdc, Color color)
            {
                COLORREF colorref = (COLORREF)(uint)ColorTranslator.ToWin32(color);
                _previousColor = SetBkColor(hdc, colorref);

                // If we didn't actually change the color, don't keep the HDC so we skip putting back the same state.
                _hdc = colorref == _previousColor ? default : hdc;
            }

            public void Dispose()
            {
                if (!_hdc.IsNull)
                {
                    SetBkColor(_hdc, _previousColor);
                }
            }
        }
    }
}
