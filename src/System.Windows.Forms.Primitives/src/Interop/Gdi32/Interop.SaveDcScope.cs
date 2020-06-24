// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope lifetime of a saved device context state.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double restore.
        /// </remarks>
        public ref struct SaveDcScope
        {
            public IntPtr HDC { get; }
            private readonly int _savedState;

            public SaveDcScope(IntPtr hdc)
            {
                _savedState = SaveDC(hdc);
                HDC = hdc;
            }

            public void Dispose()
            {
                if (_savedState != 0)
                {
                    RestoreDC(HDC, _savedState);
                }
            }
        }
    }
}
