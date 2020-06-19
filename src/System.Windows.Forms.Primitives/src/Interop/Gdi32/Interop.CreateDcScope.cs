// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        /// <summary>
        ///  Helper to scope lifetime of an HDC retrieved via CreateDC/CreateCompatibleDC.
        ///  Deletes the HDC (if any) when disposed.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass
        ///  by <see langword="ref" /> to avoid duplicating the handle and risking a double delete.
        /// </remarks>
        internal ref struct CreateDcScope
        {
            public IntPtr HDC { get; }

            /// <param name="hdc">Creates a compatible DC based off this.</param>
            public CreateDcScope(IntPtr hdc)
            {
                HDC = CreateCompatibleDC(hdc);
            }

            public static implicit operator IntPtr(CreateDcScope dcScope) => dcScope.HDC;

            public void Dispose()
            {
                if (HDC != null)
                {
                    DeleteDC(HDC);
                }
            }
        }
    }
}
