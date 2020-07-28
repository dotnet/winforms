// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;

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
#if DEBUG
        internal class CreateDcScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct CreateDcScope
#endif
        {
            public HDC HDC { get; }

            /// <summary>
            ///  Creates a compatible HDC for <paramref name="hdc"/> using <see cref="CreateCompatibleDC(HDC)"/>.
            /// </summary>
            /// <remarks>
            ///  Passing a null HDC will use the current screen.
            /// </remarks>
            public CreateDcScope(HDC hdc)
            {
                HDC = CreateCompatibleDC(hdc);
            }

            public CreateDcScope(
                string lpszDriverName,
                string? lpszDeviceName = null,
                string? lpszOutput = null,
                IntPtr lpInitData = default,
                bool informationOnly = true)
            {
                HDC = informationOnly
                    ? CreateICW(lpszDriverName, lpszDeviceName, lpszOutput, lpInitData)
                    : CreateDC(lpszDriverName, lpszDeviceName, lpszOutput, lpInitData);
            }

            public static implicit operator HDC(in CreateDcScope dcScope) => dcScope.HDC;
            public static implicit operator HGDIOBJ(in CreateDcScope dcScope) => dcScope.HDC;
            public static explicit operator IntPtr(in CreateDcScope dcScope) => dcScope.HDC.Handle;

            public bool IsNull => HDC.IsNull;

            public void Dispose()
            {
                if (!HDC.IsNull)
                {
                    DeleteDC(HDC);
                }

#if DEBUG
                GC.SuppressFinalize(this);
#endif
            }
        }
    }
}
