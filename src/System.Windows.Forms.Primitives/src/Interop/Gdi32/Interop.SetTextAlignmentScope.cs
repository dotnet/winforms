// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope selecting a given text alignment mode into a HDC. Restores the original text alignment
        ///  mode into the HDC when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass by
        ///  <see langword="ref" /> to avoid duplicating the handle and resetting multiple times.
        /// </remarks>
#if DEBUG
        internal class SetTextAlignmentScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct SetTextAlignmentScope
#endif
        {
            private readonly TA _previousTa;
            private readonly HDC _hdc;

            /// <summary>
            ///  Sets <paramref name="ta"/> in the given <paramref name="hdc"/> using <see cref="SetTextAlign(HDC, TA)"/>.
            /// </summary>
            public SetTextAlignmentScope(HDC hdc, TA ta)
            {
                _previousTa = SetTextAlign(hdc, ta);

                // If we didn't actually change the TA, don't keep the HDC so we skip putting back the same state.
                _hdc = _previousTa == ta ? default : hdc;
            }

            public void Dispose()
            {
                if (!_hdc.IsNull)
                {
                    SetTextAlign(_hdc, _previousTa);
                }

#if DEBUG
                GC.SuppressFinalize(this);
#endif
            }
        }
    }
}
